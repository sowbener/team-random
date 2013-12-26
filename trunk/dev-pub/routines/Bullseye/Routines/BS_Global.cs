using Bullseye.Core;
using Bullseye.Helpers;
using Bullseye.Managers;
using CommonBehaviors.Actions;
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
using KP = Bullseye.Managers.BsHotKeyManager.KeyboardPolling;
using SG = Bullseye.Interfaces.Settings.BsSettings;
using SH = Bullseye.Interfaces.Settings.BsSettingsH;
using Spell = Bullseye.Core.BsSpell;
using T = Bullseye.Managers.BsTalentManager;



namespace Bullseye.Routines
{
   public class BsGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
         internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                   new Decorator(ret => SG.Instance.General.CheckPreCombatHk, InitializeOnKeyActions()),
                  new Decorator(ret => SG.Instance.Beastmastery.EnableCallPet || SG.Instance.Survival.EnableCallPet, PetManager.CreateHunterCallPetBehavior()));
            }
        }

        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { BsSpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { BsUnit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; })
                );
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

                    BsLogger.CombatLog(
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

        #region Serpentsting


        internal static bool HasSerpentSting
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                //return Me.CurrentTarget.HasCachedAura(Defines.SerpentStingAura, 0, 2000);	// ID doesn't work - have to use string...why?
                //return Me.CurrentTarget.CachedHasAura("Serpent Sting", 0, true, 2000);
                return Me.CurrentTarget != null && Me.CurrentTarget.HasAura("Serpent Sting", 0, true, 2000);
            }
        }


        #endregion Diseases      

        #region AutoTarget

        private static DateTime LastAutoTarget;

       internal static Composite AutoTarget()
        {
            return new Styx.TreeSharp.Action(delegate
            {
                if (!SG.Instance.General.AutoTarget ||
                    LastAutoTarget > DateTime.Now)
                {
                    return RunStatus.Failure;
                }

                if (Me.CurrentTarget != null && Me.CurrentTarget.IsValid &&
                    !AttackableNoLoS(Me.CurrentTarget, 50) &&
                    GetBestTarget() &&
                    UnitBestTarget != null &&
                    UnitBestTarget.IsValid &&
                    Me.CurrentTarget != UnitBestTarget)
                {
                    UnitBestTarget.Target();
                    BsLogger.DebugLog("Switch to Best Unit");
                    LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(100);
                    return RunStatus.Failure;
                }

                if (Me.CurrentTarget == null &&
                    GetBestTarget() &&
                    UnitBestTarget != null &&
                    UnitBestTarget.IsValid)
                {
                    UnitBestTarget.Target();
                    BsLogger.DebugLog("Target  Best Unit");
                    LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(100);
                    return RunStatus.Failure;
                }
                LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(100);
                return RunStatus.Failure;
            });
        }

        #endregion

        #region GetBestTarget

        private static bool AttackableNoLoS(WoWUnit target, int range)
        {
            if (target == null ||
                !target.IsValid ||
                !target.Attackable ||
                !target.CanSelect ||
                !target.IsAlive ||
                //target.IsCritter ||
                //Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                Spell.SpellDistance(target) > range)
            {
                return false;
            }
            return true;
        }

        private static WoWUnit UnitBestTarget;

        private static bool GetBestTarget()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                UnitBestTarget = null;
                if (Me.CurrentMap.IsBattleground || Me.CurrentMap.IsArena)
                {
                    UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                BsUnit.Attackable(unit, 40));

                    if (UnitBestTarget == null)
                    {
                        UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.Distance).FirstOrDefault(
                            unit => unit != null && unit.IsValid &&
                                    BsUnit.Attackable(unit, 80));
                    }
                }

                if (UnitBestTarget == null && !Me.CurrentMap.IsBattleground && !Me.CurrentMap.IsArena)
                {
                    UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.ThreatInfo.RawPercent).FirstOrDefault(
                        unit => unit.IsValid &&
                                Me.IsFacing(unit) &&
                                (unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember ||
                                 unit.IsTargetingMeOrPet) &&
                                BsUnit.Attackable(unit, 40));
                }

                return UnitBestTarget != null && !UnitBestTarget.IsDisabled;
            }
        }

        #endregion

        #region GlobalHotKeys
        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => BsHotKeyManager.KeyboardPolling.IsKeyDown(SH.Instance.AMZ) && SpellManager.CanCast("Anti-Magic Zone"),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Anti-Magic Zone");
                        Styx.WoWInternals.Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        BsLogger.CombatLogP("Casting: Anti-Magic Zone - On Mouse Location");
                    })),
                new Decorator(ret => BsHotKeyManager.KeyboardPolling.IsKeyAsyncDown(SH.Instance.Deterrence),
                    new PrioritySelector(
                        Spell.Cast("Deterrence"))),
                new Decorator(ret => BsHotKeyManager.KeyboardPolling.IsKeyAsyncDown(SH.Instance.Disengage),
                    new PrioritySelector(
                        Spell.Cast("Gorefiend's Grasp", ret => T.HasTalent(16)),
                        Spell.Cast("Remorseless Winter", ret => T.HasTalent(17)),
                        Spell.Cast("Desecrated Ground", ret => T.HasTalent(18)))));
        }
        #endregion


        }
    }
