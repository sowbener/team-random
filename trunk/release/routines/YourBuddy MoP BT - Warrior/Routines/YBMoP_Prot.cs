using CommonBehaviors.Actions;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using YBMoP_BT_Warrior.Helpers;
using YBMoP_BT_Warrior.Managers;
using G = YBMoP_BT_Warrior.Routines.YBGlobal;
using I = YBMoP_BT_Warrior.Core.YBItem;
using Lua = YBMoP_BT_Warrior.Helpers.YBLua;
using S = YBMoP_BT_Warrior.Core.YBSpell;
using SG = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsG;
using SP = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsP;
using U = YBMoP_BT_Warrior.Core.YBUnit;

namespace YBMoP_BT_Warrior.Routines
{
    class YBProt
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeProt
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.CheckTreePerformance, YBLogger.TreePerformance("InitializeProt")),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.CheckAdvancedLogging, YBLogger.AdvancedLogging),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => SG.Instance.ModeChoice == YBEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SP.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        new Decorator(ret => SP.Instance.CheckInterrupts && U.CanInterrupt, ProtInterrupts()),
                                        ProtUtility(),
                                        I.ProtUseItems(),
                                        ProtOffensive(),
                                        new Decorator(ret => SP.Instance.CheckAoE && U.AttackableMeleeUnitsCount >= 2, ProtMt()),
                                        ProtSt())),
                        new Decorator(ret => SG.Instance.ModeChoice == YBEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SP.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        new Decorator(ret => SP.Instance.CheckInterrupts && U.CanInterrupt, ProtInterrupts()),
                                        ProtUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.ProtUseItems(),
                                                        ProtOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && SP.Instance.CheckAoE && U.AttackableMeleeUnitsCount >= 2, ProtMt()),
                                        ProtSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite ProtSt()
        {
            return new PrioritySelector(
                S.Cast("Execute", ret => G.ExecuteCheck && YBLua.PlayerPower >= 100 && !G.TargettingMe),
                S.Cast("Heroic Strike", ret => G.UltimatumAura || (YBLua.PlayerPower >= 100 && !G.TargettingMe)),
                S.Cast("Shield Slam", ret => Lua.PlayerPower <= 90),
                S.Cast("Revenge", ret => Lua.PlayerPower <= 100),
                S.Cast("Battle Shout", ret => SP.Instance.ShoutSelector == YBEnum.Shouts.Battle && Lua.PlayerPower <= 100),
                S.Cast("Commanding Shout", ret => SP.Instance.ShoutSelector == YBEnum.Shouts.Commanding && Lua.PlayerPower <= 100),
                S.Cast("Storm Bolt", ret => G.SBTalent&& (
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Dragon Roar", ret => G.DRTalent&& (
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Thunder Clap", ret => !G.WeakenedBlowsAura),
                S.Cast("Devastate")
                );
        }

        internal static Composite ProtMt()
        {
            return new PrioritySelector(
                S.Cast("Cleave", ret => G.UltimatumAura || (YBLua.PlayerPower >= 100 && !G.TargettingMe)),
                S.Cast("Thunder Clap"),
                S.Cast("Dragon Roar", ret => G.DRTalent && (
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Shockwave", ret => G.SWTalent && Me.IsSafelyFacing(Me.CurrentTarget) && U.AttackableMeleeUnitsCount >= 3 && (
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Bladestorm", ret => G.BSTalent && (
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Revenge", ret => Lua.PlayerPower <= 100),
                S.Cast("Shield Slam", ret => Lua.PlayerPower <= 90),
                S.Cast("Battle Shout", ret => SP.Instance.ShoutSelector == YBEnum.Shouts.Battle && Lua.PlayerPower <= 100),
                S.Cast("Commanding Shout", ret => SP.Instance.ShoutSelector == YBEnum.Shouts.Commanding && Lua.PlayerPower <= 100),
                S.Cast("Devastate"),
                S.Cast("Intimidating Shout", ret => G.ISGlyph && !U.IsTargetBoss)
                );
        }

        internal static Composite ProtDefensive()
        {
            return new PrioritySelector(
                S.Cast("Last Stand", ret => SP.Instance.CheckLastStand && Me.HealthPercent <= SP.Instance.NumLastStand),
                S.Cast("Rallying Cry", ret => SP.Instance.CheckRallyingCry && Me.HealthPercent <= SP.Instance.NumRallyingCry && !G.LastStandAura),
                S.Cast("Shield Block", ret => SP.Instance.CheckShieldBlock && !G.ShieldBarrierAura && U.IsShieldBlockTarget),
                S.Cast("Shield Barrier", ret => SP.Instance.CheckShieldBlock && !G.ShieldBlockAura && YBLua.PlayerPower >= 80 && U.IsShieldBarrierTarget),
                S.Cast("Shield Wall", ret => SP.Instance.CheckShieldWall && Me.HealthPercent <= SP.Instance.NumShieldWall),
                S.Cast("Enraged Regeneration", ret => G.EnrageAura && SP.Instance.CheckEnragedRegen && Me.HealthPercent <= SP.Instance.NumEnragedRegen),
                S.Cast("Impending Victory", ret => !G.IVCDOC && G.IVTalent && (
                    (SP.Instance.VictoryRush == YBEnum.VcTrigger.Always) ||
                    (SP.Instance.VictoryRush == YBEnum.VcTrigger.OnT15Proc)
                    )),
                S.Cast("Victory Rush", ret => !G.VRCDOC && !G.IVTalent && G.VictoriousAura && (
                    (SP.Instance.VictoryRush == YBEnum.VcTrigger.Always) ||
                    (SP.Instance.VictoryRush == YBEnum.VcTrigger.OnT15Proc)
                    )),
                S.Cast("Spell Reflection", ret => SP.Instance.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTarget == Me && Me.CurrentTarget.IsCasting),
                S.Cast("Mass Spell Reflection", ret => SP.Instance.CheckSpellReflect && G.SRCD > 0 && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTarget == Me && Me.CurrentTarget.IsCasting),
                I.ProtUseHealthStone()
                );
        }

        internal static Composite ProtUtility()
        {
            return new PrioritySelector(
                S.Cast("Demoralizing Shout", ret => G.TargettingMe && (
                    (SP.Instance.DemoralizeShout == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.DemoralizeShout == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.DemoralizeShout == YBEnum.AbilityTrigger.Always)
                    )),
                S.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SG.Instance.DemoBannerChoice == Keys.None && SP.Instance.CheckDemoBanner && Me.HealthPercent <= SP.Instance.NumDemoBanner && U.IsDoNotUseOnTgt)
                );
        }

        internal static Composite ProtOffensive()
        {
            return new PrioritySelector(
                S.Cast("Skull Banner", ret => !G.SkullBannerAura && (
                    (SP.Instance.SkullBanner == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.SkullBanner == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.SkullBanner == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Avatar", ret => G.AVTalent && (
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Berserker Rage", ret => !G.EnrageAura && (
                    (SP.Instance.BerserkerRage == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.BerserkerRage == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.BerserkerRage == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Bloodbath", ret => G.BBTalent && (
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Tier6Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Recklessness", ret => (
                    (SP.Instance.Recklessness == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.Recklessness == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Recklessness == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.ClassRacials == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Shattering Throw", ret => (
                    (SP.Instance.ShatteringThrow == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SP.Instance.ShatteringThrow == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.ShatteringThrow == YBEnum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite ProtInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    S.Cast("Disrupting Shout", ret => G.PUCDOC)
                    ),
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    S.Cast("Pummel")
                    ));
        }
        #endregion
    }
}