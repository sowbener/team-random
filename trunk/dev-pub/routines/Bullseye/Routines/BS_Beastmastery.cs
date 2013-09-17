using CommonBehaviors.Actions;
using Bullseye.Core;
using Bullseye.Helpers;
using Bullseye.Managers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = Bullseye.Routines.BsGlobal;
using I = Bullseye.Core.BsItem;
using Lua = Bullseye.Helpers.BsLua;
using T = Bullseye.Managers.BsTalentManager;
using SG = Bullseye.Interfaces.Settings.BsSettings;
using SH = Bullseye.Interfaces.Settings.BsSettingsH;
using Spell = Bullseye.Core.BsSpell;
using U = Bullseye.Core.BsUnit;
using Styx.CommonBot;

namespace Bullseye.Routines
{
    class BsBeastMastery
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeBeastMastery
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, BsLogger.TreePerformance("InitializeBeastMastery")),
                        new Decorator(ret => (BsHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => !SG.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions()),
                        new Decorator(ret => SG.Instance.General.CheckABsancedLogging, BsLogger.ABsancedLogging),
                        G.InitializeCaching(),
                        HandleCommon(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts && U.CanInterrupt, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        I.BeastmasteryUseItems(),
                                        BeastmasteryOffensive(),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAoE && (U.NearbyAttackableUnitsCount >= 2 || U.IsAoETarget), BeastmasteryMt()),
                                        BeastmasterySt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts && U.CanInterrupt, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        new Decorator(ret => BsHotKeyManager.IsCooldown,
                                        new PrioritySelector(
                                        I.BeastmasteryUseItems(),
                                        BeastmasteryOffensive())),
                                        new Decorator(ret => BsHotKeyManager.IsAoe, BeastmasteryMt()),
                                        BeastmasterySt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite BeastmasterySt()
        {
            return new PrioritySelector(
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location),
                Spell.Cast("Focus Fire", ret => FocusFireFiveStacks),
                Spell.Cast("Serpent Sting", ret => !SerpentStingRefresh),
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.Cast("Bestial Wrath", ret => BestialWrathNotUp),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Kill Command", ret => Me.Pet != null && Me.Pet.CurrentTarget != null && Me.Pet.Location.Distance(Me.Pet.CurrentTarget.Location) < 25f),
                Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
                Spell.Cast("Powershot", ret => TalentPowershot),
                Spell.Cast("Barrage", ret => TalentBarrage),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => !SerpentStingRefresh6Seconds, true),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => Focus61 || BestialWrathUp),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }


        internal static Composite BeastmasteryMt()
        {
            return new PrioritySelector(
                Spell.Cast("Serpent Sting", ret => !G.HasSerpentSting),
                Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
                Spell.Cast("Powershot", ret => TalentPowershot),
                Spell.Cast("Barrage", ret => TalentBarrage),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Multi-Shot"),              
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        internal static Composite BeastmasteryDefensive()
        {
            return new PrioritySelector(
                I.BeastmasteryUseHealthStone()
                );
        }

        internal static Composite HandleCommon()
        {
            return new PrioritySelector(
                Spell.Cast("Revive Pet", ret => !Me.Pet.IsAlive),
                Spell.Cast("Mend Pet", ret => Me.Pet.HealthPercent <= 40));
        }

        internal static Composite BeastmasteryOffensive()
        {
            return new PrioritySelector(
                   Spell.Cast("A Murder of Crows", ret => MurderofCrows && (
                    (SG.Instance.Beastmastery.MurderofCrows == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.MurderofCrows == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.MurderofCrows == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Lynx Rush", ret => LynxRush && (
                    (SG.Instance.Beastmastery.LynxRush == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.LynxRush == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.LynxRush == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Rapid Fire", ret => RapidFireAura && (
                    (SG.Instance.Beastmastery.RapidFire == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.RapidFire == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.RapidFire == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Stampede", ret => Me.CurrentTarget != null && (RapidFireAura || G.SpeedBuffsAura || Me.CurrentTarget.HealthPercent <= 25) && (
                    (SG.Instance.Beastmastery.Stampede == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.Stampede == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.Stampede == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
        }

        internal static Composite BeastmasteryUtility()
        {
            return new PrioritySelector(

                );
        }




        internal static Composite BeastmasteryInterrupts()
        {
            return new PrioritySelector(
                       );
        }
        #endregion

        #region Booleans

        internal static bool FervorReqs { get { return BsTalentManager.HasTalent(10) && Lua.PlayerPower <= 50; } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool TalentGlaiveToss { get { return BsTalentManager.HasTalent(16); } }
        internal static bool TalentPowershot { get { return BsTalentManager.HasTalent(17); } }
        internal static bool TalentBarrage { get { return BsTalentManager.HasTalent(18); } }
        internal static bool DireBeastEnabled { get { return BsTalentManager.HasTalent(11); } }
        internal static bool MurderofCrows { get { return BsTalentManager.HasTalent(13) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(131894, 0, 2000); } }
        internal static bool LynxRush { get { return BsTalentManager.HasTalent(15) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(120697, 0, 2000); } }
        internal static bool RapidFireAura { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(3045, 0, 2000); } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 2000); } }
        internal static bool SerpentStingAoE { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(1978); } }
        internal static bool SerpentStingRefresh6Seconds { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 6000); } }
        internal static bool ExplosiveShotOffCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(53301).Cooldown; } }
        internal static bool FocusFireFiveStacks { get { return Me.HasCachedAura(19615, 5); } }
        internal static bool BestialWrathNotUp { get { return Lua.PlayerPower > 60 && !Me.HasAura(34471); } }
        internal static bool BestialWrathUp { get { return Me.HasAura(34471); } }
        internal static bool TargetSoonDead { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent < 21; } }
        internal static bool MultiShotThrillProc { get { return Me.HasAura("Thrill of the Hunt") && !SerpentStingRefresh; } }
        internal static bool ThrillProc { get { return Me.HasAura("Thrill of the Hunt"); } }
        internal static bool BlackArrowIsOnCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(3674).Cooldown; } }
        internal static bool Focus60 { get { return Lua.PlayerPower < 60; } }
        internal static bool Focus61 { get { return Lua.PlayerPower >= 61; } }


        #endregion Booleans

    }
}
