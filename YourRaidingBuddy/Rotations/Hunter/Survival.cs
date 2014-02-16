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
namespace YourBuddy.Rotations.Hunter

{
    class Survival
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeSurvival
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => HotKeyManager.IsPaused || !U.DefaultCheckRanged, new ActionAlwaysSucceed()),
                    new Decorator(ret => SG.Instance.Survival.EnableCallPet, PetManager.CreateHunterCallPetBehavior()),
                    new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Binding Shot", ret => TalentManager.IsSelected(4)))),
                    G.InitializeCaching(),
                    G.ManualCastPause(),
                    G.InitializeOnKeyActionsH(),
                    new Decorator(ret => SG.Instance.Survival.EnablePetStuff, HandleCommon()),
                    new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                        new PrioritySelector(
                            new Decorator(ret => !Me.IsMoving && !Me.IsSafelyFacing(Me.CurrentTarget), new Action(ret => { Me.CurrentTarget.Face(); return RunStatus.Failure; })), //ret => SG.Instance.Survival.EnableFacing
                            new Decorator(ret => SG.Instance.Survival.CheckAutoAttack, Lua.StartAutoAttack),
                            new Decorator(ret => Me.HealthPercent < 100, SurvivalDefensive()),
                            new Decorator(ret => SG.Instance.Survival.CheckInterrupts, SurvivalInterrupts()),
                            SurvivalUtility(),
                            new Action(ret => { Item.UseSurvivalItems(); return RunStatus.Failure; }),
                            new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                            SurvivalOffensive(),
                            new Decorator(ret => SG.Instance.Survival.CheckAoE && U.NearbyTargetAttackableUnitsCount >= SG.Instance.Survival.AoECount, SurvivalMt()),
                            new Decorator(ret => U.NearbyTargetAttackableUnitsCount < SG.Instance.Survival.AoECount || !SG.Instance.Survival.CheckAoE,
                                new PrioritySelector(
                                    new Decorator(ret => !UseQuasiAoE, SurvivalSt()),
                                    new Decorator(ret => UseQuasiAoE, HandleQuasiAoE()))))),
                    new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                        new PrioritySelector(
                            new Decorator(ret => !Me.IsMoving && !Me.IsSafelyFacing(Me.CurrentTarget), new Action(ret => { Me.CurrentTarget.Face(); return RunStatus.Failure; })), //ret => SG.Instance.Survival.EnableFacing
                            new Decorator(ret => SG.Instance.Survival.CheckAutoAttack, Lua.StartAutoAttack),
                            new Decorator(ret => Me.HealthPercent < 100, SurvivalDefensive()),
                            new Decorator(ret => SG.Instance.Survival.CheckInterrupts, SurvivalInterrupts()),
                            SurvivalUtility(),
                            new Decorator(ret => HotKeyManager.IsCooldown,
                                new PrioritySelector(
                                    new Action(ret => { Item.UseSurvivalItems(); return RunStatus.Failure; }),
                                    new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                    SurvivalOffensive())),
                                    new Decorator(ret => SG.Instance.Survival.CheckAoE && U.NearbyTargetAttackableUnitsCount >= SG.Instance.Survival.AoECount, SurvivalMt()),
                                    new Decorator(ret => U.NearbyTargetAttackableUnitsCount < SG.Instance.Survival.AoECount || !SG.Instance.Survival.CheckAoE,
                                        new PrioritySelector(
                                            new Decorator(ret => !UseQuasiAoE, SurvivalSt()),
                                            new Decorator(ret => UseQuasiAoE, HandleQuasiAoE()))))),
                    new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                        new PrioritySelector(
                            new Decorator(ret => !Me.IsMoving && !Me.IsSafelyFacing(Me.CurrentTarget), new Action(ret => { Me.CurrentTarget.Face(); return RunStatus.Failure; })), //ret => SG.Instance.Survival.EnableFacing
                            new Decorator(ret => SG.Instance.Survival.CheckAutoAttack, Lua.StartAutoAttack),
                            new Decorator(ret => Me.HealthPercent < 100, SurvivalDefensive()),
                            new Decorator(ret => SG.Instance.Survival.CheckInterrupts, SurvivalInterrupts()),
                            SurvivalUtility(),
                            new Decorator(ret => HotKeyManager.IsCooldown,
                                new PrioritySelector(
                                    new Action(ret => { Item.UseSurvivalItems(); return RunStatus.Failure; }),
                                    new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                    SurvivalOffensive())),
                                    new Decorator(ret => HotKeyManager.IsAoe, SurvivalMt()),
                                    new Decorator(ret => !HotKeyManager.IsAoe, new PrioritySelector(
                                        new Decorator(ret => !HotKeyManager.IsSpecial, SurvivalSt()),
                                        new Decorator(ret => HotKeyManager.IsSpecial, HandleQuasiAoE()))))));
            }
        }
        #endregion

        #region Rotations
        internal static Composite SurvivalSt()
        {
            return new PrioritySelector(
                HunterTrapBehavior(),
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.PreventDoubleCast("Arcane Shot", 0.5, ret => Lua.PlayerPower >= 90 && LockAndLoadProc),
                Spell.Cast("Explosive Shot", ret => LockAndLoadProc),
                Spell.CastHack("Dire Beast", ret => TalentManager.IsSelected(11) && Me.CurrentTarget.CurrentHealth >= SG.Instance.General.MinHPAbility && TimeToDie >= 15),
                Spell.Cast("Glaive Toss", ret => TalentManager.IsSelected(16)),
                Spell.Cast("Powershot", ret => TalentManager.IsSelected(17)),
                Spell.Cast("Barrage", ret => TalentManager.IsSelected(18)),
                Spell.PreventDoubleCast("Serpent Sting", 0.5, ret => SerpentStingRefresh && !Me.CurrentTarget.DontDotUnit() && Me.CurrentTarget.CurrentHealth >= SG.Instance.General.MinHPAbility),
                Spell.Cast("Explosive Shot"),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Black Arrow", on => Me.FocusedUnit, ret => SG.Instance.Survival.UseBlackArrowFocusTarget && Me.FocusedUnit != null),
                Spell.Cast("Black Arrow", ret => !SG.Instance.Survival.UseBlackArrowFocusTarget && !Me.CurrentTarget.DontDotUnit() && TimeToDie >= 17),
                Spell.Cast("Arcane Shot", ret => ThrillProc && Lua.PlayerPower >= 46 || FervorAura && Lua.PlayerPower >= 56),
                Spell.PreventDoubleCast("Arcane Shot", 0.5, ret => Lua.PlayerPower >= 66),
                Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Lua.PlayerPower <= 65, true),
                Spell.PreventDoubleCastHack("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        private static Composite HandleQuasiAoE()
        {
            return new PrioritySelector(
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.Cast("Multi-Shot", ret => Lua.PlayerPower >= 90 && LockAndLoadProc),
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => Me.CurrentTarget.CurrentHealth >= SG.Instance.General.MinHPAbility && TimeToDie >= 17),
                Spell.Cast("Explosive Shot", ret => LockAndLoadProc),
                Spell.CastHack("Dire Beast", ret => TalentManager.IsSelected(11) && Me.CurrentTarget.CurrentHealth >= SG.Instance.General.MinHPAbility && TimeToDie >= 15),
                Spell.Cast("Glaive Toss", ret => TalentManager.IsSelected(16)),
                Spell.Cast("Powershot", ret => TalentManager.IsSelected(17)),
                Spell.Cast("Barrage", ret => TalentManager.IsSelected(18)),
                Spell.PreventDoubleCast("Serpent Sting", 0.5, ret => SerpentStingRefresh && !Me.CurrentTarget.DontDotUnit() && Me.CurrentTarget.CurrentHealth >= SG.Instance.General.MinHPAbility),
                Spell.Cast("Explosive Shot"),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Black Arrow", on => Me.FocusedUnit, ret => SG.Instance.Survival.UseBlackArrowFocusTarget && Me.FocusedUnit != null),
                Spell.Cast("Black Arrow", ret => !SG.Instance.Survival.UseBlackArrowFocusTarget && !Me.CurrentTarget.DontDotUnit() && TimeToDie >= 17),
                Spell.Cast("Multi-Shot", ret => ThrillProc && Lua.PlayerPower >= 56 || FervorAura && Lua.PlayerPower >= 66),
                Spell.PreventDoubleCast("Multi-Shot", 0.5, ret => Lua.PlayerPower >= 76),
                Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Lua.PlayerPower <= 75, true),
                Spell.PreventDoubleCastHack("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        internal static Composite SurvivalMt()
        {
            return new PrioritySelector(
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.Cast("Multi-Shot", ret => ThrillProc && Lua.PlayerPower >= 56 || FervorAura && Lua.PlayerPower >= 71),
                Spell.PreventDoubleCast("Multi-Shot", 0.5, ret => Lua.PlayerPower >= 76),
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => Me.CurrentTarget.CurrentHealth >= SG.Instance.General.MinHPAbility),
                Spell.CastHack("Dire Beast", ret => TalentManager.IsSelected(11) && Me.CurrentTarget.CurrentHealth >= SG.Instance.General.MinHPAbility && TimeToDie >= 15),
                Spell.Cast("Glaive Toss", ret => TalentManager.IsSelected(16)),
                Spell.Cast("Powershot", ret => TalentManager.IsSelected(17)),
                Spell.Cast("Barrage", ret => TalentManager.IsSelected(18)),
                Spell.Cast("Black Arrow", on => Me.FocusedUnit, ret => SG.Instance.Survival.UseBlackArrowFocusTarget && Me.FocusedUnit != null),
                Spell.Cast("Black Arrow", ret => !SG.Instance.Survival.UseBlackArrowFocusTarget && !Me.CurrentTarget.DontDotUnit() && TimeToDie >= 17),
                Spell.Cast("Explosive Shot", ret => LockAndLoadProc),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Lua.PlayerPower <= 75, true),
                Spell.PreventDoubleCastHack("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        internal static Composite HunterTrapBehavior()
        {
            return new PrioritySelector(
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.ExplosiveTrap),
                Spell.CastHunterTrap("Freezing Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.FreezingTrap),
                Spell.CastHunterTrap("Ice Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.IceTrap),
                Spell.CastHunterTrap("Snake Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.SnakeTrap));
        }

        internal static Composite HandleCommon()
        {
            return new PrioritySelector(
                Spell.Cast("Mend Pet", ret => Me.Pet.HealthPercent <= SG.Instance.Beastmastery.MendPetHP && Me.Pet.IsAlive && !Me.Pet.HasAura("Mend Pet")));
        }

        internal static Composite SurvivalDefensive()
        {
            return new PrioritySelector(
                PetManager.CreateCastPetAction("Heart of the Phoenix", ret => SG.Instance.Survival.EnableRevivePet && (Me.Pet == null || (Me.Pet != null && !Me.Pet.IsAlive))),
                Item.SurvivalUseHealthStone());
        }

        internal static Composite SurvivalOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("A Murder of Crows", ret => MurderofCrows && (
                    (SG.Instance.Survival.MurderofCrows == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.MurderofCrows == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.MurderofCrows == Enum.AbilityTrigger.Always))),
                Spell.Cast("Lynx Rush", ret => LynxRush && (
                    (SG.Instance.Survival.LynxRush == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.LynxRush == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.LynxRush == Enum.AbilityTrigger.Always))),
                Spell.Cast("Rapid Fire", ret => RapidFireAura && (
                    (SG.Instance.Survival.RapidFire == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.RapidFire == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.RapidFire == Enum.AbilityTrigger.Always))),
                Spell.Cast("Stampede", ret => ( 
                    (SG.Instance.Survival.Stampede == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.Stampede == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.Stampede == Enum.AbilityTrigger.Always))),
                PetManager.CreateCastPetActionOn("Rabid", ret => Me, ret => Me.CurrentTarget != null && (
                    (SG.Instance.Survival.Rabid == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.Rabid == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.Rabid == Enum.AbilityTrigger.Always))),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.Always))),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.Always))),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.Always))));
        }

        internal static Composite SurvivalUtility()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("Tranquilizing Shot", ret => Me.CurrentTarget.HasAura(145974)))); //Enrage from Kor'kron Jailer (Thok the Bloodthirsty - SoO)
        }

        internal static Composite SurvivalInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("Counter Shot", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                        (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))));
        }
        #endregion

        #region Booleans
        internal static bool FervorReqs { get { return TalentManager.IsSelected(10) && Lua.PlayerPower <= 50; } }
        internal static bool FervorAura { get { return Me.HasAura("Fervor"); } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool UseQuasiAoE { get { return SG.Instance.Survival.CheckAoE && U.NearbyTargetAttackableUnitsCount >= SG.Instance.Survival.AoEMultiShotCount; } }
        internal static bool RapidFireAura { get { return !Me.HasAura(3045); } }
        internal static bool TargetSoonDead { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 20; } }
        internal static bool MurderofCrows { get { return TalentManager.IsSelected(13) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(131894, Me.CurrentTarget) < 2; } }
        internal static bool LynxRush { get { return TalentManager.IsSelected(15) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(120697, Me.CurrentTarget) < 2; } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Serpent Sting"); } }
        internal static bool ThrillProc { get { return Me.HasAura(34720); } }

        internal static double TimeToDie
        {
            get
            {
                return StyxWoW.Me.CurrentTarget != null ? DpsMeter.GetCombatTimeLeft(StyxWoW.Me.CurrentTarget).TotalSeconds : 10;
            }
        }
        #endregion Booleans
    }
}
