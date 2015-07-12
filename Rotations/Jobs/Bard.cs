using ff14bot;
using ff14bot.Enums;
using ff14bot.Objects;
using YourRaidingBuddy.Helpers;
using System.Threading.Tasks;

namespace YourRaidingBuddy.Rotations
{
    public class Bard : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } }

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Bard, ClassJobType.Archer }; }
        }

        public override void OnInitialize() { }

        public static async Task<bool> AutoMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;

            return await BardRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;

            return await BardRotation();
        }

        public static async Task<bool> BardOpener()
        {
            return false;
        }

        private static async Task<bool> BardRotation()
        {
            // Heals
            await Spell.CastSpell("Second Wind", Me, () => Me.CurrentHealthPercent <= 50);
            await Spell.CastSpell("Featherfoot", Me, () => Me.CurrentHealthPercent <= 70);
            await Spell.CastSpell("Invigorate", Me, () => Me.CurrentTPPercent <= 50);

            await Spell.CastSpell("Repelling Shot", Me.CurrentTarget, () => Me.CurrentTarget.Distance(Me) <= 5);

            // Main Rotation
            await Spell.CastSpell("Straight Shot", Me.CurrentTarget, () => !Me.HasAura("Straight Shot"));
            await Spell.ApplyCast("Windbite", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Windbite", true, 4000));
            await Spell.ApplyCast("Venomous Bite", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Venomous Bite", true, 4000));
            await Spell.CastSpell("Straight Shot", Me.CurrentTarget, () => Me.HasAura("Straighter Shot"));
            await Spell.CastSpell("Heavy Shot", Me.CurrentTarget, () => true);

            // The following are off the GCD so we want to weave them in for optimal dps
            await Spell.CastSpell("Bloodletter", Me.CurrentTarget, () => true);

            // Buffs
            await Spell.CastSpell("Internal Release", Me, () => !Me.HasAura("Internal Release"));
            await Spell.CastSpell("Hawk's Eye", Me, () => !Me.HasAura("Hawk's Eye"));
            await Spell.CastSpell("Raging Strikes", Me, () => !Me.HasAura("Raging Strikes"));
            await Spell.CastSpell("Blood for Blood", Me, () => !Me.HasAura("Blood for Blood"));
            await Spell.CastSpell("Barrage", Me, () => !Me.HasAura("Barrage"));

            await Spell.CastSpell("Misery's End", Me.CurrentTarget, () => Me.CurrentTarget.CurrentHealthPercent <= 20);

            return false;
        }
    }
}