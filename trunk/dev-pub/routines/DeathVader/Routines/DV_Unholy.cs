using CommonBehaviors.Actions;
using DeathVader.Core;
using DeathVader.Helpers;
using DeathVader.Managers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = DeathVader.Routines.DvGlobal;
using I = DeathVader.Core.DvItem;
using Lua = DeathVader.Helpers.DvLua;
using T = DeathVader.Managers.DvTalentManager;
using SG = DeathVader.Interfaces.Settings.DvSettings;
using SH = DeathVader.Interfaces.Settings.DvSettingsH;
using Spell = DeathVader.Core.DvSpell;
using U = DeathVader.Core.DvUnit;
using PureRotation.Classes;
using Styx.CommonBot;

namespace DeathVader.Routines
{
    class DvUnholy
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeUnholy
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, DvLogger.TreePerformance("InitializeUnholy")),
                        new Decorator(ret => (DvHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckAdvancedLogging, DvLogger.AdvancedLogging),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == DvEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Unholy.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, UnholyDefensive()),
                                        new Decorator(ret => SG.Instance.Unholy.CheckInterrupts && U.CanInterrupt, UnholyInterrupts()),
                                        UnholyUtility(),
                                        I.UnholyUseItems(),
                                        UnholyOffensive(),
                                        new Decorator(ret => SG.Instance.Unholy.CheckAoE && U.NearbyAttackableUnitsCount > 2, UnholyMt()),
                                            UnholySt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == DvEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Unholy.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, UnholyDefensive()),
                                        new Decorator(ret => SG.Instance.Unholy.CheckInterrupts && U.CanInterrupt, UnholyInterrupts()),
                                        UnholyUtility(),
                                        new Decorator(ret => DvHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.UnholyUseItems(),
                                                        UnholyOffensive())),
                                        new Decorator(ret => DvHotKeyManager.IsAoe, UnholyMt()),
                                        UnholySt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite UnholySt()
        {
            return new PrioritySelector(
                //  Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks10 && RP32),
               Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HealthPercent <= SG.Instance.Unholy.SoulReaperHP),
               Spell.PreventDoubleCast("Outbreak", 1, ret => SG.Instance.Unholy.EnablePowerDots && (Me.AttackPower > DoTTracker.SpellStats(Me.CurrentTarget, SpellBook.BloodPlague).AttackPower + SG.Instance.Unholy.AttackPowerDot)),
               Spell.PreventDoubleCast("Plague Strike", 0.7, ret => SG.Instance.Unholy.EnablePowerDots && Spell.SpellOnCooldown("Outbreak") && (Me.AttackPower > DoTTracker.SpellStats(Me.CurrentTarget, SpellBook.BloodPlague).AttackPower + SG.Instance.Unholy.AttackPowerDot)),
               Spell.Cast("Unholy Blight", ret => T.HasTalent(3) && Spell.IsSpellInRange(115989) && NeedUnholyBlight),
               Spell.Cast("Outbreak", ret => NeedUnholyBlight && UnholyBlightCheck),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && Me.CurrentTarget.HealthPercent <= SG.Instance.Unholy.SoulReaperHP && SoulReaperIsComplete),
               Spell.PreventDoubleCast("Plague Strike", 0.7, ret => NeedUnholyBlight && UnholyBlightCheck),
               Spell.Cast("Dark Transformation"),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && ShadowInfusionStack5),
               Spell.PreventDoubleCast("Death Coil", 0.5, ret => Spell.IsSpellInRange(47541) && Lua.PlayerPower > 90),
               Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location, ret => G.UnholyRuneSlotsActive > 1),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && G.UnholyRuneSlotsActive == 2 && DeathAndDecayComingOffCooldown),
               Spell.Cast("Scourge Strike", ret => G.UnholyRuneSlotsActive > 1),
               Spell.Cast("Festering Strike", ret =>  G.DeathRuneSlotsActiveFesterReal && G.FrostRuneSlotsActive > 1),
               Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location, ret => G.UnholyRuneSlotsActive > 0),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks5 && DeathAndDecayComingOffCooldown),
               Spell.PreventDoubleCast("Death Coil", 0.5, ret => Spell.IsSpellInRange(47541) && (SuddenDoomProc || (TimmyDoesntHaveBuff && G.UnholyRuneSlotsActive < 1))),
               Spell.Cast("Scourge Strike"),
               Spell.Cast("Festering Strike"),
               Spell.Cast("Horn of Winter", ret => Lua.PlayerPower < 20),
               Spell.PreventDoubleCast(47541, 0.5, ret => TimmyDoesntHaveBuff || (GargoyleCooldown8seconds && TimmyBuffRemains8seconds)),
               Spell.PreventDoubleCast("Blood Tap", 0.5, ret => CanBloodTap && BloodTapStacks8));
        }

        internal static Composite UnholyMt()
        {
            return new PrioritySelector(
                     Spell.Cast("Unholy Blight", ret => T.HasTalent(3) && Me.IsWithinMeleeRange && NeedBothDisUpAoE),
                     Spell.PreventDoubleCast("Plague Strike", 0.7, ret => NeedBothDisUpAoE && UnholyBlightCheck),
                     Spell.PreventDoubleCast("Pestilence", 1, ret => U.AoeBPCheck && UnholyBlightCheck && OutBreakCooldown),
                     Spell.Cast("Summon Gargoyle", ret => SG.Instance.Unholy.UseGargoyleInAoE),
                     Spell.Cast("Dark Transformation"),
                     Spell.Cast("Blood Boil", ret => G.BloodRuneSlotsActive == 2 || G.DeathRuneSlotsActive == 2),
                     Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location, ret => G.UnholyRuneSlotsActive == 1),
                     Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && G.UnholyRuneSlotsActive == 2 && Me.CurrentTarget.HealthPercent <= SG.Instance.Unholy.SoulReaperHP),
                     Spell.Cast("Scourge Strike", ret => G.UnholyRuneSlotsActive == 2),
                     Spell.PreventDoubleCast(47541, 0.5, ret => Lua.PlayerPower > 90 || Me.HasAura("Sudden Doom") || G.UnholyRuneSlotsActive <= 1),
                     Spell.Cast("Blood Boil"),
                     Spell.Cast("Scourge Strike", ret => G.UnholyRuneSlotsActive == 1),
                     Spell.PreventDoubleCast(47541, 0.5, ret => Lua.PlayerPower > 20),
                     Spell.Cast("Horn of Winter"));
        }



        internal static Composite UnholyDefensive()
        {
            return new PrioritySelector(
                Spell.PreventDoubleCast("Death Strike", 0.5, ret => Me.HasAura(101568)),
                Spell.Cast("Death Siphon", ret => T.HasTalent(11) && Me.HealthPercent <= SG.Instance.Unholy.DeathSiphonHP),
                I.UnholyUseHealthStone()
                );
        }


        internal static Composite UnholyOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Unholy Frenzy", ret => Me,  ret =>  (
                    (SG.Instance.Unholy.UnholyFrenzy == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Unholy.UnholyFrenzy == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.UnholyFrenzy == DvEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Summon Gargoyle", ret => G.SpeedBuffsAura || Me.HasAura("Unholy Frenzy") && (
                    (SG.Instance.Unholy.SummonGargoyle == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Unholy.SummonGargoyle == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.SummonGargoyle == DvEnum.AbilityTrigger.Always)
                    )),
               Spell.Cast("Empower Rune Weapon", ret => EmpowerRuneWeapon && (
                    (SG.Instance.Unholy.EmpowerRuneWeapon == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Unholy.EmpowerRuneWeapon == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.EmpowerRuneWeapon == DvEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
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
                    new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(1000), RunStatus.Failure,
                        Spell.Cast("Mind Freeze")));
            }
        }
        #endregion

        #region Booleans


        //BloodTapStuffHere
        private static bool CanBloodTap { get { return T.HasTalent(13) && (G.BloodRuneSlotsActive == 0 || G.FrostRuneSlotsActive == 0 || G.UnholyRuneSlotsActive == 0); } }
        private static bool BloodTapStacks10 { get { return Me.AuraStackCount("Blood Charge") > 10; } }
        private static bool BloodTapStacks5 { get { return Me.AuraStackCount("Blood Charge") > 4; } }
        private static bool BloodTapStacks8 { get { return Me.AuraStackCount("Blood Charge") >= 8; } }

        // Diseases
        private static bool NeedUnholyBlight { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasCachedAura(55095, 0, 1000)) || !Me.CurrentTarget.HasCachedAura(55078, 0, 1000); } }
        private static bool NeedEitherDis { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Blood Plague", 0, 3000); } }
        private static bool NeedFrostFever { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Frost Fever", 0); } }
        private static bool NeedBloodPlague { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Blood Plague", 0); } }
        private static bool NeedBothDisUp { get { return Me.CurrentTarget != null && (NeedFrostFever || NeedBloodPlague); } }
        private static bool NeedBothDisUpAoE { get { return Me.CurrentTarget != null && (Spell.GetMyAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 2 || Spell.GetMyAuraTimeLeft("Frost Fever", Me.CurrentTarget) < 2); } }

        //RunicCorruptionBuffCheck
        private static bool RunicCorruptionDown { get { return !Me.HasCachedAura("Runic Corruption", 0); } }

        //HowlingBlastProc and ObliterateProc
        private const int KillingMachine = 51124;
        private static bool HowlingBlastProc { get { return Me.HasAura("Freezing Fog"); } }
        private static bool ObliterateProc { get { return Me.HasAura(51124); } }
        private static bool EmpowerRuneWeapon { get { return Me.CurrentTarget != null && Me.IsWithinMeleeRange && U.IsTargetBoss && Spell.GetSpellCooldown("Festering Strike").TotalMilliseconds > 1200 && Spell.GetSpellCooldown("Scourge Strike").TotalMilliseconds > 1200 && Lua.PlayerPower < 32; } }

        //RunesCheck
        private static bool ObliterateRunes6 { get { return  G.DeathRuneSlotsActive + G.FrostRuneSlotsActive + G.UnholyRuneSlotsActive == 6; } }
        private static bool FrostRunes0 { get { return G.FrostRuneSlotsActive == 0; } }

        //DeathAndDecay
        private static bool DeathAndDecayComingOffCooldown { get { return Spell.GetSpellCooldown("Death and Decay").TotalMilliseconds > 0; } }

        //RunicPowerChecks
        private static bool RP76 { get { return Lua.PlayerPower > 76; } }
        private static bool FSRP76 { get { return Lua.PlayerPower > 76; } }
        private static bool RP32 { get { return Lua.PlayerPower >= 32; } }

        //GargoyleCooldown
        private static bool GargoyleCooldown8seconds { get { return Spell.GetSpellCooldown("Summon Gargoyle").TotalMilliseconds > 8000; } }

        //DeathCoil
        private static bool SuddenDoomProc { get { return Me.HasAura(81340); } }

        //TimmyChecks
        private static bool ShadowInfusionStack5 { get { return Me.AuraStackCount("Shadow Infusion") > 4; } }
        private static bool TimmyDoesntHaveBuff { get { return !Me.Pet.HasAura("Dark Transformation"); } }
        private static bool TimmyBuffRemains8seconds { get { return Me.Pet.GetAuraTimeLeft("Dark Transformation").TotalMilliseconds < 8000; } }


        //OutbreakCooldownCheck
        private static bool OutBreakCooldown { get { return Spell.SpellOnCooldown("Outbreak"); } }

        //UnholyBlight Check (For Aura)
        private static bool UnholyBlightCheck { get { return !Me.HasAura(115989); } }

        //SoulReaper Checks
        private static bool SoulReaperNon4SetBonusHPCheck { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 36; } }
        private static bool SoulReaperIsComplete { get { return Spell.GetSpellCooldown("Soul Reaper").TotalMilliseconds > 0; } }


        // Abilities
       //private static bool NeedDeathStrike { get { return SG.Instance.DeathKnight.EnableDarkSuccor && Me.HasCachedAura(101568, 0) && Me.HealthPercent < SG.Instance.DeathKnight.DeathStrikePercentCommon; } }

        #endregion Booleans

    }
}
