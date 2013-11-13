using CommonBehaviors.Actions;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using Enum = FuryUnleashed.Core.Helpers.Enum;
using G = FuryUnleashed.Rotations.Global;
using U = FuryUnleashed.Core.Unit;
using L = FuryUnleashed.Core.Helpers.LuaClass;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SB = FuryUnleashed.Core.Helpers.SpellBook;

namespace FuryUnleashed.Rotations.Protection
{
    class ProtGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static Composite InitializeProtPreCombat
        {
            get
            {
                return new PrioritySelector(
                    new PrioritySelector(ret => !Me.Combat,
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Decorator(ret => InternalSettings.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => Unit.DefaultBuffCheck && ((InternalSettings.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Protection.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout,
                                Spell.Cast(SpellBook.BattleShout, on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout,
                                Spell.Cast(SpellBook.CommandingShout, on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeProtCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => InternalSettings.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => InternalSettings.Instance.Protection.CheckInterrupts && Unit.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.RotationVersion>(ctx => InternalSettings.Instance.General.CrProtRotVersion,
                        new SwitchArgument<Enum.RotationVersion>(Enum.RotationVersion.Development, ProtDev.DevProtCombat),
                        new SwitchArgument<Enum.RotationVersion>(Enum.RotationVersion.Release, ProtRel.RelProtCombat)));
            }
        }

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool DemoralizingShoutUsage
        {
            get
            {
                return ((IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((IS.Instance.Protection.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.MassSpellReflection == Enum.MsrTrigger.Always && G.PummelOnCooldown && G.DisruptingShoutOnCooldown));
            }
        }

        internal static bool SpellReflectionUsage
        {
            get
            {
                return ((IS.Instance.Protection.SpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.SpellReflection == Enum.MsrTrigger.Always && G.PummelOnCooldown && G.DisruptingShoutOnCooldown));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool ShatteringThrowUsage
        {
            get
            {
                return ((IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Protection.Tier6AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Tier6AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Tier6AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool ImpendingVictoryUsage
        {
            get
            {
                return ((IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousAuraT15)) ||
                        (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                        (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15));
            }
        }

        internal static bool VictoryRushUsage
        {
            get
            {
                return ((IS.Instance.Protection.VictoryRush == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousAuraT15)) ||
                        (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                        (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15));
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
                return ((G.BloodbathAura || G.AvatarTalent || G.StormBoltTalent) || (G.ReadinessAura));
            }
        }
    }
}
