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
    class BeastMastery
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeBeastMastery
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheckRanged), new ActionAlwaysSucceed()),
                        G.InitializeOnKeyActionsH(),
                        G.AutoTarget(),
                        new Decorator(ret => SG.Instance.Beastmastery.EnableCallPet, PetManager.CreateHunterCallPetBehavior()),
                        new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Binding Shot", ret => TalentManager.IsSelected(4)))),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        new Decorator(ret => SG.Instance.Beastmastery.EnablePetStuff, HandleCommon()),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        new Action(ret => { Item.UseBeastmasteryItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        BeastmasteryOffensive(),
                                       new Decorator(ret => Me.CurrentTarget != null && SG.Instance.Beastmastery.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Beastmastery.AoECount, BeastmasteryMt()),
                            new Decorator(ret => Me.CurrentTarget != null && (U.NearbyAttackableUnitsCount < SG.Instance.Beastmastery.AoECount || !SG.Instance.Beastmastery.CheckAoE),
                                new PrioritySelector
                                (
                                    new Decorator(ret => !UseQuasiAoE, BeastmasterySt()),
                                    new Decorator(ret => UseQuasiAoE, BeastmasteryCleave())
                                )))),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                        new PrioritySelector(
                                        new Action(ret => { Item.UseBeastmasteryItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        BeastmasteryOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, BeastmasteryMt()),
                                         new Decorator
                                   (ret => !HotKeyManager.IsAoe, new PrioritySelector
                                   (
                                    new Decorator(ret => !HotKeyManager.IsSpecial, BeastmasterySt()),
                                    new Decorator(ret => HotKeyManager.IsSpecial, BeastmasteryCleave())
                                )))));
            }
        }
        #endregion

        #region Rotations
        internal static Composite BeastmasterySt()
        {
            return new PrioritySelector(
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.Beastmastery.EnableTraps),
                Spell.Cast("Focus Fire", ret => FocusFireStackCount == 5 && (!Me.HasAura(34471) || RapidFireAura)),
                Spell.PreventDoubleCast("Serpent Sting", 0.5, ret => SerpentStingRefresh),
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.Cast("Bestial Wrath", ret => BestialWrathNotUp),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Kill Command", ret => Me.Pet != null && Me.Pet.CurrentTarget != null && Me.Pet.SpellDistance(Me.Pet.CurrentTarget) < 25f),
                new Decorator(ret => BestialWrathIsNotOnCooldown && BestialWrathNotUp, new ActionAlwaysSucceed()),
                Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
                Spell.Cast("Dire Beast", ret => Lua.PlayerPower <= 90),
                Spell.Cast("Powershot", ret => TalentPowershot),
                Spell.Cast("Barrage", ret => TalentBarrage),
                Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => !SerpentStingRefresh6Seconds, true),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (ThrillProc && BestialWrathNotUp && BestialWrathIsNotOnCooldown && Lua.PlayerPower > 80) || (ThrillProc && BestialWrathUp)),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (KillCommandCooldown && Focus61 || BestialWrathUp) || Lua.PlayerPower > 90),
                Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }


        internal static Composite HunterTrapBehavior()
        {
            return new PrioritySelector(
                Spell.CreateHunterTrapBehavior("Explosive Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Beastmastery.EnableTraps && SG.Instance.Beastmastery.TrapSwitch == Enum.Traps.ExplosiveTrap),
                Spell.CreateHunterTrapBehavior("Freezing Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Beastmastery.EnableTraps && SG.Instance.Beastmastery.TrapSwitch == Enum.Traps.FreezingTrap),
                Spell.CreateHunterTrapBehavior("Ice Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Beastmastery.EnableTraps && SG.Instance.Beastmastery.TrapSwitch == Enum.Traps.IceTrap),
                Spell.CreateHunterTrapBehavior("Snake Trap", true, loc => Me.CurrentTarget, ret => SG.Instance.Beastmastery.EnableTraps && SG.Instance.Beastmastery.TrapSwitch == Enum.Traps.SnakeTrap));
        }

        internal static Composite BeastmasteryCleave()
        {
            return new PrioritySelector(
                Spell.Cast("Focus Fire", ret => FocusFireStackCount == 5 && (!Me.HasAura(34471) || RapidFireAura)),
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.Cast("Bestial Wrath", ret => BestialWrathNotUp),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Kill Command", ret => Me.Pet != null && Me.Pet.CurrentTarget != null && Me.Pet.SpellDistance(Me.Pet.CurrentTarget) < 25f),
                new Decorator(ret => BestialWrathIsNotOnCooldown && BestialWrathNotUp, new ActionAlwaysSucceed()),
                Spell.PreventDoubleCast("Multi Shot", 1, ret => Lua.PlayerPower > 40),
                Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
                Spell.Cast("Dire Beast", ret => Lua.PlayerPower <= 90),
                Spell.Cast("Powershot", ret => TalentPowershot),
                Spell.Cast("Barrage", ret => TalentBarrage),
                Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => SerpentStingRefresh6Seconds, true),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (ThrillProc && BestialWrathNotUp && BestialWrathIsNotOnCooldown && Lua.PlayerPower > 80) || (ThrillProc && BestialWrathUp)),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (KillCommandCooldown && Focus61 || BestialWrathUp) || Lua.PlayerPower > 90),
                Spell.PreventDoubleCastHack("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }


        internal static Composite BeastmasteryMt()
        {
            return new PrioritySelector(
                Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
                Spell.Cast("Powershot", ret => TalentPowershot),
                Spell.Cast("Barrage", ret => TalentBarrage),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Fervor", ret => FervorReqs && SG.Instance.Beastmastery.UseTier4AoE),
                Spell.Cast("Dire Beast", ret => Lua.PlayerPower <= 90 && SG.Instance.Beastmastery.UseTier4AoE),
                Spell.Cast("Bestial Wrath", ret => BestialWrathNotUp),
                Spell.Cast("Multi-Shot"),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        internal static Composite BeastmasteryDefensive()
        {
            return new PrioritySelector(
                PetManager.CreateCastPetAction("Heart of the Phoenix", ret => SG.Instance.Beastmastery.EnableRevivePet && Me.Pet != null && !Me.Pet.IsAlive),
                Item.BeastmasteryUseHealthStone()
                );
        }

        internal static Composite HandleCommon()
        {
            return new PrioritySelector(
                Spell.Cast("Mend Pet", ret => Me.Pet.HealthPercent <= SG.Instance.Beastmastery.MendPetHP && Me.Pet.IsAlive && !Me.Pet.HasAura("Mend Pet")));
        }

        internal static Composite BeastmasteryOffensive()
        {
            return new PrioritySelector(
                   Spell.Cast("A Murder of Crows", ret => MurderofCrows && (
                    (SG.Instance.Beastmastery.MurderofCrows == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.MurderofCrows == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.MurderofCrows == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Lynx Rush", ret => LynxRush && (
                    (SG.Instance.Beastmastery.LynxRush == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.LynxRush == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.LynxRush == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Rapid Fire", ret => RapidFireAura && (
                    (SG.Instance.Beastmastery.RapidFire == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.RapidFire == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.RapidFire == Enum.AbilityTrigger.Always)
                    )),
                //Rabid
                Spell.Cast("Rapid", ret => (
                    (SG.Instance.Beastmastery.Rabid == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.Rabid == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.Rabid == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Stampede", ret => (
                    (SG.Instance.Beastmastery.Stampede == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.Stampede == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.Stampede == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite BeastmasteryUtility()
        {
            return new PrioritySelector(

                );
        }




        internal static Composite BeastmasteryInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("Counter Shot", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
                      );
        }
        #endregion

        #region Booleans

        private static uint FocusFireStackCount
        {
            get
            {
                return Spell.GetAuraStackCount("Frenzy");
            }
        }

        internal static bool FervorReqs { get { return TalentManager.IsSelected(10) && Lua.PlayerPower <= 50; } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool TalentGlaiveToss { get { return TalentManager.IsSelected(16); } }
        internal static bool TalentPowershot { get { return TalentManager.IsSelected(17); } }
        internal static bool TalentBarrage { get { return TalentManager.IsSelected(18); } }
        internal static bool DireBeastEnabled { get { return TalentManager.IsSelected(11); } }
        internal static bool UseQuasiAoE { get { return  SG.Instance.Beastmastery.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Beastmastery.AoEMultiShotCount; } }
        internal static bool RapidFireAura { get { return !Me.HasAura(3045); } }
        internal static bool SerpentStingAoE { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura(1978); } }
        internal static bool SerpentStingRefresh6Seconds { get { return Me.CurrentTarget != null && Spell.GetAuraTimeLeft("Serpent Sting", Me.CurrentTarget) < 6; } }
        internal static bool MurderofCrows { get { return TalentManager.IsSelected(13) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(131894, Me.CurrentTarget) < 2; } }
        internal static bool LynxRush { get { return TalentManager.IsSelected(15) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(120697, Me.CurrentTarget) < 2; } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Serpent Sting"); } }
        internal static bool ExplosiveShotOffCooldown { get { return !CooldownTracker.SpellOnCooldown(53301); } }
        internal static bool KillCommandCooldown { get { return CooldownTracker.SpellOnCooldown(34026); } }
        internal static bool FocusFireFiveStacks { get { return FocusFireStackCount == 5; } }
        internal static bool BestialWrathNotUp { get { return Lua.PlayerPower > 60 && !Me.HasAura(34471); } }
        internal static bool BestialWrathUp { get { return Me.HasAura(34471); } }
        internal static bool TargetSoonDead { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent < 21; } }
        internal static bool ThrillProc { get { return Me.HasAura("Thrill of the Hunt"); } }
        internal static bool BlackArrowIsOnCooldown { get { return CooldownTracker.SpellOnCooldown(3674); } }
        internal static bool BestialWrathIsNotOnCooldown { get { return !CooldownTracker.SpellOnCooldown(19574); } }
        internal static bool BWSpellCoolDown2Seconds { get { return CooldownTracker.GetSpellCooldown(19574).Milliseconds < 2000; } }
        internal static bool Focus60 { get { return Lua.PlayerPower < 64; } }
        internal static bool Focus61 { get { return Lua.PlayerPower >= 61; } }


        #endregion Booleans

    }
}
