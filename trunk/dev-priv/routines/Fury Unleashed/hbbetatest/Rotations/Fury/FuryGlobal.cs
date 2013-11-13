using CommonBehaviors.Actions;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using Enum = FuryUnleashed.Core.Helpers.Enum;
using G = FuryUnleashed.Rotations.Global;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;
using SB = FuryUnleashed.Core.Helpers.SpellBook;
using U = FuryUnleashed.Core.Unit;
using Action = Styx.TreeSharp.Action;

namespace FuryUnleashed.Rotations.Fury
{
    class FuryGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static Composite InitializeFuryPreCombat
        {
            get
            {
                return new PrioritySelector(
                    new PrioritySelector(ret => !Me.Combat,
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Decorator(ret => IS.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((IS.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
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
                    new Decorator(ret => IS.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => IS.Instance.Fury.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.RotationVersion>(ctx => IS.Instance.General.CrFuryRotVersion,
                        new SwitchArgument<Enum.RotationVersion>(Enum.RotationVersion.Development, FuryDev.DevFuryCombat),
                        new SwitchArgument<Enum.RotationVersion>(Enum.RotationVersion.Release, FuryRel.RelFuryCombat)));
            }
        }

        #region Booleans
        internal static bool UseWildStrike(bool hasSmashAura)
        {
            if (hasSmashAura)
            {
                if ((Global.BloodthirstSpellCooldown < 1500 || Global.FadingCs(1250)) && !Global.RagingBlow1S && !Global.RagingBlow2S && Me.CurrentRage >= 70)
                    return true;
            }
            else
            {
                if (Global.ColossusSmashSpellCooldown >= 8000 && !Global.RagingBlow1S && !Global.RagingBlow2S && Me.CurrentRage >= 80 && Global.BloodthirstSpellCooldown < 1500)
                    return true;
            }
            return Me.CurrentRage >= Me.MaxRage - 20;
        }

        internal static bool UseHeroicStrike(bool hasSmashAura)
        {
            if (hasSmashAura)
            {
                if ((Global.RagingBlow1S || Global.RagingBlow2S) && Me.CurrentRage >= 40)
                    return true;
                if ((!Global.RagingBlow1S && !Global.RagingBlow2S) && Me.CurrentRage >= 30)
                    return true;
                if (Global.FadingCs(2000) && !Global.RagingBlow1S && !Global.RagingBlow2S && Me.CurrentRage >= 30)
                    return true;
            }
            return Me.CurrentRage >= Me.MaxRage - 10;
        }

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((IS.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool HamstringUsage
        {
            get
            {
                return ((IS.Instance.Fury.HamString == Enum.Hamstring.Always) ||
                        (IS.Instance.Fury.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((IS.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.Always && G.PummelOnCooldown && G.DisruptingShoutOnCooldown));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((IS.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((IS.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((IS.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier6AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier6AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier6AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool VigilanceUsage
        {
            get
            {
                return ((IS.Instance.General.Vigilance == Enum.VigilanceTrigger.OnPartyMember) ||
                        (IS.Instance.General.Vigilance == Enum.VigilanceTrigger.OnTank));
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
                return (G.BloodbathAura || !G.BloodbathTalent || G.ReadinessAura);
            }
        }

        internal static bool TrinketProc
        {
            get
            {
                return Global.DeterminationAura || Global.OutrageAura;
            }
        }
        #endregion
    }
}
