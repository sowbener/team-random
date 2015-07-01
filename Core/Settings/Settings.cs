using Clio.Utilities;
using ff14bot;
using ff14bot.Helpers;
using System.ComponentModel;
using YourRaidingBuddy;
using YourRaidingBuddy.Settings;

namespace YourRaidingBuddy.Interfaces.Settings
{
    internal class InternalSettings : JsonSettings
    {
        private static InternalSettings _instance;

        internal InternalSettings()
            : base(RoutineSettingsPath + ".config")
        {
        }

        internal static string RoutineSettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\YourRaidingBuddy FFXIV\\YRB_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, Core.Player.Name, Root.Revision);
            }
        }

        internal static InternalSettings Instance
        {
            get { return _instance ?? (_instance = new InternalSettings()); }
        }

        #region General & Hotkey Loading Wrappers
        private SettingsG _settingsG;
        private SettingsH _settingsH;

        [Browsable(false)]
        internal SettingsH Hotkeys
        {
            get { return _settingsH ?? (_settingsH = new SettingsH()); }
        }

        [Browsable(false)]
        internal SettingsG General
        {
            get { return _settingsG ?? (_settingsG = new SettingsG()); }
        }

        #endregion

        #region Jobs Loading Wrappers
        private NinjaSetting _settingsN;
        private BardSetting _settingsB;
        private BlackMageSetting _settingsBM;
        private DarkKnightSetting _settingsDK;
        private DragoonSetting _settingsD;
        private MachinistSetting _settingsMA;
        private MonkSetting _settingsMO;
        private PaladinSetting _settingsPA;
        private ScholarSetting _settingsSC;
        private SummonerSetting _settingsSU;
        private WarriorSetting _settingsW;
        private WhiteMageSetting _settingsWM;
        private AstrologianSetting _settingsAS;



        [Browsable(false)]
        internal NinjaSetting Ninja
        {
            get { return _settingsN ?? (_settingsN = new NinjaSetting()); }
        }

        [Browsable(false)]
        internal BardSetting Bard
        {
            get { return _settingsB ?? (_settingsB = new BardSetting()); }
        }

        [Browsable(false)]
        internal BlackMageSetting BlackMage
        {
            get { return _settingsBM ?? (_settingsBM = new BlackMageSetting()); }
        }

        [Browsable(false)]
        internal DarkKnightSetting DarkKnight
        {
            get { return _settingsDK ?? (_settingsDK = new DarkKnightSetting()); }
        }
        [Browsable(false)]
        internal DragoonSetting Dragoon
        {
            get { return _settingsD ?? (_settingsD = new DragoonSetting()); }
        }
        [Browsable(false)]
        internal MachinistSetting Machinist
        {
            get { return _settingsMA ?? (_settingsMA = new MachinistSetting()); }
        }
        [Browsable(false)]
        internal MonkSetting Monk
        {
            get { return _settingsMO ?? (_settingsMO = new MonkSetting()); }
        }
        [Browsable(false)]
        internal PaladinSetting Paladin
        {
            get { return _settingsPA ?? (_settingsPA = new PaladinSetting()); }
        }
        [Browsable(false)]
        internal ScholarSetting Scholar
        {
            get { return _settingsSC ?? (_settingsSC = new ScholarSetting()); }
        }
        [Browsable(false)]
        internal SummonerSetting Summoner
        {
            get { return _settingsSU ?? (_settingsSU = new SummonerSetting()); }
        }
        [Browsable(false)]
        internal WarriorSetting Warrior
        {
            get { return _settingsW ?? (_settingsW = new WarriorSetting()); }
        }
        [Browsable(false)]
        internal WhiteMageSetting WhiteMage
        {
            get { return _settingsWM ?? (_settingsWM = new WhiteMageSetting()); }
        }
        [Browsable(false)]
        internal AstrologianSetting Astrologian
        {
            get { return _settingsAS ?? (_settingsAS = new AstrologianSetting()); }
        }
        #endregion

        #region Class Loading Wrappers
        private ConjurerSetting _settingsC;
        private ArcanistSetting _settingsA;
        private GladiatorSetting _settingsGL;
        private LancerSetting _settingsL;
        private PugilistSetting _settingsP;
        private RogueSetting _settingsR;
        private ThaumaturgeSetting _settingsT;
        private ArcherSetting _settingsAR;
        private MarauderSetting _settingsM;

        [Browsable(false)]
        internal ConjurerSetting Conjurer
        {
            get { return _settingsC ?? (_settingsC = new ConjurerSetting()); }
        }

        [Browsable(false)]
        internal ArcanistSetting Arcanist
        {
            get { return _settingsA ?? (_settingsA = new ArcanistSetting()); }
        }

        [Browsable(false)]
        internal GladiatorSetting Gladiator
        {
            get { return _settingsGL ?? (_settingsGL = new GladiatorSetting()); }
        }

        [Browsable(false)]
        internal LancerSetting Lancer
        {
            get { return _settingsL ?? (_settingsL = new LancerSetting()); }
        }

        [Browsable(false)]
        internal PugilistSetting Pugilist
        {
            get { return _settingsP ?? (_settingsP = new PugilistSetting()); }
        }

        [Browsable(false)]
        internal RogueSetting Rogue
        {
            get { return _settingsR ?? (_settingsR = new RogueSetting()); }
        }

        [Browsable(false)]
        internal ThaumaturgeSetting Thaumaturge
        {
            get { return _settingsT ?? (_settingsT = new ThaumaturgeSetting()); }
        }

        [Browsable(false)]
        internal ArcherSetting Archer
        {
            get { return _settingsAR ?? (_settingsAR = new ArcherSetting()); }
        }

        [Browsable(false)]
        internal MarauderSetting Marauder
        {
            get { return _settingsM ?? (_settingsM = new MarauderSetting()); }
        }
        #endregion
    }
}
