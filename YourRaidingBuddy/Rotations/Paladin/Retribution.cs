using CommonBehaviors.Actions;
using YourRaidingBuddy.Core;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using SG = YourRaidingBuddy.Interfaces.Settings.InternalSettings;
using System.Windows.Forms;
using Styx.CommonBot;
using Logger = YourRaidingBuddy.Core.Utilities.Logger;
using LoggerP = YourRaidingBuddy.Core.Utilities.PerformanceLogger;
using YourRaidingBuddy.Core.Managers;
using G = YourRaidingBuddy.Rotations.Global;
using SH = YourRaidingBuddy.Interfaces.Settings.SettingsH;
using YourRaidingBuddy.Core.Helpers;
using Lua = YourRaidingBuddy.Core.Helpers.LuaClass;
using YourRaidingBuddy.Interfaces.Settings;
using U = YourRaidingBuddy.Core.Unit;
using KP = YourRaidingBuddy.Core.Managers.HotKeyManager;

namespace YourRaidingBuddy.Rotations.Paladin
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
                        new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Fist of Justice", ret => RetributionSettings.EnableFistofJustice))),
                        new Action(r =>
                        {
                            LHLoc = LightsHammerLocation;
                            return RunStatus.Failure;
                        }),
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
        internal static double SelflessHealerStack { get { return Spell.GetAuraStack(Me, 114250); } }
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
            Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc != WoWPoint.Empty && !RetributionSettings.UseLightsHammerHotkey && InquisitionUp && (AnicentPowerDown || AnicentPowerStack == 12)),
            Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc != WoWPoint.Empty && RetributionSettings.UseLightsHammerHotkey && KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice)),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc && Lua.HolyPower == 5),
            Spell.Cast("Templar's Verdict", ret => Lua.HolyPower == SG.Instance.Retribution.TVHolyPower || (HolyAvengerUp && Lua.HolyPower >= 3)),
            Spell.Cast("Templar's Verdict", ret => DivinePurposeProc && DivinePurposeTime < 4),
            Spell.Cast("Hammer of Wrath"),
            Spell.Cast("Divine Storm", ret => DivineCrusaderProc && AvengingWrathUp),
            Spell.Cast("Templar's Verdict", ret => AvengingWrathUp),
            Spell.Cast("Crusader Strike"),
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
            Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc != WoWPoint.Empty && !RetributionSettings.UseLightsHammerHotkey && InquisitionUp && (AnicentPowerDown || AnicentPowerStack == 12)),
            Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc != WoWPoint.Empty && RetributionSettings.UseLightsHammerHotkey && KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice)),
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
            return 
                new PrioritySelector(
                    new Decorator (ret=> U.DefaultCheck,
                        new PrioritySelector(
                       //[19:59:40] dxxx: Tier 5 Talent Detecion if Divine Purpose is seleted to use Avenging Wrath 10 seconds after Guardian of Ancient Kings(Or 20 seconds left)
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
                                )))),
                    Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                        (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                        (SG.Instance.Retribution.ClassRacials == Enum.AbilityTrigger.Always)
                        ))
                    );
        }

        internal static Composite RetributionUtility()
        {
            return new PrioritySelector(
                Spell.Cast("Flash of Light", ret => TalentManager.IsSelected(7) && (SelflessHealerStack==3) && Me.HealthPercent<85),
                Spell.Cast("Eternal Flame", ret => TalentManager.IsSelected(8) && (EternalFlameSetting < 2 && (Lua.HolyPower >= 3 || DivinePurposeProc))),
                Spell.Cast("Word of Glory", ret => !TalentManager.IsSelected(8) && RetributionSettings.EnableWordofGlory && (Lua.HolyPower >= 3 || DivinePurposeProc)),
                Spell.Cast("Sacred Shield", ret => TalentManager.IsSelected(9) && !Me.Auras.ContainsKey("Sacred Shield")));
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

        #region Booleans & other stuff
        internal static double EternalFlameSetting { get { return Spell.GetAuraTimeLeft(114163); } }
        /* The logic to get the Location */
        static WoWPoint LightsHammerLocation
        {
            get
            {
                WoWPoint _loc = WoWPoint.Empty;
                WoWUnit s = null;
                switch (RetributionSettings.LightHammerLocation)
                {
                    case Enum.TriggerTarget.FocusTarget:
                        //self-explaining
                        if (Me.FocusedUnit != null && Me.FocusedUnit.Distance <= 30) _loc = Me.FocusedUnit.Location;
                        break;
                    case Enum.TriggerTarget.OnMe:
                        //self-explaining we wanne heal ourself or we need aggro on the units
                        if (Me.HealthPercent < 85 || Unit.AttackableMeleeUnitsCount > 3) _loc = Me.Location;
                        break;
                    case Enum.TriggerTarget.OnRaid:
                        //we need heal for our group, so only use unit-list with raidmembers that could need healing
                        s = Clusters.GetBestUnitForCluster(Unit.NearbyRaidMembers(Me.Location, 40f).Where(q => q.HealthPercent < 95), Enum.ClusterType.Radius, 8f);
                        if (s != null) _loc = s.Location;
                        //Cast on my raidgroup - biggest cluster of players
                        break;
                    case Enum.TriggerTarget.OnRaidMember:
                        //same as OnRaid
                        s = Clusters.GetBestUnitForCluster(Unit.NearbyRaidMembers(Me.Location, 40f).Where(q => q.HealthPercent < 95), Enum.ClusterType.Radius, 8f);
                        if (s != null) _loc = s.Location;
                        break;
                    case Enum.TriggerTarget.OnTrash:
                        //biggest pack of mobs
                        s = Clusters.GetBestUnitForCluster(Unit.AttackableUnits.Where(u => u.Combat), Enum.ClusterType.Radius, 8f);
                        if (s != null) _loc = s.Location;
                        break;
                    case Enum.TriggerTarget.OnTarget:
                        //on the feets of my target
                        if (Me.CurrentTarget != null && Me.CurrentTarget.IsAlive) _loc = Me.CurrentTarget.Location;
                        break;
                    case Enum.TriggerTarget.Automatic:
                        //do i need heal
                        if (Me.HealthPercent < 85 || Unit.AttackableMeleeUnitsCount > 3) _loc = Me.Location;
                        //does someone else need heal
                        if (_loc == WoWPoint.Empty)
                        {
                            s = Clusters.GetBestUnitForCluster(Unit.NearbyRaidMembers(Me.Location, 40f).Where(q => q.HealthPercent < 95), Enum.ClusterType.Radius, 8f);
                            if (s != null) _loc = s.Location;
                        }
                        //do we have trash
                        if (_loc == WoWPoint.Empty)
                        {
                            s = Clusters.GetBestUnitForCluster(Unit.AttackableUnits.Where(u => u.Combat), Enum.ClusterType.Radius, 8f);
                            if (s != null) _loc = s.Location;
                        }
                        //do we have trash
                        if (_loc == WoWPoint.Empty)
                        {
                            if (Me.CurrentTarget != null && Me.CurrentTarget.IsAlive) _loc = Me.CurrentTarget.Location;
                        }
                        break;
                    default:
                        _loc = WoWPoint.Empty;
                        break;
                }
                return _loc;
            }
        }
        /* The cached value of the Location, we only want to calc it once per traverse*/
        static WoWPoint LHLoc = WoWPoint.Empty;
        #endregion Booleans & other stuff

    }
}
