// ReSharper disable InconsistentNaming
using System;
using System.Linq;
using System.Windows.Forms;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using YBMoP_BT_Warrior.Core;
using YBMoP_BT_Warrior.Helpers;
using YBMoP_BT_Warrior.Managers;
using Action = Styx.TreeSharp.Action;
using SF = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsF;
using SG = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsG;
using SP = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsP;

namespace YBMoP_BT_Warrior.Routines
{
    class YBGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
        internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                    new Action(delegate { YBSpell.GetCachedAuras(); return RunStatus.Failure; }),
                    new Decorator(ret => Me.Specialization == WoWSpec.WarriorFury && YBUnit.DefaultBuffCheck,
                        new PrioritySelector(
                            YBSpell.Cast("Battle Shout", ret => SF.Instance.ShoutSelector == YBEnum.Shouts.Battle && !BattleShoutAura),
                            YBSpell.Cast("Commanding Shout", ret => SF.Instance.ShoutSelector == YBEnum.Shouts.Commanding && !CommandingShoutAura))),
                    new Decorator(ret => Me.Specialization == WoWSpec.WarriorProtection && YBUnit.DefaultBuffCheck,
                        new PrioritySelector(
                            YBSpell.Cast("Battle Shout", ret => SP.Instance.ShoutSelector == YBEnum.Shouts.Battle && !BattleShoutAura),
                            YBSpell.Cast("Commanding Shout", ret => SP.Instance.ShoutSelector == YBEnum.Shouts.Commanding && !CommandingShoutAura))));
            }
        }

        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { YBSpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { YBUnit.GetAttackableMeleeUnitsCount(); return RunStatus.Failure; })
                );
        }

        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => !HLCDOC && SG.Instance.HeroicLeapChoice != Keys.None && HotKeyManager.IsKeyDown(SG.Instance.HeroicLeapChoice),
                    new ThrottlePasses(1, TimeSpan.FromSeconds(30), RunStatus.Failure,
                        new Action(ret =>
                        {
                            SpellManager.Cast("Heroic Leap");
                            Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                            YBLogger.CombatLogP("Casting: Heroic Leap - On Mouse Location");
                        }))),
                new Decorator(ret => !DBCDOC && SG.Instance.DemoBannerChoice != Keys.None && HotKeyManager.IsKeyDown(SG.Instance.DemoBannerChoice),
                    new ThrottlePasses(1, TimeSpan.FromSeconds(180), RunStatus.Failure,
                        new Action(ret =>
                        {
                            SpellManager.Cast("Demoralizing Banner");
                            Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                            YBLogger.CombatLogP("Casting: Demoralizing Banner - On Mouse Location");
                        }))));
        }
        #endregion

        #region Booleans & Doubles
        // Cached Timed Aura's - Can only be used with MY aura's (HasCachedAura).
        internal static bool FadingRB3S
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura ragingBlow = YBSpell.CachedAuras.FirstOrDefault(a => a.SpellId == 131116 && a.CreatorGuid == StyxWoW.Me.Guid);
                return ragingBlow != null && ragingBlow.TimeLeft <= TimeSpan.FromMilliseconds(3000);
            }
        }

        internal static bool FadingCS1S
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura colossusSmash = YBSpell.CachedTargetAuras.FirstOrDefault(a => a.SpellId == 86346 && a.CreatorGuid == StyxWoW.Me.Guid);
                return colossusSmash != null && colossusSmash.TimeLeft <= TimeSpan.FromMilliseconds(1000);
            }
        }

        internal static bool FadingCS2S
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura colossusSmash = YBSpell.CachedTargetAuras.FirstOrDefault(a => a.SpellId == 86346 && a.CreatorGuid == StyxWoW.Me.Guid);
                return colossusSmash != null && colossusSmash.TimeLeft <= TimeSpan.FromMilliseconds(2000);
            }
        }

        // Booleans for multiple use.
        internal static bool AlmostDead         { get { return Me.CurrentTarget.HealthPercent <= 5; } }
        internal static bool DumpAllRage        { get { return Me.CurrentTarget.HealthPercent <= 1; } }
        internal static bool ExecuteCheck       { get { return Me.CurrentTarget.HealthPercent <= 20; } }
        internal static bool NonExecuteCheck    { get { return Me.CurrentTarget.HealthPercent >  20; } }
        internal static bool TargettingMe       { get { return Me.CurrentTarget.IsTargetingMeOrPet; } }
        internal static bool HasteAbilities     { get { return (BloodLustAura || TimeWarpAura || HeroismAura); } }

        // Cached Aura's - Can only be used with MY aura's (HasCachedAura).
        internal static bool AvatarAura         { get { return Me.HasCachedAura(107574, 0); } }
        internal static bool BloodbathAura      { get { return Me.HasCachedAura(12292, 0); } }
        internal static bool BloodsurgeAura     { get { return Me.HasCachedAura(46916, 0); } }
        internal static bool EnrageAura         { get { return Me.HasCachedAura(13046, 0); } }
        internal static bool LastStandAura      { get { return Me.HasCachedAura(12975, 0); } }
        internal static bool MeatCleaverAura    { get { return Me.HasCachedAura(85739, 0); } }
        internal static bool RagingBlowAura     { get { return Me.HasCachedAura(131116, 0); } }
        internal static bool RecklessnessAura   { get { return Me.HasCachedAura(1719, 0); } }
        internal static bool ShieldBarrierAura  { get { return Me.HasCachedAura(112048, 0); } }
        internal static bool ShieldBlockAura    { get { return Me.HasCachedAura(2565, 0); } }
        internal static bool SuddenExecAura     { get { return Me.HasCachedAura(139958, 0); } }
        internal static bool TasteforBloodAura  { get { return Me.HasCachedAura(56636, 0); } }
        internal static bool UltimatumAura      { get { return Me.HasCachedAura(122510, 0); } }
        internal static bool VictoriousAura     { get { return Me.HasCachedAura(32216, 0); } }

        internal static bool ColossusSmashAura  { get { return Me.CurrentTarget.HasCachedAura(86346, 0); } }
        internal static bool ColossusSmashAuraT { get { return Me.CurrentTarget.HasCachedAura(86346, 0, 5000); } }
        internal static bool DeepWoundsAura     { get { return Me.CurrentTarget.HasCachedAura(115768, 0); } }
        internal static bool RecklessnessAuraT  { get { return Me.HasCachedAura(1719, 0, 10000); } }

        // Cached Stacked Aura's - Can only be used with MY aura's (HasCachedAura).
        internal static bool MeatCleaverAuraS1  { get { return Me.HasCachedAura(85739, 1); } }
        internal static bool MeatCleaverAuraS2  { get { return Me.HasCachedAura(85739, 2); } }
        internal static bool MeatCleaverAuraS3  { get { return Me.HasCachedAura(85739, 3); } }
        internal static bool RagingBlowStacks   { get { return Me.HasCachedAura(131116, 2); } }
        internal static bool TasteForBloodS3    { get { return Me.HasCachedAura(56636, 3); } }

        // Cached Aura's - Can be used with ANY aura's (HasAnyCachedAura).
        internal static bool BattleShoutAura    { get { return Me.HasAnyCachedAura(6673, 0); } }
        internal static bool BloodLustAura      { get { return Me.HasAnyCachedAura(2825, 0); } }
        internal static bool CommandingShoutAura { get { return Me.HasAnyCachedAura(469, 0); } }
        internal static bool HeroismAura        { get { return Me.HasAnyCachedAura(32182, 0); } }
        internal static bool TimeWarpAura       { get { return Me.HasAnyCachedAura(80353, 0); } }
        internal static bool SkullBannerAura    { get { return Me.HasAnyCachedAura(114207, 0); } }
        internal static bool RallyingCryAura    { get { return Me.HasAnyCachedAura(97462, 0); } }

        internal static bool WeakenedBlowsAura { get { return Me.CurrentTarget.HasAnyCachedAura(115798, 0); } }
        
        // Talentmanager - HasTalents
        internal static bool JNTalent { get { return TalentManager.HasTalent(1); } }                        // Juggernaut
        internal static bool DTTalent { get { return TalentManager.HasTalent(2); } }                        // Double Time
        internal static bool WBTalent { get { return TalentManager.HasTalent(3); } }                        // Warbringer

        internal static bool ERTalent { get { return TalentManager.HasTalent(4); } }                        // Enraged Regeneration
        internal static bool SCTalent { get { return TalentManager.HasTalent(5); } }                        // Second Wind
        internal static bool IVTalent { get { return TalentManager.HasTalent(6); } }                        // Impending Victory

        internal static bool SSTalent { get { return TalentManager.HasTalent(7); } }                        // Staggering Shout
        internal static bool PHTalent { get { return TalentManager.HasTalent(8); } }                        // Piercing Howl
        internal static bool DSTalent { get { return TalentManager.HasTalent(9); } }                        // Disrupting Shout

        internal static bool BSTalent { get { return TalentManager.HasTalent(10); } }                       // Bladestorm
        internal static bool SWTalent { get { return TalentManager.HasTalent(11); } }                       // Shockwave
        internal static bool DRTalent { get { return TalentManager.HasTalent(12); } }                       // Dragon Roar

        internal static bool MRTalent { get { return TalentManager.HasTalent(13); } }                       // Mass Spell Reflection
        internal static bool SGTalent { get { return TalentManager.HasTalent(14); } }                       // Safeguard
        internal static bool VGTalent { get { return TalentManager.HasTalent(15); } }                       // Vigilance

		internal static bool AVTalent { get { return TalentManager.HasTalent(16); } }                       // Avatar
		internal static bool BBTalent { get { return TalentManager.HasTalent(17); } }                       // Bloodbath
        internal static bool SBTalent { get { return TalentManager.HasTalent(18); } }                       // Storm Bolt
        
        // Talentmanager - HasGlyphs
        internal static bool URGlyph { get { return TalentManager.HasGlyph("Unending Rage"); } }            // Unending Rage
        internal static bool ISGlyph { get { return TalentManager.HasGlyph("Intimidating Shout"); } }       // Intimidating Shout

        // Cooldown Tracker ( Translate: Impending Victory CoolDown On Cooldown)
        internal static bool BTCDOC { get { return YBSpell.SpellOnCooldown(23881); } }                      // Bloodthirst
        internal static bool DBCDOC { get { return YBSpell.SpellOnCooldown(114203); } }                     // Demoralizing Banner
        internal static bool HLCDOC { get { return YBSpell.SpellOnCooldown(6544); } }                       // Heroic Leap
        internal static bool IVCDOC { get { return YBSpell.SpellOnCooldown(103840); } }                     // Impending Victory
        internal static bool PUCDOC { get { return YBSpell.SpellOnCooldown(6552); } }                       // Pummel
        internal static bool VRCDOC { get { return YBSpell.SpellOnCooldown(34428); } }                      // Victory Rush

        // Cooldown Tracker ( Translate: Bloodbath Cooldown)
        internal static double AVCD { get { return YBSpell.GetSpellCooldown(107574).TotalSeconds; } }       // Avatar
        internal static double BBCD { get { return YBSpell.GetSpellCooldown(12292).TotalSeconds; } }        // Bloodbath
        internal static double BTCD { get { return YBSpell.GetSpellCooldown(23881).TotalSeconds; } }        // Bloodthirst
        internal static double CSCD { get { return YBSpell.GetSpellCooldown(86346).TotalSeconds; } }        // Colossus Smash
        internal static double PUCD { get { return YBSpell.GetSpellCooldown(6552).TotalSeconds; } }         // Pummel
        internal static double SBCD { get { return YBSpell.GetSpellCooldown(114207).TotalSeconds; } }       // Skull Banner
        internal static double SRCD { get { return YBSpell.GetSpellCooldown(23920).TotalSeconds; } }        // Spell Reflection
        internal static double RCCD { get { return YBSpell.GetSpellCooldown(1719).TotalSeconds; } }         // Recklessness
        #endregion
    }
}
