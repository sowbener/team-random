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

namespace YourBuddy.Rotations.Deathknight
{
    class Unholy
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeUnholy
        {
            get
            {
                return new PrioritySelector(
            //            new Decorator(ret => SG.Instance.General.CheckTreePerformance, DvLogger.TreePerformance("InitializeUnholy")),
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeOnKeyActionsDK(),
                        G.ManualCastPause(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Unholy.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, UnholyDefensive()),
                                        new Decorator(ret => SG.Instance.Unholy.CheckInterrupts, UnholyInterrupts()),
                                        UnholyUtility(),
                                        new Action(ret => { Item.UseUnholyItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                        UnholyOffensive(),
                                        new Decorator(ret => SG.Instance.Unholy.CheckAoE && Unit.NearbyAttackableUnitsCount > 2, UnholyMt()),
                                            UnholySt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Unholy.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, UnholyDefensive()),
                                        new Decorator(ret => SG.Instance.Unholy.CheckInterrupts, UnholyInterrupts()),
                                        UnholyUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                           new Action(ret => { Item.UseUnholyItems(); return RunStatus.Failure; }),
                                           new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                                        UnholyOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, UnholyMt()),
                                        UnholySt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite UnholySt()
        {
            return new PrioritySelector(
                //  Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks10 && RP32),
               Spell.PreventDoubleCast("Outbreak", 1, ret => SG.Instance.Unholy.EnablePowerDots && DoTTracker.NeedMeleeRefresh(Me.CurrentTarget, 55078)),
               Spell.PreventDoubleCast("Plague Strike", 0.7, ret => SG.Instance.Unholy.EnablePowerDots && CooldownTracker.SpellOnCooldown("Outbreak") && DoTTracker.NeedMeleeRefresh(Me.CurrentTarget, 55078)),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks10 && Lua.PlayerPower > 31),
               Spell.Cast("Unholy Blight", ret => TalentManager.IsSelected(3) && Me.IsWithinMeleeRange && (NeedBothDisUp || NeedUnholyBlight)),
               Spell.Cast("Outbreak", ret => (NeedUnholyBlight || NeedBothDisUp) && UnholyBlightCheck),
               Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HealthPercent <= SG.Instance.Unholy.SoulReaperHP),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && Me.CurrentTarget.HealthPercent <= SG.Instance.Unholy.SoulReaperHP && SoulReaperIsComplete),
               Spell.PreventDoubleCast("Plague Strike", 0.7, ret => NeedUnholyBlight && UnholyBlightCheck),
               Spell.Cast("Dark Transformation"),
               Spell.PreventDoubleCast("Death Coil", 0.5, ret => Lua.PlayerPower > 90),
               Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location, ret => G.UnholyRuneSlotsActive > 1),
                //Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && ShadowInfusionStack5),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && G.UnholyRuneSlotsActive == 2 && DeathAndDecayComingOffCooldown),
               Spell.Cast("Scourge Strike", ret => G.UnholyRuneSlotsActive > 1),
               Spell.Cast("Festering Strike", ret => G.DeathRuneSlotsActiveFesterReal && G.FrostRuneSlotsActive > 1 && G.BloodRuneSlotsActive > 1),
               Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && DeathAndDecayComingOffCooldown),
               Spell.PreventDoubleCast("Death Coil", 0.5, ret => (SuddenDoomProc || (TimmyDoesntHaveBuff && G.UnholyRuneSlotsActive <= 1))),
               Spell.Cast("Scourge Strike"),
               Spell.Cast("Plague Leech", ret => TalentManager.IsSelected(2) && G.CanCastPlagueLeechUH),
               Spell.Cast("Festering Strike"),
               Spell.Cast("Horn of Winter", ret => Lua.PlayerPower < 20),
               Spell.PreventDoubleCast("Death Coil", 0.5, ret => TimmyDoesntHaveBuff || (GargoyleCooldown8seconds && TimmyBuffRemains8seconds)),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks8));
        }

        internal static Composite UnholyMt()
        {
            return new PrioritySelector(
                     Spell.Cast("Unholy Blight", ret => TalentManager.IsSelected(3) && Me.IsWithinMeleeRange && NeedBothDisUpAoE),
                     Spell.PreventDoubleCast("Plague Strike", 0.7, ret => NeedBothDisUpAoE && UnholyBlightCheck),
                     Spell.PreventDoubleCast("Pestilence", 1, ret => U.AoeBPCheck && UnholyBlightCheck && OutBreakCooldown),
                     Spell.Cast("Summon Gargoyle", ret => SG.Instance.Unholy.UseGargoyleInAoE),
                     Spell.Cast("Dark Transformation"),
                     Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && ShadowInfusionStack5),
                     Spell.Cast("Blood Boil", ret => G.BloodRuneSlotsActive == 2 || G.DeathRuneSlotsActive == 2),
                     Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location, ret => G.UnholyRuneSlotsActive == 1),
                     Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && G.UnholyRuneSlotsActive == 2 && Me.CurrentTarget.HealthPercent <= SG.Instance.Unholy.SoulReaperHP),
                     Spell.Cast("Scourge Strike", ret => G.UnholyRuneSlotsActive == 2),
                     Spell.PreventDoubleCast("Death Coil", 0.5, ret => Lua.PlayerPower > 90 || Me.HasAura("Sudden Doom") || G.UnholyRuneSlotsActive <= 1),
                     Spell.Cast("Blood Boil"),
                     Spell.Cast("Ict Touch"),
                     Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && G.UnholyRuneSlotsActive == 1 && Me.CurrentTarget.HealthPercent <= SG.Instance.Unholy.SoulReaperHP),
                     Spell.Cast("Scourge Strike", ret => G.UnholyRuneSlotsActive == 1),
                     Spell.PreventDoubleCast("Death Coil", 0.5, ret => Lua.PlayerPower > 20),
                     Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5),
                     Spell.Cast("Plague Leech", ret => TalentManager.IsSelected(2) && G.UnholyRuneSlotsActive == 1 && G.CanCastPlagueLeechUH),
                     Spell.Cast("Horn of Winter"));
        }



        internal static Composite UnholyDefensive()
        {
            return new PrioritySelector(
                Spell.PreventDoubleCast("Death Strike", 0.5, ret => Me.HasAura(101568)),
                Spell.Cast("Death Siphon", ret => TalentManager.IsSelected(11) && Me.HealthPercent <= SG.Instance.Unholy.DeathSiphonHP),
                Item.UnholyUseHealthStone()
                );
        }


        internal static Composite UnholyOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Unholy Frenzy", ret => StyxWoW.Me, ret => (
                    (SG.Instance.Unholy.UnholyFrenzy == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Unholy.UnholyFrenzy == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.UnholyFrenzy == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Summon Gargoyle", ret => G.SpeedBuffsAura || Me.HasAura("Unholy Frenzy") && (
                    (SG.Instance.Unholy.SummonGargoyle == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Unholy.SummonGargoyle == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.SummonGargoyle == Enum.AbilityTrigger.Always)
                    )),
               Spell.Cast("Empower Rune Weapon", ret => EmpowerRuneWeapon && (
                    (SG.Instance.Unholy.EmpowerRuneWeapon == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Unholy.EmpowerRuneWeapon == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.EmpowerRuneWeapon == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite UnholyUtility()
        {
            return new PrioritySelector(
                );
        }


        internal static Composite UnholyInterrupts()
        {
            {
                return new PrioritySelector(
                    new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("Mind Freeze", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))));
            }
        }
        #endregion

        #region Booleans


        //BloodTapStuffHere
        private static bool CanBloodTap { get { return TalentManager.IsSelected(13) && (G.BloodRuneSlotsActive == 0 || G.FrostRuneSlotsActive == 0 || G.UnholyRuneSlotsActive == 0); } }
        private static bool BloodTapStacks10 { get { return Spell.GetAuraStack(Me, 114851) > 10; } }
        private static bool BloodTapStacks5 { get { return Spell.GetAuraStack(Me, 114851) > 4; } }
        private static bool BloodTapStacks8 { get { return Spell.GetAuraStack(Me, 114851) >= 8; } }

        // Diseases
        private static bool NeedUnholyBlight { get { return Me.CurrentTarget != null && NeedBothDisUpAoE; } }
        private static bool NeedEitherDis { get { return Me.CurrentTarget != null && Spell.GetAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 3; } }
        private static bool NeedFrostFever { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Frost Fever"); } }
        private static bool NeedBloodPlague { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Blood Plague"); } }
        private static bool NeedBothDisUp { get { return Me.CurrentTarget != null && (NeedFrostFever || NeedBloodPlague); } }
        private static bool NeedBothDisUpAoE { get { return Me.CurrentTarget != null && (Spell.GetAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 3 || Spell.GetAuraTimeLeft("Frost Fever", Me.CurrentTarget) < 3); } }

        //RunicCorruptionBuffCheck
        private static bool RunicCorruptionDown { get { return !Me.HasAura("Runic Corruption"); } }

        //HowlingBlastProc and ObliterateProc
        private const int KillingMachine = 51124;
        private static bool HowlingBlastProc { get { return Me.HasAura("Freezing Fog"); } }
        private static bool ObliterateProc { get { return Me.HasAura(51124); } }
        private static bool EmpowerRuneWeapon { get { return Me.CurrentTarget != null && Me.IsWithinMeleeRange && U.IsTargetBoss && CooldownTracker.GetSpellCooldown(85948).TotalMilliseconds > 1200 && CooldownTracker.GetSpellCooldown(55090).TotalMilliseconds > 1200 && Lua.PlayerPower < 32; } }

        //RunesCheck
        private static bool ObliterateRunes6 { get { return G.DeathRuneSlotsActive + G.FrostRuneSlotsActive + G.UnholyRuneSlotsActive == 6; } }
        private static bool FrostRunes0 { get { return G.FrostRuneSlotsActive == 0; } }

        //DeathAndDecay
        private static bool DeathAndDecayComingOffCooldown { get { return !CooldownTracker.SpellOnCooldown(43265); } }

        //RunicPowerChecks
        private static bool RP76 { get { return Lua.PlayerPower > 76; } }
        private static bool FSRP76 { get { return Lua.PlayerPower > 76; } }
        private static bool RP32 { get { return Lua.PlayerPower >= 32; } }

        //GargoyleCooldown
        private static bool GargoyleCooldown8seconds { get { return  CooldownTracker.GetSpellCooldown(49206).TotalMilliseconds > 8000; } }

        //DeathCoil
        private static bool SuddenDoomProc { get { return Me.HasAura(81340); } }

        //TimmyChecks
        private static bool ShadowInfusionStack5 { get { return Spell.GetAuraStack(Me, 91342) > 4; } }
        private static bool TimmyDoesntHaveBuff { get { return !Me.Pet.HasAura("Dark Transformation"); } }
        private static bool TimmyBuffRemains8seconds { get { return Me.Pet.GetAuraTimeLeft("Dark Transformation").TotalMilliseconds < 8000; } }


        //OutbreakCooldownCheck
        private static bool OutBreakCooldown { get { return CooldownTracker.SpellOnCooldown(77575); } }

        //UnholyBlight Check (For Aura)
        private static bool UnholyBlightCheck { get { return !Me.HasAura(115989); } }

        //SoulReaper Checks
        private static bool SoulReaperNon4SetBonusHPCheck { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 36; } }
        private static bool SoulReaperIsComplete { get { return !CooldownTracker.SpellOnCooldown(130736); } }


        // Abilities
        //private static bool NeedDeathStrike { get { return SG.Instance.DeathKnight.EnableDarkSuccor && Me.HasCachedAura(101568, 0) && Me.HealthPercent < SG.Instance.DeathKnight.DeathStrikePercentCommon; } }

        #endregion Booleans

    }
}
