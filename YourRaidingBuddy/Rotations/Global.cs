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
using System.Globalization;

namespace YourBuddy.Rotations
{
    class Global
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        internal static int ? _anticipationCount;
        private static ulong MouseOverTarget;



        #region Global Used Composites
        internal static Composite InitializePreBuffMonk
        {
            get
            {
                return new PrioritySelector(
                new Decorator(ret => U.DefaultBuffCheck && (SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat,
                new PrioritySelector(
                Spell.CastRaidBuff("Legacy of the White Tiger", ret => Me.Specialization == WoWSpec.MonkWindwalker && !Me.HasAnyAura(LegacyoftheWhiteTiger)),
                Spell.Cast("Stance of the Sturdy Ox", ret => Me.Specialization == WoWSpec.MonkBrewmaster && !Me.HasAura("Stance of the Sturdy Ox")),
                Spell.CastRaidBuff("Legacy of the Emperor", ret => !Me.HasAnyAura(LegacyoftheEmperor))
               )));
            }
        }

        internal static Composite InitializePreBuffRogue
        {
            get
            {
                return new PrioritySelector(
                    //           new Decorator(ret => SG.Instance.General.CheckPreCombatHk, InitializeOnKeyActions()),
                    new Decorator(ret => Me.Specialization == WoWSpec.RogueAssassination && U.DefaultBuffCheck,
                        new PrioritySelector(
                    new Decorator(ret => SG.Instance.Assassination.CheckPoison, Poisons.CreateApplyPoisonsAss()))),
                    new Decorator(ret => Me.Specialization == WoWSpec.RogueSubtlety && U.DefaultBuffCheck,
                        new PrioritySelector(
                            Spell.Cast("Ambush", ret => StyxWoW.Me.CurrentTarget.MeIsBehind && Me.HasAura("Vanish")),
                            new Decorator(ret => SG.Instance.Subtlety.CheckPoison, Poisons.CreateApplyPoisonsSub()))),
                    new Decorator(ret => Me.Specialization == WoWSpec.RogueCombat && U.DefaultBuffCheck,
                        new PrioritySelector(
                   new Decorator(ret => SG.Instance.Combat.CheckPoison, Poisons.CreateApplyPoisonsCom()))));
            }
        }

        internal static Composite InitializePreBuffShaman
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => Me.Specialization == WoWSpec.ShamanEnhancement && Unit.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new PrioritySelector(
                        Spell.Cast("Lightning Shield", ret => Me, ret => SG.Instance.Elemental.PrebuffPet && !Me.HasAura("Lightning Shield")),
                     WeaponImbue.CreateWeaponImbueEnhancement())),
                       new Decorator(ret => Me.Specialization == WoWSpec.ShamanElemental && Unit.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new PrioritySelector(
                     Spell.Cast("Lightning Shield", ret => Me, ret => SG.Instance.Elemental.PrebuffPet && !Me.HasAura("Lightning Shield")),
                     WeaponImbue.CreateWeaponImbueElemental())));
            }
        }



        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                // new Action(delegate { XISpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { U.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; }),
                new Decorator(ret => StyxWoW.Me.Specialization == WoWSpec.HunterBeastMastery && SG.Instance.Beastmastery.AutoTarget, Unit.GetUnits())
                );
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
         146250, // STR - Thok's Tail
         138737, // Blades
         138756, // Blades
         138938, // Juju Madness
         138699, // Valor Trinket
         148896, // Sigil of Rampage
         146310, // Ticking Ebon Detonator
         146311, // Ticking Ebon Detonator
         146308, // Assurance of Consequence
         148903, // Haromm's Talisman
         148903, // haromm's HC Talisman


        };

        internal static readonly HashSet<int> InterruptListMoP = new HashSet<int>
       {
           145171, // MC spell Garrosh (final phase empowered)
		   145065, // MC spell Garrosh (P2)
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

        #region RogueStuff

        internal static Composite InitializeOnKeyActionsR()
        {
            return new PrioritySelector(
            new Decorator(ret => SG.Instance.General.UseTricksAutoRogue,
                new PrioritySelector(
                    Spell.Cast("Tricks of the Trade", ret => BestTricksTarget))),
                new Decorator(ret => KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice),
                    new PrioritySelector(
                        Spell.Cast("Tricks of the Trade", ret => BestTricksTarget))));
        }

        internal static Composite InitializeOnKeyActionsM()
        {
            return new PrioritySelector(
                     YourBuddy.Core.Helpers.LuaClass.RunMacroText("/cast [@mouseover,harm] Storm, Earth, and Fire", ret => KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice)));
        }

        internal static void HandleMouseOverTarget(object sender, LuaEventArgs args)
        {          

            var unitName = Lua.GetReturnVal<string>("return GetUnitName(\"mouseover\");", 0);
            if (unitName != "")
            {
                var unitGuid = Lua.GetReturnVal<string>("return UnitGUID(\"mouseover\");", 0);
                if (unitGuid != "")
                {
                    try
                    {
                        unitGuid = unitGuid.Replace("0x", String.Empty);
                        MouseOverTarget = ulong.Parse(unitGuid, NumberStyles.HexNumber);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    MouseOverTarget = 0;
                }
            }
            else
            {
                MouseOverTarget = 0;
            }
        }

        internal static bool WeakenedBlowsAura { get { return !Me.CurrentTarget.HasAura(113746); } }

        internal static int AnticipationCount
        {
            get
            {
                if (_anticipationCount.HasValue)
                {
                    return _anticipationCount.Value;
                }

                if (!HasAnticipation)
                {
                    _anticipationCount = 0;
                    return _anticipationCount.Value;
                }

                _anticipationCount = Me.HasAura(115189)
                                         ? (int)Me.GetAuraById(115189).StackCount
                                         : 0;

                return _anticipationCount.Value;
            }
        }

        internal static bool HasAnticipation
        {
            get { return TalentManager.IsSelected(18); }
        }

        // Booleans for multiple use.
        internal static bool YourGoingDownBoss { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 35; } }
        internal static bool ImDpsingYou { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent >= 35; } }
        internal static bool TargettingMe { get { return Me.CurrentTarget.IsTargetingMeOrPet; } }
        internal static bool TargetIsClose { get { return Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange; } }

        // Cached Aura's - Can only be used with MY aura's (HasCachedAura).
        internal static bool VendettaAura { get { return Me.HasAura(79140); } }
        internal static bool ShadowbladesAura { get { return Me.HasAura(121471); } }
        internal static bool ShadowbladesAuraActive { get { return Me.HasAura(121471); } }
        internal static bool DispatchProc { get { return Me.HasAura(121153); } }
        internal static bool NoDispatchLove { get { return !Me.HasAura(121153); } }
        internal static bool TargetNoRupture { get { return !Me.CurrentTarget.HasMyAura(1943); } }
        internal static bool TargetHaveRupture { get { return Me.CurrentTarget.HasMyAura(1943); } }
        internal static bool TargetRuptureFalling { get { return Me.CurrentTarget != null && Spell.GetAuraTimeLeft(1943, Me.CurrentTarget) < 2; } }
        internal static bool HemorrhageDebuffFalling { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Hemorrhage"); } }
        internal static bool TargetRuptureFalling5Cps { get { return Me.CurrentTarget != null && Spell.GetAuraTimeLeft(1943, Me.CurrentTarget) < 3; } }
        internal static bool CrimsonTempestNotUp { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura(121411); } }
        internal static bool EnvenomRemains2Seconds { get { return Spell.GetAuraTimeLeft(32645, Me) < 2; } }
        internal static bool ShadowBladesSND { get { return Spell.GetAuraTimeLeft(5171, StyxWoW.Me) > 10; } }
        internal static bool FucknoSND { get { return !Me.HasAura(5171); } }
        internal static bool IloveyouSND { get { return Me.HasAura(5171); } }
        internal static double SNDSetting { get { return Spell.GetAuraTimeLeft(5171, Me); } }
        internal static double RuptureSetting { get { return Spell.GetAuraTimeLeft("Rupture", Me.CurrentTarget); } }
        internal static bool MySNDBabyIsFalling { get { return Spell.GetAuraTimeLeft(5171, Me) < 2; } }
        internal static bool SliceandDiceSub { get { return Spell.GetAuraTimeLeft(5171, Me) < 4; } }
        internal static bool SliceAndDiceSubGenerator { get { return Spell.GetAuraTimeLeft(5171, Me) < 6; } }
        internal static bool TargetHaveRupture4 { get { return Spell.GetAuraTimeLeft(1943) < 4; } }
        internal static bool MeHasShadowFocus { get { return Me.HasAura(108209); } }
        internal static bool Vanishisoncooldown { get { return CooldownTracker.SpellOnCooldown(1856); } }
        internal static bool ShadowDanceOnline { get { return !CooldownTracker.SpellOnCooldown(51713); } }
        internal static double FindWeakness { get { return Spell.GetAuraTimeLeft(91021, Me.CurrentTarget); } }
        internal static bool FindWeaknessOff { get { return !Spell.HasAura(Me.CurrentTarget, 91021); } }
        internal static bool PremeditationOnline { get { return !CooldownTracker.SpellOnCooldown(14183); } }
        internal static bool ShadowDanceOffline { get { return CooldownTracker.SpellOnCooldown(51713); } }
        internal static bool VanishIsOnCooldown { get { return CooldownTracker.SpellOnCooldown(1856); } }
        internal static bool VanishIsNotOnCooldown { get { return !CooldownTracker.SpellOnCooldown(1856); } }
        internal static bool Kick { get { return CooldownTracker.SpellOnCooldown("Kick"); } }

        private static readonly HashSet<int> Tier14Ids = new HashSet<int>
        {
            -526, // 509 ilvl
            1124, // 496 ilvl
            -503,  // 483 ilvl
        };

        private static readonly HashSet<int> Tier15Ids = new HashSet<int>
        {
            1167, // 522 ilvl
            -740, // 535 ilvl
            -717,  // 502 ilvl
        };

        internal static readonly HashSet<int> DoNotUseHealing = new HashSet<int>
        {
            142863, //  Red Weak Ancient Barrier
            142862, // Ancient Barrier
            142864, // Orange Ancient Barrier 
            142865,  // Green  Strong Ancient Barrier
        };

        private static WoWUnit TricksTarget
        {
            get
            {
                return BestTricksTarget;
            }
        }

        public static WoWUnit BestTricksTarget
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty &&
                     !StyxWoW.Me.GroupInfo.IsInRaid)
                {
                    return null;
                }

                // If the player has a focus target set, use it instead.
                if (StyxWoW.Me.FocusedUnitGuid != 0)
                {
                    return StyxWoW.Me.FocusedUnit;
                }

                if (RaFHelper.Leader != null &&
                     !RaFHelper.Leader.IsMe)
                {
                    // Leader first, always. Otherwise, pick a rogue/DK/War pref. Fall back to others just in case.
                    return RaFHelper.Leader;
                }

                WoWPlayer bestTank =
                    StyxWoW.Me.GroupInfo.RaidMembers.Where
                        (member => member.HasRole(WoWPartyMember.GroupRole.Tank) && member.Health > 0).Select
                        (member => member.ToPlayer()).Where(player => player != null).OrderBy
                        (player => player.DistanceSqr).FirstOrDefault();

                if (bestTank != null)
                {
                    return bestTank;
                }

                WoWPlayer bestPlayer =
                    StyxWoW.Me.GroupInfo.RaidMembers.Where
                        (member => member.HasRole(WoWPartyMember.GroupRole.Damage) && member.Health > 0).Select
                        (member => member.ToPlayer()).Where(player => player != null).OrderBy
                        (player => player.DistanceSqr).FirstOrDefault();

                return bestPlayer;
            }
        }

        internal static bool RevealingStrike
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura Reveal = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 84617);
                return Reveal != null && Reveal.TimeLeft >= TimeSpan.FromMilliseconds(3000);
            }
        }


        internal static bool SliceAndDiceEnevenom
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura SilceAndDiceEne = Me.GetAllAuras().FirstOrDefault(u => u.SpellId == 5171);
                return SilceAndDiceEne != null && SilceAndDiceEne.TimeLeft >= TimeSpan.FromMilliseconds(3000);
            }
        }
        #endregion

        #region Hunterstuff

        #region AutoTarget

        private static DateTime LastAutoTarget;

        internal static Composite AutoTarget()
        {
            return new Styx.TreeSharp.Action(delegate
            {
                if (!SG.Instance.Beastmastery.AutoTarget ||
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
                    Logger.DebugLog("Switch to Best Unit");
                    LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(100);
                    return RunStatus.Failure;
                }

                if (Me.CurrentTarget == null &&
                    GetBestTarget() &&
                    UnitBestTarget != null &&
                    UnitBestTarget.IsValid)
                {
                    UnitBestTarget.Target();
                    Logger.DebugLog("Target  Best Unit");
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
                //     if (Me.CurrentMap.IsBattleground || Me.CurrentMap.IsArena)
                //     {
                //          UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                //               unit => unit != null && unit.IsValid &&
                //                      BsUnit.Attackable(unit, 40));

                //           if (UnitBestTarget == null)
                //            {
                // //                 UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.Distance).FirstOrDefault(
                //                      unit => unit != null && unit.IsValid &&
                //                              BsUnit.Attackable(unit, 80));
                //           }
                //        }

                if (UnitBestTarget == null)
                {
                    UnitBestTarget = Unit.NearbyUnFriendlyUnits.OrderBy(unit => unit.ThreatInfo.RawPercent).FirstOrDefault(
                        unit => unit.IsValid &&
                                Me.IsFacing(unit) &&
                                (unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember ||
                                 unit.IsTargetingMeOrPet) &&
                                Unit.Attackable(unit, 40));
                }

                return UnitBestTarget != null && !UnitBestTarget.IsDisabled;
            }
        }
        #endregion

        internal static string TrapSwitchingBM
        {
            get
            {
                switch (InternalSettings.Instance.Beastmastery.TrapSwitch)
                {

                    case Enum.Traps.ExplosiveTrap:
                        return "Explosive Trap";
                    case Enum.Traps.FreezingTrap:
                        return "Freezing Trap";
                    case Enum.Traps.IceTrap:
                        return "Ice Trap";
                    case Enum.Traps.SnakeTrap:
                        return "Snake Trap";
                    default:
                        return null;
                }
            }
        }

        internal static string TrapSwitchingSV
        {
            get
            {
                switch (InternalSettings.Instance.Survival.TrapSwitch)
                {

                    case Enum.Traps.ExplosiveTrap:
                        return "Explosive Trap";
                    case Enum.Traps.FreezingTrap:
                        return "Freezing Trap";
                    case Enum.Traps.IceTrap:
                        return "Ice Trap";
                    case Enum.Traps.SnakeTrap:
                        return "Snake Trap";
                    default:
                        return null;
                }
            }
        }

        internal static string TrapSwitchingMM
        {
            get
            {
                switch (InternalSettings.Instance.Marksmanship.TrapSwitch)
                {

                    case Enum.Traps.ExplosiveTrap:
                        return "Explosive Trap";
                    case Enum.Traps.FreezingTrap:
                        return "Freezing Trap";
                    case Enum.Traps.IceTrap:
                        return "Ice Trap";
                    case Enum.Traps.SnakeTrap:
                        return "Snake Trap";
                    default:
                        return null;
                }
            }
        }

        internal static Composite InitializeOnKeyActionsH()
        {
            return new PrioritySelector(
                //     new Decorator(ret => KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice) && SpellManager.CanCast("Anti-Magic Zone"),
                //      new Action(ret =>
                //       {
                //           SpellManager.Cast("Anti-Magic Zone");
                //            Styx.WoWInternals.Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                //           Logger.CombatLog("Casting: Anti-Magic Zone - On Mouse Location");
                //         })),
            new Decorator(ret => KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice) && Me.FocusedUnit != null && !Me.FocusedUnit.HasAura(34477),
               new PrioritySelector(
                   Spell.Cast("Misdirect", ret => StyxWoW.Me.FocusedUnit))),
             new Decorator(ret => KP.IsKeyAsyncDown(SettingsH.Instance.DemoBannerChoice),
                new PrioritySelector(
                    Spell.Cast("Deterrence", ret => !Me.HasAura(19263)))));
         //   new Decorator(ret => Me.CurrentTarget != null && KP.IsKeyAsyncDown(SettingsH.Instance.MockingBannerChoice),
        //       new PrioritySelector(
        //           Spell.Cast("Gorefiend's Grasp", ret => TalentManager.IsSelected(16)),
        //            Spell.Cast("Remorseless Winter", ret => TalentManager.IsSelected(17)),
        //           Spell.Cast("Desecrated Ground", ret => TalentManager.IsSelected(18)))));
        }
        #endregion

        #region Deathknight Stuff

        #region AMZ stuff
        internal static Composite InitializeOnKeyActionsDK()
        {
            return new PrioritySelector(
              new Decorator(ret => KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice) && SpellManager.CanCast("Anti-Magic Zone"),
                new Action(ret =>
                {
                    SpellManager.Cast("Anti-Magic Zone");
                    Styx.WoWInternals.Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                    Logger.CombatLog("Casting: Anti-Magic Zone - On Mouse Location");
                })),
            new Decorator(ret => KP.IsKeyAsyncDown(SettingsH.Instance.DemoBannerChoice),
               new PrioritySelector(
                   Spell.Cast("Raise Ally", ret => Me.FocusedUnit))),
             new Decorator(ret => KP.IsKeyAsyncDown(SettingsH.Instance.HeroicLeapChoice),
                new PrioritySelector(
                    Spell.Cast("Army of the Dead"))),
            new Decorator(ret => Me.CurrentTarget != null && KP.IsKeyAsyncDown(SettingsH.Instance.MockingBannerChoice),
               new PrioritySelector(
                   Spell.Cast("Gorefiend's Grasp", ret => TalentManager.IsSelected(16)),
                    Spell.Cast("Remorseless Winter", ret => TalentManager.IsSelected(17)),
                   Spell.Cast("Desecrated Ground", ret => TalentManager.IsSelected(18)))));
        }
        #endregion

        internal static int DeathRuneSlotsActive
        {
            get { return Me.DeathRuneCount; }
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
                return TalentManager.IsSelected(2) && Me.CurrentTarget != null && Me.CurrentTarget.HasMyAura(55078) && (BloodRuneSlotsActive == 0 || FrostRuneSlotsActive == 0);
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

        internal static bool CanCastPlagueLeechUH
        {
            get
            {
                if (!Me.GotTarget)
                    return false;

                int frostFever = (int)Me.CurrentTarget.GetAuraTimeLeft("Frost Fever").TotalMilliseconds;
                int bloodPlague = (int)Me.CurrentTarget.GetAuraTimeLeft("Blood Plague").TotalMilliseconds;
                // if there is 3 or less seconds left on the diseases and we have a fully depleted rune then return true.
                return (frostFever.Between(350, 3000) || bloodPlague.Between(350, 3000))
                    && (BloodRuneSlotsActive == 0 || UnholyRuneSlotsActive == 0) && !CooldownTracker.SpellOnCooldown("Outbreak");
            }
        }
        #endregion

        #region Paladin Stuff

        #region DevastatingAbilities

        internal static readonly HashSet<int> DevastatingAbilities = new HashSet<int>
       {

           143502, // Execute General Nazgrim HC 

       };


        #endregion
        #endregion

        #region FlameShockTargets

        internal static bool TargetsHaveFlameShock4 { get { return Unit.NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasMyAura("Flame Shock")) >= 3; } }
        internal static bool TargetsHaveFlameShock1 { get { return Unit.NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasMyAura("Flame Shock")) >= 1; } }

        #endregion FlameShockTargets

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
