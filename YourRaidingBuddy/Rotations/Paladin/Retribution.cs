using CommonBehaviors.Actions;
using YourBuddy.Core;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using System.Windows.Forms;
using Styx.CommonBot;
using Logger = YourBuddy.Core.Utilities.Logger;
using LoggerP = YourBuddy.Core.Utilities.PerformanceLogger;
using YourBuddy.Core.Managers;
using G = YourBuddy.Rotations.Global;
using SH = YourBuddy.Interfaces.Settings.SettingsH;
using YourBuddy.Core.Helpers;
using Lua = YourBuddy.Core.Helpers.LuaClass;
using YourBuddy.Interfaces.Settings;
using U = YourBuddy.Core.Unit;

namespace YourBuddy.Rotations.Paladin
{
    class Retribution
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeRetribution
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheckRanged), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Retribution.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, RetributionDefensive()),
                                        new Decorator(ret => SG.Instance.Retribution.CheckInterrupts, RetributionInterrupts()),
                                        RetributionUtility(),
                                        new Action(ret => { Item.UseRetributionItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                        RetributionOffensive(),
                                        new Decorator(ret => SG.Instance.Retribution.CheckAoE && Unit.NearbyAttackableUnitsCount > 3, RetributionMt()),
                                            RetributionSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Retribution.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, RetributionDefensive()),
                                        new Decorator(ret => SG.Instance.Retribution.CheckInterrupts, RetributionInterrupts()),
                                        RetributionUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                        new Action(ret => { Item.UseRetributionItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                                        RetributionOffensive())),
                                        new Decorator(ret => SG.Instance.Retribution.CheckAoE && Unit.NearbyAttackableUnitsCount > 3, RetributionMt()),
                                            RetributionSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Retribution.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, RetributionDefensive()),
                                        new Decorator(ret => SG.Instance.Retribution.CheckInterrupts, RetributionInterrupts()),
                                        RetributionUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                        new Action(ret => { Item.UseRetributionItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                                        RetributionOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, RetributionMt()),
                                        RetributionSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool TalentChiBrewEnabled { get { return TalentManager.IsSelected(9); } }

        internal static bool InquisitionUp { get { return Me.HasAura(84963); } }
        internal static double InquisitionTimer { get { return Spell.GetAuraTimeLeft(84963); } }
        internal static bool AnicentPowerDown { get { return !Me.HasAura(86700); } }
        internal static bool AnicentPowerUp { get { return Me.HasAura(86700); } }
        internal static double AnicentPowerStack { get { return Spell.GetAuraStack(Me, 86700); } }
        internal static bool DivineCrusaderProc { get {  return Me.HasAura(144595);} }
        internal static bool DivinePurposeProc { get {  return Me.HasAura(86172);} }
        internal static double DivinePurposeTime { get { return Spell.GetAuraTimeLeft(86172); } }
        internal static bool HolyAvengerUp { get { return Me.HasAura(105809); } }
        internal static bool AvengingWrathUp { get { return Me.HasAura(31884); } }
        internal static bool CrusaderStrikeCooldownRemains { get { return CooldownTracker.SpellOnCooldown(35395); } }
        internal static bool CrusaderStrikeUnder0 { get { return CooldownTracker.GetSpellCooldown(35395).TotalMilliseconds <= 500; } }
        internal static bool HammeroftheRighteousCooldownRemains { get { return CooldownTracker.SpellOnCooldown(53595); } }
        internal static bool HammeroftheRighteousUnder0 { get { return CooldownTracker.GetSpellCooldown(53595).TotalMilliseconds <= 500; } }
        internal static bool HammerofWrathCooldownRemains { get { return CooldownTracker.SpellOnCooldown(24275); } }
        internal static bool HammerofWrathUnder0 { get { return CooldownTracker.GetSpellCooldown(24275).TotalMilliseconds <= 500; } }
        internal static bool JudgmentCooldownRemains { get { return CooldownTracker.SpellOnCooldown(20271); } }
        internal static bool JudgmentUnder0 { get { return CooldownTracker.GetSpellCooldown(20271).TotalMilliseconds <= 500; } }
        internal static bool ExorcismCooldownRemains { get { return CooldownTracker.SpellOnCooldown(879); } }
        internal static bool ExorcismUnder0 { get { return CooldownTracker.GetSpellCooldown(879).TotalMilliseconds <= 500; } }



        private static YbSettingsRE RetributionSettings { get { return SG.Instance.Retribution; } }

       #endregion

        #region Rotations
        internal static Composite RetributionSt()
        {
            return new PrioritySelector(
           Spell.PreventDoubleCast("Seal of Truth", 0.5, ret => RetributionSettings.UseSealSwapping && !Me.HasAura("Seal of Truth") && Unit.NearbyAttackableUnitsCount <= RetributionSettings.SealofRighteousnessCount),
            Spell.Cast("Inquisition", ret => (!InquisitionUp || InquisitionTimer <= 2) && (Lua.HolyPower >= 3 || DivinePurposeProc)),
            Spell.Cast("Execution Sentence", ret => InquisitionUp && ((CooldownTracker.GetSpellCooldown(86698).TotalMilliseconds > 1000 && AnicentPowerDown) || (AnicentPowerUp && AnicentPowerStack >= 12))),
            Spell.Cast("Lights Hammer", ret => InquisitionUp && (AnicentPowerDown || AnicentPowerStack == 12)),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc && Lua.HolyPower == 5),
            Spell.Cast("Templar's Verdict", ret => Lua.HolyPower == 5 || (HolyAvengerUp && Lua.HolyPower >= 3)),
            Spell.Cast("Templar's Verdict", ret => DivinePurposeProc && DivinePurposeTime < 4),
            Spell.Cast("Hammer of Wrath"),
            new Decorator(ret => HammerofWrathCooldownRemains && HammerofWrathUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc && AvengingWrathUp),
            Spell.Cast("Templar's Verdict", ret => AvengingWrathUp),
            Spell.Cast("Crusader Strike"),
            new Decorator(ret => CrusaderStrikeCooldownRemains && CrusaderStrikeUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Judgment"),
            new Decorator(ret => JudgmentCooldownRemains && JudgmentUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc),
            Spell.Cast("Templar's Verdict", ret => DivinePurposeProc),
            Spell.Cast("Exorcism"),
            new Decorator(ret => ExorcismCooldownRemains && ExorcismUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Holy Prism")
                    );

        }

        internal static Composite RetributionMt()
        {
            return new PrioritySelector(
            Spell.Cast("Inquisition", ret => (!InquisitionUp || InquisitionTimer <= 2) && (Lua.HolyPower >= 3 || DivinePurposeProc)),
            Spell.PreventDoubleCast("Seal of Righteousness", 0.5, ret => RetributionSettings.UseSealSwapping && !Me.HasAura("Seal of Righteousness") && Unit.NearbyAttackableUnitsCount >= RetributionSettings.SealofRighteousnessCount),
            Spell.PreventDoubleCast("Seal of Truth", 0.5, ret => RetributionSettings.UseSealSwapping && !Me.HasAura("Seal of Truth") && Unit.NearbyAttackableUnitsCount <= RetributionSettings.SealofRighteousnessCount),
            Spell.Cast("Execution Sentence", ret => InquisitionUp && ((CooldownTracker.GetSpellCooldown(86698).TotalMilliseconds > 1000 && AnicentPowerDown) || (AnicentPowerUp && AnicentPowerStack >= 12))),
            Spell.Cast("Lights Hammer", ret => InquisitionUp && (AnicentPowerDown || AnicentPowerStack == 12)),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc && Lua.HolyPower == 5),
            Spell.Cast("Divine Storm", ret => Lua.HolyPower == 5 || (HolyAvengerUp && Lua.HolyPower >= 3)),
            Spell.Cast("Divine Storm", ret => DivinePurposeProc && DivinePurposeTime < 4),
            Spell.Cast("Hammer of Wrath"),
            new Decorator(ret => HammerofWrathCooldownRemains && HammerofWrathUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc && AvengingWrathUp),
            Spell.Cast("Divine Storm", ret => AvengingWrathUp),
            Spell.Cast("Hammer of the Righteous"),
            new Decorator(ret => HammeroftheRighteousCooldownRemains && HammeroftheRighteousUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Judgment"),
            new Decorator(ret => JudgmentCooldownRemains && JudgmentUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc),
            Spell.Cast("Divine Storm", ret => DivinePurposeProc),
            Spell.Cast("Exorcism"),
            new Decorator(ret => ExorcismCooldownRemains && ExorcismUnder0, new ActionAlwaysSucceed()),
            Spell.Cast("Holy Prism")
               );
        }



        internal static Composite RetributionDefensive()
        {
            return new PrioritySelector(
                Item.RetributionUseHealthStone()
                );
        }


        internal static Composite RetributionOffensive()
        {
            return new PrioritySelector(
                //actions+=/inquisition,if=(buff.inquisition.down|buff.inquisition.remains<=2)&(holy_power>=3|target.time_to_die<holy_power*20|buff.divine_purpose.react)
                //actions+=/avenging_wrath,if=buff.inquisition.up
                //actions+=/guardian_of_ancient_kings,if=buff.inquisition.up
                //actions+=/holy_avenger,if=talent.holy_avenger.enabled&(buff.inquisition.up&holy_power<=2)
                //actions+=/use_item,name=gauntlets_of_winged_triumph,if=buff.inquisition.up&(buff.ancient_power.down|buff.ancient_power.stack=12)
                Spell.Cast("Avenging Wrath", ret => InquisitionUp && (
                    (SG.Instance.Retribution.AvengingWrath == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.AvengingWrath == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.AvengingWrath == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Guardian of Ancient Kings", ret => InquisitionUp && (
                    (SG.Instance.Retribution.GuardianofAnicentKings == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.GuardianofAnicentKings == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.GuardianofAnicentKings == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Holy Avenger", ret => InquisitionUp && Lua.HolyPower <= 2 && (
                    (SG.Instance.Retribution.HolyAvenger == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.HolyAvenger == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.HolyAvenger == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite RetributionUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite RetributionInterrupts()
        {
            {
                 return new PrioritySelector(
                   new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Rebuke", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))

                  );
            }
        }
        #endregion


        #region Booleans


        #endregion Booleans

    }
}
