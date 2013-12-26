using CommonBehaviors.Actions;
using DeathVader.Core;
using DeathVader.Helpers;
using DeathVader.Managers;
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
using KP = DeathVader.Managers.DvHotKeyManager.KeyboardPolling;
using SG = DeathVader.Interfaces.Settings.DvSettings;
using SH = DeathVader.Interfaces.Settings.DvSettingsH;
using Spell = DeathVader.Core.DvSpell;
using T = DeathVader.Managers.DvTalentManager;


namespace DeathVader.Routines
{
   public class DvGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
        internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => (Me.Specialization == WoWSpec.DeathKnightUnholy && DvUnit.DefaultBuffCheck && (SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new PrioritySelector(
                            Spell.Cast("Raise Dead", ret => Me, ret => SG.Instance.Unholy.PrebuffPet && !Me.GotAlivePet))));
            }
        }


        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

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

                    DvLogger.CombatLog(
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

        #region  StuffDK
        internal static bool PillarofFrost { get { return Me.HasCachedAura(51271, 0); } }

        #endregion

        #region Runes

        internal static int DeathRuneSlotsActive
        {
            get {  return Me.DeathRuneCount; }
        }

        internal static int BloodRuneSlotsActive
        {
            get
            {
             //   DvLogger.InfoLog("DeathRunes: {0}", Me.DeathRuneCount);
                return StyxWoW.Me.BloodRuneCount;
            }
        }

        internal static int FrostRuneSlotsActive
        {
            get
            {
             //   DvLogger.InfoLog("FrostRunes: {0}", Me.FrostRuneCount);
                return StyxWoW.Me.FrostRuneCount;
            }
        }

        internal static int BloodRuneSlotsActivePL { get { return Me.GetRuneCount(0) + Me.GetRuneCount(1); } }
        internal static int FrostRuneSlotsActivePL { get { return Me.GetRuneCount(2) + Me.GetRuneCount(3); } }
        internal static int UnholyRuneSlotsActivePL { get { return Me.GetRuneCount(4) + Me.GetRuneCount(5); } }


        internal static bool DeathRuneSlotsActiveFesterReal { get { return StyxWoW.Me.BloodRuneCount > 1 || StyxWoW.Me.DeathRuneCount > 1; } }

        internal static int UnholyRuneSlotsActive
        {
            get
            {
              //  DvLogger.InfoLog("UnholyRunes: {0}", Me.UnholyRuneCount);
                return StyxWoW.Me.UnholyRuneCount;
            }
        }

        /// <summary>
        /// check that we are in the last tick of Frost Fever or Blood Plague on current target and have a fully depleted rune
        /// </summary>
        internal static bool CanCastPlagueLeechDW
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                int bloodPlague = (int)Me.CurrentTarget.GetAuraTimeLeft("Blood Plague").TotalMilliseconds;
                return DvTalentManager.HasTalent(2) && Me.CurrentTarget != null && Me.CurrentTarget.HasMyAura(55078) && (BloodRuneSlotsActive == 0 || FrostRuneSlotsActive == 0);
            }
        }


        internal static bool CanCastPlagueLeech
        {
            get
            {
                if (!Me.GotTarget)
                    return false;

                int frostFever = (int)Me.CurrentTarget.GetAuraTimeLeft("Frost Fever").TotalMilliseconds;
                int bloodPlague = (int)Me.CurrentTarget.GetAuraTimeLeft("Blood Plague").TotalMilliseconds;
                // if there is 3 or less seconds left on the diseases and we have a fully depleted rune then return true.
                return (frostFever.Between(350, 3000) || bloodPlague.Between(350, 3000))
                    && (FrostRuneSlotsActive == 0 || UnholyRuneSlotsActive == 0);
            }
        }

        internal static Composite HandlePestilence()
        {
            return new PrioritySelector(
                new Decorator(ret => Me.CurrentTarget != null && DvUnit.AoeBPCheck,
                    new PrioritySelector(
                        Spell.PreventDoubleCast("Pestilence", 2, ret => Me.CurrentTarget != null && Me.CurrentTarget.HasAura(55078)))));
        }

        internal static int ActiveRuneCount
        {
            get
            {
                return StyxWoW.Me.BloodRuneCount + StyxWoW.Me.FrostRuneCount + StyxWoW.Me.UnholyRuneCount +
                       StyxWoW.Me.DeathRuneCount;
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

        #region Diseases

        internal static bool HasBothDis
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura frostFever =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55095);
                WoWAura bloodPlague =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);
                return frostFever != null && frostFever.TimeLeft >= TimeSpan.FromSeconds(2) ||
                       (bloodPlague != null && bloodPlague.TimeLeft >= TimeSpan.FromSeconds(2));
            }
        }

        internal static bool HasffDis
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura frostFever = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 55095);
                return frostFever != null && frostFever.TimeLeft >= TimeSpan.FromSeconds(2);
            }
        }

        public static WoWUnit BestUnholyFrenzyTarget
        {
            get
            {
                // If the player has a focus target set, use it instead.
                if (StyxWoW.Me.FocusedUnitGuid != 0 && StyxWoW.Me.FocusedUnit.IsAlive)
                    return StyxWoW.Me.FocusedUnit;

                return Me;
            }
        }

        internal static bool HasbpDis
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura bloodPlague = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);
                return bloodPlague != null && bloodPlague.TimeLeft >= TimeSpan.FromSeconds(2);
            }
        }



        #endregion Diseases

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

        #region Misc

        internal static bool CanEmpowerRuneWeapon
        {
            get
            {
                return StyxWoW.Me.CurrentRunicPower < 26 && DeathRuneSlotsActive + FrostRuneSlotsActive == 0 && UnholyRuneSlotsActive < 2;
            }
        }

        internal static bool CanEmpowerRuneWeaponDW
        {
            get
            {
                return DvLua.PlayerPower < 20 && DvSpell.GetSpellCooldown(49020).TotalMilliseconds > 2000 && DvSpell.GetSpellCooldown(49184).TotalMilliseconds > 2000 && Me.IsWithinMeleeRange;
            }
        }

        #endregion Misc

        #region AMZ stuff
        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => DvHotKeyManager.AMZKey && SpellManager.CanCast("Anti-Magic Zone"),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Anti-Magic Zone");
                        Styx.WoWInternals.Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        DvLogger.CombatLogP("Casting: Anti-Magic Zone - On Mouse Location");
                    })),
                new Decorator(ret => DvHotKeyManager.RaiseAllyKey,
                    new PrioritySelector(
                        Spell.Cast("Raise Ally", ret => Me.FocusedUnit))),
                new Decorator(ret => DvHotKeyManager.ArmyofTheDeadKey,
                    new PrioritySelector(
                        Spell.Cast("Army of the Dead"))),
                new Decorator(ret => Me.CurrentTarget != null && DvHotKeyManager.Tier6AbilitiesKey,
                    new PrioritySelector(
                        Spell.Cast("Gorefiend's Grasp", ret => T.HasTalent(16)),
                        Spell.Cast("Remorseless Winter", ret => T.HasTalent(17)),
                        Spell.Cast("Desecrated Ground", ret => T.HasTalent(18)))));
        }
        #endregion

        #region BloodTapCommonStuff

        private static bool NeedBloodTap
        {
            get { return Me.HasCachedAura("Blood Charge", 5); }

        #endregion

        }
    }
}
