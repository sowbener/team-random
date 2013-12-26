using CommonBehaviors.Actions;
using Shammy.Core;
using Shammy.Helpers;
using Shammy.Managers;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Action = Styx.TreeSharp.Action;
using KP = Shammy.Managers.SmHotKeyManager.KeyboardPolling;
using SG = Shammy.Interfaces.Settings.SmSettings;
using SH = Shammy.Interfaces.Settings.SmSettingsH;
using Spell = Shammy.Core.SmSpell;
using T = Shammy.Managers.SmTalentManager;
using W = Shammy.Helpers.WeaponImbue;

namespace Shammy.Routines
{
    class SmGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
        internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                   new Decorator(ret => SG.Instance.General.CheckPreCombatHk, InitializeOnKeyActions()),
                    new Decorator(ret => Me.Specialization == WoWSpec.ShamanEnhancement && SmUnit.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new PrioritySelector(
                        Spell.Cast("Lightning Shield", ret => Me, ret => SG.Instance.Elemental.PrebuffPet && !Me.HasAura("Lightning Shield")),
                     W.CreateWeaponImbueEnhancement())),
                       new Decorator(ret => Me.Specialization == WoWSpec.ShamanElemental && SmUnit.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new PrioritySelector(
                     Spell.Cast("Lightning Shield", ret => Me, ret => SG.Instance.Elemental.PrebuffPet && !Me.HasAura("Lightning Shield")),
                     W.CreateWeaponImbueElemental())),
                    new Action(delegate { SmSpell.GetCachedAuras(); return RunStatus.Failure; }));
            }
        }

        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { SmSpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { SmUnit.GetAttackableMeleeUnitsCount(); return RunStatus.Failure; })
                );
        }

        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

        #endregion

        #region  StuffDK
        internal static bool PillarofEnhancement { get { return Me.HasCachedAura(51271, 0); } }

        #endregion

        #region Runes

        internal static int DeathRuneSlotsActive
        {
            get { return StyxWoW.Me.DeathRuneCount; }
        }

        internal static int BloodRuneSlotsActive
        {
            get
            {
                //Logger.InfoLog("B {0}, b {1}", StyxWoW.Me.GetRuneCount(0) != 0 ? "Ready" : "depleted", StyxWoW.Me.GetRuneCount(1) != 0 ? "Ready" : "depleted");
                return StyxWoW.Me.BloodRuneCount;
            }
        }

        #region Tier Piece Armor Detection

        /// <summary>Checks the number of Tier pieces a player is wearing </summary>
        private static int NumTierPieces(int txxItemSetId)
        {
            {
                var
                count = Me.Inventory.Equipped.Hands != null && Me.Inventory.Equipped.Hands.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Hands.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Legs != null && Me.Inventory.Equipped.Legs.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Legs.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Chest != null && Me.Inventory.Equipped.Chest.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Chest.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Shoulder != null && Me.Inventory.Equipped.Shoulder.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Shoulder.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Head != null && Me.Inventory.Equipped.Head.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Head.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                return count;
            }
        }

        /// <summary>Returns true if player has 2pc Tier set Bonus</summary>
        public static bool Has2PcTeirBonus(int txxItemSetId)
        {
            {
                return NumTierPieces(txxItemSetId) >= 2;
            }
        }

        /// <summary>Returns true if player has 4pc Tier set Bonus</summary>
        public static bool Has4PcTeirBonus(int txxItemSetId)
        {
            {
                return NumTierPieces(txxItemSetId) == 4;
            }
        }

        #endregion Tier Piece Armor Detection

        private static readonly HashSet<int> Tier15Ids = new HashSet<int>
        {
            -724, // 535 ilvl
            1152, // 522 ilvl
            -701,  // 502 ilvl
        };

        internal static bool Has4PcTier15 { get { return Tier15Ids.Any(Has4PcTeirBonus); } }

        internal static bool Has2PcTier15 { get { return Tier15Ids.Any(Has2PcTeirBonus); } }

        #endregion Runes

        #region InterruptList

        internal static Random _random = new Random();

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

        #region FlameShockTargets

        internal static bool TargetsHaveFlameShock4 { get { return SmUnit.NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasMyAura("Flame Shock")) >= 3; } }
        internal static bool TargetsHaveFlameShock1 { get { return SmUnit.NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasMyAura("Flame Shock")) >= 1; } }

        #endregion FlameShockTargets

        #region YOUWANNALOOKHERE stuff
        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => SmHotKeyManager.KeyboardPolling.IsKeyDown(SH.Instance.AMZ) && SpellManager.CanCast("Healing Rain"),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Healing Rain");
                        Styx.WoWInternals.Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        SmLogger.CombatLogP("Casting: Healing Rain - On Mouse Location");
                    })),
                new Decorator(ret => SmHotKeyManager.KeyboardPolling.IsKeyAsyncDown(SH.Instance.Tier6),
                    new PrioritySelector(
                        Spell.PreventDoubleCast("Healing Tide Totem", 1, ret => T.HasTalent(13)),
                        Spell.Cast("Ancestral Guidance", ret => T.HasTalent(14)),
                        Spell.Cast("Conductivity", ret => T.HasTalent(15)))));
        }
        #endregion

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
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
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
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
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

                    SmLogger.CombatLog(
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


        }
    }
