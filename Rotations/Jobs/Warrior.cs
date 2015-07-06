using System.Threading.Tasks;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Books;

namespace YourRaidingBuddy.Rotations
{
    public class Warrior : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Warrior }; }
        }

        public override void OnInitialize()
        {
            ;
        }

        #region NewRotation


        public static async Task<bool> AutoMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            return await WarriorRotation();
        }

        public static async Task<bool> WarriorDefiance()
        {
            await Spell.CastSpell("Defiance", Me, () => !Me.HasAura("Defiance"));
            await Spell.CastSpell("Infuriate", Me, () => !Me.HasAura("Infuriated") && !Me.HasAura("Wrath") && !Me.HasAura("Wrath II") && !Me.HasAura("Wrath III") && !Me.HasAura("Wrath IV") && !Me.HasAura("Uncontrollable") && !Me.HasAura("Abandon") && !Me.HasAura("Abandon II") && !Me.HasAura("Abandon III") && !Me.HasAura("Abandon IV"));
            await Spell.CastSpell("Butcher's Block", () => Actionmanager.LastSpell.Name == "Skull Sunder");
            await Spell.CastSpell("Skull Sunder", () => Actionmanager.LastSpell.Name == "Heavy Swing");
            await Spell.CastSpell("Heavy Swing", () => true);

            // off the Gcd weaving

            await Spell.CastSpell("Bloodbath", Me, () => Me.CurrentHealthPercent <= 95);
            await Spell.CastSpell("Equilibrium", Me, () => Me.CurrentHealthPercent <= 60);
            await Spell.CastSpell("Featherfoot", Me, () => Me.CurrentHealthPercent <= 85);
            await Spell.CastSpell("Mercy Stroke", Me.CurrentTarget, () => Me.CurrentTarget.CurrentHealthPercent <= 20);
            await Spell.CastSpell("Internal Release", Me, () => !Me.HasAura("Internal Release"));
            await Spell.CastSpell("Mercy Stroke", Me.CurrentTarget, () => Me.CurrentTarget.CurrentHealthPercent <= 20);

            return false;
        }

        public static async Task<bool> WarriorDeliverance()
        {
            await Spell.CastSpell("Deliverance", Me, () => !Me.HasAura("Deliverance"));
            await Spell.ApplyCast("Fracture", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Fracture", true, 4000) && Actionmanager.LastSpell.Name != ("Heavy Swing") && Actionmanager.LastSpell.Name != "Skull Sunder");
            await Spell.CastSpell("Fell Cleave", Me.CurrentTarget, () => Me.CurrentTarget.CurrentHealthPercent >= 0);
            await Spell.CastSpell("Raw Intuition", Me, () => Actionmanager.LastSpell.Name == "Fell Cleave");
            await Spell.CastSpell("Infuriate", Me, () => !Me.HasAura("Infuriated") && !Me.HasAura("Wrath") && !Me.HasAura("Wrath II") && !Me.HasAura("Wrath III") && !Me.HasAura("Wrath IV") && !Me.HasAura("Uncontrollable") && !Me.HasAura("Abandon") && !Me.HasAura("Abandon II") && !Me.HasAura("Abandon III") && !Me.HasAura("Abandon IV"));
            await Spell.CastSpell("Storm's Eye", () => Actionmanager.LastSpell.Name == "Maim");
            await Spell.CastSpell("Maim", () => !Me.HasAura("Maim", true, 7000) && !Me.CurrentTarget.HasAura("Storm's Eye", true, 7000) && Actionmanager.LastSpell.Name == "Heavy Swing");
            await Spell.CastSpell("Butcher's Block", () => Actionmanager.LastSpell.Name == "Skull Sunder");
            await Spell.CastSpell("Skull Sunder", () => Actionmanager.LastSpell.Name == "Heavy Swing");
            await Spell.CastSpell("Heavy Swing", () => true);

            // off the Gcd weaving

            await Spell.CastSpell("Internal Release", Me, () => !Me.HasAura("Internal Release"));
            await Spell.CastSpell("Berserk", Me, () => !Me.HasAura("Berserk"));
            await Spell.CastSpell("Mercy Stroke", Me.CurrentTarget, () => Me.CurrentTarget.CurrentHealthPercent <= 20);

            return false;
        }

        public static async Task<bool> WarriorDefianceAoE()
        {
            // work in progress

            await Spell.CastSpell("Defiance", Me, () => !Me.HasAura("Defiance"));
            await Spell.CastSpell("Steel Cyclone", Me, () => Me.HasAura("Infuriated"));
            await Spell.CastSpell("Infuriate", Me, () => !Me.HasAura("Infuriated") && !Me.HasAura("Wrath") && !Me.HasAura("Wrath II") && !Me.HasAura("Wrath III") && !Me.HasAura("Wrath IV") && !Me.HasAura("Uncontrollable") && !Me.HasAura("Abandon") && !Me.HasAura("Abandon II") && !Me.HasAura("Abandon III") && !Me.HasAura("Abandon IV"));
            await Spell.CastSpell("Overpower", Me.CurrentTarget, () => Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealthPercent >= 0);
            await Spell.CastSpell("Flash", Me, () => true);

            // off the Gcd weaving (pops light cds while tanking multiple mobs)

            await Spell.CastSpell("Bloodbath", Me, () => Me.CurrentHealthPercent <= 95);
            await Spell.CastSpell("Equilibrium", Me, () => Me.CurrentHealthPercent <= 60);
            await Spell.CastSpell("Featherfoot", Me, () => Me.CurrentHealthPercent <= 85);
            await Spell.CastSpell("Mercy Stroke", Me.CurrentTarget, () => Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealthPercent <= 20);


            return false;
        }


        public static async Task<bool> WarriorRotation()
        {
            if (!Me.CurrentTarget.IsViable())
                await Spell.CastSpell("Defiance", Me, () => !Me.HasAura("Defiance"));
            await Spell.CastSpell("Infuriate", Me, () => !Me.HasAura("Infuriated") && !Me.HasAura("Wrath") && !Me.HasAura("Wrath II") && !Me.HasAura("Wrath III") && !Me.HasAura("Wrath IV") && !Me.HasAura("Uncontrollable") && !Me.HasAura("Abandon") && !Me.HasAura("Abandon II") && !Me.HasAura("Abandon III") && !Me.HasAura("Abandon IV"));
            await Spell.CastSpell("Storm's Path", () => !Me.CurrentTarget.HasAura("Storm's Path", true, 4000) && Actionmanager.LastSpell.Name == "Maim");
            await Spell.CastSpell("Storm's Eye", () => !Me.CurrentTarget.HasAura("Storm's Eye", true, 4000) && Actionmanager.LastSpell.Name == "Maim");
            await Spell.CastSpell("Maim", () => !Me.CurrentTarget.HasAura("Storm's Eye", true, 15000) && Actionmanager.LastSpell.Name == "Heavy Swing");
            await Spell.CastSpell("Butcher's Block", () => Actionmanager.LastSpell.Name == "Skull Sunder");
            await Spell.CastSpell("Skull Sunder", () => Actionmanager.LastSpell.Name == "Heavy Swing");
            await Spell.CastSpell("Heavy Swing", () => true);

            // off the Gcd weaving

            await Spell.CastSpell("Bloodbath", Me, () => Me.CurrentHealthPercent <= 95);
            await Spell.CastSpell("Equilibrium", Me, () => Me.CurrentHealthPercent <= 60);
            await Spell.CastSpell("Featherfoot", Me, () => Me.CurrentHealthPercent <= 85);
            await Spell.CastSpell("Internal Release", Me, () => !Me.HasAura("Internal Release"));
            await Spell.CastSpell("Mercy Stroke", Me.CurrentTarget, () => Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealthPercent <= 20);


            return false;
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            if (VariableBook.HkmSpecialKey) await WarriorDefiance();
            if (VariableBook.HkmSpecialKey1) await WarriorDeliverance();
            if (VariableBook.HkmMultiTarget) await WarriorDefianceAoE();

            return await WarriorRotation();
        }
        #endregion

    }
}
