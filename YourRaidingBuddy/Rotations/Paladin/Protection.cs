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

namespace YourBuddy.Rotations.Paladin
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
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Protection.CheckAutoAttack, Lua.StartAutoAttack), 
                                            ProtectionDefensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckInterrupts, ProtectionInterrupts()),
                                        ProtectionUtility(),
                                        //I.ProtectionUseItems(),
                                        ProtectionOffensive(),
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
                                                        //I.ProtectionUseItems(),
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
              Spell.Cast("Consecration"));

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
              Spell.Cast("Light's Hammer"),
              Spell.Cast("Holy Wrath"));
        }

        internal static Composite ProtectionUtility()
        {
            return new PrioritySelector(
                Spell.Cast("Eternal Flame", ret => (EternalFlameSetting < 2 && BastionofGloryCount > 2 && (Lua.HolyPower >= 3 || DivinePurposeProc))));
        }


        private static Composite HandleHealingCooldowns()
        {
            return new PrioritySelector(
                   
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


        #region Booleans
        //thanks PR
        internal static bool UnitIsFleeing { get { return Me.CurrentTarget != null && ((Me.CurrentTarget.IsPlayer || Me.CurrentTarget.Fleeing) && Me.CurrentTarget.MovementInfo.RunSpeed > 3.5); } }
        #endregion Booleans

    }
}
