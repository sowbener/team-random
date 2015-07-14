using System.Threading.Tasks;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Interfaces.Settings;
using YourRaidingBuddy.Settings;

namespace YourRaidingBuddy.Rotations
{
    public class Dragoon : Root
    {
        public static LocalPlayer Me
        {
            get { return Core.Player; }
        }

        public static SettingsG GeneralSettings
        {
            get { return InternalSettings.Instance.General; }
        }

        public static DragoonSetting DragoonSettings
        {
            get { return InternalSettings.Instance.Dragoon; }
        }

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Dragoon }; }
        }

        public override void OnInitialize()
        {
            ;
        }

        public static async Task<bool> AutoMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;

            if (GeneralSettings.Aoe && VariableBook.HostileUnitsCount >= GeneralSettings.AoeCount)
                await DragoonMultipleTarget();

            await DragoonSingleTarget();

            if (GeneralSettings.Cooldowns == CooldownUse.Always)
                await DragoonWeave();

            if (!DragoonSettings.SmartAnimationLock ||
                (Me.HasAura("Heavy Thrust", true, DragoonSettings.LancerClipHeavyThrust) &&
                 !Me.HasAura("Sharper Fang and Claw") &&
                 !Me.HasAura("Enhanced Wheeling Thrust") &&
                 Actionmanager.LastSpell.Name != "Disembowel"))
                await DragoonAnimationLock();

            return false;
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;

            if (VariableBook.HkmMultiTarget)
                await DragoonMultipleTarget();

            if (VariableBook.HkmCooldowns)
                await DragoonWeave();

            if (VariableBook.HkmSpecialKey)
                await DragoonAnimationLock();

            await DragoonSingleTarget();
            return false;
        }

        public static async Task<bool> DragoonMultipleTarget()
        {
            await Spell.CastSpell("Ring of Thorns", () =>
                Actionmanager.LastSpell.Name == "Heavy Thrust");

            await Spell.CastSpell("Heavy Thrust", () =>
                !Me.HasAura("Heavy Thrust", true, DragoonSettings.LancerClipHeavyThrust));

            await Spell.CastSpell("Doom Spike", () =>
                true);

            return false;
        }

        public static async Task<bool> DragoonSingleTarget()
        {
            await Spell.CastSpell("Wheeling Thrust", () =>
                Me.HasAura("Enhanced Wheeling Thrust"));

            await Spell.CastSpell("Fang and Claw", () =>
                Me.HasAura("Sharper Fang and Claw"));

            await Spell.ApplyCast("Full Thrust", Me.CurrentTarget, () =>
                Actionmanager.LastSpell.Name == "Vorpal Thrust");

            await Spell.ApplyCast("Vorpal Thrust", Me.CurrentTarget, () =>
                Actionmanager.LastSpell.Name == "True Thrust");

            await Spell.ApplyCast("Chaos Thrust", Me.CurrentTarget, () =>
                Actionmanager.LastSpell.Name == "Disembowel");

            await Spell.ApplyCast("Disembowel", Me.CurrentTarget, () =>
                Actionmanager.LastSpell.Name == "Impulse Drive");

            await Spell.CastSpell("Heavy Thrust", () =>
                !Me.HasAura("Heavy Thrust", true, DragoonSettings.LancerClipHeavyThrust));

            await Spell.CastSpell("Impulse Drive", () =>
                !Core.Player.CurrentTarget.HasAura("Chaos Thrust", true, DragoonSettings.LancerClipChaosThrust) ||
                (!Me.CurrentTarget.HasAura("Disembowel", false, DragoonSettings.LancerClipDisembowel) ||
                 !Core.Player.CurrentTarget.HasAura("Disembowel", true, DragoonSettings.LancerClipDisembowel)));

            await Spell.CastSpell("Haymaker", () =>
                DragoonSettings.PuglistUseHaymaker);

            await Spell.CastSpell("Phlebotomize", () =>
                !Me.CurrentTarget.HasAura("Phlebotomize", true, DragoonSettings.LancerClipPhlebotomize));

            await Spell.CastSpell("Fracture", () =>
                !Me.CurrentTarget.HasAura("Fracture", true, DragoonSettings.ClipFracture));

            await Spell.CastSpell("True Thrust", () =>
                true);

            await Spell.CastSpell("Piercing Talon", () =>
                DragoonSettings.LancerUsePiercingTalon);

            return false;
        }

        public static async Task<bool> DragoonWeave()
        {
            await Spell.CastSpell("Second Wind", Me.CurrentTarget, () =>
                DragoonSettings.PuglistSecondWindHP >= Me.CurrentHealthPercent);

            await Spell.CastSpell("Life Surge", Me, () =>
                DragoonSettings.LancerUseLifeSurge &&
                Actionmanager.LastSpell.Name == "Vorpal Thrust");

            await Spell.CastSpell("Invigorate", Me, () =>
                Me.CurrentTP <= DragoonSettings.LancerInvigorateTP);

            await Spell.ApplyCast("Blood of the Dragon", Me, () =>
                DragoonSettings.DragoonUseBloodOfTheDragon &&
                !Me.HasAura("Blood of the Dragon") &&
                (Actionmanager.LastSpell.Name == "Disembowel" ||
                 Actionmanager.LastSpell.Name == "Vorpal Thrust"));

            await Spell.CastSpell("Battle Litany", Me, () =>
                DragoonSettings.DragoonUseBttleLitany);

            await Spell.CastSpell("Bloodbath", Me, () =>
                DragoonSettings.MarauderBloodbathHP >= Me.CurrentHealthPercent);

            await Spell.CastSpell("Blood for Blood", Me, () => Me.CurrentTarget != null &&
                DragoonSettings.LancerBloodForBloodHP <= Me.CurrentTarget.CurrentHealthPercent);

            await Spell.CastSpell("Internal Release", Me, () => Me.CurrentTarget != null &&
                DragoonSettings.PuglistInternalReleaseHP <= Me.CurrentTarget.CurrentHealthPercent);

            await Spell.CastSpell("Power Surge", Me, () =>
                DragoonSettings.DragoonUsePowerSurge &&
                (!DragoonSettings.DragoonUseJumpBlood ||
                 (Actionmanager.HasSpell("Blood of the Dragon") &&
                  Me.HasAura("Blood of the Dragon"))));

            await Spell.CastSpell("Leg Sweep", Me.CurrentTarget, () =>
                DragoonSettings.LancerUseLegSweep);

            await Spell.CastSpell("Geirskogul", Me.CurrentTarget, () =>
                Me.HasAura("Blood of the Dragon", true, DragoonSettings.DragoonTimeGeirskogulCombo) &&
                (Me.HasAura("Sharper Fang and Claw") ||
                 Me.HasAura("Enhanced Wheeling Thrust")));

            await Spell.CastSpell("Geirskogul", Me.CurrentTarget, () =>
                Me.HasAura("Blood of the Dragon", true, DragoonSettings.DragoonTimeGeirskogul));

            await Spell.CastSpell("Mercy Stroke", Me.CurrentTarget, () =>
                DragoonSettings.MarauderUseMercyStroke);

            await Spell.CastSpell("Foresight", Me.CurrentTarget, () =>
                DragoonSettings.MarauderForesightHP >= Me.CurrentHealthPercent);

            await Spell.CastSpell("Keen Flurry", Me.CurrentTarget, () =>
                DragoonSettings.LancerKeenFlurryHP >= Me.CurrentHealthPercent);

            await Spell.CastSpell("Featherfoot", Me.CurrentTarget, () =>
                DragoonSettings.PuglistFeatherfootHP >= Me.CurrentHealthPercent);

            return false;
        }

        public static async Task<bool> DragoonAnimationLock()
        {
            await Spell.ApplyCast("Jump", Me.CurrentTarget, () =>
                DragoonSettings.DragoonUseJump &&
                (!DragoonSettings.DragoonUseJumpBlood ||
                 (Actionmanager.HasSpell("Blood of the Dragon") &&
                  Me.HasAura("Blood of the Dragon"))));

            await Spell.ApplyCast("Dragonfire Dive", Me.CurrentTarget, () =>
                DragoonSettings.DragoonUseDragonfireDive);

            await Spell.ApplyCast("Spineshatter Dive", Me.CurrentTarget, () =>
                DragoonSettings.DragoonUseSpineshatterDive &&
                (!DragoonSettings.DragoonUseSpineshatterDiveBlood ||
                 (Actionmanager.HasSpell("Blood of the Dragon") &&
                  Me.HasAura("Blood of the Dragon"))));

            return false;
        }
    }
}