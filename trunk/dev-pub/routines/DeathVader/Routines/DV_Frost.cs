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
using Styx.WoWInternals;

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
                        Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTapFirstCheckDW),
                        Spell.Cast("Outbreak", ret => NeedEitherDis && SG.Instance.Frost.EnableOutbreak),
                        Spell.Cast("Unholy Blight", ret => Me.CurrentTarget != null && Me.CurrentTarget.Distance < 6 && UnholyBlightTalent && OutBreakCooldown && UnholyBlightCheck && NeedEitherDis),
                        Spell.Cast("Frost Strike", ret => ObliterateProc || Lua.PlayerPower > 88),
                        Spell.Cast("Howling Blast", ret => G.BloodRuneSlotsActive > 1 || G.FrostRuneSlotsActive > 1),
                        Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTapSecondCheckDW),
                        Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP),
                        Spell.Cast("Howling Blast", ret => UnholyBlightCheck && NeedFrostFever),
                        Spell.Cast("Plague Strike", ret => (UnholyBlightCheck && OutBreakCooldown && SG.Instance.Frost.EnableOutbreak && NeedBloodPlague) || (UnholyBlightCheck && !SG.Instance.Frost.EnableOutbreak && NeedBloodPlague)),
                        Spell.Cast("Howling Blast", ret => HowlingBlastProc),
                        Spell.Cast("Frost Strike", ret => Lua.PlayerPower > 76),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.Cast("Obliterate", ret => G.UnholyRuneSlotsActive > 0 && !Me.HasAura(51124)),
                        Spell.Cast("Howling Blast"),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.Cast("Frost Strike", ret => T.HasTalent(14) && G.FrostRuneSlotsActive == 0 || G.DeathRuneSlotsActive == 0),
                        Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTapThirdCheckDW),
                        Spell.Cast("Plague Leech", ret => G.CanCastPlagueLeechDW),
                        Spell.Cast("Frost Strike", ret => Lua.PlayerPower > 40));
        }

        internal static Composite FrostTHSt()
        {
            return new PrioritySelector(
                Spell.Cast("Outbreak", ret => NeedBothDisUp2H && UnholyBlightCheck),
                Spell.Cast("Unholy Blight", ret => NeedBothDisUp2H && UnholyBlightTalent),
                Spell.Cast("Soul Reaper", ret => Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP),
                Spell.PreventDoubleCast("Blood Tap", 0.5, ret => BloodTapFirstCheck2H),
                Spell.Cast("Howling Blast", ret => OutBreakCooldown && UnholyBlightCheck && NeedFrostFever2H),
                Spell.Cast("Plague Strike", ret => OutBreakCooldown && UnholyBlightCheck && NeedBloodPlague2H),
                Spell.Cast("Rime", ret => HowlingBlastProc),
                Spell.Cast("Obliterate", ret => ObliterateProc),
                Spell.PreventDoubleCast("Blood Tap", 0.5, ret => BloodTapSecondCheck2H),
                Spell.PreventDoubleCast("Blood Tap", 0.5, ret => BloodTapThirdCheck2H),
                Spell.Cast("Frost Strike", ret => FSRP76),
                Spell.Cast("Obliterate", ret => ObliterateRunes6),
                Spell.Cast("Plague Leech", ret => G.CanCastPlagueLeech),
                Spell.Cast("Outbreak", ret => NeedBothDisUpAoE && UnholyBlightCheck),
                Spell.Cast("Unholy Blight", ret => NeedBothDisUp),
                Spell.Cast("Frost Strike", ret => RunicEmpowermentTalent && AllRunesDown),
                Spell.Cast("Frost Strike", ret => BloodStacksunder10),
                Spell.Cast("Horn of Winter"),
                Spell.Cast("Obliterate"),
                Spell.PreventDoubleCast("Blood Tap", 0.5, ret => BloodTapForthCheck2H),
                Spell.Cast("Frost Strike"));
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
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(1000), RunStatus.Failure,
                    Spell.Cast("Mind Freeze")
                      ),
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(1000), RunStatus.Failure,
                    Spell.Cast("Strangulate", ret => !Spell.SpellOnCooldown("Strangulate") && !Spell.SpellOnCooldown("Asphyxiate") && Spell.SpellOnCooldown("Mind Freeze")
                    )));
        }
        #endregion

        #region Booleans

        //Blood Tap
        //buff.blood_charge.stack>10&(runic_power>76|(runic_power>=20&buff.killing_machine.react))
        private static bool NeedBloodTapFirstCheckDW { get { return  DvTalentManager.HasTalent(13) && Me.HasCachedAura(114851, 10) && (DvLua.PlayerPower > 76 || (Me.HasAura(51124) && DvLua.PlayerPower >= 20)); } }
        //Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP
        private static bool NeedBloodTapSecondCheckDW { get { return DvTalentManager.HasTalent(13) && Me.HasCachedAura(114851, 5) && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP && !Me.CurrentTarget.HasAura(130735); } }
        //actions.single_target+=/blood_tap,if=talent.blood_tap.enabled&(target.health.pct-3*(target.health.pct%target.time_to_die)>45|buff.blood_charge.stack>=8)
        private static bool NeedBloodTapThirdCheckDW { get { return DvTalentManager.HasTalent(13) && Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent >= SG.Instance.Frost.SoulReaperHP && Me.HasCachedAura(114851, 5) || Me.HasCachedAura(114851, 8); } }
        private static bool RunicEmpowermentTalent { get { return DvTalentManager.HasTalent(14); } }
        private static bool AllRunesDown { get { return G.FrostRuneSlotsActive == 0 || G.UnholyRuneSlotsActive == 0 || G.BloodRuneSlotsActive == 0; } }
        private static bool BloodStacksunder10 { get { return BloodTapStackCount <= 10 && DvTalentManager.HasTalent(13); } }
        private static bool BloodTapFirstCheck2H { get { return Me.CurrentTarget != null && DvTalentManager.HasTalent(13) && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP && SoulReaperNotOnCooldown && BloodTapStackCount > 4; } }
        private static bool BloodTapSecondCheck2H { get { return BloodTapStackCount > 4 && DvTalentManager.HasTalent(13) && ObliterateProc; } }
        private static bool BloodTapThirdCheck2H { get { return BloodTapStackCount > 10 && Lua.PlayerPower > 76 && DvTalentManager.HasTalent(13); } }
        private static bool BloodTapForthCheck2H { get { return BloodTapStackCount > 10 && Lua.PlayerPower >= 20 && DvTalentManager.HasTalent(13); } }


            private static uint BloodTapStackCount
        {
            get
            {
                return Spell.GetAuraStackCount("Blood Charge");
            }
        }

        //BloodTapCheckLua
        private static bool CanBloodTap { get { return DvTalentManager.HasTalent(13) && (Lua.GetRuneCooldown(1) <= 1 || Lua.GetRuneCooldown(2) <= 1 || Lua.GetRuneCooldown(3) <= 1 || Lua.GetRuneCooldown(4) <= 1 || Lua.GetRuneCooldown(5) <= 1 || Lua.GetRuneCooldown(6) <= 1); } }

        // Diseases
        private static bool NeedUnholyBlight { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasCachedAura(55095, 0, 2000)) || !Me.CurrentTarget.HasCachedAura(55078, 0, 2000); } }
        private static bool NeedEitherDis { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Blood Plague", 0, 3000); } }
        private static bool NeedFrostFever { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Frost Fever", 0); } }
        private static bool NeedBloodPlague { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura("Blood Plague", 0); } }
        private static bool NeedFrostFever2H { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Frost Fever"); } }
        private static bool NeedBloodPlague2H { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Blood Plague"); } }
        private static bool NeedBothDisUp { get { return Me.CurrentTarget != null && (NeedFrostFever || NeedBloodPlague); } }
        private static bool NeedBothDisUp2H { get { return Me.CurrentTarget != null && (NeedFrostFever || NeedBloodPlague); } }
        private static bool NeedBothDisUpAoE { get { return Me.CurrentTarget != null && (Spell.GetMyAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 2 || Spell.GetMyAuraTimeLeft("Frost Fever", Me.CurrentTarget) < 2); } }

        //RunicCorruptionBuffCheck
        private static bool RunicCorruptionDown { get { return !Me.HasCachedAura("Runic Corruption", 0); } }

        //HowlingBlastProc and ObliterateProc
        private const int KillingMachine = 51124;

        private static bool HowlingBlastProc { get { return Me.ActiveAuras.ContainsKey("Freezing Fog"); } }
        private static bool ObliterateProc { get { return Me.HasAura(51124); } }
        private static bool HornofWinterCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(57330).Cooldown; } }
        private static bool SoulReaperNotOnCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(130735).Cooldown; } }

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
                return G.DeathRuneSlotsActive + G.FrostRuneSlotsActive == 0 && G.UnholyRuneSlotsActive == 0;
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
        private static bool OutBreakCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(77575).Cooldown; } }

        //UnholyBlight Check (For Aura)
        private static bool UnholyBlightCheck { get { return !Me.HasAura(115989); } }
        private static bool UnholyBlightTalent { get { return DvTalentManager.HasTalent(3); } }

        //SoulReaper Checks
        private static bool SoulReaperNon4SetBonusHPCheck { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 36; } }
        private static bool SoulReaperCooldownCheck { get { return Spell.SpellOnCooldown("Soul Reaper"); } }


        // Abilities
       //private static bool NeedDeathStrike { get { return SG.Instance.DeathKnight.EnableDarkSuccor && Me.HasCachedAura(101568, 0) && Me.HealthPercent < SG.Instance.DeathKnight.DeathStrikePercentCommon; } }

        #endregion Booleans

        }
}
