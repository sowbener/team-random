using CommonBehaviors.Actions;
using YourBuddy.Core;
using YourBuddy.Core.Helpers;
using YourBuddy.Core.Managers;
using YourBuddy.Core.Utilities;
using YourBuddy.Interfaces.Settings;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using Enum = YourBuddy.Core.Helpers.Enum;
using U = YourBuddy.Core.Unit;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using System.Windows.Forms;
using KP = YourBuddy.Core.Managers.HotKeyManager;

namespace YourBuddy.Rotations
{
    class Global
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }


        #region Global Used Composites
        internal static Composite InitializePreBuffMonk
        {
            get
            {
                return new PrioritySelector(
                new Decorator(ret => Unit.DefaultBuffCheck && (SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat,
                new PrioritySelector(
                Spell.CastRaidBuff("Legacy of the White Tiger", ret => Me.Specialization == WoWSpec.MonkWindwalker && !Me.HasAnyAura(LegacyoftheWhiteTiger)),
                Spell.Cast("Stance of the Sturdy Ox", ret => Me.Specialization == WoWSpec.MonkBrewmaster && !Me.HasAura("Stance of the Sturdy Ox")),
                Spell.CastRaidBuff("Legacy of the Emperor", ret => !Me.HasAnyAura(LegacyoftheEmperor))
               )));
            }
        }


        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                // new Action(delegate { XISpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { Unit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; }),
                new Action(delegate { Unit.GetNearbyAggroUnitsCount(); return RunStatus.Failure; })
                );
        }

        #region ManualCastPause

        private static readonly HashSet<string> MovementKeyHS = new HashSet<string>
        {
            "MOVEFORWARD",
            "MOVEBACKWARD",
            "TURNLEFT",
            "TURNRIGHT",
            "STRAFELEFT",
            "STRAFERIGHT",
            "JUMP",
            "TURNORACTION",
            "CAMERAORSELECTORMOVE",
        };

        private static readonly HashSet<Keys> MovementKey = new HashSet<Keys> { };

        private static string KeySystemtoKeyBind(string key)
        {
            switch (key)
            {
                case "D1":
                    return "1";
                case "D2":
                    return "2";
                case "D3":
                    return "3";
                case "D4":
                    return "4";
                case "D5":
                    return "5";
                case "D6":
                    return "6";
                case "D7":
                    return "7";
                case "D8":
                    return "8";
                case "D9":
                    return "9";
                case "D0":
                    return "0";
                case "OemMinus":
                    return "-";
                case "Oemplus":
                    return "=";
                default:
                    return key;
            }
        }

        internal static void GetBinding()
        {
            //Logging.Write("----------------------------------");
            foreach (Keys key in System.Enum.GetValues(typeof(Keys)))
            {
                //Logging.Write(key.ToString());

                //Logging.Write("Trying to run Lua.GetReturnVal<string>('return GetBindingAction('" + key.ToString() + "')', 0);");

                var BlackListKey =
                    Lua.GetReturnVal<string>("return GetBindingAction('" + KeySystemtoKeyBind(key.ToString()) + "')", 0);

                if (MovementKeyHS.Contains(BlackListKey))
                {
                    //Logging.Write("Movement Key: " + BlackListKey + " = " + key);
                    //Logging.Write(key + " Binding to " + BlackListKey + "  Blacklist it");
                    //Logging.Write("Add {0} to BlackListKeyHS", key.ToString());
                    MovementKey.Add(key);
                }
            }
            //Logging.Write("----------------------------------");
        }

        private static bool MovementKeyPressed()
        {
            if (KP.GetAsyncKeyState(Keys.LButton) < 0 &&
                KP.GetAsyncKeyState(Keys.RButton) < 0)
            {
                //Logging.Write("MovementKeyPressed: LButton and RButton Pressed");
                return true;
            }

            //foreach (Keys key in MovementKey)
            //{
            //    if (GetAsyncKeyState(key) < 0)
            //    {
            //        //Logging.Write("MovementKeyPressed: {0} Pressed", KeySystemtoKeyBind(key.ToString()));
            //        return true;
            //    }
            //}

            return MovementKey.Any(key => KP.GetAsyncKeyState(key) < 0);
        }

        private static bool AnyKeyPressed()
        {
            foreach (Keys key in System.Enum.GetValues(typeof(Keys)))
            {
                if (KP.GetAsyncKeyState(key) != 0 &&
                    key != Keys.LButton &&
                    key != Keys.RButton &&
                    key != Keys.LWin &&
                    key != Keys.RWin &&
                    key != Keys.ShiftKey &&
                    key != Keys.LShiftKey &&
                    key != Keys.RShiftKey &&
                    key != Keys.ControlKey &&
                    key != Keys.LControlKey &&
                    key != Keys.RControlKey &&
                    key != Keys.Menu &&
                    key != Keys.LMenu &&
                    key != Keys.RMenu &&
                    key != Keys.Tab &&
                    key != Keys.CapsLock &&
                    !MovementKey.Contains(key))
                {
                    //Logging.Write(Colors.Gray,
                    //    "Key {0} is pressed. Manual CastPause Activated. Combat Routine Pause for {1} ms",
                    //    KeySystemtoKeyBind(key.ToString()),
                    //    THSettings.Instance.AutoDetectManualCastMS);

                    Logger.CombatLogLg(
                        "Auto Pause on Manual Cast: Key press detected - Combat Routine Pause for {0} ms",
                        SG.Instance.General.DetectKeyPress);

                    return true;
                }
            }
            return false;
        }

        internal static Composite ManualCastPause()
        {
            return new Sequence(
                new Decorator(ret =>
                    SG.Instance.General.AutoDetectManualCast &&
                    AnyKeyPressed(), new ActionAlwaysSucceed()),
                //new Action(
                //    delegate
                //    {
                //        Logging.Write(LogLevel.Diagnostic, Colors.Gray, "{0} Manual Cast Detected, Pause for {1} ms",
                //            DateTime.Now.ToString("ss:fff"),
                //            THSettings.Instance.AutoDetectManualCastMS);
                //    }),
                new WaitContinue(TimeSpan.FromMilliseconds(SG.Instance.General.DetectKeyPress), ret => false,
                    new ActionAlwaysSucceed())
                //new Action(delegate { Logging.Write("{0} Rotation Resume", DateTime.Now.ToString("ss:fff")); })
                );
        }

        #endregion

        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

        #region InterruptList

        internal static Random _random = new Random();

        internal static readonly HashSet<int> LegacyoftheEmperor = new HashSet<int>
       {

           117666, // Legacy of the Emperor
           115921, // Legacy of the emperor
           1126, // Mark of the Wild
           20217, // Blessing of Kings
           90363, // Embrace of the Shale Spider	

       };

        internal static readonly HashSet<int> LegacyoftheWhiteTiger = new HashSet<int>
       {

         116781, // Legacy of the White Tiger
         17007, // Leader of the Pack
         1459, // Arcane Brilliance
         61316, // Dalaran Brilliance
         24604, // Furious Howl
         90309, // Terrifying Roar	
         126373, // Fearless Roar
         126309, // Still Water
       };

        internal static readonly HashSet<int> AgilityProcList = new HashSet<int>
        {

         126489, // Searing Words
         126554, // Bottle of Infinite Stars
         128984, // Relic of Xuen
         126707, // Tyrannical
         126690, // Call of Conquest
         146312, // Celestial Trinket
         126649, // Terror in the Mists
         138737, // Blades
         138756, // Blades
         138938, // Juju Madness
         138699, // Valor Trinket
         148896, // Sigil of Rampage
         146310, // Ticking Ebon Detonator
         146308, // Assurance of Consequence
         148903, // Haromm's Talisman

        };

        internal static readonly HashSet<int> InterruptListMoP = new HashSet<int>
       {
           136797, // Dino-Mending (ToT)
           136587, // Venom Bolt Volley (ToT)
           61909, // Fireball (ToT)
           136189, // Sandbolt (ToT)
           144583, // Wulf Healing (SoO)
           144018, // Corruption (SoO)
           144649, // Hurl Corruption (SoO)
           143473, // Empowered Chain Heal (SoO)
           145555, // Empowered Chain Heal (SoO)
           143432, // Arcane Shock (SoO)
           143431, // Magistrike (SoO)
           145230, // Forbidden Magic (SoO)
           144922, // Harden Flesh
           143958, // Corruption Shock
           145631, // Corruption Chain
           144923, // Earthen Shard
           144379, // Mocking Blast (Manifestation of Pride)
           144468, // Inspiring Song (Chi Ji Timeless Isle)
           146728, // Chain-Heal (SoO)
           146757, // Chain-Heal (SoO)
           143423, // Sha Sear
           123654, // Ming (5man HC instance)
           121284, // Wing Leader (5man HC instance)
           117833, // Crazy Thought (MSV)
           117628, // Shadow Blast (MSV)
           125877, // Dispatch (HoF)
           124077, // Dispatch (HoF)


       };

        internal static readonly HashSet<int> InterruptListTBA = new HashSet<int>
        {

        };

        #endregion

        #endregion

        #region AllowedClasses

        internal static readonly HashSet<WoWClass> AllowedClassList = new HashSet<WoWClass>
        {

         WoWClass.Hunter, //Hunter Class
         WoWClass.Rogue, //Rogue Class
         WoWClass.Shaman, //Shaman Class
         WoWClass.DeathKnight, //DeathKnight Class
         WoWClass.Monk, //Monk Class
         WoWClass.Paladin, //Paladin Class

        };
        #endregion

        #region TalentManager Functions
        // Talentmanager - HasGlyphs

        // Talentmanager CurrentSpec
        internal static bool IsWWSpec
        {
            get { return TalentManager.CurrentSpec == WoWSpec.MonkWindwalker; }
        }

        internal static bool IsBMMSpec
        {
            get { return TalentManager.CurrentSpec == WoWSpec.MonkBrewmaster; }
        }


               #endregion

        #region Cooldowntracker
        // Cooldowntracker - Timeleft in TotalMilliseconds
      
        internal static bool VictoryRushOnCooldown
        {
            get { return CooldownTracker.SpellOnCooldown(SpellBook.VictoryRush); } // Victory Rush
        }
        #endregion
    }
}
