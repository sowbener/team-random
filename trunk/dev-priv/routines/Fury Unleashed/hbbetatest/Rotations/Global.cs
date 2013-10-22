﻿using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Linq;
using AB = FuryUnleashed.Core.Helpers.AuraBook;
using Action = Styx.TreeSharp.Action;
using Enum = FuryUnleashed.Core.Helpers.Enum;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SB = FuryUnleashed.Core.Helpers.SpellBook;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Rotations
{
    class Global
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { U.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; }),
                new Switch<WoWSpec>(ret => Me.Specialization,

                    new SwitchArgument<WoWSpec>(WoWSpec.WarriorArms,
                        new PrioritySelector(
                            new Decorator(ret => IS.Instance.Arms.CheckAoE && IS.Instance.Arms.CheckAoEThunderclap && U.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { U.GetNeedThunderclapUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => IS.Instance.Arms.CheckInterruptsAoE && U.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { U.GetInterruptableUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => IS.Instance.Arms.CheckRallyingCry,
                                new Action(delegate { U.GetRaidMembersNeedCryCount(); return RunStatus.Failure; })),
                            new Action(delegate { U.GetNearbySlamCleaveUnitsCount(); return RunStatus.Failure; }))),

                    new SwitchArgument<WoWSpec>(WoWSpec.WarriorFury,
                        new PrioritySelector(
                            new Decorator(ret => IS.Instance.Fury.CheckInterruptsAoE && U.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { U.GetInterruptableUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => IS.Instance.Fury.CheckRallyingCry,
                                new Action(delegate { U.GetRaidMembersNeedCryCount(); return RunStatus.Failure; })))),

                    new SwitchArgument<WoWSpec>(WoWSpec.WarriorProtection,
                        new PrioritySelector(
                            new Decorator(ret => IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { U.GetNeedThunderclapUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => IS.Instance.Protection.CheckInterruptsAoE && U.NearbyAttackableUnitsCount > 1,
                                new Action(delegate { U.GetInterruptableUnitsCount(); return RunStatus.Failure; })),
                            new Decorator(ret => IS.Instance.Protection.CheckRallyingCry,
                                new Action(delegate { U.GetRaidMembersNeedCryCount(); return RunStatus.Failure; }))))
                                ));
        }

        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => HotKeyManager.IsKeyAsyncDown(SettingsH.Instance.Tier4Choice),
                    new PrioritySelector(
                        Spell.Cast("Bladestorm", ret => BsTalent),
                        Spell.Cast("Dragon Roar", ret => DrTalent),
                        Spell.Cast("Shockwave", ret => SwTalent))),
                new Decorator(ret => HotKeyManager.IsKeyAsyncDown(SettingsH.Instance.ShatteringThrowChoice),
                    Spell.Cast("Shattering Throw")),
                new Decorator(ret => HotKeyManager.IsKeyDown(SettingsH.Instance.HeroicLeapChoice),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Heroic Leap");
                        Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        Logger.CombatLogPu("Casting: Heroic Leap - On Mousecursor Location");
                    })),
                new Decorator(ret => HotKeyManager.IsKeyDown(SettingsH.Instance.DemoBannerChoice),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Demoralizing Banner");
                        Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        Logger.CombatLogPu("Casting: Demoralizing Banner - On Mousecursor Location");
                    })),
                new Decorator(ret => HotKeyManager.IsKeyDown(SettingsH.Instance.MockingBannerChoice),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Mocking Banner");
                        Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        Logger.CombatLogPu("Casting: Mocking Banner - On Mousecursor Location");
                    })));
        }

        internal static Composite InitializeInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, TimeSpan.FromMilliseconds(750), RunStatus.Failure,
                    Spell.Cast("Pummel")
                    ),
                new ThrottlePasses(1, TimeSpan.FromMilliseconds(750), RunStatus.Failure,
                    Spell.Cast("Disrupting Shout", ret => DsTalent && (PuOc || U.InterruptableUnitsCount >= 1))
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
                    case "Shadowmeld": return Targeting.GetAggroOnMeWithin(Me.Location, 15) >= 1 && Me.HealthPercent < IS.Instance.General.RacialNum && !Me.IsMoving;
                    case "Gift of the Naaru": return Me.HealthPercent <= IS.Instance.General.RacialNum;
                    case "Darkflight": return Me.IsMoving;
                    case "Blood Fury": return true;
                    case "War Stomp": return Targeting.GetAggroOnMeWithin(StyxWoW.Me.Location, 8) >= 1;
                    case "Berserking": return !HasteAbilities;
                    case "Will of the Forsaken": return IsLazy;
                    case "Arcane Torrent": return Me.ManaPercent < IS.Instance.General.RacialNum && Me.Class != WoWClass.DeathKnight;
                    case "Rocket Barrage": return true;
                    default: return false;
                }
            }
            return false;
        }
        #endregion

        #region Multi Category
        internal static bool AlmostDead
        {
            get { return Me.CurrentTarget.HealthPercent <= 10; }
        }

        internal static bool DumpAllRage
        {
            get { return Me.CurrentTarget.HealthPercent <= 2.5; }
        }

        internal static bool ExecutePhase
        {
            get { return Me.CurrentTarget.HealthPercent <= 20; }
        }

        internal static bool HasteAbilities
        {
            get { return (AncientHysteriaAura || BloodLustAura || HeroismAura || TimeWarpAura); }
        }

        internal static bool HotkeyMode
        {
            get { return SettingsH.Instance.ModeSelection == Enum.Mode.Hotkey || SettingsH.Instance.ModeSelection == Enum.Mode.SemiHotkey; }
        }

        internal static bool NormalPhase
        {
            get { return Me.CurrentTarget.HealthPercent > 20; }
        }

        internal static bool TargetNotNull
        {
            get { return Me.CurrentTarget != null; }
        }

        internal static bool TargettingMe
        {
            get { return Me.CurrentTarget.CurrentTargetGuid == Me.Guid; }
        }

        internal static bool WieldsTwoHandedWeapons
        {
            get { return Item.WieldsTwoHandedWeapons; }
        }

        // Arms Specific
        internal static bool SlamViable
        {
            get
            {
                var wwcost = WoWSpell.FromId(SB.Whirlwind).PowerCost;
                var slamcost = WoWSpell.FromId(SB.Slam).PowerCost;

                return (U.NearbySlamCleaveUnitsFloat * 3.4) / slamcost >= U.NearbyAttackableUnitsFloat / wwcost;
            }
        }

        internal static bool WhirlwindViable
        {
            get
            {
                var wwcost = WoWSpell.FromId(SB.Whirlwind).PowerCost;
                var slamcost = WoWSpell.FromId(SB.Slam).PowerCost;

                return U.NearbyAttackableUnitsFloat / wwcost >= (U.NearbySlamCleaveUnitsFloat * 3.4) / slamcost;
            }
        }
        #endregion

        #region HasAura - (wowunit, spellid, self(true) or any(false))
        // Needs checking
        internal static bool ReadinessAura
        {
            get { return Me.ActiveAuras.ContainsKey("Readiness"); } // Evil Eye of Galakras trinket Aura - Multi-ID
        }

        internal static bool Tier15TwoPieceBonus
        {
            get { return Me.ActiveAuras.ContainsKey("Item - Warrior T15 DPS 2P Bonus"); } // Works - 138120
        }

        internal static bool Tier15TwoPieceBonusT
        {
            get { return Me.ActiveAuras.ContainsKey("Item - Warrior T15 Protection 2P Bonus"); } // Works - 138280
        }

        internal static bool Tier15FourPieceBonusT
        {
            get { return Me.ActiveAuras.ContainsKey("Item - Warrior T15 Protection 4P Bonus"); } // Works - 138281
        }

        internal static bool Tier16TwoPieceBonus
        {
            get { return Me.ActiveAuras.ContainsKey("Item - Warrior T16 DPS 2P Bonus"); } // Checked - Works - 144436 is one of the ID's.
        }

        internal static bool Tier16TwoPieceBonusT
        {
            get { return Me.ActiveAuras.ContainsKey("Item - Warrior T16 Protection 2P Bonus"); } // Unchecked
        }

        internal static bool Tier16FourPieceBonusT
        {
            get { return Me.ActiveAuras.ContainsKey("Item - Warrior T16 Protection 4P Bonus"); } // Unchecked
        }

        // StyxWoW.Me
        internal static bool AncientHysteriaAura
        {
            get { return Spell.HasAura(Me, AB.AncientHysteria); }
        }

        internal static bool AvatarAura
        {
            get { return Spell.HasAura(Me, AB.Avatar, true); }
        }

        internal static bool BattleShoutAura
        {
            get { return Spell.HasAura(Me, AB.BattleShout); }
        }

        internal static bool BattleStanceAura
        {
            get { return Spell.HasAura(Me, AB.BattleStance, true); }
        }

        internal static bool BerserkerStanceAura
        {
            get { return Spell.HasAura(Me, AB.BerserkerStance, true); }
        }

        internal static bool BloodbathAura
        {
            get { return Spell.HasAura(Me, AB.Bloodbath, true); }
        }

        internal static bool BloodLustAura
        {
            get { return Spell.HasAura(Me, AB.Bloodlust); }
        }

        internal static bool BloodsurgeAura
        {
            get { return Spell.HasAura(Me, AB.Bloodsurge, true); }
        }

        internal static bool CommandingShoutAura
        {
            get { return Spell.HasAura(Me, AB.CommandingShout); }
        }

        internal static bool DeathSentenceAuraT16
        {
            get { return Spell.HasAura(Me, AB.DeathSentence, true); }
        }

        internal static bool EnrageAura
        {
            get { return Spell.HasAura(Me, AB.Enrage1, true) || Spell.HasAura(Me, AB.Enrage2, true); }
        }

        internal static bool HeroismAura
        {
            get { return Spell.HasAura(Me, AB.Heroism); }
        }

        internal static bool LastStandAura
        {
            get { return Spell.HasAura(Me, AB.LastStand, true); }
        }

        internal static bool MeatCleaverAura
        {
            get { return Spell.HasAura(Me, AB.MeatCleaver, true); }
        }

        internal static bool RagingBlowAura
        {
            get { return Spell.HasAura(Me, AB.RagingBlow, true); }
        }

        internal static bool RallyingCryAura
        {
            get { return Spell.HasAura(Me, AB.RallyingCry); }
        }

        internal static bool RecklessnessAura
        {
            get { return Spell.HasAura(Me, AB.Recklessness, true); }
        }

        internal static bool ShieldBarrierAura
        {
            get { return Spell.HasAura(Me, AB.ShieldBarrier, true); }
        }

        internal static bool ShieldBlockAura
        {
            get { return Spell.HasAura(Me, AB.ShieldBlock, true); }
        }

        internal static bool SkullBannerAura
        {
            get { return Spell.HasAura(Me, AB.SkullBanner1) || Spell.HasAura(Me, AB.SkullBanner2); }
        }

        internal static bool SkullBannerAuraT15
        {
            get { return Spell.HasAura(Me, AB.SkullBanner1); }
        }

        internal static bool SweepingStrikesAura
        {
            get { return Spell.HasAura(Me, AB.SweepingStrikes, true); }
        }

        internal static bool SuddenExecAura
        {
            get { return Spell.HasAura(Me, AB.SuddenExecute, true); }
        }

        internal static bool TasteforBloodAura
        {
            get { return Spell.HasAura(Me, AB.TasteforBlood, true); }
        }

        internal static bool TimeWarpAura
        {
            get { return Spell.HasAura(Me, AB.Timewarp); }
        }

        internal static bool UltimatumAura
        {
            get { return Spell.HasAura(Me, AB.Ultimatum, true); }
        }

        internal static bool VictoriousAura
        {
            get { return Spell.HasAura(Me, AB.Victorious, true) || Spell.HasAura(Me, AB.VictoriousT15, true); }
        }

        internal static bool VictoriousAuraT15
        {
            get { return Spell.HasAura(Me, AB.VictoriousT15, true); }
        }

        // StyxWoW.Me.CurrentTarget
        internal static bool ColossusSmashAura
        {
            get { return Spell.HasAura(Me.CurrentTarget, AB.ColossusSmash, true); }
        }

        internal static bool DeepWoundsAura
        {
            get { return Spell.HasAura(Me.CurrentTarget, AB.DeepWounds, true); }
        }

        internal static bool HamstringAura
        {
            get { return Spell.HasAura(Me.CurrentTarget, AB.Hamstring); }
        }

        internal static bool SunderArmorAura
        {
            get { return Spell.HasAura(Me.CurrentTarget, AB.SunderArmor); }
        }

        internal static bool WeakenedBlowsAura
        {
            get { return Spell.HasAura(Me.CurrentTarget, AB.WeakenedBlows); }
        }
        #endregion

        #region StackCount  - (wowunit, spellid, self(true) or any(false))
        // StyxWoW.Me.CurrentTarget
        internal static bool WeakenedArmor1S
        {
            get { return Spell.StackCount(Me.CurrentTarget, AB.SunderArmor, false) == 1; }
        }

        internal static bool WeakenedArmor2S
        {
            get { return Spell.StackCount(Me.CurrentTarget, AB.SunderArmor, false) == 2; }
        }

        internal static bool WeakenedArmor3S
        {
            get { return Spell.StackCount(Me.CurrentTarget, AB.SunderArmor, false) == 3; }
        }

        // StyxWoW.Me
        internal static bool MeatCleaverAuraS1
        {
            get { return Spell.StackCount(Me, AB.MeatCleaver) == 1; }
        }

        internal static bool MeatCleaverAuraS2
        {
            get { return Spell.StackCount(Me, AB.MeatCleaver) == 2; }
        }

        internal static bool MeatCleaverAuraS3
        {
            get { return Spell.StackCount(Me, AB.MeatCleaver) >= 3; }
        }

        internal static bool RagingBlow1S
        {
            get { return Spell.StackCount(Me, AB.RagingBlow) == 1; }
        }

        internal static bool RagingBlow2S
        {
            get { return Spell.StackCount(Me, AB.RagingBlow) >= 2; }
        }

        internal static bool TasteForBloodS3
        {
            get { return Spell.StackCount(Me, AB.TasteForBlood) == 3; }
        }

        internal static bool TasteForBloodS4
        {
            get { return Spell.StackCount(Me, AB.TasteForBlood) == 4; }
        }

        internal static bool TasteForBloodS5
        {
            get { return Spell.StackCount(Me, AB.TasteForBlood) >= 5; }
        }
        #endregion

        #region Remaining Aura's (int remainingtime)
        // StyxWoW.Me.CurrentTarget
        internal static bool RemainingCs(int remainingtime)
        {
            return Spell.RemainingAura(Me.CurrentTarget, AB.ColossusSmash, remainingtime);
        }
        #endregion

        #region Fading Aura's (int fadingtime)
        // StyxWoW.Me.CurrentTarget
        internal static bool FadingCs(int fadingtime)
        {
            return Spell.FadingAura(Me.CurrentTarget, AB.ColossusSmash, fadingtime);
        }

        internal static bool FadingDw(int fadingtime)
        {
            return Spell.FadingAura(Me.CurrentTarget, AB.DeepWounds, fadingtime);
        }

        internal static bool FadingSunder(int fadingtime)
        {
            return Spell.FadingAura(Me.CurrentTarget, AB.SunderArmor, fadingtime);
        }

        internal static bool FadingWb(int fadingtime)
        {
            return Spell.FadingAura(Me.CurrentTarget, AB.WeakenedBlows, fadingtime);
        }

        // StyxWoW.Me
        internal static bool FadingDeathSentence(int fadingtime)
        {
            return Spell.FadingAura(Me, AB.DeathSentence, fadingtime);
        }

        internal static bool FadingEnrage(int fadingtime)
        {
            return Spell.FadingAura(Me, AB.Enrage1, fadingtime) || Spell.FadingAura(Me, AB.Enrage2, fadingtime);
        }

        internal static bool FadingRb(int fadingtime)
        {
            return Spell.FadingAura(Me, AB.RagingBlow, fadingtime);
        }

        internal static bool FadingVc(int fadingtime)
        {
            return Spell.FadingAura(Me, AB.Victorious, fadingtime) || Spell.FadingAura(Me, AB.VictoriousT15, fadingtime);
        }
        #endregion

        #region Character States
        internal static bool IsImpaired
        {
            get
            {
                return Me.GetAllAuras().Any(a =>
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
                return Me.GetAllAuras().Any(a =>
                        a.Spell.Mechanic == WoWSpellMechanic.Fleeing || a.Spell.Mechanic == WoWSpellMechanic.Asleep ||
                        a.Spell.Mechanic == WoWSpellMechanic.Charmed);
            }
        }

        internal static bool IsSick
        {
            get
            {
                return Me.GetAllAuras().Any(a =>
                        a.Spell.Mechanic == WoWSpellMechanic.Bleeding || a.Spell.DispelType == WoWDispelType.Disease ||
                        a.Spell.DispelType == WoWDispelType.Poison);
            }
        }
        #endregion

        #region TalentManager Functions
        // Talentmanager - HasGlyphs
        internal static bool CsGlyph
        {
            get { return TalentManager.HasGlyph("Colossus Smash"); }
        }

        internal static bool HsGlyph
        {
            get { return TalentManager.HasGlyph("Hindering Strikes"); }
        }

        internal static bool UrGlyph
        {
            get { return TalentManager.HasGlyph("Unending Rage"); }
        }

        internal static bool IsGlyph
        {
            get { return TalentManager.HasGlyph("Intimidating Shout"); }
        }

        // Talentmanager CurrentSpec
        internal static bool IsArmsSpec
        {
            get { return TalentManager.CurrentSpec == WoWSpec.WarriorArms; }
        }

        internal static bool IsFurySpec
        {
            get { return TalentManager.CurrentSpec == WoWSpec.WarriorFury; }
        }

        internal static bool IsProtSpec
        {
            get { return TalentManager.CurrentSpec == WoWSpec.WarriorProtection; }
        }

        // Talentmanager - HasTalents
        // Tier 1 Talents
        internal static bool JnTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Juggernaut); }
        }

        internal static bool DtTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.DoubleTime); }
        }

        internal static bool WbTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Warbringer); }
        }

        // Tier 2 Talents
        internal static bool ErTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.EnragedRegeneration); }
        }

        internal static bool ScTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.SecondWind); }
        }

        internal static bool IvTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.ImpendingVictory); }
        }

        // Tier 3 Talents
        internal static bool SsTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.StaggeringShout); }
        }

        internal static bool PhTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.PiercingHowl); }
        }

        internal static bool DsTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.DisruptingShout); }
        }

        // Tier 4 Talents
        internal static bool BsTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Bladestorm); }
        }

        internal static bool SwTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Shockwave); }
        }

        internal static bool DrTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.DragonRoar); }
        }

        // Tier 5 Talents
        internal static bool MrTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.MassSpellReflection); }
        }

        internal static bool SgTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Safeguard); }
        }

        internal static bool VgTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Vigilance); }
        }

        // Tier 6 Talents
        internal static bool AvTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Avatar); }
        }

        internal static bool BbTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.Bloodbath); }
        }

        internal static bool SbTalent
        {
            get { return TalentManager.HasTalent(Enum.WarriorTalents.StormBolt); }
        }
        #endregion

        #region Cooldowntracker
        // Cooldowntracker - Timeleft in TotalMilliseconds
        internal static double AvCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.Avatar).TotalMilliseconds; } // Avatar
        }

        internal static double BbCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.Bloodbath).TotalMilliseconds; } // Bloodbath
        }

        internal static double BtCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.Bloodthirst).TotalMilliseconds; } // Bloodthirst
        }

        internal static double CsCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.ColossusSmash).TotalMilliseconds; } // Colossus Smash
        }

        internal static double PuCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.Pummel).TotalMilliseconds; } // Pummel
        }

        internal static double SsCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.ShieldSlam).TotalMilliseconds; } // Shield Slam
        }

        internal static double SbCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.SkullBanner).TotalMilliseconds; } // Skull Banner
        }

        internal static double SrCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.SpellReflection).TotalMilliseconds; } // Spell Reflection
        }

        internal static double RcCd
        {
            get { return CooldownTracker.GetSpellCooldown(SB.Recklessness).TotalMilliseconds; } // Recklessness
        }
 
        // Spells on Cooldown
        internal static bool BrOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.BerserkerRage); } // Berserker Rage
        }

        internal static bool BsOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.Bladestorm); } // Bladestorm
        }

        internal static bool BtOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.Bloodthirst); } // Bloodthirst
        }

        internal static bool CsOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.ColossusSmash); } // Colossus Smash
        }

        internal static bool DbOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.DemoralizingBanner); } // Demoralizing Banner
        } 

        internal static bool DrOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.DragonRoar); } // Dragon Roar
        }

        internal static bool DsOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.DisruptingShout); } // Disrupting Shout
        }

        internal static bool HlOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.HeroicLeap); } // Heroic Leap
        }

        internal static bool IvOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.ImpendingVictory); } // Impending Victory
        }

        internal static bool MsOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.MortalStrike); } // Mortal Strike
        }

        internal static bool PuOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.Pummel); } // Pummel
        }

        internal static bool RbOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.RagingBlow); } // Raging Blow
        }

        internal static bool RvOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.Revenge); } // Revenge
        }

        internal static bool SbOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.StormBolt); } // Storm Bolt
        }

        internal static bool SlOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.Slam); } // Slam
        }

        internal static bool SsOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.ShieldSlam); }// Shield Slam
        }

        internal static bool SwOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.Shockwave); } // Shockwave
        }

        internal static bool VrOc
        {
            get { return CooldownTracker.SpellOnCooldown(SB.VictoryRush); } // Victory Rush
        }
        #endregion
    }
}
