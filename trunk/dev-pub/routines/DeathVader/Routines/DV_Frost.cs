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
using Styx.CommonBot;

namespace DeathVader.Routines
{
    class DvFrost
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeFrost
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, DvLogger.TreePerformance("InitializeFrost")),
                        new Decorator(ret => (DvHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),   
                        new Decorator(ret => SG.Instance.General.CheckAdvancedLogging, DvLogger.AdvancedLogging),                     
                        G.InitializeCaching(),
                         G.InitializeOnKeyActions(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == DvEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Frost.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FrostDefensive()),
                                        new Decorator(ret => SG.Instance.Frost.CheckInterrupts && U.CanInterrupt, FrostInterrupts()),
                                        FrostUtility(),
                                        I.FrostUseItems(),
                                        FrostOffensive(),
                                        new Decorator(ret => SG.Instance.Frost.CheckAoE && (U.NearbyAttackableUnitsCount >= 2 || U.IsAoETarget), FrostMt()),
                                        new Decorator(ret => !I.WieldsTwoHandedWeapons, FrostDWSt()),
                                        new Decorator(ret => I.WieldsTwoHandedWeapons, FrostTHSt()))),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == DvEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Frost.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FrostDefensive()),
                                        new Decorator(ret => SG.Instance.Frost.CheckInterrupts && U.CanInterrupt, FrostInterrupts()),
                                        FrostUtility(),
                                        new Decorator(ret => DvHotKeyManager.IsCooldown, 
                                        new PrioritySelector(
                                        I.FrostUseItems(),
                                        FrostOffensive())),
                                        new Decorator(ret => DvHotKeyManager.IsAoe, FrostMt()),
                                        new Decorator(ret => !I.WieldsTwoHandedWeapons, FrostDWSt()),
                                        new Decorator(ret => I.WieldsTwoHandedWeapons, FrostTHSt()))));
            }
        }
        #endregion

        #region Rotations
        internal static Composite FrostDWSt()
        {
            return new PrioritySelector(
                        Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTapFirstCheck),
                        Spell.Cast("Outbreak", ret => NeedEitherDis && SG.Instance.Frost.EnableOutbreak),
                        Spell.Cast("Unholy Blight", ret => Me.CurrentTarget != null && Me.CurrentTarget.Distance < 6 && OutBreakCooldown && UnholyBlightCheck && NeedEitherDis),
                        Spell.Cast("Frost Strike", ret => ObliterateProc || Lua.PlayerPower > 88),
                        Spell.Cast("Howling Blast", ret => G.BloodRuneSlotsActive > 1 || G.FrostRuneSlotsActive > 1),
                        Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTapSecondCheck),
                        Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && Me.CurrentTarget.Distance < 6 && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP),
                        Spell.Cast("Howling Blast", ret => UnholyBlightCheck && NeedFrostFever),
                        Spell.Cast("Plague Strike", ret => (UnholyBlightCheck && OutBreakCooldown && SG.Instance.Frost.EnableOutbreak && NeedBloodPlague) || (UnholyBlightCheck && !SG.Instance.Frost.EnableOutbreak && NeedBloodPlague)),
                        Spell.Cast("Howling Blast", ret => HowlingBlastProc),
                        Spell.Cast("Frost Strike", ret => Lua.PlayerPower > 76),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.Cast("Obliterate", ret => G.UnholyRuneSlotsActive > 0 && !Me.HasAura(51124)),
                        Spell.Cast("Howling Blast"),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.Cast("Frost Strike", ret => T.HasTalent(14) && G.FrostRuneSlotsActive == 0 || G.DeathRuneSlotsActive == 0),
                        Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTapThirdCheck),
                        Spell.Cast("Plague Leech", ret => G.CanCastPlagueLeechDW),
                        Spell.Cast("Frost Strike", ret => Lua.PlayerPower > 40));
        }

        internal static Composite FrostTHSt()
        {
            return new PrioritySelector(
                    //    Spell.Cast("Plague Leech", ret => TalentManager.HasTalent(2) && (!PLBP || !PLFF)),
                       Spell.Cast("Outbreak", ret => SG.Instance.Frost.EnableOutbreak && UnholyBlightCheck && NeedEitherDis),
                       Spell.Cast("Unholy Blight", ret => Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && OutBreakCooldown && UnholyBlightCheck && NeedEitherDis),
                       Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP),
                        Spell.Cast("Howling Blast", ret => (NeedFrostFever && UnholyBlightCheck && OutBreakCooldown && SG.Instance.Frost.EnableOutbreak) || (!SG.Instance.Frost.EnableOutbreak && NeedFrostFever && UnholyBlightCheck)),
                        Spell.Cast("Plague Strike", ret => (UnholyBlightCheck && OutBreakCooldown && SG.Instance.Frost.EnableOutbreak && !Me.CurrentTarget.HasMyAura("Blood Plague")) || (UnholyBlightCheck && !SG.Instance.Frost.EnableOutbreak && !Me.CurrentTarget.HasMyAura("Blood Plague"))),
                        Spell.Cast("Howling Blast", ret => HowlingBlastProc),
                        Spell.Cast("Obliterate", ret => ObliterateProc),
                       // Spell.PreventDoubleCast("Blood Tap", ThrottleTime, ret => TalentManager.HasTalent(13) && KMProcBloodTap),
                     //   Spell.PreventDoubleCast("Blood Tap", ThrottleTime, ret => TalentManager.HasTalent(13) && BloodTapStackOver10 && RP76),
                        Spell.Cast("Frost Strike", ret => FSRP76),
                        Spell.Cast("Obliterate", ret => ObliterateRunes6),
                        Spell.Cast("Plague Leech", ret => G.CanCastPlagueLeech),
                        Spell.Cast("Frost Strike", ret => T.HasTalent(14) && FrostRunes0),
                 //       Spell.Cast("Frost Strike", ret => T.HasTalent(13) && BloodTapChargesUnder10),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.Cast("Obliterate",ret=> !NeedBloodPlague && !NeedFrostFever),
                        //actions.single_target+=/frost_strike,if=talent.runic_corruption.enabled&buff.runic_corruption.down
                        Spell.Cast("Frost Strike", ret => !Me.HasAura("Runic Corruption")),
                 //       Spell.PreventDoubleCast("Blood Tap", ThrottleTime, ret => TalentManager.HasTalent(13) && BloodTapStackOver10 && RP20),
                        Spell.Cast("Frost Strike")                                                                   
                        );
        }

        internal static Composite FrostMt()
        {
            return new PrioritySelector(
                        Spell.Cast("Unholy Blight", ret => Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && UnholyBlightCheck && NeedBothDisUpAoE),
                        Spell.Cast("Outbreak", ret => NeedBothDisUpAoE && UnholyBlightCheck || !T.HasTalent(3)),
                        Spell.PreventDoubleCast("Pestilence", 1, ret => Me.DeathRuneCount > 1 && DvUnit.AoeBPCheck && Spell.SpellOnCooldown(77575)),
                        Spell.Cast("Howling Blast"),
                        Spell.Cast("Frost Strike", ret => Lua.PlayerPower > 76),
                        Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location, ret => Me.UnholyRuneCount > 0),
                        Spell.Cast("Plague Strike", ret => Me.UnholyRuneCount > 1),
                        Spell.Cast("Frost Strike"),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.Cast("Plague Strike", ret => Me.UnholyRuneCount > 0));
        }

        internal static Composite FrostDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Death Strike", ret => Me.HasAura(101568)),
                Spell.Cast("Death Siphon", ret => T.HasTalent(11) && Me.HealthPercent <= SG.Instance.Frost.DeathSiphonHP),
                I.FrostUseHealthStone()
                );
        }


        internal static Composite FrostOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Pillar of Frost", ret => Me.CurrentTarget != null && (
                    (SG.Instance.Frost.PillarofFrost == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.PillarofFrost == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.PillarofFrost == DvEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Raise Dead", ret => Me.CurrentTarget != null && Me.HasAura("Pillar of Frost") && (
                    (SG.Instance.Frost.RaiseDead == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.RaiseDead == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.RaiseDead == DvEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == DvEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Empower Rune Weapon", ret => ((I.WieldsTwoHandedWeapons && CanEmpowerRuneWeapon) || (!I.WieldsTwoHandedWeapons && CanEmpowerRuneWeaponDW)) && (
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

        internal static Composite FrostUtility()
        {
            return new PrioritySelector(
                 
                );
        }


  

        internal static Composite FrostInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    Spell.Cast("Mind Freeze")
                      ),
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    Spell.Cast("Strangulate", ret => !Spell.SpellOnCooldown("Strangulate") && !Spell.SpellOnCooldown("Asphyxiate") && Spell.SpellOnCooldown("Mind Freeze")
                    )));
        }
        #endregion

        #region Booleans

        //Blood Tap
        //buff.blood_charge.stack>10&(runic_power>76|(runic_power>=20&buff.killing_machine.react))
        private static bool NeedBloodTapFirstCheck { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura(114851, 10) && (DvLua.PlayerPower > 76 || (Me.HasAura(51124) && DvLua.PlayerPower >= 20)); } }
        //Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP
        private static bool NeedBloodTapSecondCheck { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura(114851, 5) && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP && !Me.CurrentTarget.HasAura(130735); } }
        //actions.single_target+=/blood_tap,if=talent.blood_tap.enabled&(target.health.pct-3*(target.health.pct%target.time_to_die)>45|buff.blood_charge.stack>=8)
        private static bool NeedBloodTapThirdCheck { get { return DvTalentManager.HasTalent(13) && Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent >= SG.Instance.Frost.SoulReaperHP && Me.HasCachedAura(114851, 5) || Me.HasCachedAura(114851, 8); } }
        private static bool NeedBloodTap { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura("Blood Charge", 5); } }
        private static bool SoulReaperBloodTap { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura("Blood Charge", 5); } }
        private static bool KMProcBloodTap { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura("Blood Charge", 5); } }
        private static bool BloodTapRP20 { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura("Blood Charge", 8); } }
        private static bool BloodTapStackOver10 { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura("Blood Charge", 11); } }
        private static bool BloodTapChargesUnder10 { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura("Blood Charge", 10); } }

        //BloodTapCheckLua
        private static bool CanBloodTap { get { return DvTalentManager.HasTalent(13) && (Lua.GetRuneCooldown(1) <= 1 || Lua.GetRuneCooldown(2) <= 1 || Lua.GetRuneCooldown(3) <= 1 || Lua.GetRuneCooldown(4) <= 1 || Lua.GetRuneCooldown(5) <= 1 || Lua.GetRuneCooldown(6) <= 1); } }

        // Diseases
        private static bool NeedUnholyBlight { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasCachedAura(55095, 0, 2000)) || !Me.CurrentTarget.HasCachedAura(55078, 0, 2000); } }
        private static bool NeedEitherDis { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Blood Plague", 0, 3000); } }
        private static bool NeedFrostFever { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Frost Fever", 0); } }
        private static bool NeedBloodPlague { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Blood Plague", 0); } }
        private static bool NeedBothDisUp { get { return Me.CurrentTarget != null && (NeedFrostFever || NeedBloodPlague); } }
        private static bool NeedBothDisUpAoE { get { return Me.CurrentTarget != null && (Spell.GetMyAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 2 || Spell.GetMyAuraTimeLeft("Frost Fever", Me.CurrentTarget) < 2); } }

        //RunicCorruptionBuffCheck
        private static bool RunicCorruptionDown { get { return !Me.HasCachedAura("Runic Corruption", 0); } }

        //HowlingBlastProc and ObliterateProc
        private const int KillingMachine = 51124;

        private static bool HowlingBlastProc { get { return Me.ActiveAuras.ContainsKey("Freezing Fog"); } }
        private static bool ObliterateProc { get { return Me.HasAura(51124); } }
        private static bool HornofWinterCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(57330).Cooldown; } }

        internal static bool CanEmpowerRuneWeapon
        {
            get
            {
                return G.DeathRuneSlotsActive + G.FrostRuneSlotsActive == 0 && G.UnholyRuneSlotsActive < 2;
            }
        }

        internal static bool CanEmpowerRuneWeaponDW
        {
            get
            {
                return Lua.PlayerPower < 20 && Spell.GetSpellCooldown(49020).TotalMilliseconds > 2000 && Spell.GetSpellCooldown(49184).TotalMilliseconds > 2000  && Me.CurrentTarget.IsBoss && Me.IsWithinMeleeRange;
            }
        }

        //RunesCheck
        private static bool ObliterateRunes6 { get { return G.DeathRuneSlotsActive + G.FrostRuneSlotsActive + G.UnholyRuneSlotsActive == 6; } }
        private static bool FrostRunes0 { get { return G.FrostRuneSlotsActive == 0; } }

        //RunicPowerChecks
        private static bool RP76 { get { return Lua.PlayerPower > 76; } }
        private static bool FSRP76 { get { return Lua.PlayerPower > 76; } }
        private static bool RP20 { get { return Lua.PlayerPower >= 20; } }

        //OutbreakCooldownCheck
        private static bool OutBreakCooldown { get { return Spell.GetSpellCooldown("Outbreak").TotalMilliseconds > 500; } }

        //UnholyBlight Check (For Aura)
        private static bool UnholyBlightCheck { get { return DvTalentManager.HasTalent(3) && !Me.HasAura(115989); } }

        //SoulReaper Checks
        private static bool SoulReaperNon4SetBonusHPCheck { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 36; } }
        private static bool SoulReaperCooldownCheck { get { return Spell.SpellOnCooldown("Soul Reaper"); } }


        // Abilities
       //private static bool NeedDeathStrike { get { return SG.Instance.DeathKnight.EnableDarkSuccor && Me.HasCachedAura(101568, 0) && Me.HealthPercent < SG.Instance.DeathKnight.DeathStrikePercentCommon; } }

        #endregion Booleans

        }
}
