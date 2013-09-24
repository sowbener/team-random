using CommonBehaviors.Actions;
using FuryUnleashed.Shared.Helpers;
using FuryUnleashed.Shared.Managers;
using FuryUnleashed.Shared.Utilities;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = FuryUnleashed.Routines.FuGlobal;
using I = FuryUnleashed.Core.Item;
using SB = FuryUnleashed.Shared.Helpers.SpellBook;
using SG = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SH = FuryUnleashed.Interfaces.Settings.SettingsH;
using Spell = FuryUnleashed.Core.Spell;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Routines
{
    class FuryCombat
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeFuryPreCombat
        {
            get
            {
                return new PrioritySelector(
                    new PrioritySelector(ret => !Me.Combat,
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Decorator(ret => SG.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => SG.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, 
                                Spell.Cast(SB.BattleShout, on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast(SB.CommandingShout, on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => SG.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                    new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => SG.Instance.Fury.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => SG.Instance.General.CrFuryRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development,
                            new Decorator(ret => true, DevFuryCombat)),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release,
                            new Decorator(ret => true, RelFuryCombat))));
                            //!Spell.IsGlobalCooldown()
            }
        }

        internal static Composite DevFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryUtility(),
                                Dev_FuryVictorious(),
                                I.CreateItemBehaviour(),
                                Dev_FuryRacials(),
                                Dev_FuryOffensive(),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                new Decorator(ret => G.ExecuteCheck, Dev_FuryExec()),
                                Dev_FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryUtility(),
                                Dev_FuryVictorious(),
                                I.CreateItemBehaviour(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive())),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                new Decorator(ret => G.ExecuteCheck, Dev_FuryExec()),
                                Dev_FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryUtility(),
                                Dev_FuryVictorious(),
                                I.CreateItemBehaviour(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                new Decorator(ret => G.ExecuteCheck, Dev_FuryExec()),
                                Dev_FurySt()))));
            }
        }

        internal static Composite RelFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                FuryUtility(),
                                FuryVictorious(),
                                I.CreateItemBehaviour(),
                                FuryRacials(),
                                FuryOffensive(),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, FuryMt()),
                                FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                FuryUtility(),
                                FuryVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        FuryRacials(),
                                        FuryOffensive())),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, FuryMt()),
                                FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                FuryUtility(),
                                FuryVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        FuryRacials(),
                                        FuryOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, FuryMt()),
                                FurySt()))));
            }
        }
        #endregion

        #region Development Rotations
        // TODO: Check all Settings.
        // TODO: Improve, Add and Recheck.
        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage >= 30),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage > Me.MaxRage - 10 && G.CSCD >= 3000),
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow2S && G.CSCD >= 3000),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),

                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),

                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow1S && G.CSCD >= 3000),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Me.CurrentRage < 70))),

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CSCD >= 6000 && Tier4AbilityUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),

                        Spell.Cast(SB.WildStrike, ret => !G.BloodsurgeAura && G.CSCD >= 3000 && Me.CurrentRage >= 90),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage),

                        Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && !G.IVOC && SG.Instance.Fury.CheckRotImpVic), // Added for the sake of supporting it rotational.
                        Spell.Cast(SB.HeroicThrow, ret => SG.Instance.Fury.CheckHeroicThrow) // Added for the sake of supporting it rotational.
                        )));
        }

        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),

                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added

                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.Execute, ret => Me.CurrentRage == Me.MaxRage - 10),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage),

                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.BTCD >= 2000 && G.CSCD >= 6000 && Tier4AbilityUsage) // Added
                        )));
        }

        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 5,
                    new PrioritySelector(

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.ThunderClap, ret => SG.Instance.Fury.CheckAoEThunderclap && U.NeedThunderclapUnitsCount > 0), // Added

                        Spell.Cast(SB.Bloodthirst), 
                        Spell.Cast(SB.Whirlwind),
                        Spell.Cast(SB.RagingBlow, ret => Me.CurrentRage <= 60 && G.MeatCleaverAuraS3)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 4,
                    new PrioritySelector(

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.ThunderClap, ret => U.NeedThunderclapUnitsCount > 0 && SG.Instance.Fury.CheckAoEThunderclap), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS3),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS3),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.ThunderClap, ret => U.NeedThunderclapUnitsCount > 0 && SG.Instance.Fury.CheckAoEThunderclap), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS2),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS2),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.ThunderClap, ret => U.NeedThunderclapUnitsCount > 0 && SG.Instance.Fury.CheckAoEThunderclap), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS1),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS1),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10),
                        new PrioritySelector(
                            new Decorator(ret => G.ExecuteCheck, Dev_FuryExec()),
                            new Decorator(ret => G.NonExecuteCheck, Dev_FurySt()))
                        )));
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(500)) && G.BTOC && G.ColossusSmashAura && BerserkerRageUsage),
                Spell.Cast(SB.Bloodbath, ret => G.BbTalent && Tier6AbilityUsage),
                Spell.Cast(SB.Recklessness, ret => RecklessnessUsage),
                Spell.Cast(SB.Avatar, ret => G.AvTalent && RecklessnessSync && Tier6AbilityUsage),
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && RecklessnessSync)
                );
        }

        internal static Composite Dev_FuryVictorious()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.IVOC && G.IvTalent && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VROC && G.VictoriousAura && SG.Instance.Fury.CheckVicRush && Me.HealthPercent <= SG.Instance.Fury.CheckVicRushNum)
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.DiebytheSword, ret => SG.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && SG.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => SG.Instance.Fury.CheckShieldWall && Me.HealthPercent <= SG.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => SG.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                I.FuryUseHealthStone()
                );
        }

        internal static Composite Dev_FuryUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Fury.HamString == Enum.Hamstring.Always || SG.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.IntimidatingShout, ret => SG.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && SG.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.ShatteringThrow, ret => SG.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000)),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && SG.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Release Rotations
        internal static Composite FurySt()
        {
            return new PrioritySelector(
                // SimulationCraft 530-6 (r16981) - 1H/2H Fury - Slightly Optimized.
                Spell.Cast("Execute", ret => G.DeathScentenceAuraT16),
                new Decorator(ret => G.AlmostDead,
                        new PrioritySelector(
                                Spell.Cast("Colossus Smash", ret => G.ColossusSmashAura),
                                Spell.Cast("Execute"),
                                Spell.Cast("Heroic Strike", ret => G.DumpAllRage && Me.CurrentRage > 60)
                                )),
                Spell.Cast("Heroic Strike", ret => Me.CurrentRage >= 110 || (G.NonExecuteCheck && G.ColossusSmashAura && Me.CurrentRage >= 40)),
                Spell.Cast("Raging Blow", ret => G.RagingBlow2S && G.ColossusSmashAura && G.NonExecuteCheck),
                Spell.Cast("Bloodthirst"),
                Spell.Cast("Wild Strike", ret => G.BloodsurgeAura && G.NonExecuteCheck && G.BTCD <= 1000),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (G.BloodbathAura || G.AvTalent || G.SbTalent) && !G.ColossusSmashAura && Tier4AbilityUsage),
                Spell.Cast("Colossus Smash"),
                Spell.Cast("Execute", ret => G.ExecuteCheck || G.DeathScentenceAuraT16),
                Spell.Cast("Storm Bolt", ret => G.SbTalent && Tier6AbilityUsage),
                Spell.Cast("Raging Blow", ret => G.RagingBlow2S || (G.RagingBlowAura && (G.ColossusSmashAura || (G.BTCD >= 1000 && G.FadingRb(3000))))),
                Spell.Cast("Wild Strike", ret => G.BloodsurgeAura),
                Spell.Cast("Raging Blow", ret => G.RagingBlowAura && G.CSCD >= 3000),
                Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow && !G.ColossusSmashAura),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura))),
                Spell.Cast("Wild Strike", ret => !G.WieldsTwoHandedWeapons && G.ColossusSmashAura && G.NonExecuteCheck),
                Spell.Cast("Whirlwind", ret => G.WieldsTwoHandedWeapons && G.ColossusSmashAura && G.NonExecuteCheck),
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && G.NonExecuteCheck && ((SG.Instance.Fury.CheckRotImpVic) || (SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum))),
                Spell.Cast("Wild Strike", ret => !G.WieldsTwoHandedWeapons && G.CSCD >= 2000 && Me.CurrentRage >= 80 && G.NonExecuteCheck),
                Spell.Cast("Whirlwind", ret => G.WieldsTwoHandedWeapons && G.CSCD >= 2000 && Me.CurrentRage >= 80 && G.NonExecuteCheck),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70)))
                );
        }

        internal static Composite FuryMt()
        {
            return new PrioritySelector(
                Spell.Cast("Execute", ret => G.DeathScentenceAuraT16),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast("Cleave", ret => Me.CurrentRage >= 110),
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || G.AvTalent || G.SbTalent) && Tier4AbilityAoEUsage),
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast("Bladestorm", ret => G.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast("Execute", ret => G.ColossusSmashAura),
                        Spell.Cast("Bloodthirst"),
                        Spell.Cast("Colossus Smash"),
                        Spell.Cast("Raging Blow", ret => G.MeatCleaverAuraS1),
                        Spell.Cast("Whirlwind", ret => !G.MeatCleaverAuraS1),
                        Spell.Cast("Raging Blow", ret => G.RagingBlow2S || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3000 || G.FadingRb(3000)))),
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && Tier6AbilityUsage),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow))),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        Spell.Cast("Cleave", ret => Me.CurrentRage >= 110),
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || G.AvTalent || G.SbTalent) && Tier4AbilityAoEUsage),
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast("Bladestorm", ret => G.BsTalent && G.EnrageAura && Tier4AbilityAoEUsage),
                        Spell.Cast("Raging Blow", ret => G.FadingRb(2000) || G.MeatCleaverAuraS2),
                        Spell.Cast("Bloodthirst"),
                        Spell.Cast("Whirlwind"),
                        Spell.Cast("Colossus Smash"),
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && Tier6AbilityUsage),
                        Spell.Cast("Raging Blow", ret => G.RagingBlow2S || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3000 || G.FadingRb(3000)))),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow))),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 4,
                    new PrioritySelector(
                        Spell.Cast("Cleave", ret => Me.CurrentRage >= 110),
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || G.AvTalent || G.SbTalent) && Tier4AbilityAoEUsage),
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast("Bladestorm", ret => G.BsTalent && G.EnrageAura && Tier4AbilityAoEUsage),
                        Spell.Cast("Bloodthirst", ret => !G.EnrageAura),
                        Spell.Cast("Colossus Smash"),
                        Spell.Cast("Raging Blow", ret => G.MeatCleaverAuraS3),
                        Spell.Cast("Whirlwind"),
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && Tier6AbilityUsage),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast("Bloodthirst"))));
        }

        internal static Composite FuryVictorious()
        {
            return new PrioritySelector(
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Fury.CheckVicRush && Me.HealthPercent <= SG.Instance.Fury.CheckVicRushNum)
                );
        }

        internal static Composite FuryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Bloodbath", ret => G.BbTalent && (G.CSCD < 2000 || G.ColossusSmashAuraT) && Tier6AbilityUsage),
                Spell.Cast("Recklessness", ret => (G.ColossusSmashAuraT || G.CSCD <= 4000) && ((G.SbTalent) || (G.AvTalent && (G.AvatarAura || G.AVCD <= 3000)) || (G.BbTalent && (G.BloodbathAura || G.BBCD <= 3000))) && RecklessnessUsage),
                Spell.Cast("Avatar", ret => G.AvTalent && (G.RecklessnessAura || G.RCCD >= 180000) && Tier6AbilityUsage),
                Spell.Cast("Skull Banner", ret => (!G.SkullBannerAura && (G.RCCD >= 180000 || G.RecklessnessAura)) && SkullBannerUsage),
                Spell.Cast("Berserker Rage", ret => (!G.EnrageAura || (G.RagingBlow2S && G.NonExecuteCheck) || (G.RecklessnessAuraT && !G.RagingBlowAura)) && BerserkerRageUsage));
        }

        internal static Composite FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Die by the Sword", ret => SG.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast("Enraged Regeneration", ret => G.ErTalent && SG.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast("Shield Wall", ret => SG.Instance.Fury.CheckShieldWall && Me.HealthPercent <= SG.Instance.Fury.CheckShieldWallNum),
                I.FuryUseHealthStone()
                );
        }

        internal static Composite FuryUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Fury.HamString == Enum.Hamstring.Always || SG.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Intimidating Shout", ret => SG.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast("Mass Spell Reflection", ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast("Piercing Howl", ret => G.PhTalent && SG.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast("Shattering Throw", ret => SG.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000)),
                Spell.Cast("Spell Reflection", ret => SG.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Staggering Shout", ret => G.SsTalent && SG.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((SG.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.Always && G.PUOC && G.DSOC));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessSync
        {
            get
            {
                return ((G.RecklessnessAura) || (G.ReadinessAura));
            }
        }

        internal static bool BloodbathSync
        {
            get
            {
                return ((G.BloodbathAura || G.AvTalent || G.SbTalent) || (G.ReadinessAura));
            }
        }
        #endregion
    }
}