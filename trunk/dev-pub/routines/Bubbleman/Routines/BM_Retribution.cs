using CommonBehaviors.Actions;
using Bubbleman.Core;
using Bubbleman.Helpers;
using Bubbleman.Managers;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = Bubbleman.Routines.BMGlobal;
using I = Bubbleman.Core.BMItem;
using Lua = Bubbleman.Helpers.BMLua;
using T = Bubbleman.Managers.BMTalentManager;
using SG = Bubbleman.Interfaces.Settings.BMSettings;
using SH = Bubbleman.Interfaces.Settings.BMSettingsH;
using Spell = Bubbleman.Core.BMSpell;
using U = Bubbleman.Core.BMUnit;
using Styx.CommonBot;
using Bubbleman.Interfaces.Settings;

namespace Bubbleman.Routines
{
    class BMRetribution
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeRetribution
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, BMLogger.TreePerformance("InitializeRetribution")),
                        new Decorator(ret => (BMHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckABMancedLogging, BMLogger.ABMancedLogging),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BMEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Retribution.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, RetributionDefensive()),
                                        new Decorator(ret => SG.Instance.Retribution.CheckInterrupts, RetributionInterrupts()),
                                        RetributionUtility(),
                                        I.RetributionUseItems(),
                                        RetributionOffensive(),
                                        new Decorator(ret => SG.Instance.Retribution.CheckAoE && U.NearbyAttackableUnitsCount > 3, RetributionMt()),
                                            RetributionSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BMEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Retribution.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, RetributionDefensive()),
                                        new Decorator(ret => SG.Instance.Retribution.CheckInterrupts, RetributionInterrupts()),
                                        RetributionUtility(),
                                        new Decorator(ret => BMHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.RetributionUseItems(),
                                                        RetributionOffensive())),
                                        new Decorator(ret => BMHotKeyManager.IsAoe, RetributionMt()),
                                        RetributionSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool TalentChiBrewEnabled { get { return BMTalentManager.HasTalent(9); } }

        internal static bool InquisitionUp { get { return Me.HasAura(84963); } }
        internal static double InquisitionTimer { get { return Spell.GetAuraTimeLeft(84963); } }
        internal static bool AnicentPowerDown { get { return !Me.HasAura(86700); } }
        internal static double AnicentPowerStack { get { return Spell.GetAuraStack(Me, 86700); } }
        internal static bool DivineCrusaderProc { get {  return  Me.HasAura(144595);} }
        internal static bool DivinePurposeProc { get {  return Me.HasAura(86172);} }
        internal static double DivinePurposeTime { get { return Spell.GetAuraTimeLeft(86172); } }
        internal static bool HolyAvengerUp { get { return Me.HasAura(105809); } }
        internal static bool AvengingWrathUp { get { return Me.HasAura(31884); } }
        internal static bool CrusaderStrikeCooldownRemains { get { return CooldownWatcher.OnCooldown(35395); } }
        internal static bool CrusaderStrikeUnder0 { get { return CooldownWatcher.GetSpellCooldownTimeLeft(35395) <= 200; } }
        internal static bool HammerofWrathCooldownRemains { get { return CooldownWatcher.OnCooldown(24275); } }
        internal static bool HammerofWrathUnder0 { get { return CooldownWatcher.GetSpellCooldownTimeLeft(24275) <= 200; } }
        internal static bool JudgmentCooldownRemains { get { return CooldownWatcher.OnCooldown(20271); } }
        internal static bool JudgmentUnder0 { get { return CooldownWatcher.GetSpellCooldownTimeLeft(20271) <= 200; } }
        internal static bool ExorcismCooldownRemains { get { return CooldownWatcher.OnCooldown(879); } }
        internal static bool ExorcismUnder0 { get { return CooldownWatcher.GetSpellCooldownTimeLeft(879) <= 200; } }



        private static BMSettingsRE Retribution { get { return SG.Instance.Retribution; } }

       #endregion

        #region Rotations
        internal static Composite RetributionSt()
        {
            return new PrioritySelector(
            Spell.Cast("Execution Sentence", ret => InquisitionUp && (AnicentPowerDown || AnicentPowerStack == 12)),
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
               );
        }



        internal static Composite RetributionDefensive()
        {
            return new PrioritySelector(
                I.RetributionUseHealthStone()
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
                Spell.Cast("Inquisition", ret => (!InquisitionUp || InquisitionTimer <= 2) && (Lua.HolyPower >= 3 || DivinePurposeProc)),
                Spell.Cast("Avenging Wrath", ret => InquisitionUp),
                Spell.Cast("Guardian of Ancient Kings", ret => InquisitionUp),
                Spell.Cast("Holy Avenger", ret => InquisitionUp && Lua.HolyPower <= 2),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
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
                    Spell.Cast("xxx", ret => (SG.Instance.General.InterruptList == BMEnum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == BMEnum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))

                  );
            }
        }
        #endregion


        #region Booleans


        #endregion Booleans

    }
}
