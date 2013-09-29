using System;
using System.Collections.Generic;
using Styx.CommonBot;
using Styx.Plugins;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.CommonBot.Profiles;

namespace BattlePetSwapper
{
    public class BPSPlugin
    {
        private static IPluginLogger _logger;
        private static IPluginSettings _BPSPluginSettings;
        private static IPluginProperties _BPSPluginProperties;
        private static IPetLua _petLua;
        private static IPetJournal _petJournal;
        private static IPetChooser _petChooser;
        private static IPetLoader _petLoader;

        private static int _lastPulse = Environment.TickCount;
        private static int _tickCountOfLastPetEquip = 0;
        private static bool _hasPetBattledSinceLastPetLoad = true;
        private static int _lastPetBattledLevel = 0;

        public BPSPlugin()
        {
            _logger = new PluginLogger();
            _petLua = new PetLua(_logger);
            _BPSPluginSettings = new PluginSettings(_logger);
            _BPSPluginProperties = _BPSPluginSettings as IPluginProperties;
            _petJournal = new PetJournal(_logger, _BPSPluginProperties, _petLua);
            _petChooser = new PetChooser(_logger, _BPSPluginProperties, _petLua);
            _petLoader = new PetLoader(_logger, _petLua);
        }

        public  void Pulse()
        {
            try
            {
                if (_lastPulse + 1000 < Environment.TickCount)
                {
                    _lastPulse = Environment.TickCount;

                    if (!_petLua.IsInBattle())
                    {
                        PulseWhenNotInBattle();
                    }
                    else
                    {
                        PulseWhenInBattle();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Write(ex.Message);
            }
        }

        private static void PulseWhenInBattle()
        {
            if (!_hasPetBattledSinceLastPetLoad)
            {
                _logger.Write("We are in a battle, I will refresh after it is over.");
                _hasPetBattledSinceLastPetLoad = true;
            }

            int enemyPetLevel = _petLua.GetLevelBySlotID_Enemy(1);
            if (enemyPetLevel > 1 && enemyPetLevel < 26)
            {
                if (_lastPetBattledLevel != enemyPetLevel)
                {
                    _lastPetBattledLevel = enemyPetLevel;
                    _logger.Write("We are battling a level " + enemyPetLevel.ToString() + " pet.");
                }
            }
        }

        private static void PulseWhenNotInBattle()
        {
            if (_hasPetBattledSinceLastPetLoad)
            {
                if (Environment.TickCount - _tickCountOfLastPetEquip > 10000) // Don't refresh pets too often
                {
                    _hasPetBattledSinceLastPetLoad = false;

                    if (_lastPetBattledLevel > 0 || _BPSPluginProperties.Mode == eMode.Ringer)
                    {
                        _petLua.ResurrectPets();

                        if (_BPSPluginProperties.Mode != eMode.Capture || !_petJournal.IsLoaded)
                        {
                            _petJournal.PopulatePetJournal();
                        }

                        LoadPetsForLevel(_lastPetBattledLevel);
                    }
                    else
                    {
                        _logger.Write("Waiting for the next battle to determine the level of the pets in this zone.");
                    }
                    _tickCountOfLastPetEquip = Environment.TickCount;
                }
            }
        }

        private static void LoadPetsForLevel(int levelToLoad)
        {
            if (!_petJournal.IsLoaded) { return; }
            List<Pet> Selectedpets = _petChooser.SelectPetsForLevel(_petJournal.OwnedPetsList, _petJournal.FavouritePetsList, levelToLoad);
            _petLoader.Load(Selectedpets);
        }

        #region BPSPlugin Properties / Settings Button

        private static LocalPlayer Me { get { return Styx.StyxWoW.Me; } }
/*        public override string Name { get { return "Battle Pet Swapper"; } }
        public override string Author { get { return "Andy West (Based on PetsAng Apoc/Ang)"; } }
        public override Version Version { get { return new Version(1, 2, 0, 0); } }
        public override string ButtonText { get { return "Configuration"; } }
        public override bool WantButton { get { return true; } }
        */
        public   void OnButtonPress()
        {
            new PluginSettingsForm(_BPSPluginSettings, _logger).Show();
        }

        #endregion

        #region Bot Events

     /*  public override void Initialize()
        {
            BotEvents.OnBotStarted += BotEvents_OnBotStarted;
            BotEvents.OnBotStopped += BotEvents_OnBotStopped;
            _logger.Write(Name + " loaded (V" + Version.ToString() + ")");
            _logger.Write(_BPSPluginSettings.ToString());
        }*/

        void BotEvents_OnBotStarted(EventArgs args)
        {
            if (Me.IsDead || Me.IsGhost || !Styx.StyxWoW.IsInGame) return;
        }

        void BotEvents_OnBotStopped(EventArgs args)
        {
        }

        #endregion
    }
}