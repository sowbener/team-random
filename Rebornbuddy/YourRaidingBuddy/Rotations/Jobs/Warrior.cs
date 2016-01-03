using System.Net.Configuration;
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
            return await MainRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            return await MainRotation();
        }

        public static async Task<bool> MainRotation()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            if (Me.CurrentHealthPercent < 75) await MitigationRotation();
            if (Me.CurrentHealthPercent < 35) await HealingRotation();
            if (VariableBook.HkmMultiTarget && await AoeRotation()) return true;
            if (await
                Spell.CastSpell("Fell Cleave",
                    () => Me.HasAura("Uncontrollable") && (Actionmanager.LastSpell.Name == "Skull Sunder" ||
                                                           Actionmanager.LastSpell.Name == "Butcher's Block")))
                return true;
            if (!Me.CurrentTarget.HasAura("Storm's Eye") && await StormEyeRotation()) return true;
            if (!Me.CurrentTarget.HasAura("Storm's Path") && await StormPathRotation()) return true;
            // if (await Spell.CastSpell("Fracture", () => !Me.CurrentTarget.HasAura("Fracture"))) return true;
            return await DpsRotation();
        }

        public static async Task<bool> StormEyeRotation()
        {
            if (await Spell.CastSpell("Storm's Eye", () => Actionmanager.LastSpell.Name == "Maim")) return true;
            if (await Spell.CastSpell("Maim", () => Actionmanager.LastSpell.Name == "Heavy Swing")) return true;
            if (await Spell.CastSpell("Heavy Swing", () => true)) return true;

            return false;
        }

        public static async Task<bool> StormPathRotation()
        {
            if (await Spell.CastSpell("Storm's Path", () => Actionmanager.LastSpell.Name == "Maim")) return true;
            if (await Spell.CastSpell("Maim", () => Actionmanager.LastSpell.Name == "Heavy Swing")) return true;
            if (await Spell.CastSpell("Heavy Swing", () => true)) return true;

            return false;
        }
        // Me.HasAura("Deliverance") ; !Me.HasAura("Abandon") && !Me.HasAura("Abandon II") && !Me.HasAura("Abandon III") ; Me.HasAura("Uncontrollable")
        // Me.HasAura("Defiance") ; !Me.HasAura("Wrath") && !Me.HasAura("Wrath II") && !Me.HasAura("Wrath III") ; Me.HasAura("Infuriated")

        public static async Task<bool> DpsRotation()
        {
            await Spell.CastSpell("Internal Release", Me, () => true);
            await Spell.CastSpell("Berserk", Me, () => Me.HasAura("Deliverance"));
            if (await Spell.CastSpell("Flash", Me, () => Me.HasAura("Defiance") && Me.CurrentMana > 800)) return true;
            await Spell.CastSpell("Unchained", Me, () => Me.HasAura("Infuriated"));
            if (await Spell.CastSpell("Steel Cyclone", Me, () => Me.HasAura("Infuriated"))) return true;
            await Spell.CastSpell("Brutal Swing", () => true);
            await
                Spell.CastSpell("Mercy Stroke", () =>
                        Me.CurrentTarget.CurrentHealthPercent < 20 && Me.CurrentTarget != null &&
                        (Actionmanager.LastSpell.Name == "Skull Sunder" ||
                         Actionmanager.LastSpell.Name == "Butcher's Block"));
            await Spell.CastSpell("Infuriate", Me, () => (Me.HasAura("Abandon") || Me.HasAura("Abandon II") || Me.HasAura("Abandon III")) && (Actionmanager.LastSpell.Name == "Skull Sunder" ||
                                                           Actionmanager.LastSpell.Name == "Butcher's Block"));
            if (await
                Spell.CastSpell("Fell Cleave",
                    () => Me.HasAura("Uncontrollable") && (Actionmanager.LastSpell.Name == "Skull Sunder" ||
                                                           Actionmanager.LastSpell.Name == "Butcher's Block")))
                return true;
            if (await Spell.CastSpell("Butcher's Block", () => Actionmanager.LastSpell.Name == "Skull Sunder")) return true;
            if (await Spell.CastSpell("Skull Sunder", () => Actionmanager.LastSpell.Name == "Heavy Swing")) return true;
            if (await Spell.CastSpell("Heavy Swing", () => true)) return true;
            return false;
        }

        public static async Task<bool> AoeRotation()
        {
            if (Me.HasAura("Deliverance"))
            {
                await Spell.CastSpell("Infuriate", Me, () => Me.HasAura("Abandon") || Me.HasAura("Abandon II") || Me.HasAura("Abandon III"));
                if (await Spell.CastSpell("Decimate", () => Me.HasAura(734))) return true;
            }
            else if (Me.HasAura("Defiance"))
            {
                await Spell.CastSpell("Infuriate", Me, () => Me.HasAura("Wrath") || Me.HasAura("Wrath II") || Me.HasAura("Wrath III"));
                if (await Spell.CastSpell("Steel Cyclone", () => Me.HasAura("Infuriated"))) return true;
            }
            return false;
        }

        public static async Task<bool> MitigationRotation()
        {
            await Spell.CastSpell("Foresight", Me, () => true);
            await Spell.CastSpell("Awareness", Me, () => true);

            return true;
        }

        public static async Task<bool> HealingRotation()
        {
            await Spell.CastSpell("Thrill of Battle", Me, () => true);
            await Spell.CastSpell("Convalescence", Me, () => true);
            await Spell.CastSpell("Equilibrium", Me, () => Me.HasAura("Defiance"));
            await Spell.CastSpell("Infuriate", Me, () => true);
            await Spell.CastSpell("Bloodbath", Me, () => true);
            await Spell.CastSpell("Inner Beast", Me, () => Me.HasAura("Infuriated"));
            return true;
        }

        #endregion

    }
}