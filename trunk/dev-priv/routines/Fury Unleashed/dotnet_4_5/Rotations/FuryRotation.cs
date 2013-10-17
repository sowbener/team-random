using System.Windows.Forms;
using CommonBehaviors.Actions;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using G = FuryUnleashed.Rotations.Global;
using SB = FuryUnleashed.Core.Helpers.SpellBook;

namespace FuryUnleashed.Rotations
{
    class FuryRotation
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
                        new Decorator(ret => InternalSettings.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => InternalSettings.Instance.General.CheckPreCombatHk, Global.InitializeOnKeyActions())),
                    new Decorator(ret => Unit.DefaultBuffCheck && ((InternalSettings.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, 
                                Spell.Cast(SpellBook.BattleShout, on => Me, ret => !Global.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast(SpellBook.CommandingShout, on => Me, ret => !Global.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => InternalSettings.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                    new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    Global.InitializeCaching(),
                    Global.InitializeOnKeyActions(),
                    new Decorator(ret => InternalSettings.Instance.Fury.CheckInterrupts && Unit.CanInterrupt, Global.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => InternalSettings.Instance.General.CrFuryRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development, DevFuryCombat),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release, RelFuryCombat)));
            }
        }

        internal static Composite DevFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                Dev_FuryRacials(),
                                Dev_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_FurySt())
                                        ))))));
            }
        }

        internal static Composite RelFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                Rel_FuryNonGcdUtility(),
                                Rel_FuryRacials(),
                                Rel_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                Rel_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_FuryRacials(),
                                        Rel_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                Rel_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_FuryRacials(),
                                        Rel_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_FurySt())
                                        ))))));
            }
        }
        #endregion

        #region Development Rotations
        // Test based on http://www.icy-veins.com/fury-warrior-wow-pve-dps-rotation-cooldowns-abilities
        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 && G.FadingDeathSentence(3000)),
                        Spell.Cast(SB.StormBolt),
                        Spell.Cast(SB.Bloodthirst, ret => !G.EnrageAura && G.BROC || G.RagingBlow1S),
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.WildStrike, ret => G.BTOC && G.RBOC)
                        )),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.StormBolt, ret => !G.ReadinessAura),
                        Spell.Cast(SB.ColossusSmash, ret => G.RagingBlow1S && !G.BTOC || G.RagingBlow2S && G.BTOC),
                        Spell.Cast(SB.Bloodthirst, ret => G.RagingBlow1S && G.CSCD < 1500),
                        Spell.Cast(SB.HeroicStrike, ret => G.RagingBlow2S && G.CSCD < 3000 && Me.CurrentRage > Me.MaxRage - 20),
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow2S && G.CSCD < 3000),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow1S && G.CSCD < 8000),
                        Spell.Cast(SB.WildStrike, ret => Me.CurrentRage > Me.MaxRage - 20 && G.CSCD > 3000),
                        Spell.Cast(SB.ImpendingVictory, ret => Me.CurrentRage > 50),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Me.CurrentRage < 50)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Me.CurrentRage < 50))),
                        Spell.Cast(SB.HeroicThrow)
                        )));
        }

        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 && G.FadingDeathSentence(3000)),
                        Spell.Cast(SB.StormBolt),
                        Spell.Cast(SB.HeroicStrike, ret => G.Tier16TwoPieceBonus && Me.CurrentRage > 100 && G.RemainingCs(4500)),
                        Spell.Cast(SB.Bloodthirst, ret => !G.EnrageAura && G.BROC),
                        Spell.Cast(SB.Execute),
                        Spell.Cast(SB.RagingBlow)
                        )),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.StormBolt, ret => !G.ReadinessAura),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.HeroicStrike, ret => G.RagingBlow2S && Me.CurrentRage >= Me.MaxRage - 10 && !G.BTOC && G.CSOC && G.SBOC),
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow2S),
                        Spell.Cast(SB.Execute, ret => G.BTOC && G.CSOC && G.RagingBlow1S && Me.CurrentRage > 70 && G.CSCD > 3000),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Me.CurrentRage < 50)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Me.CurrentRage < 50))),
                        Spell.Cast(SB.HeroicThrow)
                        )));
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage > 30)
                        )),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => G.CSCD < 3000 && Me.CurrentRage > Me.MaxRage - 20),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage)
                        )));
        }

        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                );
        }
        #endregion

        #region Release Rotations
        internal static Composite Rel_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage >= 30), // Also in Rel_FuryHeroicStrike().

                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16), // Added T16 P4.
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added - Inside CS window.

                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.RagingBlow),
                        Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura))),
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(

                        Spell.Cast(SpellBook.Execute, ret => Global.FadingDeathSentence(3000) && Global.CSCD >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added - Outside CS window.
                        Spell.Cast(SpellBook.StormBolt, ret => Global.ReadinessAura && Global.CSCD >= 16000 && Tier6AbilityUsage), // Added - When new one is ready in next CS window - With Eye of Galakras.

                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Global.CSCD >= 3000 && ((Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 5))), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow2S && Global.CSCD >= 3000),
                        Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow1S && Global.CSCD >= 3000),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Global.CSCD >= 6000 && Tier4AbilityUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                        Spell.Cast(SpellBook.WildStrike, ret => !Global.BloodsurgeAura && Global.CSCD >= 3000 && Me.CurrentRage >= 90),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SpellBook.ImpendingVictory, ret => Global.IvTalent && !Global.IVOC && InternalSettings.Instance.Fury.CheckRotImpVic), // Added for the sake of supporting it rotational.
                        Spell.Cast(SpellBook.HeroicThrow, ret => InternalSettings.Instance.Fury.CheckHeroicThrow) // Added for the sake of supporting it rotational.
                        )));
        }

        internal static Composite Rel_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage),
                        Spell.Cast(SpellBook.Execute),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.FadingDeathSentence(3000) && Global.CSCD >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.RagingBlow),
                        Spell.Cast(SpellBook.Execute, ret => Me.CurrentRage == Me.MaxRage - 10),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage),
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added
                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Global.BTCD >= 2000 && Global.CSCD >= 6000 && Tier4AbilityUsage) // Added
                        )));
        }

        internal static Composite Rel_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.ColossusSmashAura && Global.NormalPhase,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage >= 30))),
                new Decorator(ret => !Global.ColossusSmashAura && Global.NormalPhase,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.HeroicStrike, ret => Global.CSCD >= 3000 && ((Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 5))))),
                Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage));
        }

        internal static Composite Rel_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 5,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.Whirlwind),
                        Spell.Cast(SpellBook.RagingBlow, ret => Me.CurrentRage <= 60 && Global.MeatCleaverAuraS3)
                        )),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 4,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Whirlwind, ret => !Global.MeatCleaverAuraS3),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS3),
                        Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10)
                        )),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Whirlwind, ret => !Global.MeatCleaverAuraS2),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS2),
                        Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10)
                        )),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Whirlwind, ret => !Global.MeatCleaverAuraS1),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS1),
                        Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10),
                        new PrioritySelector(
                            new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                            new Decorator(ret => Global.NormalPhase, Rel_FurySt()))
                        )));
        }

        internal static Composite Rel_FuryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.BerserkerRage, ret => (!Global.EnrageAura || Global.FadingEnrage(500)) && Global.BTOC && Global.ColossusSmashAura && BerserkerRageUsage),
                Spell.Cast(SpellBook.Bloodbath, ret => Global.BbTalent && Tier6AbilityUsage),
                Spell.Cast(SpellBook.Recklessness, ret => RecklessnessUsage),
                Spell.Cast(SpellBook.Avatar, ret => Global.AvTalent && RecklessnessSync && Tier6AbilityUsage),
                Spell.Cast(SpellBook.SkullBanner, ret => !Global.SkullBannerAura && RecklessnessSync && SkullBannerUsage)
                );
        }

        internal static Composite Rel_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ImpendingVictory, ret => !Global.IVOC && Global.IvTalent && InternalSettings.Instance.Fury.CheckImpVic && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SpellBook.VictoryRush, ret => !Global.VROC && Global.VictoriousAura && InternalSettings.Instance.Fury.CheckVicRush && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SpellBook.IntimidatingShout, ret => InternalSettings.Instance.Fury.CheckIntimidatingShout && Global.IsGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.ShatteringThrow, ret => InternalSettings.Instance.Fury.CheckShatteringThrow && Unit.IsTargetBoss && (Global.CSCD <= 3000 || Global.SBCD <= 3000))
                );
        }

        internal static Composite Rel_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite Rel_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.DiebytheSword, ret => InternalSettings.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.EnragedRegeneration, ret => Global.ErTalent && InternalSettings.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.ShieldWall, ret => InternalSettings.Instance.Fury.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection, ret => InternalSettings.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.FuryUseHealthStone()
                );
        }

        internal static Composite Rel_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckDemoBannerNum && Unit.IsDoNotUseOnTgt),
                Spell.Cast(SpellBook.Hamstring, ret => !Unit.IsTargetBoss && !Global.HamstringAura && (InternalSettings.Instance.Fury.HamString == Enum.Hamstring.Always || InternalSettings.Instance.Fury.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget)),
                Spell.Cast(SpellBook.MassSpellReflection, ret => Global.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SpellBook.PiercingHowl, ret => Global.PhTalent && InternalSettings.Instance.Fury.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0),
                Spell.Cast(SpellBook.StaggeringShout, ret => Global.SsTalent && InternalSettings.Instance.Fury.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.Always && Global.PUOC && Global.DSOC));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Fury.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Fury.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Fury.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((InternalSettings.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessSync
        {
            get
            {
                return ((Global.RecklessnessAura) || (Global.ReadinessAura));
            }
        }

        internal static bool BloodbathSync
        {
            get
            {
                return (Global.BloodbathAura || !Global.BbTalent || Global.ReadinessAura);
            }
        }
        #endregion
    }
}