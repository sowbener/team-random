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
using U = YourBuddy.Core.Unit;
using YourRaidingBuddy.Core.Helpers;
using Enum = YourBuddy.Core.Helpers.Enum;

namespace YourBuddy.Rotations.Druid
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
                        new Decorator(a => CastingStarfire && EclipseDirMoon && !CelestialalignmentUp, new Action(delegate { SpellManager.StopCasting(); return RunStatus.Failure; })),
                        new Decorator(a => CastingWrath && EclipseDirSun && !CelestialalignmentUp, new Action(delegate { SpellManager.StopCasting(); return RunStatus.Failure; })),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Boomkin.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BoomkinDefensive()),
                                        new Decorator(ret => SG.Instance.Boomkin.CheckInterrupts, BoomkinInterrupts()),
                                        BoomkinUtility(),
                    //      new Action(ret => { Item.UseBoomkinItems(); return RunStatus.Failure; }),
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
                    //         new Action(ret => { Item.UseBoomkinItems(); return RunStatus.Failure; }),
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
                    //            new Action(ret => { Item.UseBoomkinItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        BoomkinOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, BoomkinMt()),
                                        new Decorator(ret => !HotKeyManager.IsAoe, BoomkinSt()))));
            }
        }


        internal static Composite BoomkinSt()
        {
            return new PrioritySelector(
               // actions+=/starfall,if=!buff.starfall.up
               Spell.Cast("Starfall", ret => StarfallDown),
               // actions+=/force_of_nature,if=talent.force_of_nature.enabled
               // actions+=/wild_mushroom_detonate,moving=0,if=buff.wild_mushroom.stack>0&buff.solar_eclipse.up
               // actions+=/natures_swiftness,if=talent.dream_of_cenarius.enabled
               // actions+=/healing_touch,if=talent.dream_of_cenarius.enabled&!buff.dream_of_cenarius.up&mana.pct>25
               // actions+=/incarnation,if=talent.incarnation.enabled&(buff.lunar_eclipse.up|buff.solar_eclipse.up)
               Spell.Cast("Incarnation", ret => LunarEclipseUp || SolarEclipseUp),
               // actions+=/celestial_alignment,if=(!buff.lunar_eclipse.up&!buff.solar_eclipse.up)&(buff.chosen_of_elune.up|!talent.incarnation.enabled|cooldown.incarnation.remains>10)
               Spell.Cast("Celestial Alignment", ret => (!LunarEclipseUp || SolarEclipseUp) && (ChoenofEluneUp || !TalentManager.IsSelected(11) || (TalentManager.IsSelected(11) && CooldownTracker.GetSpellCooldown("Incarnation").TotalSeconds > 10))),
               // actions+=/natures_vigil,if=talent.natures_vigil.enabled
               Spell.Cast("Natures Vigil", ret => TalentManager.IsSelected(18)),
               // actions+=/starsurge,if=buff.shooting_stars.react&(active_enemies<5|!buff.solar_eclipse.up)
               Spell.Cast("Starsurge", ret => ShootingStarsUp && (Unit.NearbyAttackableUnitsCount < 5 || !SolarEclipseUp)),
               // actions+=/moonfire,cycle_targets=1,if=buff.lunar_eclipse.up&ticks_remain<2
               Spell.Cast("Moonfire", ret => LunarEclipseUp && (MoonFireDown || MoonSetting < 2)),
               // actions+=/sunfire,cycle_targets=1,if=buff.solar_eclipse.up&ticks_remain<2
               Spell.Cast("Sunfire", ret => SolarEclipseUp && (SunFireDown || SunSetting < 2)),
               // actions+=/starsurge,if=cooldown_react
               Spell.Cast("Starsurge", ret => StarsurgeCooldown),
               // actions+=/starfire,if=buff.celestial_alignment.up&cast_time<buff.celestial_alignment.remains
               Spell.Cast("Starfire", ret => CelestialalignmentUp && Spell.GetSpellCastTime("Starfire") < CelestialalignmentSetting),
               // actions+=/wrath,if=buff.celestial_alignment.up&cast_time<buff.celestial_alignment.remains
               Spell.Cast("Wrath", ret => CelestialalignmentUp && Spell.GetSpellCastTime("Wrath") < CelestialalignmentSetting),
               // actions+=/starfire,if=eclipse_dir=1|(eclipse_dir=0&eclipse>0)
               Spell.Cast("Starfire", ret => (EclipseDirSun || (EclipseDirNothing && Eclipse > 0))), //Spell.GetSpellCastTime("Starfire") <= TimeToDie && (EclipseDirSun || (EclipseDirNothing && Eclipse > 0))),
               // actions+=/wrath,if=eclipse_dir=-1|(eclipse_dir=0&eclipse<=0)
               Spell.Cast("Wrath", ret => (EclipseDirMoon || (EclipseDirNothing && Eclipse <= 0))) //Spell.GetSpellCastTime("Wrath") <= TimeToDie && (EclipseDirMoon || (EclipseDirNothing && Eclipse <= 0)))
               // actions+=/moonfire,moving=1,cycle_targets=1,if=ticks_remain<2
               // actions+=/sunfire,moving=1,cycle_targets=1,if=ticks_remain<2
               // actions+=/wild_mushroom,moving=1,if=buff.wild_mushroom.stack<buff.wild_mushroom.max_stack
               // actions+=/starsurge,moving=1,if=buff.shooting_stars.react
               // actions+=/moonfire,moving=1,if=buff.lunar_eclipse.up
                // actions+=/sunfire,moving=1
 
                );
        }


        internal static Composite BoomkinFiller()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite BoomkinMt()
        {
            return new PrioritySelector(
                // actions+=/hurricane,if=active_enemies>4&buff.solar_eclipse.up&buff.natures_grace.up
                // actions+=/hurricane,if=active_enemies>5&buff.solar_eclipse.up&mana.pct>25
                // actions+=/hurricane,if=active_enemies>4&buff.solar_eclipse.up&mana.pct>25
                );
        }

        internal static Composite BoomkinDefensive()
        {
            return new PrioritySelector(

            //    Item.BoomkinUseHealthStone()
                );
        }


        internal static Composite BoomkinOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Boomkin.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
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
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("xxxx", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
                      );
        }
        #endregion

        #region Booleans


        internal static bool LunarEclipseUp { get { return Me.HasAura(48518); } }
        internal static bool SolarEclipseUp { get { return Me.HasAura(48517); } }
        internal static bool ChoenofEluneUp { get { return Me.HasAura(102560); } }
        internal static bool ShootingStarsUp { get { return Me.HasAura(93400);  } }
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
