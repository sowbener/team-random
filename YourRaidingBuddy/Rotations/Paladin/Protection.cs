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
using KP = YourRaidingBuddy.Core.Managers.HotKeyManager;

namespace YourRaidingBuddy.Rotations.Paladin
{
    class Protection
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeProtection
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        G.InitializeOnKeyActionsHandofSalvation(),
                        G.InitializeOnKeyActionsLayonHands(),
                        new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Fist of Justice", ret => ProtectionSettings.EnableFistofJustice))),
                        new Action(r =>
                        {
                            LHLoc = LightsHammerLocation;
                            return RunStatus.Failure;
                        }),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Protection.CheckAutoAttack, Lua.StartAutoAttack), 
                                            ProtectionDefensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckInterrupts, ProtectionInterrupts()),
                                      //  new Decorator(ret => SG.Instance.Protection.CheckDPS, ProtectioonDPS()),
                                        new Decorator(ret => SG.Instance.Protection.CheckSpecial, ProtectionSpecial()),
                                        new Decorator(ret => SG.Instance.Protection.HandsEnable, ProtectionHands()),
                                        ProtectionUtility(),
                                        new Action(ret => { Item.UseProtectionItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                        ProtectionOffensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount > 2, ProtectionMt()),
                                            ProtectionSt())),
                       new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Protection.CheckAutoAttack, Lua.StartAutoAttack),
                                            ProtectionDefensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckInterrupts, ProtectionInterrupts()),
                                        ProtectionUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        new Action(ret => { Item.UseProtectionItems(); return RunStatus.Failure; }),
                                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                                        ProtectionOffensive())),
                                        new Decorator(ret => SG.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount > 2, ProtectionMt()),
                                            ProtectionSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Protection.CheckAutoAttack, Lua.StartAutoAttack),
                                        ProtectionDefensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckInterrupts, ProtectionInterrupts()),
                                        ProtectionUtility(),
                                      //  new Decorator(ret => HotKeyManager.ElusiveBrew, new PrioritySelector(Spell.Cast("Elusive Brew"))),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        new Action(ret => { Item.UseProtectionItems(); return RunStatus.Failure; }),
                                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                                        ProtectionOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, ProtectionMt()),
                                        ProtectionSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool SanctifiedWrathTalent { get { return TalentManager.IsSelected(14); } }

        internal static double EternalFlameSetting { get { return Spell.GetAuraTimeLeft(114163); } }

        internal static double BastionofGloryCount { get { return Spell.GetAuraStack(Me, 114637); } }

        internal static bool CrusaderStrikeCooldownRemains { get { return CooldownTracker.SpellOnCooldown(35395); } }
        internal static bool CrusaderStrikeUnder0 { get { return CooldownTracker.GetSpellCooldown(35395).TotalMilliseconds <= 400; } }

        internal static bool DivinePurposeProc { get { return Me.HasAura(86172); } }
        internal static bool AvengingWrathBuff { get { return Me.HasAura(31884); } }
        internal static bool GrandCrusaderProc { get { return Me.HasAura(85416); } }
        private static YbSettingsPR ProtectionSettings { get { return SG.Instance.Protection; } }




        #endregion

        #region Rotations

        internal static Composite ProtectionSt()
        {
            return new PrioritySelector(
                
                Spell.Cast("Shield of the Righteous", ret => Lua.HolyPower >= 5 || DivinePurposeProc || (Lua.HolyPower >= 3 && (G.DevastatingAbilities.Contains(Me.CurrentTarget.CurrentCastorChannelId())))),
                Spell.Cast("Judgment", ret => SanctifiedWrathTalent && AvengingWrathBuff),
                Spell.Cast("Crusader Strike"),
                new Decorator(ret => CrusaderStrikeCooldownRemains && CrusaderStrikeUnder0, new ActionAlwaysSucceed()),
                Spell.Cast("Judgment"),
                Spell.Cast("Avenger's Shield"),
              Spell.Cast("Hammer of Wrath"),
              Spell.Cast("Execution Sentence"),
              Spell.Cast("Holy Prism"),
              Spell.Cast("Holy Wrath"),
              Spell.Cast("Consecration", ret => !TalentManager.HasGlyph("Consecration")),
              Spell.CastOnGround("Consecration", ret => Me.CurrentTarget != null ? Me.CurrentTarget.Location : Me.Location, ret => TalentManager.HasGlyph("Consecration")),
              Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc!=WoWPoint.Empty && !ProtectionSettings.UseLightsHammerHotkey),
              Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc!=WoWPoint.Empty && ProtectionSettings.UseLightsHammerHotkey && KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice))
              );

        }

        internal static Composite ProtectionMt()
        {
            return new PrioritySelector(
                Spell.Cast("Shield of the Righteous", ret => Lua.HolyPower >= 5 || DivinePurposeProc || (Lua.HolyPower >= 3 && (G.DevastatingAbilities.Contains(Me.CurrentTarget.CurrentCastorChannelId())))),
                Spell.Cast("Judgment", ret => SanctifiedWrathTalent && AvengingWrathBuff),
                Spell.Cast("Hammer of the Righteous"),
                Spell.Cast("Judgment"),
                Spell.Cast("Avenger's Shield", ret => GrandCrusaderProc),
                Spell.Cast("Consecration"),
                Spell.Cast("Avenger's Shield"),
              Spell.Cast("Hammer of Wrath"),
              Spell.Cast("Execution Sentence"),
              Spell.Cast("Holy Prism"),
              Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc!=WoWPoint.Empty && !ProtectionSettings.UseLightsHammerHotkey),
              Spell.CastOnGround("Light's Hammer", ret => LHLoc, ret => LHLoc!=WoWPoint.Empty && ProtectionSettings.UseLightsHammerHotkey && KP.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice)),
              Spell.Cast("Holy Wrath"));
        }

        internal static Composite ProtectionHands()
        {
            return new PrioritySelector(
                Spell.Cast("Hand of Purity", On => Me, ret => TalentManager.IsSelected(10) && G.Hands()));
            
        }

        internal static Composite ProtectionSpecial()
        {
            {
                return new PrioritySelector(
        Spell.Cast("Reckoning", ret => G.IsReconingCastDesiredP()));
                        
            }
        }

        internal static Composite ProtectionUtility()
        {
            return new PrioritySelector(
                Spell.Cast("Eternal Flame", ret => TalentManager.IsSelected(8) && (EternalFlameSetting < 2 && BastionofGloryCount > 2 && (Lua.HolyPower >= 3 || DivinePurposeProc))),
                Spell.Cast("Word of Glory", ret => !TalentManager.IsSelected(8) && ProtectionSettings.EnableWordofGlory && (BastionofGloryCount > 2 && (Lua.HolyPower >= 3 || DivinePurposeProc))),
                Spell.Cast("Sacred Shield", ret=> TalentManager.IsSelected(9) && !Me.Auras.ContainsKey("Sacred Shield")));
        }


        private static Composite HandleHealingCooldowns()
        {
            return new PrioritySelector(
                   Item.ProtectionUseHealthStone()
                   );
        }


        internal static Composite ProtectionDefensive()
        {
         return new PrioritySelector(
          Spell.Cast("Divine Protection", On => Me, ret => SG.Instance.Protection.DivineProtectionEnable && Me.HealthPercent <= SG.Instance.Protection.DivineProtectionHP),
          Spell.Cast("Guardian of Ancient Kings", ret => SG.Instance.Protection.GuardianofAncientKingsEnable && Me.HealthPercent <= SG.Instance.Protection.GuardianofAncientKingsHP),
          Spell.Cast("Ardent Defender", ret => SG.Instance.Protection.ArdentDefenderEnable && Me.HealthPercent <= SG.Instance.Protection.ArdentDefenderHP),
          Spell.Cast("Lay on Hands", ret => SG.Instance.Protection.LayonHandsEnable && Me.HealthPercent <= SG.Instance.Protection.LayonHandsHP)
                );
        }


        internal static Composite ProtectionOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Avenging Wrath", ret => (
                    (SG.Instance.Protection.AvengingWrath == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Protection.AvengingWrath == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Protection.AvengingWrath == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }


        internal static Composite ProtectionInterrupts()
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

        #region internal stuff
        /* The logic to get the Location */
        static WoWPoint LightsHammerLocation
        {
            get
            {
                WoWPoint _loc = WoWPoint.Empty;
                WoWUnit s = null;
                switch (SG.Instance.Protection.LightHammerLocation)
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
                        s = Clusters.GetBestUnitForCluster(Unit.NearbyRaidMembers(Me.Location,40f).Where(q=>q.HealthPercent<95), Enum.ClusterType.Radius, 8f);
                        if (s != null) _loc = s.Location;
                        //Cast on my raidgroup - biggest cluster of players
                        break;
                    case Enum.TriggerTarget.OnRaidMember:
                        //same as OnRaid
                        s = Clusters.GetBestUnitForCluster(Unit.NearbyRaidMembers(Me.Location,40f).Where(q=>q.HealthPercent<95), Enum.ClusterType.Radius, 8f);
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
        #endregion internal stuff

        #region Booleans
        //thanks PR
        internal static bool UnitIsFleeing { get { return Me.CurrentTarget != null && ((Me.CurrentTarget.IsPlayer || Me.CurrentTarget.Fleeing) && Me.CurrentTarget.MovementInfo.RunSpeed > 3.5); } }
        #endregion Booleans

    }
}
