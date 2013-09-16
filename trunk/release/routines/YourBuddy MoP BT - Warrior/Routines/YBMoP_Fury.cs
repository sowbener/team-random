using CommonBehaviors.Actions;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using YBMoP_BT_Warrior.Core;
using YBMoP_BT_Warrior.Helpers;
using YBMoP_BT_Warrior.Managers;
using G = YBMoP_BT_Warrior.Routines.YBGlobal;
using I = YBMoP_BT_Warrior.Core.YBItem;
using Lua = YBMoP_BT_Warrior.Helpers.YBLua;
using S = YBMoP_BT_Warrior.Core.YBSpell;
using SF = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsF;
using SG = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsG;
using U = YBMoP_BT_Warrior.Core.YBUnit;

namespace YBMoP_BT_Warrior.Routines
{
    class YBFury
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeFury
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.CheckTreePerformance, YBLogger.TreePerformance("InitializeFury")),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.CheckAdvancedLogging, YBLogger.AdvancedLogging),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => SG.Instance.ModeChoice == YBEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SF.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                        new Decorator(ret => SF.Instance.CheckInterrupts && U.CanInterrupt, FuryInterrupts()),
                                        FuryUtility(),
                                        I.FuryUseItems(),
                                        FuryOffensive(),
                                        new Decorator(ret => SF.Instance.CheckAoE && U.AttackableMeleeUnitsCount >= 3, FuryMt()),
                                        new Decorator(ret => !YBItem.WieldsTwoHandedWeapons, FurySt1H()),
                                        new Decorator(ret => YBItem.WieldsTwoHandedWeapons, FurySt2H()))),
                        new Decorator(ret => SG.Instance.ModeChoice == YBEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SF.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                        new Decorator(ret => SF.Instance.CheckInterrupts && U.CanInterrupt, FuryInterrupts()),
                                        FuryUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.FuryUseItems(),
                                                        FuryOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && SF.Instance.CheckAoE && U.AttackableMeleeUnitsCount >= 3, FuryMt()),
                                        new Decorator(ret => !YBItem.WieldsTwoHandedWeapons, FurySt1H()),
                                        new Decorator(ret => YBItem.WieldsTwoHandedWeapons, FurySt2H()))));
            }
        }
        #endregion

        #region Rotations
        internal static Composite FurySt1H()
        {
            return new PrioritySelector(
                // SimulationCraft 520-8 (r16290) - 1H Fury - Slightly adjusted.
                new Decorator(ret => G.AlmostDead,
                        new PrioritySelector(
                                S.Cast("Execute"),
                                S.Cast("Heroic Strike", ret => G.DumpAllRage)
                                )),
                S.Cast("Heroic Strike", ret => Lua.PlayerPower >= 110 || (G.NonExecuteCheck && G.ColossusSmashAura && Lua.PlayerPower >= 40)),
                S.Cast("Raging Blow", ret => G.RagingBlowStacks && G.ColossusSmashAura && G.NonExecuteCheck),
                S.Cast("Bloodthirst"),
                S.Cast("Wild Strike", ret => G.BloodsurgeAura && G.ExecuteCheck && G.BTCD <= 1),
                S.Cast("Dragon Roar", ret => G.DRTalent && (G.BloodbathAura || G.AvatarAura || G.SBTalent) && !G.ColossusSmashAura && (
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Colossus Smash"),
                S.Cast("Execute", ret => G.EnrageAura || G.ColossusSmashAura || G.RecklessnessAura || Lua.PlayerPower >= 90 || G.ExecuteCheck),
                S.Cast("Storm Bolt", ret => G.SBTalent && (
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Raging Blow", ret => G.RagingBlowStacks || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3 || (G.BTCD >= 1 && G.FadingRB3S)))),
                S.Cast("Wild Strike", ret => G.BloodsurgeAura),
                S.Cast("Shockwave", ret => G.SWTalent && Me.IsSafelyFacing(Me.CurrentTarget) && (
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Heroic Throw", ret => SF.Instance.CheckHeroicThrow && !G.ColossusSmashAura),
                S.Cast("Battle Shout", ret => Lua.PlayerPower < 70 && !G.ColossusSmashAura && SF.Instance.ShoutSelector == YBEnum.Shouts.Battle),
                S.Cast("Commanding Shout", ret => Lua.PlayerPower < 70 && !G.ColossusSmashAura && SF.Instance.ShoutSelector == YBEnum.Shouts.Commanding),
                S.Cast("Wild Strike", ret => G.ColossusSmashAura && G.NonExecuteCheck),
                S.Cast("Impending Victory", ret => !G.IVCDOC && G.IVTalent && G.NonExecuteCheck && ((SF.Instance.CheckRotImpVic) || (SF.Instance.CheckImpVic && Me.HealthPercent <= SF.Instance.NumImpVic))),
                S.Cast("Wild Strike", ret => G.CSCD >= 2 && Lua.PlayerPower >= 80 && G.NonExecuteCheck),
                S.Cast("Battle Shout", ret => Lua.PlayerPower < 70 && SF.Instance.ShoutSelector == YBEnum.Shouts.Battle),
                S.Cast("Commanding Shout", ret => Lua.PlayerPower < 70 && SF.Instance.ShoutSelector == YBEnum.Shouts.Commanding)
                );
        }

        internal static Composite FurySt2H()
        {
            return new PrioritySelector(
                // SimulationCraft 520-8 (r16290) - 1H Fury - Slightly adjusted.
                new Decorator(ret => G.AlmostDead,
                        new PrioritySelector(
                                S.Cast("Execute"),
                                S.Cast("Heroic Strike", ret => G.DumpAllRage)
                                )),
                S.Cast("Heroic Strike", ret => Lua.PlayerPower >= 110 || (G.NonExecuteCheck && G.ColossusSmashAura && Lua.PlayerPower >= 40)),
                S.Cast("Raging Blow", ret => G.RagingBlowStacks && G.ColossusSmashAura && G.NonExecuteCheck),
                S.Cast("Bloodthirst"),
                S.Cast("Wild Strike", ret => G.BloodsurgeAura && G.NonExecuteCheck && G.BTCD <= 1),
                S.Cast("Dragon Roar", ret => G.DRTalent && (G.BloodbathAura || G.AvatarAura || G.SBTalent) && !G.ColossusSmashAura && (
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Colossus Smash"),
                S.Cast("Execute", ret => G.EnrageAura || G.ColossusSmashAura || G.RecklessnessAura || Lua.PlayerPower >= 90 || G.ExecuteCheck),
                S.Cast("Storm Bolt", ret => G.SBTalent && (
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Raging Blow", ret => G.RagingBlowStacks || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3 || (G.BTCD >= 1 && G.FadingRB3S)))),
                S.Cast("Wild Strike", ret => G.BloodsurgeAura),
                S.Cast("Shockwave", ret => G.SWTalent && Me.IsSafelyFacing(Me.CurrentTarget) && (
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Heroic Throw", ret => SF.Instance.CheckHeroicThrow && !G.ColossusSmashAura),
                S.Cast("Battle Shout", ret => Lua.PlayerPower < 70 && !G.ColossusSmashAura && SF.Instance.ShoutSelector == YBEnum.Shouts.Battle),
                S.Cast("Commanding Shout", ret => Lua.PlayerPower < 70 && !G.ColossusSmashAura && SF.Instance.ShoutSelector == YBEnum.Shouts.Commanding),
                S.Cast("Whirlwind", ret => G.ColossusSmashAura && G.NonExecuteCheck),
                S.Cast("Impending Victory", ret => !G.IVCDOC && G.IVTalent && G.NonExecuteCheck && ((SF.Instance.CheckRotImpVic) || (SF.Instance.CheckImpVic && Me.HealthPercent <= SF.Instance.NumImpVic))),
                S.Cast("Whirlwind", ret => G.CSCD >= 2 && Lua.PlayerPower >= 80 && G.NonExecuteCheck),
                S.Cast("Battle Shout", ret => Lua.PlayerPower < 70 && SF.Instance.ShoutSelector == YBEnum.Shouts.Battle),
                S.Cast("Commanding Shout", ret => Lua.PlayerPower < 70 && SF.Instance.ShoutSelector == YBEnum.Shouts.Commanding)
                );
        }


        internal static Composite FuryMt()
        {
            return new PrioritySelector(
                S.Cast("Dragon Roar", ret => G.DRTalent && (
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Whirlwind", ret => (!G.MeatCleaverAuraS3 || U.AttackableMeleeUnitsCount >= 6)),
                S.Cast("Raging Blow", ret =>
                    (G.MeatCleaverAuraS3 && U.AttackableMeleeUnitsCount >= 4) ||
                    (G.MeatCleaverAuraS2 && U.AttackableMeleeUnitsCount == 3) ||
                    (G.MeatCleaverAuraS1 && U.AttackableMeleeUnitsCount == 2)
                    ),
                S.Cast("Shockwave", ret => G.SWTalent && Me.IsSafelyFacing(Me.CurrentTarget) && U.AttackableMeleeUnitsCount >= 3 && (
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Bladestorm", ret => G.BSTalent && (
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier4Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Cleave", ret => (G.URGlyph && Lua.PlayerPower >= 110) || (G.NonExecuteCheck && !G.URGlyph && Lua.PlayerPower >= 90)),
                S.Cast("Execute", ret => G.ExecuteCheck && (G.EnrageAura || G.ColossusSmashAura || G.RecklessnessAura || Lua.PlayerPower >= 90)),
                S.Cast("Colossus Smash"),
                S.Cast("Bloodthirst")
                );
        }

        internal static Composite FuryDefensive()
        {
            return new PrioritySelector(
                S.Cast("Die by the Sword", ret => SF.Instance.CheckDiebytheSword && Me.HealthPercent <= SF.Instance.NumDiebytheSword),
                S.Cast("Enraged Regeneration", ret => SF.Instance.CheckEnragedRegen && Me.HealthPercent <= SF.Instance.NumEnragedRegen),
                S.Cast("Lifeblood", ret => SF.Instance.CheckLifeblood && Me.HealthPercent <= SF.Instance.NumLifeblood),
                S.Cast("Impending Victory", ret => !G.IVCDOC && G.IVTalent && SF.Instance.CheckImpVic && Me.HealthPercent <= SF.Instance.NumImpVic),
                S.Cast("Victory Rush", ret => !G.VRCDOC && !G.IVTalent && G.VictoriousAura && SF.Instance.CheckImpVic && Me.HealthPercent <= SF.Instance.NumImpVic),
                I.FuryUseHealthStone()
                );
        }

        internal static Composite FuryOffensive()
        {
            return new PrioritySelector(
                S.Cast("Bloodbath", ret => G.BBTalent && (G.CSCD < 2 || G.ColossusSmashAuraT) && (
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Recklessness", ret => (G.ColossusSmashAuraT || G.CSCD <= 4) && ((G.SBTalent) || (G.AVTalent && (G.AvatarAura || G.AVCD <= 3)) || (G.BBTalent && (G.BloodbathAura || G.BBCD <= 3))) && (
                    (SF.Instance.Recklessness == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Recklessness == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Recklessness == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Avatar", ret => G.AVTalent && (G.RecklessnessAura || G.RCCD >= 180) && (
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Tier6Abilities == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Skull Banner", ret => !G.SkullBannerAura && G.RecklessnessAura && (
                    (SF.Instance.SkullBanner == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.SkullBanner == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.SkullBanner == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Berserker Rage", ret => (!G.EnrageAura || (G.RagingBlowStacks && G.NonExecuteCheck) || (G.RecklessnessAuraT && !G.RagingBlowAura)) && (
                    (SF.Instance.BerserkerRage == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.BerserkerRage == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.BerserkerRage == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.Always)
                    )),
                S.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.ClassRacials == YBEnum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite FuryUtility()
        {
            return new PrioritySelector(
                S.Cast("Intimidating Shout", ret => G.ISGlyph && !U.IsTargetBoss),
                S.Cast("Rallying Cry", ret => SF.Instance.CheckRallyingCry && Me.HealthPercent <= SF.Instance.NumRallyingCry && !G.RallyingCryAura),
                S.Cast("Shattering Throw", ret => SF.Instance.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3 || G.SBCD <= 3)),
                new Decorator(ret => HotKeyManager.IsSpecialKey, new PrioritySelector(S.Cast("Shattering Throw", on => Me.CurrentTarget, ret => true))),
                S.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SG.Instance.DemoBannerChoice == Keys.None && SF.Instance.CheckDemoBanner && Me.HealthPercent <= SF.Instance.NumDemoBanner && U.IsDoNotUseOnTgt)
                );
        }

        internal static Composite FuryInterrupts()
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
