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
using U = FuryUnleashed.Core.Unit;
using L = FuryUnleashed.Core.Helpers.LuaClass;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SB = FuryUnleashed.Core.Helpers.SpellBook;

namespace FuryUnleashed.Rotations.Protection
{
    class ProtRel
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static Composite RelProtCombat
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => IS.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => IS.Instance.Protection.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                        new Decorator(ret => SettingsH.Instance.ModeSelection == Enum.Mode.Auto && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        Item.CreateItemBehaviour(),
                                        ProtRacials(),
                                        ProtOffensive(),
                                        new Decorator(ret => IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum, ProtMt()),
                                        ProtSt())),
                        new Decorator(ret => SettingsH.Instance.ModeSelection == Enum.Mode.SemiHotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        Item.CreateItemBehaviour(),
                                                        ProtRacials(),
                                                        ProtOffensive())),
                                        new Decorator(ret => IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum, ProtMt()),
                                        ProtSt())),
                        new Decorator(ret => SettingsH.Instance.ModeSelection == Enum.Mode.Hotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        Item.CreateItemBehaviour(),
                                                        ProtRacials(),
                                                        ProtOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum, ProtMt()),
                                        ProtSt())));
            }
        }

        internal static Composite ProtSt()
        {
            return new PrioritySelector(
                Spell.Cast("Taunt", ret => IS.Instance.Protection.CheckAutoTaunt && !G.TargettingMe),
                //Spell.Cast("Execute", ret => G.ExecutePhase && Me.CurrentRage >= 100 && !G.TargettingMe),
                Spell.Cast("Execute", ret => G.ExecutePhase && Me.CurrentRage > 60),
                Spell.Cast("Heroic Strike", ret => G.UltimatumAura || (Me.CurrentRage >= Me.MaxRage - 10 && G.NormalPhase)),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Storm Bolt", ret => G.StormBoltTalent && (
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Dragon Roar", ret => G.DragonRoarTalent && (
                    (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Thunder Clap", ret => !G.WeakenedBlowsAura),
                Spell.Cast("Devastate")
                );
        }

        internal static Composite ProtMt()
        {
            return new PrioritySelector(
                Spell.Cast("Cleave", ret => G.UltimatumAura || (Me.CurrentRage >= Me.MaxRage - 10 && !G.TargettingMe)),
                Spell.Cast("Thunder Clap"),
                Spell.Cast("Dragon Roar", ret => G.DragonRoarTalent && (
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shockwave", ret => G.ShockwaveTalent && Global.ShockwaveFacing && U.NearbyAttackableUnitsCount >= 3 && (
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bladestorm", ret => G.BladestormTalent && (
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Devastate"),
                Spell.Cast("Intimidating Shout", ret => IS.Instance.Protection.CheckIntimidatingShout && G.IntimidatingShoutGlyph && !U.IsTargetBoss)
                );
        }

        internal static Composite ProtDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Last Stand", ret => IS.Instance.Protection.CheckLastStand && Me.HealthPercent <= IS.Instance.Protection.CheckLastStandNum),
                new Decorator(ret => HotKeyManager.IsSpecial && G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock,
                    new PrioritySelector(
                        Spell.Cast("Shield Barrier"))),
                new Decorator(ret => !HotKeyManager.IsSpecial && G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock,
                    new PrioritySelector(
                        Spell.Cast("Shield Block"))),
                Spell.Cast("Shield Block", ret => !G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock && IS.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBlock),
                Spell.Cast("Shield Barrier", ret => !G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock && Me.CurrentRage >= 80 && IS.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBarrier),
                Spell.Cast("Shield Wall", ret => IS.Instance.Protection.CheckShieldWall && Me.HealthPercent <= IS.Instance.Protection.CheckShieldWallNum),
                Spell.Cast("Enraged Regeneration", ret => G.EnragedRegenerationTalent && IS.Instance.Protection.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Protection.CheckEnragedRegenNum),
                Spell.Cast("Spell Reflection", ret => IS.Instance.Protection.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Mass Spell Reflection", ret => IS.Instance.Protection.CheckSpellReflect && G.SpellReflectionSpellCooldown > 0 && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Item.ProtUseHealthStone()
                );
        }

        internal static Composite ProtVictorious()
        {
            return new PrioritySelector(
                //138279 Victorious - T15 Proc ID (Victory Rush & Impending Victory).
                //32216	Victorious - Regular Kill Proc ID (Victory Rush & Impending Victory).
                Spell.Cast("Impending Victory", ret => G.ImpendingVictoryTalent && (Me.HealthPercent <= IS.Instance.Protection.ImpendingVictoryNum || G.FadingVc(2000)) && (
                    (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousAuraT15)) ||
                    (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                    (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15))),
                Spell.Cast("Victory Rush", ret => !G.ImpendingVictoryTalent && (Me.HealthPercent <= IS.Instance.Protection.VictoryRushNum || G.FadingVc(2000)) && (
                    (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousAuraT15)) ||
                    (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                    (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15)))
                    );
        }

        internal static Composite ProtUtility()
        {
            return new PrioritySelector(
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (IS.Instance.Protection.HamString == Enum.Hamstring.Always || IS.Instance.Protection.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Staggering Shout", ret => G.StaggeringShoutTalent && IS.Instance.Protection.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckPiercingHowlNum),
                Spell.Cast("Piercing Howl", ret => G.PiercingHowlTalent && IS.Instance.Protection.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckPiercingHowlNum),
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Spell.Cast("Demoralizing Shout", ret => G.TargettingMe && Me.HealthPercent <= IS.Instance.Protection.DemoShoutNum && (!G.Tier15FourPieceBonusT || (G.Tier15FourPieceBonusT && (!G.ShieldSlamOnCooldown || G.ShieldSlamSpellCooldown <= 1000))) && (
                    (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0 && !G.LastStandAura && IS.Instance.Protection.CheckRallyingCry),
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && IS.Instance.Protection.CheckDemoBanner && Me.HealthPercent <= IS.Instance.Protection.CheckDemoBannerNum && U.IsDoNotUseOnTgt)
                );
        }

        internal static Composite ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret =>
                    (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always),
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite ProtOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Skull Banner", ret => !G.SkullBannerAura && (
                    (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Avatar", ret => G.AvatarTalent && (
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserker Rage", ret => !G.EnrageAura && (
                    (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bloodbath", ret => G.BloodbathTalent && (
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Recklessness", ret => Unit.IsViable(Me.CurrentTarget) && (
                    (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shattering Throw", ret => (
                    (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.Always)
                    )));
        }
    }
}
