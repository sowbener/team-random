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
    class Frost
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static int _currentRunicPower;

        #region Initialize Rotations
        internal static Composite InitializeFrost
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),                
                 //        G.InitializeOnKeyActions(),
                         G.ManualCastPause(),
                         G.InitializeOnKeyActionsDK(),
                         G.InitializeCaching(),
                         new Action(delegate { CacheLocalVars(); return RunStatus.Failure; }),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Frost.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FrostDefensive()),
                                        new Decorator(ret => SG.Instance.Frost.CheckInterrupts, FrostInterrupts()),
                                        FrostUtility(),
                                        new Action(ret => { Item.UseFrostItems(); return RunStatus.Failure; }),
                                        FrostOffensive(),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                        new Decorator(ret => SG.Instance.Frost.CheckAoE && Unit.NearbyAttackableUnitsCount >= 2, FrostMt()),
                                        new Decorator(ret => !Item.WieldsTwoHandedWeapons, FrostDWSt()),
                                        new Decorator(ret => Item.WieldsTwoHandedWeapons, FrostTHSt()))),
                      //     new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Abilities,
                     //           new PrioritySelector(
                    //                    new Decorator(ret => SG.Instance.Frost.CheckAutoAttack, Lua.StartAutoAttack),
                     //                   new Decorator(ret => Me.HealthPercent < 100, FrostDefensive()),
                     //                   new Decorator(ret => SG.Instance.Frost.CheckInterrupts, FrostInterrupts()),
                    //                    FrostUtility(),
                     //                   I.FrostUseItems(),
                    //                    FrostOffensive(),
                  //                      new Decorator(ret => SG.Instance.Frost.CheckAoE && U.NearbyAttackableUnitsCount >= 2, FrostMt()),
                  //                      new Decorator(ret => !I.WieldsTwoHandedWeapons, FrostDWSt()),
                 //                       new Decorator(ret => I.WieldsTwoHandedWeapons, FrostTHSt()))),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Frost.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, FrostDefensive()),
                                        new Decorator(ret => SG.Instance.Frost.CheckInterrupts, FrostInterrupts()),
                                        FrostUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown, 
                                        new PrioritySelector(
                                        new Action(ret => { Item.UseFrostItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                        FrostOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, FrostMt()),
                                        new Decorator(ret => !Item.WieldsTwoHandedWeapons, FrostDWSt()),
                                        new Decorator(ret => Item.WieldsTwoHandedWeapons, FrostTHSt()))));
            }
        }
        #endregion

        private static void CacheLocalVars()
        {
            _currentRunicPower = (int)Lua.PlayerPower;
        }

        #region Rotations
        internal static Composite FrostDWSt()
        {
            return new PrioritySelector(
                        Spell.PreventDoubleCast("Blood Tap", 0.6, ret => NeedBloodTapFirstCheckDW),
                        Spell.Cast("Outbreak", ret => NeedEitherDis && SG.Instance.Frost.EnableOutbreak),
                        Spell.Cast("Frost Strike", ret => ObliterateProc || _currentRunicPower > 88),
                        Spell.Cast("Howling Blast", ret => G.BloodRuneSlotsActive > 1 || G.FrostRuneSlotsActive > 1),
                        Spell.Cast("Unholy Blight", ret => Me.CurrentTarget != null && Me.CurrentTarget.Distance < 6 && UnholyBlightTalent && OutBreakCooldown && UnholyBlightCheck && NeedEitherDis),
                        Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP),
                        Spell.Cast("Howling Blast", ret => UnholyBlightCheck && NeedFrostFever),
                        Spell.Cast("Plague Strike", ret => (UnholyBlightCheck && OutBreakCooldown && SG.Instance.Frost.EnableOutbreak && NeedBloodPlague) || (UnholyBlightCheck && !SG.Instance.Frost.EnableOutbreak && NeedBloodPlague)),
                        Spell.Cast("Howling Blast", ret => HowlingBlastProc),
                        Spell.Cast("Frost Strike", ret => _currentRunicPower > 76),
                        Spell.Cast("Obliterate", ret => !SG.Instance.Frost.MasterSimple && G.UnholyRuneSlotsActive > 0 && !Me.HasAura(51124)),
                        Spell.Cast("Howling Blast"),
                        Spell.Cast("Frost Strike", ret => TalentManager.IsSelected(14) && G.FrostRuneSlotsActive == 0 || G.DeathRuneSlotsActive == 0),
                        Spell.PreventDoubleCast("Blood Tap", 0.6, ret => NeedBloodTapSecondCheckDW),
                        Spell.Cast("Frost Strike", ret => _currentRunicPower >= 40),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.PreventDoubleCast("Blood Tap", 0.6, ret => NeedBloodTapThirdCheckDW),
                        Spell.Cast("Plague Leech", ret => G.CanCastPlagueLeechDW));
        }

        internal static Composite FrostTHSt()
        {
            return new PrioritySelector(
                Spell.Cast("Outbreak", ret => NeedBothDisUp2H && UnholyBlightCheck),
                Spell.Cast("Unholy Blight", ret => NeedBothDisUp2H && UnholyBlightTalent),
                Spell.Cast("Soul Reaper", ret => Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP),
                Spell.PreventDoubleCast("Blood Tap", 0.5, ret => BloodTapFirstCheck2H),
                Spell.Cast("Howling Blast", ret => OutBreakCooldown && UnholyBlightCheck && NeedFrostFever2H),
                Spell.Cast("Plague Strike", ret => OutBreakCooldown && UnholyBlightCheck && NeedBloodPlague2H),
                Spell.Cast("Howling Blast", ret => HowlingBlastProc),
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
                 //       Spell.Cast("Unholy Blight", ret => Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && UnholyBlightCheck && NeedBothDisUpAoE),
                        Spell.Cast("Outbreak", ret => (NeedBothDisUpAoE && UnholyBlightCheck) || !TalentManager.IsSelected(3)),
                        Spell.PreventDoubleCast("Pestilence", 1, ret => Me.DeathRuneCount > 1 && U.AoeBPCheck && CooldownTracker.SpellOnCooldown(77575)),
                        Spell.Cast("Howling Blast"),
                        Spell.Cast("Frost Strike", ret => _currentRunicPower > 76),
                        Spell.CastOnGround("Death and Decay", ret => Me.CurrentTarget.Location, ret => Me.UnholyRuneCount > 0),
                        Spell.Cast("Plague Strike", ret => Me.UnholyRuneCount > 1),
                        Spell.Cast("Frost Strike"),
                        Spell.Cast("Horn of Winter", ret => HornofWinterCooldown),
                        Spell.Cast("Plague Strike", ret => Me.UnholyRuneCount > 0));
        }

        internal static Composite FrostDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Death Strike", ret => Me.HasAura(101568) && Me.HealthPercent <= SG.Instance.Frost.DeathstrikeHP),
                Spell.Cast("Death Siphon", ret => TalentManager.IsSelected(11) && Me.HealthPercent <= SG.Instance.Frost.DeathSiphonHP)
                //I.FrostUseHealthStone()
                );
        }


        internal static Composite FrostOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Pillar of Frost", ret => Me.CurrentTarget != null && (
                    (SG.Instance.Frost.PillarofFrost == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.PillarofFrost == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.PillarofFrost == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Raise Dead", ret => Me.CurrentTarget != null && Me.HasAura("Pillar of Frost") && (
                    (SG.Instance.Frost.RaiseDead == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.RaiseDead == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.RaiseDead == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Empower Rune Weapon", ret => ((Item.WieldsTwoHandedWeapons && CanEmpowerRuneWeapon) || (!Item.WieldsTwoHandedWeapons && CanEmpowerRuneWeaponDW)) && (
                    (SG.Instance.Frost.EmpowerRuneWeapon == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.EmpowerRuneWeapon == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.EmpowerRuneWeapon == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite FrostUtility()
        {
            return new PrioritySelector(
                 
                );
        }


        internal static Composite FrostInterrupts()
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
        // actions.single_target+=/blood_tap,if=talent.blood_tap.enabled&(target.health.pct-3*(target.health.pct%target.time_to_die)>35|buff.blood_charge.stack>=8)
        // actions.single_target+=/blood_tap,if=talent.blood_tap.enabled
        //Blood Tap
       private static bool NeedBloodTapFirstCheckDW { get { return TalentManager.IsSelected(13) && AllRunesDown && Spell.GetAuraStack(Me, 114851) > 10 && (_currentRunicPower > 76 || (Me.HasAura(51124) && _currentRunicPower >= 20)); } }

       private static bool NeedBloodTapSecondCheckDW { get { return TalentManager.IsSelected(13) && Spell.GetAuraStack(Me, 114851) >= 8 && AllRunesDown; } }
       private static bool NeedBloodTapThirdCheckDW { get { return TalentManager.IsSelected(13) && Spell.GetAuraStack(Me, 114851) > 4 && AllRunesDown; ; } }
        private static bool RunicEmpowermentTalent { get { return TalentManager.IsSelected(14); } }
        private static bool AllRunesDown { get { return G.FrostRuneSlotsActive == 0 || G.UnholyRuneSlotsActive == 0 || G.BloodRuneSlotsActive == 0; } }
        private static bool BloodStacksunder10 { get { return BloodTapStackCount <= 10 && TalentManager.IsSelected(13); } }
        private static bool BloodTapFirstCheck2H { get { return Me.CurrentTarget != null && TalentManager.IsSelected(13) && Me.CurrentTarget.HealthPercent <= SG.Instance.Frost.SoulReaperHP && SoulReaperNotOnCooldown && BloodTapStackCount > 4; } }
        private static bool BloodTapSecondCheck2H { get { return BloodTapStackCount > 4 && TalentManager.IsSelected(13) && ObliterateProc; } }
        private static bool BloodTapThirdCheck2H { get { return BloodTapStackCount > 10 && _currentRunicPower > 76 && TalentManager.IsSelected(13); } }
        private static bool BloodTapForthCheck2H { get { return BloodTapStackCount > 10 && _currentRunicPower >= 20 && TalentManager.IsSelected(13); } }


            private static uint BloodTapStackCount
        {
            get
            {
                return Spell.GetAuraStack(Me, 114851);
            }
        }

        // Diseases
        internal static double FrostFeverRemains { get { return Spell.GetMyAuraTimeLeft(55095, Me.CurrentTarget); } }
        internal static double BloodFeverRemains { get { return Spell.GetMyAuraTimeLeft(55078, Me.CurrentTarget); } }
        internal static bool NeedUnholyBlight { get { return Me.CurrentTarget != null && (BloodFeverRemains < 2 || FrostFeverRemains < 2); } }
        private static bool NeedEitherDis { get { return Me.CurrentTarget != null && BloodFeverRemains < 3; } }
        private static bool NeedFrostFever { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Frost Fever"); } }
        private static bool NeedBloodPlague { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Blood Plague"); } }
        private static bool NeedFrostFever2H { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Frost Fever"); } }
        private static bool NeedBloodPlague2H { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Blood Plague"); } }
        private static bool NeedBothDisUp { get { return Me.CurrentTarget != null && (NeedFrostFever || NeedBloodPlague); } }
        private static bool NeedBothDisUp2H { get { return Me.CurrentTarget != null && (NeedFrostFever || NeedBloodPlague); } }
        private static bool NeedBothDisUpAoE { get { return Me.CurrentTarget != null && (Spell.GetAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 2 || Spell.GetAuraTimeLeft("Frost Fever", Me.CurrentTarget) < 2); } }

        //RunicCorruptionBuffCheck
        private static bool RunicCorruptionDown { get { return !Me.HasAura("Runic Corruption"); } }

        //HowlingBlastProc and ObliterateProc
        private const int KillingMachine = 51124;

        private static bool HowlingBlastProc { get { return Me.HasAura(59052); } }
        private static bool ObliterateProc { get { return Me.HasAura(KillingMachine); } }
        private static bool HornofWinterCooldown { get { return !CooldownTracker.SpellOnCooldown(57330); } }
        private static bool SoulReaperNotOnCooldown { get { return !CooldownTracker.SpellOnCooldown(130735); } }

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
        private static bool RP76 { get { return _currentRunicPower > 76; } }
        private static bool FSRP76 { get { return _currentRunicPower > 76; } }
        private static bool RP20 { get { return _currentRunicPower >= 20; } }

        //OutbreakCooldownCheck
        private static bool OutBreakCooldown { get { return CooldownTracker.SpellOnCooldown(77575); } }

        //UnholyBlight Check (For Aura)
        private static bool UnholyBlightCheck { get { return !Me.HasAura(115989); } }
        private static bool UnholyBlightTalent { get { return TalentManager.IsSelected(3); } }

        //SoulReaper Checks
        private static bool SoulReaperNon4SetBonusHPCheck { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 36; } }
        private static bool SoulReaperCooldownCheck { get { return CooldownTracker.SpellOnCooldown("Soul Reaper"); } }


        // Abilities
       //private static bool NeedDeathStrike { get { return SG.Instance.DeathKnight.EnableDarkSuccor && Me.HasCachedAura(101568, 0) && Me.HealthPercent < SG.Instance.DeathKnight.DeathStrikePercentCommon; } }

        #endregion Booleans

        }
}
