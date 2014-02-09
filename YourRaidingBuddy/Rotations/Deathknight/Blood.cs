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

namespace YourBuddy.Rotations.Deathknight
{
    class Blood
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeBlood
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.CastOnGround("Death and Decay", on => Me.CurrentTarget.Location, ret => Me.CurrentTarget != null))),
                    //    AutoGoreGrasp(),
                    G.InitializeOnKeyActionsDK(),
                        G.ManualCastPause(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Blood.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BloodDefensive()),
                                        new Decorator(ret => SG.Instance.Blood.CheckInterrupts, BloodInterrupts()),
                                        BloodUtility(),
                                        new Action(ret => { Item.UseBloodItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                        BloodOffensive(),
                                        new Decorator(ret => SG.Instance.Blood.CheckAoE && Unit.NearbyAttackableUnitsCount > 2, BloodMt()),
                                            BloodSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Blood.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BloodDefensive()),
                                        new Decorator(ret => SG.Instance.Blood.CheckInterrupts, BloodInterrupts()),
                                        BloodUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        new Action(ret => { Item.UseBloodItems(); return RunStatus.Failure; }),
                                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                                        BloodOffensive())),
                                        new Decorator(ret => SG.Instance.Blood.CheckAoE && Unit.NearbyAttackableUnitsCount > 2, BloodMt()),
                                            BloodSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Blood.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BloodDefensive()),
                                        new Decorator(ret => SG.Instance.Blood.CheckInterrupts, BloodInterrupts()),
                                        BloodUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        new Action(ret => { Item.UseBloodItems(); return RunStatus.Failure; }),
                                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76095, ret => true, "Using Mogu Power Potion")),
                                                        BloodOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, BloodMt()),
                                        BloodSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite BloodSt()
        {
            return new PrioritySelector(
                Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTap),
                Spell.Cast("Outbreak", ret => NeedEitherDis),
                Spell.Cast("Blood Boil", ret => (NeedEitherDis && EitherDisIsUp) || (!NeedDeathStrike && HasCrimsonScourge) || (!NeedDeathStrike && HasCrimsonScourge && Me.IsMoving)),
                Spell.Cast("Plague Strike", ret => NeedBloodPlague),
                Spell.Cast("Icy Touch", ret => NeedFrostFever),
                Spell.Cast("Death Strike", ret => NeedDeathStrike),
                Spell.Cast("Rune Strike", ret => NeedRuneStrike),
                //Spell.PreventDoubleCast("Blood Boil", 0.5, ret => HasCrimsonScourge), // get rid of the Proc
                Spell.CastOnGround("Death and Decay", on => Me.CurrentTarget.Location, ret => Me.CurrentTarget != null && DeathKnightSettings.EnableDeathAndDecayAuto),
                Spell.Cast("Soul Reaper", ret => NeedSoulReaper && OkToUseBloodRuneForDamage),  // Never use Soul Reaping if you have no Blood runes, as this will cause Heart Strike to consume a Death rune, which should be saved for Death Strike.
                Spell.Cast(55050, ret => Me.BloodRuneCount > 0 && OkToUseBloodRuneForDamage));
          //      Spell.Cast("Death Coil", ret => Me.CurrentTarget != null && !Me.CurrentTarget.IsWithinMeleeRange && Me.CurrentRunicPower >= 90));
        }

        internal static Composite BloodMt()
        {
            return new PrioritySelector(
                Spell.PreventDoubleCast("Blood Tap", 0.5, ret => NeedBloodTap),
                Spell.Cast("Blood Boil", ret => HasCrimsonScourge), // get rid of the Proc
                Spell.Cast("Unholy Blight", ret => NeedEither && DeathKnightSettings.EnableUnholyBlight),
                Spell.Cast("Outbreak", ret => NeedEither && !TalentManager.IsSelected(1)),
                Spell.Cast("Plague Strike", ret => NeedBloodPlague && OutBreakOnCooldown),
                Spell.Cast("Icy Touch", ret => NeedFrostFever && OutBreakOnCooldown),
                Spell.CastOnGround("Death and Decay", on => Me.CurrentTarget.Location, ret => Me.CurrentTarget != null && DeathKnightSettings.EnableDeathAndDecayAuto),
                Spell.Cast("Death Strike", ret => NeedDeathStrike),
                Spell.PreventDoubleCast("Blood Boil", 0.5, ret => ((Me.BloodRuneCount > 0 && OkToUseBloodRuneForDamage) || HasCrimsonScourge) && !NeedEitherDis),
                Spell.Cast("Rune Strike", ret => NeedRuneStrike));
        }

        internal static Composite AutoGoreGrasp()
        {
            return new PrioritySelector(
                new Decorator(ret => DeathKnightSettings.EnableAutoGoreGrasp && TalentManager.IsSelected(16) && Unit.NearbyAttackableUnitsCount > 2,
                    new PrioritySelector(
                        Spell.Cast("Gorefiend's Grasp"))));
        }

        internal static Composite BloodDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Anti-Magic Shell", on => Me, ret => NeedAntiMagicShell),
                Spell.Cast("Bone Shield", on => Me, ret => NeedBoneShield),
                                  Spell.Cast("Rune Tap", on => Me, ret => NeedRuneTapWoTn), // Instant
                                  Spell.Cast("Rune Tap", ret => Me, ret => NeedRuneTap), // 30 sec cooldown - default 90 percentHP
                                  Spell.Cast("Death Pact", on => Me, ret => NeedDeathPact), // 2 min cooldown  - default 50 percentHP
                                  Spell.Cast("Raise Dead", on => Me, ret => NeedRaiseDead), // 2 min cooldown - default 50 percentHP
                                  Spell.Cast("Death Coil", on => Me, ret => NeedDeathCoilHeal), // 2 min cooldown  - default 60 percentHP
                                  Spell.Cast("Lichborne", on => Me, ret => NeedLichborne), // 2 min cooldown - default 60 percentHP
                                  Spell.Cast("Vampiric Blood", on => Me, ret => NeedVampiricBlood), // 1 min cooldown - default 60 percentHP
                                  Spell.Cast("Icebound Fortitude", on => Me, ret => NeedIceboundFortitude), // 3 min cooldown - default 60 percentHP
                                  Spell.Cast("Dancing Rune Weapon", ret => NeedDancingRuneWeapon), // 1.5 min cooldown - default 80 percentHP
                                  Spell.Cast("Empower Rune Weapon", on => Me, ret => NeedEmpowerRuneWeapon), // 5 min cooldown
                Item.BloodUseHealthStone()
                );
        }


        internal static Composite BloodOffensive()
        {
            return new PrioritySelector(
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

        internal static Composite BloodUtility()
        {
            return new PrioritySelector(
                );
        }
      

        internal static Composite BloodInterrupts()
        {
            {
                return new PrioritySelector(
                    new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("Mind Freeze", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))));
            }
        }
        #endregion


        #region booleans

        private static bool HasCrimsonScourge { get { return Me.HasAura(81141); } } // StyxWoW.Me.ActiveAuras.ContainsKey("Crimson Scourge")

        // The Idea here is to not stack our defensive abilitys but use them one at a time for maximum effectiveness.
        private static bool NoOtherCooldownActive(string cooldownToCheck)
        {
            {
                switch (cooldownToCheck)
                {
                    case "VampiricBlood":
                        if (!Me.HasAura("Bone Shield") &&
                            !Me.HasAura("Icebound Fortitude") &&
                            !Me.HasAura("Dancing Rune Weapon") &&
                            !Me.HasAura("Lichborne")) return true;
                        break;

                    case "BoneShield":
                        if (!Me.HasAura("Vampiric Blood") &&
                            !Me.HasAura("Icebound Fortitude") &&
                            !Me.HasAura("Dancing Rune Weapon") &&
                            !Me.HasAura("Lichborne") &&
                            CooldownTracker.SpellOnCooldown(49998)) return true;
                        break;

                    case "IceboundFortitude":
                        if (!Me.HasAura("Bone Shield") &&
                            !Me.HasAura("Vampiric Blood") &&
                            !Me.HasAura("Dancing Rune Weapon") &&
                            !Me.HasAura("Lichborne") &&
                            CooldownTracker.SpellOnCooldown(49998)) return true;
                        break;

                    case "DeathCoilHeal":
                        if (!Me.HasAura("Bone Shield") &&
                            !Me.HasAura("Vampiric Blood") &&
                            !Me.HasAura("Icebound Fortitude") &&
                            !Me.HasAura("Dancing Rune Weapon") &&
                            CooldownTracker.SpellOnCooldown(49998)) return true;
                        break;

                    case "DancingRuneWeapon":
                        if (!Me.HasAura("Bone Shield") &&
                            !Me.HasAura("Icebound Fortitude") &&
                            !Me.HasAura("Vampiric Blood") &&
                            !Me.HasAura("Lichborne") &&
                            CooldownTracker.SpellOnCooldown(49998)) return true;
                        break;
                }

                return false;
            }
        }

        // HandleDefensiveCooldowns
        private static bool NeedAntiMagicShell
        {
            get
            {
                if (!UseAntiMagicShell) return false;

                return Me.CurrentTarget != null &&
                       Me.CurrentTarget.Entry != 62983 && // if the target is Lei Shi we want to save AMS for the above Spray stackcount
                       Me.CurrentTarget.Entry != 62442 && // if the target is Tsulong we want to save AMS for the above Shadow Breath
                       Me.CurrentTarget.Entry != 60410 && // if the target is Elegon we want to save AMS for the above Celestial Breath
                       (Me.CurrentTarget.IsCasting || Me.CurrentTarget.ChanneledCastingSpellId != 0) &&
                       Me.CurrentTarget.IsTargetingMeOrPet; // If there targeting us
            }
        }

        private static bool NeedBloodTap { get { return TalentManager.IsSelected(13) && Spell.GetAuraStack(Me, 114851) > 4 && (G.DeathRuneSlotsActive < 1 || G.BloodRuneSlotsActive < 1); } }

        private static bool NeedBoneShield { get { return !Me.HasAura("Bone Shield"); } }

        private static bool NeedDeathPact { get { return UsePetSacrifice && Me.HealthPercent < PetSacrificePercent; } }

        private static bool NeedRuneTapWoTn { get { return UseRuneTapWoTn && Me.HealthPercent < RuneTapWoTnPercent && Me.HasAura("Will of the Necropolis"); } }

        private static bool NeedRuneTap { get { return UseRuneTap && Me.HealthPercent <= RuneTapPercent && Me.BloodRuneCount > 0 && !CooldownTracker.SpellOnCooldown(48982); } }

        private static bool NeedDeathCoilHeal { get { return Me.HealthPercent < DeathCoilHealPercent && Me.HasAura("Lichborne"); } }

        private static bool NeedVampiricBlood { get { return UseVampiricBlood && Me.HealthPercent < VampiricBloodPercent && NoOtherCooldownActive("VampiricBlood"); } }

        private static bool NeedDancingRuneWeapon { get { return UseDancingRuneWeapon && Me.HealthPercent < DancingRuneWeaponPercent && NoOtherCooldownActive("DancingRuneWeapon"); } }

        private static bool NeedLichborne { get { return UseLichborne && Me.HealthPercent < LichbornePercent && Me.CurrentRunicPower >= 60; } }

        private static bool NeedRaiseDead { get { return UsePetSacrifice && Me.HealthPercent < PetSacrificePercent; } }

        private static bool DeathAndDecayCooldown { get { return CooldownTracker.SpellOnCooldown(43265); } }

        private static bool OutBreakOnCooldown { get { return CooldownTracker.SpellOnCooldown(77575); } }

        private static bool NeedIceboundFortitude { get { return UseIceboundFortitude && Me.HealthPercent < IceboundFortitudePercent && NoOtherCooldownActive("IceboundFortitude"); } }

        private static bool NeedEmpowerRuneWeapon { get { return Me.CurrentTarget != null && DeathKnightSettings.UseEmpowerRuneWeapon && Me.CurrentTarget.IsBoss && StyxWoW.Me.FrostRuneCount < 1 && StyxWoW.Me.UnholyRuneCount < 2 && StyxWoW.Me.DeathRuneCount < 1; } }



        // HandleSingleTarget
        private static bool NeedDeathStrike
        {
            get
            {
                return Me.HealthPercent < DeathStrikePercent ||
                        DeathStrikeTracker.DeathStrikeOK || // Tracks if its ok to deathstrike.
                       (Me.HealthPercent < DeathStrikeBloodShieldPercent &&
                       (Me.HasAura("Blood Shield") && Me.ActiveAuras["Blood Shield"].TimeLeft.TotalSeconds < DeathStrikeBloodShieldTimeRemaining));
            }
        }

        private static bool NeedRuneStrike { get { return (Me.CurrentRunicPower >= RuneStrikePercent || Me.HealthPercent > 90) && Me.CurrentRunicPower >= 30 || (Me.CurrentRunicPower >= Me.MaxRunicPower); } }

        private static bool NeedSoulReaper { get { return Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange && OkToUseBloodRuneForDamage && Me.BloodRuneCount > 0 && Me.CurrentTarget.HealthPercent < 35; } }


        private static bool OkToUseBloodRuneForDamage { get { return !NeedRuneTap; } } // Lets not heart strike or use SoulReaper or blood boil if we need too use RuneTap

        // Diseases
        private static bool NeedToRefreshDiseasesWithBloodBoil { get { return ((Me.BloodRuneCount > 0 || HasCrimsonScourge || Me.DeathRuneCount >= 4) && NeedEitherDis); } }

        private static bool NeedEitherDis { get { return (NeedFrostFever || NeedBloodPlague); } }

        private static bool EitherDisIsUp { get { return (FrostFeverUp && BloodPlagueUp); } }

        private static bool FrostFeverUp { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasMyAura("Frost Fever") || Spell.GetAuraTimeLeft("Frost Fever", Me.CurrentTarget) < 2); } }

        private static bool BloodPlagueUp { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasMyAura("Blood Plague") || Spell.GetAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 2); } }

        private static bool NeedFrostFever { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasMyAura("Frost Fever") || Spell.GetAuraTimeLeft("Frost Fever", Me.CurrentTarget) < 2); } }

        private static bool NeedBloodPlague { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasMyAura("Blood Plague") || Spell.GetAuraTimeLeft("Blood Plague", Me.CurrentTarget) < 2); } }

        private static bool NeedEither { get { return NeedBloodPlague || NeedFrostFever; } }


        #endregion booleans

        #region Settings

        private static YbSettingsBD DeathKnightSettings { get { return InternalSettings.Instance.Blood; } }

        private static int PetSacrificePercent { get { return DeathKnightSettings.PetSacrificePercent; } }

        private static bool UsePetSacrifice { get { return DeathKnightSettings.UsePetSacrifice; } }

        private static int RuneTapPercent { get { return DeathKnightSettings.RuneTapPercent; } }

        private static bool UseRuneTap { get { return DeathKnightSettings.UseRuneTap; } }

        private static int RuneTapWoTnPercent { get { return DeathKnightSettings.RuneTapWoTNPercent; } }

        private static bool UseRuneTapWoTn { get { return DeathKnightSettings.UseRuneTapWoTN; } }

        private static int DeathCoilHealPercent { get { return DeathKnightSettings.DeathCoilHealPercent; } }

        private static int VampiricBloodPercent { get { return DeathKnightSettings.VampiricBloodPercent; } }

        private static bool UseVampiricBlood { get { return DeathKnightSettings.UseVampiricBlood; } }

        private static int DancingRuneWeaponPercent { get { return DeathKnightSettings.DancingRuneWeaponPercent; } }

        private static bool UseDancingRuneWeapon { get { return DeathKnightSettings.UseDancingRuneWeapon; } }

        private static int LichbornePercent { get { return DeathKnightSettings.LichbornePercent; } }

        private static bool UseLichborne { get { return DeathKnightSettings.UseLichborne; } }

        private static int IceboundFortitudePercent { get { return DeathKnightSettings.BloodIceboundFortitudePercent; } }

        private static bool UseIceboundFortitude { get { return DeathKnightSettings.UseIceboundFortitude; } }

        private static bool UseBoneShieldDefensively { get { return DeathKnightSettings.UseBoneShieldDefensively; } }

        private static bool UseAntiMagicShell { get { return DeathKnightSettings.UseAntiMagicShell; } }

        private static int DeathStrikePercent { get { return DeathKnightSettings.DeathStrikePercent; } }

        private static int DeathStrikeBloodShieldPercent { get { return DeathKnightSettings.DeathStrikeBloodShieldPercent; } }

        private static int DeathStrikeBloodShieldTimeRemaining { get { return DeathKnightSettings.DeathStrikeBloodShieldTimeRemaining; } }

        private static int RuneStrikePercent { get { return DeathKnightSettings.RuneStrikePercent; } }

        #endregion Settings

    }
}
