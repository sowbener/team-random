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

namespace YourBuddy.Rotations.Druid
{
    class Feral
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeFeral
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
          //              G.InitializeOnKeyActionsH(),
                        new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Binding Shot", ret => TalentManager.IsSelected(4)))),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Feral.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FeralDefensive()),
                                        new Decorator(ret => SG.Instance.Feral.CheckInterrupts, FeralInterrupts()),
                                        FeralUtility(),
                                  //      new Action(ret => { Item.UseFeralItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        FeralOffensive(),
                                       new Decorator(ret => Me.CurrentTarget != null && SG.Instance.Feral.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Feral.AoECount, FeralMt()),
                                       FeralSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Feral.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FeralDefensive()),
                                        new Decorator(ret => SG.Instance.Feral.CheckInterrupts, FeralInterrupts()),
                                        FeralUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                        new PrioritySelector(
                               //         new Action(ret => { Item.UseFeralItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        FeralOffensive())),
                                       new Decorator(ret => Me.CurrentTarget != null && SG.Instance.Feral.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Feral.AoECount, FeralMt()),
                                         FeralSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Feral.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FeralDefensive()),
                                        new Decorator(ret => SG.Instance.Feral.CheckInterrupts, FeralInterrupts()),
                                        FeralUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                        new PrioritySelector(
                            //            new Action(ret => { Item.UseFeralItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        FeralOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, FeralMt()),
                                        FeralSt())));
            }
        }


        internal static Composite FeralSt()
        {
            return new PrioritySelector(
                Spell.Cast("Ferocious Bite", ret => RipUp && RipSetting <= 3 && Me.CurrentTarget.HealthPercent <= 25),
                Spell.Cast("Faerie Fire", ret => G.WeakenedBlowsAura),
                Spell.PreventDoubleCast("Healing Touch", 0.7, On => Me, ret => TalentManager.IsSelected(17) && PredatorySwiftnessUp && DreamDown && (PredatorySwiftnessRemains1 || Lua.PlayerComboPts >= 4)),
                Spell.Cast("Savage Roar", ret => SavageRoardown),
                Spell.Cast("Thrash", ret => !OmenofClarityDown && (ThrashDown || ThrashSetting < 3) && TimeToDie >= 6),
                Spell.Cast("Ferocious Bite", ret => TimeToDie <= 1 && Lua.PlayerComboPts >= 3),
                Spell.Cast("Savage Roar", ret => SavageRoarSetting <= 3 && Lua.PlayerComboPts > 0 && (Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent < 25)),
                Spell.Cast("Rip", ret => Lua.PlayerComboPts >= 5 && rip_ratio() >= 1.15 && TimeToDie > 30),
                Spell.Cast("Rip", ret => Lua.PlayerComboPts >= 4 && rip_ratio() >= 0.95 && TimeToDie > 30 && RuneofReoriginationUp && RuneofReoriginationRemains <= 1.5),
                new Decorator(ret => Lua.PlayerComboPts >= 5 && (Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 25) && RipUp && !(Lua.PlayerPower >= 50 || (BerserkUp && Lua.PlayerPower >= 25)), new ActionAlwaysSucceed()),
                Spell.Cast("Ferocious Bite", ret => Lua.PlayerComboPts >= 5 && RipUp && (Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 25)),
                Spell.Cast("Rip", ret => Lua.PlayerComboPts >= 5 && TimeToDie >= 6 && RipSetting < 2 && (BerserkUp || RipSetting + 1.9 <= CooldownTracker.GetSpellCooldown("Tigers Fury").TotalSeconds)),
                Spell.Cast("Savage Roar", ret => SavageRoarSetting <= 3 && Lua.PlayerComboPts > 0 && (SavageRoarSetting + 2 > RipSetting)),
                Spell.Cast("Savage Roar", ret => SavageRoarSetting <= 6 && Lua.PlayerComboPts >= 5 && (SavageRoarSetting + 2 <= RipSetting && RipUp)),
                Spell.Cast("Savage Roar", ret => SavageRoarSetting <= 12 && Lua.PlayerComboPts >= 5 && Lua.TimeToEnergyCap() <= 1 && SavageRoarSetting <= RipSetting && RipUp),
                Spell.Cast("Rake", ret => RuneofReoriginationUp && RakeRemains < 9 && RuneofReoriginationRemains <= 1.5),
                //# Rake if we can apply a stronger Rake or if it's about to fall off and clipping the last tick won't waste too much damage.
                //actions.advanced+=/rake,cycle_targets=1,if=target.time_to_die-dot.rake.remains>3&(action.rake.tick_damage>dot.rake.tick_dmg|(dot.rake.remains<3&action.rake.tick_damage%dot.rake.tick_dmg>=0.75))
                new Decorator(ret => TimeToDie >= 6 && ThrashSetting < 3 && (RipSetting >= 8 && SavageRoarSetting >= 12 || BerserkUp || Lua.PlayerComboPts >= 5) && RipUp, new ActionAlwaysSucceed()),
                Spell.Cast("Thrash", ret => TimeToDie >= 6 && ThrashSetting < 3 && (RipSetting >= 8 && SavageRoarSetting >= 12 || BerserkUp || Lua.PlayerComboPts >= 5) && RipUp),
                new Decorator(ret => TimeToDie >= 6 && ThrashSetting < 9 && RuneofReoriginationUp && RuneofReoriginationRemains <= 1.5 && RipUp, new ActionAlwaysSucceed()),
                Spell.Cast("Thrash", ret => TimeToDie >= 6 && ThrashSetting < 9 && RuneofReoriginationUp && RuneofReoriginationRemains <= 1.5 && RipUp),
                new Decorator(ret => Lua.PlayerComboPts >= 5 && !(Lua.TimeToEnergyCap() <= 1 || (BerserkUp && Lua.PlayerPower >= 25) || (FeralRageUp && FeralRageRemains <= 1)) && RipUp, new ActionAlwaysSucceed()),
                Spell.Cast("Ferocious Bite", ret => Lua.PlayerComboPts >= 5 && RipUp),
                new Decorator(ret => !OmenofClarityDown, FeralFiller()),
                new Decorator(ret => FeralFuryUp, FeralFiller()),
                new Decorator(ret => (Lua.PlayerComboPts < 5 && RipSetting < 3) || (Lua.PlayerComboPts < 1 && SavageRoarSetting < 2), FeralFiller()),
                new Decorator(ret => TimeToDie <= 8.5, FeralFiller()),
                new Decorator(ret => TigersFuryUp || BerserkUp, FeralFiller()),
                new Decorator(ret => CooldownTracker.GetSpellCooldown("Tiger's Fury").TotalSeconds <= 3, FeralFiller()),
                new Decorator(ret => Lua.TimeToEnergyCap() <= 1, FeralFiller())
                //actions.advanced+=/run_action_list,name=filler,if=buff.omen_of_clarity.react
                //actions.advanced+=/run_action_list,name=filler,if=buff.feral_fury.react
                //actions.advanced+=/run_action_list,name=filler,if=(combo_points<5&dot.rip.remains<3.0)|(combo_points=0&buff.savage_roar.remains<2)
                //actions.advanced+=/run_action_list,name=filler,if=target.time_to_die<=8.5
                //actions.advanced+=/run_action_list,name=filler,if=buff.tigers_fury.up|buff.berserk.up
                //actions.advanced+=/run_action_list,name=filler,if=cooldown.tigers_fury.remains<=3
                //actions.advanced+=/run_action_list,name=filler,if=energy.time_to_max<=1.0
                );
        }


        internal static Composite FeralFiller()
        {
            return new PrioritySelector(
               Spell.Cast("Ravage"),
               Spell.Cast("Rake", ret => (TimeToDie - RakeRemains > 3 && Rake_sDamage * (RakeRemains + 1) - Spell.GetRakeStrength(StyxWoW.Me.CurrentTarget.Guid) * RakeRemains > Mangle_sDamage)),
               Spell.Cast("Shred", ret => (!OmenofClarityDown || BerserkUp || Lua.LuaGetEnergyRegen() >= 15) && KingoftheJungleDown),
                Spell.Cast("Mangle", ret => KingoftheJungleDown)
                );
        }

          internal static Composite FeralMt()
        {
            return new PrioritySelector(
                //actions.aoe=swap_action_list,name=default,if=active_enemies<5
                //actions.aoe+=/auto_attack
                //actions.aoe+=/faerie_fire,cycle_targets=1,if=debuff.weakened_armor.stack<3
                Spell.Cast("Faerie Fire", ret => G.WeakenedBlowsAura),
                //actions.aoe+=/savage_roar,if=buff.savage_roar.down|(buff.savage_roar.remains<3&combo_points>0)
                Spell.Cast("Savage Roar", ret => SavageRoardown || (SavageRoarSetting < 3 && Lua.PlayerComboPts > 0))
                //actions.aoe+=/pool_resource,for_next=1
                //actions.aoe+=/thrash_cat,if=buff.rune_of_reorigination.up
                //actions.aoe+=/pool_resource,wait=0.1,for_next=1
                //actions.aoe+=/thrash_cat,if=dot.thrash_cat.remains<3|(buff.tigers_fury.up&dot.thrash_cat.remains<9)
                //actions.aoe+=/savage_roar,if=buff.savage_roar.remains<9&combo_points>=5
                //actions.aoe+=/rip,if=combo_points>=5
                //actions.aoe+=/rake,cycle_targets=1,if=(active_enemies<8|buff.rune_of_reorigination.up)&dot.rake.remains<3&target.time_to_die>=15
                //actions.aoe+=/swipe_cat,if=buff.savage_roar.remains<=5
                //actions.aoe+=/swipe_cat,if=buff.tigers_fury.up|buff.berserk.up
                //actions.aoe+=/swipe_cat,if=cooldown.tigers_fury.remains<3
                //actions.aoe+=/swipe_cat,if=buff.omen_of_clarity.react
                //actions.aoe+=/swipe_cat,if=energy.time_to_max<=1
                );
        }

        internal static Composite FeralDefensive()
        {
            return new PrioritySelector(
                
            //    Item.FeralUseHealthStone()
                );
        }


        internal static Composite FeralOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Tiger's Fury", ret => Lua.PlayerPower <= 35 && OmenofClarityDown),
                Spell.Cast("Berserk", ret => TigersFuryUp || (TimeToDie < 18 && TigersFuryCooldown6)),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Feral.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite FeralUtility()
        {
            return new PrioritySelector(

                );
        }




        internal static Composite FeralInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("xxxx", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
                      );
        }
        #endregion

        #region Booleans

        private static uint FocusFireStackCount
        {
            get
            {
                return Spell.GetAuraStackCount("Frenzy");
            }
        }

        internal static bool RipUp { get { return Me.CurrentTarget!= null && Me.CurrentTarget.HasMyAura(1079); } }
        internal static double RipSetting { get { return Spell.GetMyAuraTimeLeft(1079, Me.CurrentTarget); } }
        internal static bool PredatorySwiftnessUp { get { return Me.HasAura(16974); } }
        internal static bool DreamDown { get { return !Me.HasAura(145152); } }
        internal static bool PredatorySwiftnessRemains1 { get { return Spell.GetAuraTimeLeft(16974, Me) < 1.5; } }
        internal static bool SavageRoardown { get { return !Me.HasAura(52610); } }
        internal static bool OmenofClarityDown { get { return !Me.HasAura(135700); } }
        internal static bool KingoftheJungleDown { get { return !Me.HasAura(102543); } }
        internal static bool TigersFuryUp { get { return Me.HasAura(5217); } }
        internal static bool TigersFuryCooldown6 { get { return CooldownTracker.GetSpellCooldown(5217).TotalSeconds > 6; } }
        internal static bool ThrashDown { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(106830); } }
        internal static double ThrashSetting { get { return Spell.GetMyAuraTimeLeft(106830, Me.CurrentTarget); } }
        internal static double SavageRoarSetting { get { return Spell.GetAuraTimeLeft(52610, Me); } }
        internal static bool RuneofReoriginationUp { get { return Me.HasAura(139120); } }
        internal static double RuneofReoriginationRemains { get { return Spell.GetAuraTimeLeft(139120, Me); } }
        internal static double RakeRemains { get { return Spell.GetMyAuraTimeLeft(1822, Me.CurrentTarget); } }
        internal static bool BerserkUp { get { return Me.HasAura(106952); } }
        internal static bool FeralRageUp { get { return Me.HasAura(146874); } }
        internal static bool FeralFuryUp { get { return Me.HasAura(144865); } }
        internal static double FeralRageRemains { get { return Spell.GetAuraTimeLeft(146874, Me); } }
        internal static double TimeToDie
        {
            get
            {
                return StyxWoW.Me.CurrentTarget != null
                    ? DpsMeter.GetCombatTimeLeft(StyxWoW.Me.CurrentTarget).TotalSeconds
                    : 10;
            }
        }

        public static double rip_ratio()
        {
            if (!RipUp)
                return 2;
            return Rip_sDamage / _dot_rip_multiplier;
        }

        public static double Mangle_sDamage
        {
            get
            {
                double weaponDmg = YourBuddy.Root.dps;
                return 5 * weaponDmg + 390 * YourBuddy.Root.Multiplier;
            }
        }


        public static double _dot_rip_multiplier;
        public static double Damagemultiplier = 1;

        public static double Rake_sDamage
        {
            get
            {
                double Rake_sAP = YourBuddy.Root.AP;
                double Rake_sMastery = YourBuddy.Root.Mastery;
                double Rake_sMult = YourBuddy.Root.Multiplier;
                return (((99 + (Rake_sAP * 0.3)) * Rake_sMastery) * Rake_sMult);
            }
        }

        public static double Rip_sDamage
        {
            get
            {
                double Rip_sAP = YourBuddy.Root.AP;
                double Rip_sMastery = YourBuddy.Root.Mastery;
                double Rip_sMult = YourBuddy.Root.Multiplier;
                return (((113 * Rip_sMastery) + 320 * 5 * Rip_sMastery + 0.0484 * 5 * Rip_sAP * Rip_sMastery) * Rip_sMult);
            }
        }

        // 16974 Predatory Swiftness Aura
        // 145152 Dream aura
        // 135700 Omen of Clarity

        #endregion Booleans

    }
}
