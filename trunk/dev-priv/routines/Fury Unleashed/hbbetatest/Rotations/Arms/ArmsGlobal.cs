using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Action = System.Action;
using Enum = System.Enum;

namespace FuryUnleashed.Rotations.Arms
{
    class ArmsGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static Composite InitializeArmsPreCombat
        {
            get
            {
                return new PrioritySelector(
                    new PrioritySelector(ret => !Me.Combat,
                        new Styx.TreeSharp.Action(delegate
                        {
                            Spell.GetCachedAuras();
                            return RunStatus.Failure;
                        }),
                        new Decorator(ret => !StyxWoW.Me.IsInInstance && StyxWoW.Me.CurrentTarget != null &&
                                             StyxWoW.Me.CurrentTarget.IsPlayer && !StyxWoW.Me.CurrentTarget.IsFriendly &&
                                             StyxWoW.Me.CurrentTarget.Distance > 12 &&
                                             StyxWoW.Me.CurrentTarget.Distance < 30,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Charge),
                                Spell.CastOnGround(SpellBook.HeroicLeap, ret => StyxWoW.Me.CurrentTarget),
                    //new Decorator(ret => InternalSettings.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                                new Decorator(ret => InternalSettings.Instance.General.CheckPreCombatHk,
                                    Global.InitializeOnKeyActions()),
                                new Decorator(
                                    ret =>
                                        Unit.DefaultBuffCheck &&
                                        ((InternalSettings.Instance.General.CheckPreCombatBuff && !Me.Combat) ||
                                         Me.Combat),
                                    new Switch<Core.Helpers.Enum.Shouts>(ctx => InternalSettings.Instance.Arms.ShoutSelection,
                                        new SwitchArgument<Core.Helpers.Enum.Shouts>(Core.Helpers.Enum.Shouts.BattleShout,
                                            Spell.Cast(SpellBook.BattleShout, on => Me, ret => !Global.BattleShoutAura)),
                                        new SwitchArgument<Core.Helpers.Enum.Shouts>(Core.Helpers.Enum.Shouts.CommandingShout,
                                            Spell.Cast(SpellBook.CommandingShout, on => Me,
                                                ret => !Global.CommandingShoutAura))))
                                )
                            )
                        )
                    );
            }
        }

        internal static Composite InitializeArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => InternalSettings.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                    new Styx.TreeSharp.Action(delegate
                    {
                        ObjectManager.Update();
                        return RunStatus.Failure;
                    }),
                    Global.InitializeCaching(),
                    Global.InitializeOnKeyActions(),
                    new Decorator(ret => InternalSettings.Instance.Arms.CheckInterrupts && Unit.CanInterrupt,
                        Global.InitializeInterrupts()),
                    new Switch<Core.Helpers.Enum.RotationVersion>(ctx => InternalSettings.Instance.General.CrArmsRotVersion,
                        new SwitchArgument<Core.Helpers.Enum.RotationVersion>(Core.Helpers.Enum.RotationVersion.Development, ArmsDev.DevArmsCombat),
                        new SwitchArgument<Core.Helpers.Enum.RotationVersion>(Core.Helpers.Enum.RotationVersion.PvP, ArmsPvP.PvPArmsCombat),
                        new SwitchArgument<Core.Helpers.Enum.RotationVersion>(Core.Helpers.Enum.RotationVersion.Release, ArmsRel.RelArmsCombat)));
            }
        }

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.BerserkerRage == Core.Helpers.Enum.AbilityTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.BerserkerRage == Core.Helpers.Enum.AbilityTrigger.OnBlTwHr &&
                         Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.BerserkerRage == Core.Helpers.Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((Unit.NearbyCastingUnits(StyxWoW.Me.Location, 45)) != null ||
                        (InternalSettings.Instance.Arms.MassSpellReflection == Core.Helpers.Enum.MsrTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.MassSpellReflection == Core.Helpers.Enum.MsrTrigger.Always && Global.PummelOnCooldown &&
                         Global.DisruptingShoutOnCooldown));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.ClassRacials == Core.Helpers.Enum.AbilityTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.ClassRacials == Core.Helpers.Enum.AbilityTrigger.OnBlTwHr &&
                         Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.ClassRacials == Core.Helpers.Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Recklessness == Core.Helpers.Enum.AbilityTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Recklessness == Core.Helpers.Enum.AbilityTrigger.OnBlTwHr &&
                         Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Recklessness == Core.Helpers.Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.SkullBanner == Core.Helpers.Enum.AbilityTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.SkullBanner == Core.Helpers.Enum.AbilityTrigger.OnBlTwHr &&
                         Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.SkullBanner == Core.Helpers.Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Tier4Abilities == Core.Helpers.Enum.AbilityTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Tier4Abilities == Core.Helpers.Enum.AbilityTrigger.OnBlTwHr &&
                         Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Tier4Abilities == Core.Helpers.Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Tier6Abilities == Core.Helpers.Enum.AbilityTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Tier6Abilities == Core.Helpers.Enum.AbilityTrigger.OnBlTwHr &&
                         Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Tier6Abilities == Core.Helpers.Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Tier4AoeAbilities == Core.Helpers.Enum.AbilityTrigger.OnBossDummy &&
                         Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Tier4AoeAbilities == Core.Helpers.Enum.AbilityTrigger.OnBlTwHr &&
                         Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Tier4AoeAbilities == Core.Helpers.Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessSync
        {
            get
            {
                return ((Global.RecklessnessAura) || (Global.DeterminationAura || Global.OutrageAura));
            }
        }

        internal static bool BloodbathSync
        {
            get
            {
                return ((Global.BloodbathAura || Global.AvatarTalent || Global.StormBoltTalent) || (Global.DeterminationAura || Global.OutrageAura));
            }
        }
    }
}
