﻿using CommonBehaviors.Actions;
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
using Styx.WoWInternals;

namespace YourBuddy.Rotations.Shaman
{
    class Elemental
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeElemental
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheckRanged), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        new Decorator(ret => !SG.Instance.Elemental.PvPRotationCheck && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Elemental.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => SG.Instance.Elemental.EnableSelfHealing && Me.HealthPercent < 100, ElementalDefensive()),
                                        new Decorator(ret => SG.Instance.Elemental.CheckInterrupts && Unit.CanInterrupt, ElementalInterrupts()),
                                        ElementalUtility(),
                                        new Decorator(a => ChannelingLightingBolt, new Action(delegate { SpellManager.StopCasting(); return RunStatus.Failure; })),
                                        //I.ElementalUseItems(),
                                        ElementalOffensive(),
                                        new Decorator(ret => SG.Instance.Elemental.CheckAoE && Unit.NearbyAttackableUnitsCount >= 2, ElementalMt()),
                                            ElementalSt())),
//FirstPvPElemental
                   new Decorator(ret => SG.Instance.Elemental.PvPRotationCheck && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Elemental.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => SG.Instance.Elemental.EnableSelfHealing && Me.HealthPercent < 100, ElementalDefensive()),
                                        new Decorator(ret => SG.Instance.Elemental.CheckInterrupts && Unit.CanInterrupt, ElementalInterruptsPvP()),
                                        ElementalUtility(),
                               //         I.ElementalUseItems(),
                                        ElementalOffensive(),
                                       new Decorator(ret => HotKeyManager.IsSpecial, 
                                        new PrioritySelector(
                                         PurgeShit(),
                                         TotemsShit(),
                                        HexFocusTarget())),
                                        ElementalStPvP())),
//EndPvPElemental
                        new Decorator(ret => !SG.Instance.Elemental.PvPRotationCheck && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Elemental.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => SG.Instance.Elemental.EnableSelfHealing &&  Me.HealthPercent < 100, ElementalDefensive()),
                                        new Decorator(a => ChannelingLightingBolt, new Action(delegate { SpellManager.StopCasting(); return RunStatus.Failure; })),
                                        new Decorator(ret => SG.Instance.Elemental.CheckInterrupts && Unit.CanInterrupt, ElementalInterrupts()),
                                        ElementalUtility(),
                                         new Decorator(ret => HotKeyManager.IsCooldown,
                                         new PrioritySelector(
                                            //            I.ElementalUseItems(),
                                                        ElementalOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, ElementalMt()),
                                        ElementalSt())));
            }
        }
        #endregion

        #region Rotation PvE
        internal static Composite ElementalSt()
        {
            return new PrioritySelector(
            //    new Decorator(ret => HotKeyManager.IsSpecialKey, new PrioritySelector(Spell.PreventDoubleCast("Spiritwalker's Grace", 1, target => Me, ret => Me.IsMoving, true))),
                Spell.Cast("Unleash Elements", ret => TalentManager.IsSelected(16) && !Me.HasAura(114050)),
                Spell.Cast("Lava Burst", ret => !NeedFlameShock && (Me.HasAura(114050) || Me.HasAura(77756))),
                Spell.PreventDoubleCast("Lava Burst", 0.5, target => Me.CurrentTarget, ret => !FlameShock5 && (Me.HasAura(77756) || Me.HasAura("Spiritwalker's Grace")), true),
                Spell.PreventDoubleCast("Flame Shock", 1, ret => !Me.CurrentTarget.HasAura("Flame Shock") || NeedFlameShockRefresh),
                Spell.Cast("Elemental Blast", ret => TalentManager.IsSelected(18) && !FlameShock5),
                Spell.Cast("Earth Shock", ret => LightningShield7Stacks && !FlameShock5),
                Spell.PreventDoubleCast("Searing Totem", 1, ret => NeedSearingTotem),
                Spell.PreventDoubleCast("Lightning Bolt", 1, target => Me.CurrentTarget, ret => !FlameShock5 && (ElementalBlastAndLavaBurstIsOnCoolDown || !Me.ActiveAuras.ContainsKey("Lava Surge") || Me.CurrentTarget.HasAura("Flame Shock")), true));
        }
      

        
               //actions.aoe+=/flame_shock,cycle_targets=1,if=!ticking&active_enemies<3
               //actions.aoe+=/thunderstorm,if=mana.pct_nonproc<80
        internal static Composite ElementalMt()
        {
            WoWUnit seedTarget = null;
            return new PrioritySelector(
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                Spell.Cast("Unleash Elements", ret => TalentManager.IsSelected(16) && !Me.HasAura(114050)),
                Spell.Cast("Lava Burst", ret => !NeedFlameShock && (Me.HasAura(114050) || Me.HasAura(77756))),
                Spell.PreventDoubleCast("Lava Burst", 0.5, target => Me.CurrentTarget, ret => !FlameShock5 && (Me.HasAura(77756) || Me.HasAura("Spiritwalker's Grace")), true),
                Spell.PreventDoubleCast("Flame Shock", 1, ret => !Me.CurrentTarget.HasAura("Flame Shock") || NeedFlameShockRefresh),
                Spell.Cast("Elemental Blast", ret => TalentManager.IsSelected(18) && !FlameShock5),
                Spell.Cast("Chain Lightning"))),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
             //       Spell.PreventDoubleMultiDoT("Flame Shock", 1, Me, 20, 3, ret => U.NearbyAttackableUnitsCount < 4 && seedTarget == null),
                    Spell.Cast("Earth Shock", ret => LightningShield7Stacks && !FlameShock5),
                     Spell.Cast("Lava Burst", ret => !NeedFlameShock && (Me.HasAura(114050) || Me.HasAura(77756))),
                    Spell.PreventDoubleCast("Lava Burst", 0.5, target => Me.CurrentTarget, ret => !FlameShock5 && (Me.HasAura(77756) || Me.HasAura("Spiritwalker's Grace")), true),
                    Spell.Cast("Unleash Elements", ret => TalentManager.IsSelected(16) && !Me.HasAura(114050)),
                    Spell.Cast("Elemental Blast", ret => TalentManager.IsSelected(18) && !FlameShock5),
                    Spell.Cast("Chain Lightning"))),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 4,
                    new PrioritySelector(
                     Spell.Cast("Chain Lightning")
                     )));
        }


        internal static Composite ElementalDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shamanistic Rage", ret => PVEShamanisticRage),
                Spell.Cast("Ancestral Guidance", ret => PVEAncestralGuidance),
                Spell.Cast("Stone Bulwark Totem", ret => PVEStoneBulwarkTotem),
                Spell.Cast("Healing Surge", ret => PVEHealingSurge),
                Spell.Cast("Gift of the Naaru", ret => PVEGiftOfTheNaaru)
                //I.ElementalUseHealthStone()
                );
        }


        internal static Composite ElementalOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Lifeblood", ret => SpellManager.HasSpell(121279) && (
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Elemental.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite ElementalUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite PurgeShit()
        {
            return new PrioritySelector(
         //        Spell.PreventDoubleCast("Purge", 1, ret => Me.CurrentTarget, ret => Me.CurrentTarget.HasAnyAura(new[] {31884,110909,1044,6940,69369,12472,1022,79206,116849,974,33076}) && Me.CurrentTarget.Distance < 30)
                 );
        }

        internal static Composite TotemsShit()
        {
            return new PrioritySelector(
                 );
        }

        internal static Composite HexFocusTarget()
        {
            return new PrioritySelector(
                 );
        }

        internal static Composite ElementalInterrupts()
        {
            {
                return new PrioritySelector(
                   new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Wind Shear", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))));
            }
        }

        #endregion

        #region Rotation - PvP

        internal static Composite ElementalStPvP()
        {
            return new PrioritySelector(
                Spell.Cast("Unleash Elements", ret => TalentManager.IsSelected(16) && !Me.HasAura(114050)),
                Spell.Cast("Elemental Blast", ret => TalentManager.IsSelected(18)),
                Spell.PreventDoubleCast("Lava Burst", 1, ret => !NeedFlameShock && (Me.HasAura(114050) || Me.HasAura(77756))),
                Spell.PreventDoubleCast("Flame Shock", 1, ret => !Me.CurrentTarget.HasAura("Flame Shock") || NeedFlameShock),                
                Spell.PreventDoubleCast("Searing Totem", 1, ret => !Totems.Exist(WoWTotemType.Fire) && FireElementalOnCooldown),
                Spell.PreventDoubleCast("Chain Lightning", 1),
                Spell.Cast("Earth Shock", ret => LightningShield7Stacks)
                );
        }


        internal static Composite ElementalDefensivePvP()
        {
            return new PrioritySelector(
                //I.ElementalUseHealthStone()
                );
        }

        internal static Composite ElementalInterruptsPvP()
        {
            {
                return new PrioritySelector(
                    new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                        Spell.Cast("Wind Shear", ret => Me.CurrentTarget != null && Me.CurrentTarget.IsCastingHealingSpell)
                        ));
            }
        }

        #endregion


        #region Booleans


        private static bool NeedFlameShock { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Flame Shock", 0, 1000); } }
        private static bool NeedFlameShockRefresh { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Flame Shock", 0, 3000); } }
        private static bool FlameShock5 { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Flame Shock", 0, 4000); } }

        private static bool NeedSearingTotem
        {
            get
            {
                return Me.GotTarget
                            && Me.CurrentTarget.SpellDistance() < Totems.GetTotemRange(WoWTotem.Searing) - 2f
                            && !Totems.Exist(WoWTotemType.Fire);
            }
        }

        private static bool NeedMagmaTotem
        {
            get
            {
                return Me.GotTarget
                            && Me.CurrentTarget.SpellDistance() < Totems.GetTotemRange(WoWTotem.Magma) - 2f
                            && !Totems.Exist(WoWTotemType.Fire);
            }
        }

        private static bool PVESpiritWalk { get { return SG.Instance.Elemental.UseSpiritWalkAuto && Me.Rooted && !Me.Stunned && !Me.CurrentTarget.IsWithinMeleeRange; } }
        private static bool PVEShamanisticRage { get { return SG.Instance.Elemental.EnableShamanisticRage && Me.HealthPercent <= SG.Instance.Elemental.ShamanisticRageHP; } }
        private static bool PVEHealingStreamTotem { get { return SG.Instance.Elemental.EnableHealingStreamTotem && Me.HealthPercent <= SG.Instance.Elemental.HealingStreamHP; } }
        private static bool PVEAncestralGuidance { get { return TalentManager.IsSelected(14) && SG.Instance.Elemental.EnableAncestralGuidance && Me.HealthPercent <= SG.Instance.Elemental.AncestralGuidanceHP && !Me.Stunned; } }
        private static bool PVEStoneBulwarkTotem { get { return SG.Instance.Elemental.EnableStoneBulwarkTotem && Me.HealthPercent <= SG.Instance.Elemental.StoneBulwarkTotemHP; } }
        private static bool PVEGiftOfTheNaaru { get { return SG.Instance.Elemental.EnableGiftOfTheNaaru && Me.HealthPercent <= SG.Instance.Elemental.GiftOfTheNaaruHP; } }
        private static bool PVEHealingSurge
        {
            get
            {
                return (Me.HealthPercent <= SG.Instance.Elemental.HealingSurgeHP);

            }
        }

        private static bool LightningShield7Stacks { get { return Me.HasAura("Lightning Shield") && Spell.GetAuraStack(Me, 324) == 7; } }
        private static bool LightningShield4Stacks { get { return Me.HasAura("Lightning Shield") && Spell.GetAuraStack(Me, 324) > 3; } }
        internal static bool ElementalBlastAndLavaBurstIsOnCoolDown { get { return Styx.WoWInternals.WoWSpell.FromId(51505).Cooldown || (TalentManager.IsSelected(18) && Styx.WoWInternals.WoWSpell.FromId(117014).Cooldown); } }
        private static bool FireElementalOnCooldown { get { return CooldownTracker.SpellOnCooldown("Fire Elemental Totem"); } }
        private static bool EarthShockOnCooldown { get { return CooldownTracker.SpellOnCooldown("Earth Shock"); } }
        private static bool ChannelingLightingBolt { get { return ((Me.CurrentCastEndTime - Me.CurrentCastStartTime).TotalMilliseconds / 2) <= Me.CurrentCastTimeLeft.TotalMilliseconds && Me.CastingSpellId == 403 && Me.ActiveAuras.ContainsKey("Lava Surge"); } }

        #endregion Booleans

    }
}