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
                        new Decorator(ret => SG.Instance.Beastmastery.EnableCallPet, PetManager.CreateHunterCallPetBehavior()),
                        new Decorator(ret => BsHotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Binding Shot", ret => BsTalentManager.HasTalent(4)))),
                        G.InitializeCaching(),
                        new Decorator(ret => SG.Instance.Beastmastery.EnablePetStuff, HandleCommon()),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        I.BeastmasteryUseItems(),
                                        BeastmasteryOffensive(),
                                       new Decorator(ret => Me.CurrentTarget != null && SG.Instance.Beastmastery.CheckAoE && BsUnit.NearbyAttackableUnitsCount >= SG.Instance.Beastmastery.AoECount, BeastmasteryMt()),
                            new Decorator(ret => Me.CurrentTarget != null && (BsUnit.NearbyAttackableUnitsCount < SG.Instance.Beastmastery.AoECount || !SG.Instance.Beastmastery.CheckAoE),
                                new PrioritySelector
                                (
                                    new Decorator(ret => !UseQuasiAoE, BeastmasterySt()),
                                    new Decorator(ret => UseQuasiAoE, BeastmasteryCleave())
                                )))),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        new Decorator(ret => BsHotKeyManager.IsCooldown,
                                        new PrioritySelector(
                                        I.BeastmasteryUseItems(),
                                        BeastmasteryOffensive())),
                                        new Decorator(ret => BsHotKeyManager.IsAoe, BeastmasteryMt()),
                                         new Decorator
                                   (ret => !BsHotKeyManager.IsAoe, new PrioritySelector
                                   (
                                    new Decorator(ret => !BsHotKeyManager.IsSpecialKey, BeastmasterySt()),
                                    new Decorator(ret => BsHotKeyManager.IsSpecialKey, BeastmasteryCleave())
                                )))));
            }
        }
        #endregion

        #region Rotations
        internal static Composite BeastmasterySt()
        {
            return new PrioritySelector(
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.General.EnableTraps),
                Spell.Cast("Focus Fire", ret => FocusFireStackCount == 5 && (!Me.HasAura(34471) || RapidFireAura)),
                Spell.PreventDoubleCast("Serpent Sting", 0.7, ret => !SerpentStingRefresh),
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.Cast("Bestial Wrath", ret => BestialWrathNotUp),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Kill Command", ret => Me.Pet != null && Me.Pet.CurrentTarget != null && Me.Pet.SpellDistance(Me.Pet.CurrentTarget) < 25f),
                new Decorator(ret => BestialWrathIsNotOnCooldown && BestialWrathNotUp, new ActionAlwaysSucceed()),
                Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
                Spell.Cast("Dire Beast", ret => Lua.PlayerPower <= 90),
                Spell.Cast("Powershot", ret => TalentPowershot),
                Spell.Cast("Barrage", ret => TalentBarrage),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => !SerpentStingRefresh6Seconds, true),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (ThrillProc && BestialWrathNotUp && BestialWrathIsNotOnCooldown && Lua.PlayerPower > 80) || (ThrillProc && BestialWrathUp)),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (KillCommandCooldown && Focus61 || BestialWrathUp) || Lua.PlayerPower > 90),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }


        internal static Composite BeastmasteryCleave()
        {
            return new PrioritySelector(
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.General.EnableTraps),
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
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => !SerpentStingRefresh6Seconds, true),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (ThrillProc && BestialWrathNotUp && BestialWrathIsNotOnCooldown && Lua.PlayerPower > 80) || (ThrillProc && BestialWrathUp)),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => (KillCommandCooldown && Focus61 || BestialWrathUp) || Lua.PlayerPower > 90),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
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
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus60, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        internal static Composite BeastmasteryDefensive()
        {
            return new PrioritySelector(
                PetManager.CreateCastPetAction("Heart of the Phoenix", ret => SG.Instance.Beastmastery.EnableRevivePet && Me.Pet != null && !Me.Pet.IsAlive),
                I.BeastmasteryUseHealthStone()
                );
        }

        internal static Composite HandleCommon()
        {
            return new PrioritySelector(
                Spell.Cast("Mend Pet", ret => Me.Pet.HealthPercent <= SG.Instance.General.MendPetHP && Me.Pet.IsAlive && !Me.Pet.HasAura("Mend Pet")));
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
                //Rabid
                Spell.Cast("Rapid", ret => (
                    (SG.Instance.Beastmastery.Rabid == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.Rabid == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.Rabid == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Stampede", ret => (
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
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(700, 2000)), RunStatus.Failure,
                    Spell.Cast("Counter Shot", ret => (SG.Instance.General.InterruptList == BsEnum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == BsEnum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
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

        internal static bool FervorReqs { get { return BsTalentManager.HasTalent(10) && Lua.PlayerPower <= 50; } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool TalentGlaiveToss { get { return BsTalentManager.HasTalent(16); } }
        internal static bool TalentPowershot { get { return BsTalentManager.HasTalent(17); } }
        internal static bool TalentBarrage { get { return BsTalentManager.HasTalent(18); } }
        internal static bool DireBeastEnabled { get { return BsTalentManager.HasTalent(11); } }
        internal static bool UseQuasiAoE { get { return SG.Instance.Beastmastery.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Beastmastery.AoEMultiShotCount; } }
        internal static bool MurderofCrows { get { return BsTalentManager.HasTalent(13) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(131894, 0, 2000); } }
        internal static bool LynxRush { get { return BsTalentManager.HasTalent(15) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(120697, 0, 2000); } }
        internal static bool RapidFireAura { get { return !Me.HasAura(3045); } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 2000); } }
        internal static bool SerpentStingAoE { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(1978); } }
        internal static bool SerpentStingRefresh6Seconds { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 6000); } }
        internal static bool ExplosiveShotOffCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(53301).Cooldown; } }
        internal static bool KillCommandCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(34026).Cooldown; } }
        internal static bool FocusFireFiveStacks { get { return FocusFireStackCount == 5; } }
        internal static bool BestialWrathNotUp { get { return Lua.PlayerPower > 60 && !Me.HasAura(34471); } }
        internal static bool BestialWrathUp { get { return Me.HasAura(34471); } }
        internal static bool TargetSoonDead { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent < 21; } }
        internal static bool MultiShotThrillProc { get { return Me.HasAura("Thrill of the Hunt") && !SerpentStingRefresh; } }
        internal static bool ThrillProc { get { return Me.HasAura("Thrill of the Hunt"); } }
        internal static bool BlackArrowIsOnCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(3674).Cooldown; } }
        internal static bool BestialWrathIsNotOnCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(19574).Cooldown; } }
        internal static bool BWSpellCoolDown2Seconds { get { return Styx.WoWInternals.WoWSpell.FromId(19574).CooldownTimeLeft.Milliseconds < 2000; } }
        internal static bool Focus60 { get { return Lua.PlayerPower < 64; } }
        internal static bool Focus61 { get { return Lua.PlayerPower >= 61; } }


        #endregion Booleans

    }
}
