using FuryUnleashed.Core;
using FuryUnleashed.Shared.Helpers;
using FuryUnleashed.Shared.Managers;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using HK = FuryUnleashed.Shared.Managers.HotKeyManager;
using SG = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SH = FuryUnleashed.Interfaces.Settings.SettingsH;
using Spell = FuryUnleashed.Core.Spell;
using Enum = FuryUnleashed.Shared.Helpers.Enum;

namespace FuryUnleashed.Routines
{
    class FuGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { Unit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; }),
                new Decorator(ret => SG.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                new Switch<WoWSpec>(ret => Me.Specialization,

                    new SwitchArgument<WoWSpec>(WoWSpec.WarriorArms,
                        new PrioritySelector(
                            new Decorator(ret => SG.Instance.Arms.CheckAoE && SG.Instance.Arms.CheckAoEThunderclap && Unit.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { Unit.GetNeedThunderclapUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => SG.Instance.Arms.CheckInterruptsAoE && Unit.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { Unit.GetInterruptableUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => SG.Instance.Arms.CheckRallyingCry,
                                new Action(delegate { Unit.GetRaidMembersNeedCryCount(); return RunStatus.Failure; })),
                            new Action(delegate { Unit.GetNearbySlamCleaveUnitsCount(); return RunStatus.Failure; }))),

                    new SwitchArgument<WoWSpec>(WoWSpec.WarriorFury,
                        new PrioritySelector(
                            new Decorator(ret => SG.Instance.Fury.CheckInterruptsAoE && Unit.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { Unit.GetInterruptableUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => SG.Instance.Fury.CheckRallyingCry,
                                new Action(delegate { Unit.GetRaidMembersNeedCryCount(); return RunStatus.Failure; })))),

                    new SwitchArgument<WoWSpec>(WoWSpec.WarriorProtection,
                        new PrioritySelector(
                            new Decorator(ret => SG.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { Unit.GetNeedThunderclapUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => SG.Instance.Protection.CheckInterruptsAoE && Unit.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { Unit.GetInterruptableUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => SG.Instance.Protection.CheckRallyingCry,
                                new Action(delegate { Unit.GetRaidMembersNeedCryCount(); return RunStatus.Failure; }))))
                                ));
        }

        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => HK.IsKeyAsyncDown(SH.Instance.Tier4Choice),
                    new PrioritySelector(
                        Spell.Cast("Bladestorm", ret => BsTalent),
                        Spell.Cast("Dragon Roar", ret => DrTalent),
                        Spell.Cast("Shockwave", ret => SwTalent))),
                new Decorator(ret => HK.IsKeyAsyncDown(SH.Instance.ShatteringThrowChoice),
                    Spell.Cast("Shattering Throw")),
                new Decorator(ret => HK.IsKeyDown(SH.Instance.HeroicLeapChoice),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Heroic Leap");
                        Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        Logger.CombatLogP("Casting: Heroic Leap - On Mousecursor Location");
                    })),
                new Decorator(ret => HK.IsKeyDown(SH.Instance.DemoBannerChoice),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Demoralizing Banner");
                        Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        Logger.CombatLogP("Casting: Demoralizing Banner - On Mousecursor Location");
                    })),
                new Decorator(ret => HK.IsKeyDown(SH.Instance.MockingBannerChoice),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Mocking Banner");
                        Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        Logger.CombatLogP("Casting: Mocking Banner - On Mousecursor Location");
                    })));
        }

        internal static Composite InitializeInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, TimeSpan.FromMilliseconds(750), RunStatus.Failure,
                    Spell.Cast("Pummel")
                    ),
                new ThrottlePasses(1, TimeSpan.FromMilliseconds(750), RunStatus.Failure,
                    Spell.Cast("Disrupting Shout", ret => DsTalent && (PUOC || Unit.InterruptableUnitsCount >= 1))
                    ));
        }
        #endregion

        #region Racials
        internal static string SelectRacialSpell()
        {
            switch (Me.Race)
            {
                case WoWRace.BloodElf: return "Arcane Torrent";
                case WoWRace.Draenei: return "Gift of the Naaru";
                case WoWRace.Dwarf: return "Stoneform";
                case WoWRace.Gnome: return "Escape Artist";
                case WoWRace.Goblin: return "Rocket Barrage";
                case WoWRace.Human: return "Every Man for Himself";
                case WoWRace.NightElf: return "Shadowmeld";
                case WoWRace.Orc: return "Blood Fury";
                case WoWRace.Pandaren: return null;
                case WoWRace.Tauren: return "War Stomp";
                case WoWRace.Troll: return "Berserking";
                case WoWRace.Undead: return "Will of the Forsaken";
                case WoWRace.Worgen: return "Darkflight";
                default: return null;
            }
        }

        internal static bool RacialUsageSatisfied(string racial)
        {
            if (racial != null)
            {
                switch (racial)
                {
                    case "Stoneform": return IsSick;
                    case "Escape Artist": return Me.Rooted;
                    case "Every Man for Himself": return IsImpaired;
                    case "Shadowmeld": return Targeting.GetAggroOnMeWithin(Me.Location, 15) >= 1 && Me.HealthPercent < SG.Instance.General.RacialNum && !Me.IsMoving;
                    case "Gift of the Naaru": return Me.HealthPercent <= SG.Instance.General.RacialNum;
                    case "Darkflight": return Me.IsMoving;
                    case "Blood Fury": return true;
                    case "War Stomp": return Targeting.GetAggroOnMeWithin(StyxWoW.Me.Location, 8) >= 1;
                    case "Berserking": return !HasteAbilities;
                    case "Will of the Forsaken": return IsLazy;
                    case "Arcane Torrent": return Me.ManaPercent < SG.Instance.General.RacialNum && Me.Class != WoWClass.DeathKnight;
                    case "Rocket Barrage": return true;
                    default: return false;
                }
            }
            return false;
        }
        #endregion

        #region Booleans & Doubles

        // Fading Target Aura's
        internal static bool FadingCs(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura colossusSmash = Spell.CachedTargetAuras.FirstOrDefault(a => a.SpellId == 86346 && a.CreatorGuid == StyxWoW.Me.Guid);
            return colossusSmash != null && colossusSmash.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        internal static bool FadingDw(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura deepwounds = Spell.CachedTargetAuras.FirstOrDefault(a => a.SpellId == 115767 && a.CreatorGuid == StyxWoW.Me.Guid);
            return deepwounds != null && deepwounds.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        internal static bool FadingSunder(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura sunderArmor = Spell.CachedTargetAuras.FirstOrDefault(a => a.SpellId == 7386 && a.CreatorGuid == StyxWoW.Me.Guid);
            return sunderArmor != null && sunderArmor.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        internal static bool FadingWb(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura weakenedBlows = Spell.CachedTargetAuras.FirstOrDefault(a => a.SpellId == 115798 && a.CreatorGuid == StyxWoW.Me.Guid);
            return weakenedBlows != null && weakenedBlows.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        // Fading Self Aura's
        internal static bool FadingDeathSentence(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura deathsentence = Spell.CachedAuras.FirstOrDefault(a => a.SpellId == 144442 && a.CreatorGuid == StyxWoW.Me.Guid);
            return deathsentence != null && deathsentence.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        internal static bool FadingEnrage(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura enrage = Spell.CachedAuras.FirstOrDefault(a => (a.SpellId == 13046 || a.SpellId == 12880) && a.CreatorGuid == StyxWoW.Me.Guid);
            return enrage != null && enrage.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        internal static bool FadingRb(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura ragingBlow = Spell.CachedAuras.FirstOrDefault(a => a.SpellId == 131116 && a.CreatorGuid == StyxWoW.Me.Guid);
            return ragingBlow != null && ragingBlow.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        internal static bool FadingVc(int fadingtime)
        {
            if (!Me.GotTarget)
                return false;
            WoWAura vicRush = Spell.CachedAuras.FirstOrDefault(a => (a.SpellId == 32216 || a.SpellId == 138279) && a.CreatorGuid == StyxWoW.Me.Guid);
            return vicRush != null && vicRush.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
        }

        // Booleans for multiple use.
        internal static bool AlmostDead             { get { return Me.CurrentTarget.HealthPercent <= 10; } }
        internal static bool DumpAllRage            { get { return Me.CurrentTarget.HealthPercent <= 2.5; } }
        internal static bool ExecutePhase           { get { return Me.CurrentTarget.HealthPercent <= 20; } }
        internal static bool HotkeyMode             { get { return SH.Instance.ModeSelection == Enum.Mode.Hotkey || SH.Instance.ModeSelection == Enum.Mode.SemiHotkey; } }
        internal static bool NormalPhase            { get { return Me.CurrentTarget.HealthPercent > 20; } }
        internal static bool TargetNotNull          { get { return Me.CurrentTarget != null; } }
        internal static bool TargettingMe           { get { return Me.CurrentTarget.CurrentTargetGuid == Me.Guid; } }
        internal static bool HasteAbilities         { get { return (AncientHysteriaAura || BloodLustAura || HeroismAura || TimeWarpAura); } }
        internal static bool WieldsTwoHandedWeapons { get { return Item.WieldsTwoHandedWeapons; } }

        internal static bool IsImpaired
        {
            get
            {
                return Spell.CachedAuras.Any(a =>
                        a.Spell.Mechanic == WoWSpellMechanic.Dazed || a.Spell.Mechanic == WoWSpellMechanic.Disoriented ||
                        a.Spell.Mechanic == WoWSpellMechanic.Frozen || a.Spell.Mechanic == WoWSpellMechanic.Incapacitated ||
                        a.Spell.Mechanic == WoWSpellMechanic.Rooted || a.Spell.Mechanic == WoWSpellMechanic.Slowed ||
                        a.Spell.Mechanic == WoWSpellMechanic.Snared);
            }
        }

        internal static bool IsLazy
        {
            get
            {
                return Spell.CachedAuras.Any(a =>
                        a.Spell.Mechanic == WoWSpellMechanic.Fleeing || a.Spell.Mechanic == WoWSpellMechanic.Asleep ||
                        a.Spell.Mechanic == WoWSpellMechanic.Charmed);
            }
        }

        internal static bool IsSick
        {
            get
            {
                return Spell.CachedAuras.Any(a =>
                        a.Spell.Mechanic == WoWSpellMechanic.Bleeding || a.Spell.DispelType == WoWDispelType.Disease ||
                        a.Spell.DispelType == WoWDispelType.Poison);
            }
        }

        internal static bool SlamViable
        {
            get
            {
                var wwcost = WoWSpell.FromId(SpellBook.Whirlwind).PowerCost;
                var slamcost = WoWSpell.FromId(SpellBook.Slam).PowerCost;

                return (Unit.NearbySlamCleaveUnitsFloat * 3.4) / slamcost >= Unit.NearbyAttackableUnitsFloat / wwcost;
            }
        }

        internal static bool WhirlwindViable
        {
            get
            {
                var wwcost = WoWSpell.FromId(SpellBook.Whirlwind).PowerCost;
                var slamcost = WoWSpell.FromId(SpellBook.Slam).PowerCost;

                return Unit.NearbyAttackableUnitsFloat / wwcost >= (Unit.NearbySlamCleaveUnitsFloat * 3.4) / slamcost;
            }
        }

        // Specs
        internal static bool IsArmsSpec             { get { return TalentManager.CurrentSpec == WoWSpec.WarriorArms; } }
        internal static bool IsFurySpec             { get { return TalentManager.CurrentSpec == WoWSpec.WarriorFury; } }
        internal static bool IsProtSpec             { get { return TalentManager.CurrentSpec == WoWSpec.WarriorProtection; } }

        // Tierset Aura Detection
        // Somehow doesnt work with ID
        internal static bool Tier15TwoPieceBonus    { get { return Me.HasAura("Item - Warrior T15 DPS 2P Bonus"); } }               // Works - 138120
        internal static bool Tier15FourPieceBonus   { get { return Me.HasAura("Item - Warrior T15 DPS 4P Bonus"); } }               // Does not work - Triggers SkullBannerAuraT15
        internal static bool Tier15TwoPieceBonusT   { get { return Me.HasAura("Item - Warrior T15 Protection 2P Bonus"); } }        // Works - 138280
        internal static bool Tier15FourPieceBonusT  { get { return Me.HasAura("Item - Warrior T15 Protection 4P Bonus"); } }        // Works - 138281

        internal static bool Tier16TwoPieceBonus    { get { return Me.HasAura("Item - Warrior T16 DPS 2P Bonus"); } }               // Checked - Works - 144436 is one of the ID's.
        internal static bool Tier16FourPieceBonus   { get { return Me.HasAura("Item - Warrior T16 DPS 4P Bonus"); } }               // Unchecked
        internal static bool Tier16TwoPieceBonusT   { get { return Me.HasAura("Item - Warrior T16 Protection 2P Bonus"); } }        // Unchecked
        internal static bool Tier16FourPieceBonusT  { get { return Me.HasAura("Item - Warrior T16 Protection 4P Bonus"); } }        // Unchecked

        // Tierset Item Procs
        internal static bool DeathSentenceAuraT16   { get { return Me.HasCachedAura(144442, 0); } }         // T16 4P DPS
        internal static bool SkullBannerAuraT15     { get { return Me.HasAnyCachedAura(138127, 0); } }      // T15 4P DPS
        internal static bool VictoriousAuraT15      { get { return Me.HasCachedAura(138279, 0); } }         // T15 2P PROT

        // Cached Aura's - Can only be used with MY aura's (HasCachedAura).
        internal static bool AvatarAura             { get { return Me.HasCachedAura(107574, 0); } }
        internal static bool BattleStanceAura       { get { return Me.HasCachedAura(2457, 0); } }
        internal static bool BerserkerStanceAura    { get { return Me.HasCachedAura(2458, 0); } }
        internal static bool BloodbathAura          { get { return Me.HasCachedAura(12292, 0); } }
        internal static bool BloodsurgeAura         { get { return Me.HasCachedAura(46916, 0); } }
        internal static bool EnrageAura             { get { return Me.HasCachedAura(13046, 0) || Me.HasCachedAura(12880, 0); } }
        internal static bool LastStandAura          { get { return Me.HasCachedAura(12975, 0); } }
        internal static bool MeatCleaverAura        { get { return Me.HasCachedAura(85739, 0); } }
        internal static bool RagingBlowAura         { get { return Me.HasCachedAura(131116, 0); } }
        internal static bool ReadinessAura          { get { return Me.HasCachedAura("Readiness", 0); } } // Evil Eye of Galakras trinket Aura - Multi-ID ...
        internal static bool RecklessnessAura       { get { return Me.HasCachedAura(1719, 0); } }
        internal static bool RecklessnessAuraT      { get { return Me.HasCachedAura(1719, 0, 10000); } }
        internal static bool ShieldBarrierAura      { get { return Me.HasCachedAura(112048, 0); } }
        internal static bool ShieldBlockAura        { get { return Me.HasCachedAura(2565, 0); } }
        internal static bool SweepingStrikesAura    { get { return Me.HasCachedAura(12328, 0); } }
        internal static bool SuddenExecAura         { get { return Me.HasCachedAura(139958, 0); } }
        internal static bool TasteforBloodAura      { get { return Me.HasCachedAura(56636, 0); } }
        internal static bool UltimatumAura          { get { return Me.HasCachedAura(122510, 0); } }
        internal static bool VictoriousAura         { get { return Me.HasCachedAura(32216, 0) || Me.HasCachedAura(138279, 0); } }

        // Cached Target Aura's
        internal static bool ColossusSmashAura      { get { return Me.CurrentTarget.HasCachedAura(86346, 0); } }
        internal static bool ColossusSmashAuraT     { get { return Me.CurrentTarget.HasCachedAura(86346, 0, 5000); } }
        internal static bool DeepWoundsAura         { get { return Me.CurrentTarget.HasCachedAura(115767, 0); } }
        internal static bool HamstringAura          { get { return Me.CurrentTarget.HasAnyCachedAura(1715, 0); } }
        internal static bool SunderArmorAura        { get { return Me.CurrentTarget.HasCachedAura(7386, 0); } }
        internal static bool WeakenedBlowsAura      { get { return Me.CurrentTarget.HasAnyCachedAura(115798, 0); } }

        // Cached Stacked Aura's - Can only be used with MY aura's (HasCachedAura).
        internal static bool MeatCleaverAuraS1      { get { return Me.HasCachedAura(85739, 1); } }
        internal static bool MeatCleaverAuraS2      { get { return Me.HasCachedAura(85739, 2); } }
        internal static bool MeatCleaverAuraS3      { get { return Me.HasCachedAura(85739, 3); } }
        internal static bool RagingBlow1S           { get { return Me.HasCachedAura(131116, 1); } }
        internal static bool RagingBlow2S           { get { return Me.HasCachedAura(131116, 2); } }
        internal static bool TasteForBloodS3        { get { return Me.HasCachedAura(60503, 3); } }
        internal static bool TasteForBloodS4        { get { return Me.HasCachedAura(60503, 4); } }
        internal static bool TasteForBloodS5        { get { return Me.HasCachedAura(60503, 5); } }
        internal static bool SunderArmorAura3S      { get { return Me.CurrentTarget.HasCachedAura(7386, 3); } }

        // Cached Aura's - Can be used with ANY aura's (HasAnyCachedAura).
        internal static bool AncientHysteriaAura    { get { return Me.HasAnyCachedAura(90355, 0); } }
        internal static bool BattleShoutAura        { get { return Me.HasAnyCachedAura(6673, 0); } }
        internal static bool BloodLustAura          { get { return Me.HasAnyCachedAura(2825, 0); } }
        internal static bool CommandingShoutAura    { get { return Me.HasAnyCachedAura(469, 0); } }
        internal static bool HeroismAura            { get { return Me.HasAnyCachedAura(32182, 0); } }
        internal static bool TimeWarpAura           { get { return Me.HasAnyCachedAura(80353, 0); } }
        internal static bool SkullBannerAura        { get { return Me.HasAnyCachedAura(114206, 0) || Me.HasAnyCachedAura(138127, 0); } }
        internal static bool RallyingCryAura        { get { return Me.HasAnyCachedAura(97462, 0); } }
        
        // Talentmanager - HasTalents
        internal static bool JnTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Juggernaut); } }
        internal static bool DtTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.DoubleTime); } }
        internal static bool WbTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Warbringer); } }

        internal static bool ErTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.EnragedRegeneration); } }
        internal static bool ScTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.SecondWind); } }
        internal static bool IvTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.ImpendingVictory); } }

        internal static bool SsTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.StaggeringShout); } }
        internal static bool PhTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.PiercingHowl); } }
        internal static bool DsTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.DisruptingShout); } }

        internal static bool BsTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Bladestorm); } }
        internal static bool SwTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Shockwave); } }
        internal static bool DrTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.DragonRoar); } }

        internal static bool MrTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.MassSpellReflection); } }
        internal static bool SgTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Safeguard); } }
        internal static bool VgTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Vigilance); } }

		internal static bool AvTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Avatar); } }
		internal static bool BbTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.Bloodbath); } }
        internal static bool SbTalent { get { return TalentManager.HasTalent(Enum.WarriorTalents.StormBolt); } }
        
        // Talentmanager - HasGlyphs
        internal static bool HsGlyph { get { return TalentManager.HasGlyph("Hindering Strikes"); } }        // Hindering Strikes
        internal static bool UrGlyph { get { return TalentManager.HasGlyph("Unending Rage"); } }            // Unending Rage
        internal static bool IsGlyph { get { return TalentManager.HasGlyph("Intimidating Shout"); } }       // Intimidating Shout

        // Cooldown Tracker (Translate: Impending Victory On Cooldown)
        // ReSharper disable InconsistentNaming
        internal static bool BSOC { get { return Spell.SpellOnCooldown(46924); } }                        // Bladestorm
        internal static bool BTOC { get { return Spell.SpellOnCooldown(23881); } }                        // Bloodthirst
        internal static bool DBOC { get { return Spell.SpellOnCooldown(114203); } }                       // Demoralizing Banner
        internal static bool DROC { get { return Spell.SpellOnCooldown(118000); } }                       // Dragon Roar
        internal static bool DSOC { get { return Spell.SpellOnCooldown(102060); } }                       // Disrupting Shout
        internal static bool HLOC { get { return Spell.SpellOnCooldown(6544); } }                         // Heroic Leap
        internal static bool IVOC { get { return Spell.SpellOnCooldown(103840); } }                       // Impending Victory
        internal static bool MSOC { get { return Spell.SpellOnCooldown(12294); } }                        // Mortal Strike
        internal static bool PUOC { get { return Spell.SpellOnCooldown(6552); } }                         // Pummel
        internal static bool RVOC { get { return Spell.SpellOnCooldown(6572); } }                         // Revenge
        internal static bool SLOC { get { return Spell.SpellOnCooldown(1464); } }                         // Slam
        internal static bool SSOC { get { return Spell.SpellOnCooldown(23922); } }                        // Shield Slam
        internal static bool SWOC { get { return Spell.SpellOnCooldown(46968); } }                        // Shockwave
        internal static bool VROC { get { return Spell.SpellOnCooldown(34428); } }                        // Victory Rush

        // Cooldown Tracker (Translate: Bloodbath Cooldown)
        internal static double AVCD { get { return Spell.GetSpellCooldown(107574).TotalMilliseconds; } }       // Avatar
        internal static double BBCD { get { return Spell.GetSpellCooldown(12292).TotalMilliseconds; } }        // Bloodbath
        internal static double BTCD { get { return Spell.GetSpellCooldown(23881).TotalMilliseconds; } }        // Bloodthirst
        internal static double CSCD { get { return Spell.GetSpellCooldown(86346).TotalMilliseconds; } }        // Colossus Smash
        internal static double PUCD { get { return Spell.GetSpellCooldown(6552).TotalMilliseconds; } }         // Pummel
        internal static double SSCD { get { return Spell.GetSpellCooldown(23922).TotalMilliseconds; } }        // Shield Slam
        internal static double SBCD { get { return Spell.GetSpellCooldown(114207).TotalMilliseconds; } }       // Skull Banner
        internal static double SRCD { get { return Spell.GetSpellCooldown(23920).TotalMilliseconds; } }        // Spell Reflection
        internal static double RCCD { get { return Spell.GetSpellCooldown(1719).TotalMilliseconds; } }         // Recklessness
        // ReSharper restore InconsistentNaming
        #endregion

    }
}
