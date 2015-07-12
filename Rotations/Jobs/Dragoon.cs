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
        public static LocalPlayer Me { get { return Core.Player; } }
        public static SettingsG GeneralSettings { get { return InternalSettings.Instance.General; } }
        public static DragoonSetting DragoonSettings { get { return InternalSettings.Instance.Dragoon;  } }

        public override ClassJobType[] Class { get { return new[] {ClassJobType.Dragoon}; } }

        public override void OnInitialize() { ; }

        #region NewRotation
        public static async Task<bool> AutoMode()
        {
            if (!Me.CurrentTarget.IsViable()) { return false; }
            if (VariableBook.HostileUnitsCount >= GeneralSettings.AoeCount) await DragoonMultipleTarget();
            await DragoonSingleTarget();
            await DragoonWeave();
            return await DragoonAnimationLock();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable()) { return false; }
            if (VariableBook.HkmMultiTarget) await DragoonMultipleTarget();
            if (VariableBook.HkmCooldowns) await DragoonWeave();
            if (VariableBook.HkmSpecialKey) await DragoonAnimationLock();
            return await DragoonSingleTarget();
        }

        public static async Task<bool> SemiHotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable()) { return false; }
            if (VariableBook.HkmMultiTarget) await DragoonMultipleTarget();
            await DragoonSingleTarget();
            await DragoonWeave();
            return await DragoonAnimationLock();
        }

        public static async Task<bool> DragoonMultipleTarget()
        {
            await Spell.CastSpell("Ring of Thorns", () => Actionmanager.LastSpell.Name == "Heavy Thrust");
            await Spell.CastSpell("Heavy Thrust", () => !Me.HasAura("Heavy Thrust", true, DragoonSettings.ClipHeavyThrust));
            await Spell.CastSpell("Doom Spike", () => true);
            return false;
        }

        public static async Task<bool> DragoonSingleTarget()
        {
            await Spell.CastSpell("Fang and Claw", () => Me.HasAura("Sharper Fang and Claw"));
            await Spell.CastSpell("Wheeling Thrust", () => Me.HasAura("Enhanced Wheeling Thrust"));
            await Spell.CastSpell("Full Thrust", () => Actionmanager.LastSpell.Name == "Vorpal Thrust");
            await Spell.CastSpell("Vorpal Thrust", () => Actionmanager.LastSpell.Name == "True Thrust");
            await Spell.CastSpell("Chaos Thrust", () => Actionmanager.LastSpell.Name == "Disembowel");
            await Spell.CastSpell("Disembowel", () => Actionmanager.LastSpell.Name == "Impulse Drive");
            await Spell.CastSpell("Heavy Thrust", () => !Me.HasAura("Heavy Thrust", true, DragoonSettings.ClipHeavyThrust));
            await Spell.CastSpell("Impulse Drive", () => !Me.CurrentTarget.HasAura("Disembowel", false, DragoonSettings.ClipDisembowel));
            await Spell.CastSpell("Phlebotomize", () => !Me.CurrentTarget.HasAura("Phlebotomize", true, DragoonSettings.ClipPhlebotomize));
            await Spell.CastSpell("True Thrust", () => true);
            return false;
        }

        public static async Task<bool> DragoonWeave()
        {
            await Spell.CastSpell("Life Surge", () => Actionmanager.LastSpell.Name == "Vorpal Thrust");
            await Spell.CastSpell("Invigorate", Me, () => Me.CurrentTP <= DragoonSettings.InvigorateTP);
            await Spell.CastSpell("Blood of the Dragon", Me, () => !Me.HasAura("Blood of the Dragon") && (Actionmanager.LastSpell.Name == "Disembowel" || Actionmanager.LastSpell.Name == "Vorpal Thrust"));
            await Spell.CastSpell("Battle Litany", Me, () => DragoonSettings.UseBttleLitany);
            await Spell.CastSpell("Blood for Blood", Me, () => DragoonSettings.UseBloodForBlood);
            await Spell.CastSpell("Internal Release", Me, () => true);
            await Spell.CastSpell("Power Surge", Me, () => DragoonSettings.UsePowerSurge && Actionmanager.CanCast("Jump", Me.CurrentTarget));
            await Spell.CastSpell("Leg Sweep", () => DragoonSettings.UseLegSweep);
            await Spell.CastSpell("Geirskogul", () => Me.HasAura("Blood of the Dragon", true, DragoonSettings.ClipGeirskogul) && (Me.HasAura("Sharper Fang and Claw") || Me.HasAura("Enhanced Wheeling Thrust")));
            await Spell.CastSpell("Mercy Stroke", () => DragoonSettings.UseMercyStroke);
            return false;
        }

        public static async Task<bool> DragoonAnimationLock()
        {
            await Spell.CastSpell("Jump", () => DragoonSettings.UseJump && Me.HasAura("Blood of the Dragon"));
            await Spell.CastSpell("Dragonfire Dive", () => DragoonSettings.UseDragonfireDive && Me.HasAura("Blood of the Dragon"));
            await Spell.CastSpell("Spineshatter Dive", () => DragoonSettings.UseSpineshatterDive);
            return false;
        }
        #endregion
    }
}