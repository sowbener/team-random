using CommonBehaviors.Actions;
using YourRaidingBuddy.Core;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
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
using U = YourRaidingBuddy.Core.Unit;
using Enum = YourRaidingBuddy.Core.Helpers.Enum;

namespace YourRaidingBuddy.Rotations.Druid
{
    class Boomkin
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }   

        #region Initialize Rotations
        internal static Composite InitializeBoomkin
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheckRanged), new ActionAlwaysSucceed()),
                    //              G.InitializeOnKeyActionsH(),
                    //               new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Binding Shot", ret => TalentManager.IsSelected(4)))),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        BoomkinRebirth(),
                        new Decorator(ret => SG.Instance.Boomkin.EnableFacing && !Me.IsMoving && !Me.IsSafelyFacing(Me.CurrentTarget), new Action(ret => { Me.CurrentTarget.Face(); return RunStatus.Failure; })),
                        new Decorator(a => CastingStarfire && EclipseDirMoon && !CelestialalignmentUp, new Action(delegate { SpellManager.StopCasting(); return RunStatus.Failure; })),
                        new Decorator(a => CastingWrath && EclipseDirSun && !CelestialalignmentUp, new Action(delegate { SpellManager.StopCasting(); return RunStatus.Failure; })),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Boomkin.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BoomkinDefensive()),
                                        new Decorator(ret => SG.Instance.Boomkin.CheckInterrupts, BoomkinInterrupts()),
                                        BoomkinUtility(),
                          new Action(ret => { Item.UseBoomkinItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        BoomkinOffensive(),
                                       new Decorator(ret => Me.CurrentTarget != null && SG.Instance.Boomkin.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Boomkin.AoECount, BoomkinMt()),
                                       BoomkinSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Boomkin.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BoomkinDefensive()),
                                        new Decorator(ret => SG.Instance.Boomkin.CheckInterrupts, BoomkinInterrupts()),
                                        BoomkinUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                        new PrioritySelector(
                             new Action(ret => { Item.UseBoomkinItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        BoomkinOffensive())),
                                       new Decorator(ret => Me.CurrentTarget != null && SG.Instance.Boomkin.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Boomkin.AoECount, BoomkinMt()),
                                         BoomkinSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Boomkin.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BoomkinDefensive()),
                                        new Decorator(ret => SG.Instance.Boomkin.CheckInterrupts, BoomkinInterrupts()),
                                        BoomkinUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                        new PrioritySelector(
                                new Action(ret => { Item.UseBoomkinItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        BoomkinOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, BoomkinMt()),
                                        new Decorator(ret => !HotKeyManager.IsAoe, BoomkinSt()))));
            }
        }



        private static Composite MultiDoT()
        {
            return new PrioritySelector(
                ctx => Unit.NearbyUnfriendlyUnits.Where(u => (u.Combat || Unit.IsDummy(Me.CurrentTarget)) && !u.IsCrowdControlled() && Me.IsSafelyFacing(u)).ToList(),
                Spell.Cast("Sunfire", ret => ((List<WoWUnit>)ret).FirstOrDefault(u => u.HasAuraExpired("Sunfire", 2))),
                Spell.Cast("Moonfire", ret => ((List<WoWUnit>)ret).FirstOrDefault(u => u.HasAuraExpired("Moonfire", 2)))
             );
        }


        internal static Composite BoomkinSt()
        {
            return new PrioritySelector(

               Spell.Cast("Starfall", ret => TimeToDie > 2 && StarfallDown),
               Spell.PreventDoubleCast("Healing Touch", 0.7, ret => TalentManager.IsSelected(17) && DreamDown && Me.ManaPercent > 25),
               Spell.Cast("Natures Vigil", ret => TalentManager.IsSelected(18)),
               Spell.Cast("Starsurge", ret => ShootingStarsUp && !SolarEclipseUp),
               Spell.Cast("Moonfire", ret => MoonFireDown || ((LunarEclipseUp || CelestialalignmentUp) && MoonSetting < 3 && MoonFireUp) || (MoonFireUp && MoonSetting < 2)),
               Spell.Cast("Sunfire", ret => SunFireDown || (SolarEclipseUp && SunSetting < 3 && SunFireUp) || (SunFireUp && SunSetting < 2)),
               Spell.Cast("Starfire", ret => CelestialalignmentUp && Spell.GetSpellCastTime("Starfire") < CelestialalignmentSetting),
               Spell.Cast("Wrath", ret => CelestialalignmentUp && Spell.GetSpellCastTime("Wrath") < CelestialalignmentSetting),
               Spell.Cast("Starfire", ret => (EclipseDirSun || (EclipseDirNothing && Eclipse > 0))),
               Spell.Cast("Wrath", ret => (EclipseDirMoon || (EclipseDirNothing && Eclipse <= 0))), //Spell.GetSpellCastTime("Wrath") <= TimeToDie && (EclipseDirMoon || (EclipseDirNothing && Eclipse <= 0)))
               Spell.Cast("Starsurge"),
               Spell.PreventDoubleCast("Moonfire", 0.5, target => Me.CurrentTarget, ret => LunarEclipseUp && Me.IsMoving, true),
               Spell.PreventDoubleCast("Sunfire", 0.5, target => Me.CurrentTarget, ret => !MoonFireDown && Me.IsMoving, true)
 
                );
        }

        internal static Composite BoomkinRebirth()
        {
            return new PrioritySelector(

                //new Decorator(ret => Me.FocusedUnit != null && SG.Instance.Protection.UseLayonHandsFocusTarget, Spell.Cast("Lay on Hands", on => Me.FocusedUnit)
             new Decorator(ret => SG.Instance.Boomkin.RebrithLogic == Enum.TriggerTarget.FocusTarget && Me.FocusedUnit != null && Me.FocusedUnit.Distance <= 30, Spell.Cast("Rebirth", ret => Me.FocusedUnit)
        //    (SG.Instance.Boomkin.RebrithLogic == Enum.DruidRebirth.OnTank && U.IsTargetBoss) ||
        //    (SG.Instance.Boomkin.RebrithLogic == Enum.TriggerTarget.FocusTarget && Me.FocusedUnit != null && Me.FocusedUnit.Distance <= 30)
       //    (SG.Instance.Boomkin.RebrithLogic == Enum.DruidRebirth.OnHealer) ||
      //     (SG.Instance.Boomkin.RebrithLogic == Enum.DruidRebirth.OnRaidMembers) ||
      //     (SG.Instance.Boomkin.RebirthLogic == Enum.DruidRebirth.OnMouseOver)
                    ));
        }



        internal static Composite BoomkinMt()
        {
            return new PrioritySelector(
                new Decorator(ret=> true, MultiDoT())
                // actions+=/hurricane,if=active_enemies>4&buff.solar_eclipse.up&buff.natures_grace.up
                // actions+=/hurricane,if=active_enemies>5&buff.solar_eclipse.up&mana.pct>25
                // actions+=/hurricane,if=active_senemies>4&buff.solar_eclipse.up&mana.pct>25
                );
        }

        internal static Composite BoomkinDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Barkskin", ret => SG.Instance.Boomkin.EnableBarkskin && Me.HealthPercent <= SG.Instance.Boomkin.BarkskinHP),
                Spell.Cast("Healing Touch", ret => SG.Instance.Boomkin.EnableHealingTouch && (Me.HealthPercent <= SG.Instance.Boomkin.HealingTouchHP || Me.HasAura(132158))),
                Spell.Cast("Rejuvenation", ret => SG.Instance.Boomkin.EnableRejuvenation && Me.HealthPercent <= SG.Instance.Boomkin.RejuvenationHP && !Me.HasAura(774)),
                Spell.Cast("Nature's Swiftness", ret => SG.Instance.Boomkin.EnableNatureSwiftness && Me.HealthPercent <= SG.Instance.Boomkin.NatureSwiftnessHP),
                Spell.Cast("Innervate", ret => SG.Instance.Boomkin.EnableInnervate && Me.ManaPercent <= SG.Instance.Boomkin.InnervateMP),
                Item.BoomkinUseHealthStone()
                );
        }


        internal static Composite BoomkinOffensive()
        {
            return new PrioritySelector(
                 Spell.Cast("Incarnation", ret =>  (LunarEclipseUp || SolarEclipseUp) && (
                    (SG.Instance.Boomkin.Incarnation == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.Incarnation == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.Incarnation == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Celestial Alignment", ret => ((!LunarEclipseUp && !SolarEclipseUp) && (ChoenofEluneUp || !TalentManager.IsSelected(11) || CooldownTracker.GetSpellCooldown(106731).TotalSeconds > 10)) && (
                    (SG.Instance.Boomkin.Celestial == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.Celestial == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.Celestial == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Force of Nature", ret => Me.GetAllAuras().Any(a => G.IntTrinketProcs.Contains(a.SpellId)) && TalentManager.IsSelected(12) && (
                    (SG.Instance.Boomkin.ForceofNature == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.ForceofNature == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.ForceofNature == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (!LunarEclipseUp || !SolarEclipseUp) && (
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (!LunarEclipseUp || !SolarEclipseUp) && (
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (!LunarEclipseUp || !SolarEclipseUp) && (
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite BoomkinUtility()
        {
            return new PrioritySelector(

                );
        }




        internal static Composite BoomkinInterrupts()
        {
            return new PrioritySelector(
              //  new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                //  &&  Spell.Cast("xxxx", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
              //      (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
                      );
        }
        #endregion

        #region Booleans


        internal static bool LunarEclipseUp { get { return Me.HasAura(48518); } }
        internal static bool SolarEclipseUp { get { return Me.HasAura(48517); } }
        internal static bool ChoenofEluneUp { get { return Me.HasAura(102560); } }
        internal static bool ShootingStarsUp { get { return Me.HasAura(93400); } }
        internal static bool DreamDown { get { return !Me.HasAura(145152); } }
        internal static bool StarsurgeCooldown { get { return !CooldownTracker.SpellOnCooldown("Starsurge"); } }
        internal static bool StarfallDown { get { return !Me.HasAura(48505); } }
        internal static double Eclipse { get { return Lua.PlayerEclipsePower; } }
        internal static bool CastingStarfire { get { return Me.CastingSpellId == 2912; } }
        internal static bool CastingWrath { get { return Me.CastingSpellId == 5176; } }
        internal static bool EclipseDirMoon { get { return Lua.GetEclipseDirection() == Enum.EclipseType.Lunar; } }
        internal static bool EclipseDirSun { get { return Lua.GetEclipseDirection() == Enum.EclipseType.Solar; } }
        internal static bool EclipseDirNothing { get { return Lua.GetEclipseDirection() == Enum.EclipseType.None; } }
        internal static bool CelestialalignmentUp { get { return Me.HasAura(112071); } }
        internal static double CelestialalignmentSetting { get { return Spell.GetMyAuraTimeLeft(112071, Me); } }
        internal static double MoonSetting { get { return Spell.GetMyAuraTimeLeft(8921, Me.CurrentTarget); } }
        internal static bool MoonFireDown { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(8921); } }
        internal static double SunSetting { get { return Spell.GetMyAuraTimeLeft(93402, Me.CurrentTarget); } }
        internal static bool SunFireDown { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(93402); } }
        internal static bool SunFireUp { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasMyAura(93402); } }
        internal static bool MoonFireUp { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasMyAura(8921); } }

        internal static double TimeToDie
        {
            get
            {
                return StyxWoW.Me.CurrentTarget != null
                    ? DpsMeter.GetCombatTimeLeft(StyxWoW.Me.CurrentTarget).TotalSeconds
                    : 10;
            }
        }


        // 16974 Predatory Swiftness Aura
        // 145152 Dream aura
        // 135700 Omen of Clarity

        #endregion Booleans

    }
}
