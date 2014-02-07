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
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheckRanged), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.Survival.EnableCallPet, PetManager.CreateHunterCallPetBehavior()),
                        new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Binding Shot", ret => TalentManager.IsSelected(4)))),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        G.InitializeOnKeyActionsH(),
                        new Decorator(ret => SG.Instance.Survival.EnablePetStuff, HandleCommon()),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Survival.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, SurvivalDefensive()),
                                        new Decorator(ret => SG.Instance.Survival.CheckInterrupts, SurvivalInterrupts()),
                                        SurvivalUtility(),
                                        new Action(ret => { Item.UseSurvivalItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        SurvivalOffensive(),
                                        new Decorator(ret => SG.Instance.Survival.CheckAoE && Unit.NearbyAttackableUnitsCount > 2, SurvivalMt()),
                                            SurvivalSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
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
                                        SurvivalSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite SurvivalSt()
        {
            return new PrioritySelector
            (
            HunterTrapBehavior(),
            Spell.Cast("Fervor", ret => FervorReqs),
            Spell.Cast("Explosive Shot", ret => LockAndLoadProc),
            Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
            Spell.PreventDoubleCast("Serpent Sting", 0.5, ret => SerpentStingRefresh),
            Spell.Cast("Explosive Shot"),
            Spell.Cast("Kill Shot", ret => TargetSoonDead),
            Spell.Cast("Black Arrow", on => Me.FocusedUnit, ret => SG.Instance.Survival.UseBlackArrowFocusTarget && Me.FocusedUnit != null),
            Spell.Cast("Black Arrow", ret => !SG.Instance.Survival.UseBlackArrowFocusTarget),
            Spell.Cast("Arcane Shot", ret => ThrillProc && BlackArrowIsOnCooldown),
            Spell.Cast("Dire Beast"),
            Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => SerpentStingRefresh6Seconds, true),
            Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => Focus67),
            Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus66, true),
            Spell.PreventDoubleCastHack("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        internal static Composite HunterTrapBehavior()
        {
            return new PrioritySelector(
                Spell.CreateHunterTrapBehavior("Explosive Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.ExplosiveTrap),
                Spell.CreateHunterTrapBehavior("Freezing Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.FreezingTrap),
                Spell.CreateHunterTrapBehavior("Ice Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.IceTrap),
                Spell.CreateHunterTrapBehavior("Snake Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Survival.EnableTraps && SG.Instance.Survival.TrapSwitch == Enum.Traps.SnakeTrap));
        }

        internal static Composite HandleCommon()
        {
            return new PrioritySelector(
                Spell.Cast("Mend Pet", ret => Me.Pet.HealthPercent <= SG.Instance.Beastmastery.MendPetHP && Me.Pet.IsAlive && !Me.Pet.HasAura("Mend Pet")));
        }


        internal static Composite SurvivalMt()
        {
            return new PrioritySelector(
                Spell.Cast("Glaive Toss"),
                Spell.Cast("Powershot"),
                Spell.Cast("Barrage"),
                Spell.Cast("Multi-Shot", ret => Lua.PlayerPower > 79),
                Spell.Cast("Black Arrow", on => Me.FocusedUnit, ret => SG.Instance.Survival.UseBlackArrowFocusTarget && Me.FocusedUnit != null),
                Spell.Cast("Black Arrow", ret => !SG.Instance.Survival.UseBlackArrowFocusTarget),
                Spell.Cast("Explosive Shot"),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Multi-Shot"),
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus66, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }


        internal static Composite SurvivalDefensive()
        {
            return new PrioritySelector(
                PetManager.CreateCastPetAction("Heart of the Phoenix", ret => SG.Instance.Survival.EnableRevivePet && (Me.Pet == null || (Me.Pet != null && !Me.Pet.IsAlive))),
                Item.SurvivalUseHealthStone()
                );
        }


        internal static Composite SurvivalOffensive()
        {
            return new PrioritySelector(
                //actions+=/a_murder_of_crows,if=enabled&!ticking
                //actions+=/lynx_rush,if=enabled&!dot.lynx_rush.ticking
                //actions+=/rapid_fire,if=!buff.rapid_fire.up
                //actions+=/stampede,if=buff.rapid_fire.up|buff.bloodlust.react|target.time_to_die<=25
                //PetManager.CreateCastPetActionOn("Rabid", ret => Me, ret => PetSettings.Rabid && Lists.TargetIsBoss(Target)), 
                Spell.Cast("A Murder of Crows", ret => MurderofCrows && (
                    (SG.Instance.Survival.MurderofCrows == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.MurderofCrows == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.MurderofCrows == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Lynx Rush", ret => LynxRush && (
                    (SG.Instance.Survival.LynxRush == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.LynxRush == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.LynxRush == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Rapid Fire", ret => RapidFireAura && (
                    (SG.Instance.Survival.RapidFire == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.RapidFire == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.RapidFire == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Stampede", ret => ( 
                    (SG.Instance.Survival.Stampede == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.Stampede == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.Stampede == Enum.AbilityTrigger.Always)
                    )),
                    PetManager.CreateCastPetActionOn("Rabid", ret => Me, ret => Me.CurrentTarget != null && (
                    (SG.Instance.Survival.Rabid == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.Rabid == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.Rabid == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite SurvivalUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite SurvivalInterrupts()
        {
            {
                return new PrioritySelector(
                    new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("Counter Shot", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
                      );
            }
        }

        #endregion


        #region Booleans

        internal static bool FervorReqs { get { return TalentManager.IsSelected(10) && Lua.PlayerPower <= 50; } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool TalentGlaiveToss { get { return TalentManager.IsSelected(16); } }
        internal static bool TalentPowershot { get { return TalentManager.IsSelected(17); } }
        internal static bool TalentBarrage { get { return TalentManager.IsSelected(18); } }
        internal static bool DireBeastEnabled { get { return TalentManager.IsSelected(11); } }
        internal static bool RapidFireAura { get { return !Me.HasAura(3045); } }
        internal static bool SerpentStingRefresh6Seconds { get { return Me.CurrentTarget != null && Spell.GetAuraTimeLeft("Serpent Sting", Me.CurrentTarget) < 6; } }
        internal static bool ExplosiveShotOffCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(53301).Cooldown; } }
        internal static bool TargetSoonDead { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent < 21; } }
        internal static bool MurderofCrows { get { return TalentManager.IsSelected(13) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(131894, Me.CurrentTarget) < 2; } }
        internal static bool LynxRush { get { return TalentManager.IsSelected(15) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(120697, Me.CurrentTarget) < 2; } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Serpent Sting"); } }
        internal static bool ThrillProc { get { return Me.HasAura(34720); } }
        internal static bool BlackArrowIsOnCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(3674).Cooldown; } }
        internal static bool Focus66 { get { return Lua.PlayerPower < 66; } }
        internal static bool Focus67 { get { return Lua.PlayerPower >= 67; } }



        #endregion Booleans

    }
}
