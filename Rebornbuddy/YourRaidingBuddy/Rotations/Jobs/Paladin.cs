﻿using System.Threading.Tasks;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Books;

namespace YourRaidingBuddy.Rotations
{
    public class Paladin : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Paladin }; }
        }

        public override void OnInitialize()
        {
            ;
        }

        #region NewRotation
        public static async Task<bool> AutoMode()
        {
            if (Unit.ExceptionCheck()) return true;
            return await PaladinRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            // if (VariableBook.HkmMultiTarget) await NinjaAoE();
            return await PaladinRotation();
        }

        public static async Task<bool> PaladinRotation()
        {
            await Spell.CastSpell("Royal Authority", () => Actionmanager.LastSpell.Name == "Savage Blade" && Me.CurrentTarget.HasAura("Strength Down") && Me.CurrentTarget.HasAura("Goring Blade"));
            await Spell.CastSpell("Rage of Halone", () => Actionmanager.LastSpell.Name == "Savage Blade" && !Me.CurrentTarget.HasAura("Strength Down"));
            await Spell.CastSpell("Goring Blade", () => Actionmanager.LastSpell.Name == "Riot Blade" && Me.CurrentTarget.HasAura("Strength Down") && !Me.CurrentTarget.HasAura("Goring Blade"));
            await Spell.CastSpell("Savage Blade", () => Actionmanager.LastSpell.Name == "Fast Blade" && !Me.CurrentTarget.HasAura("Strength Down"));
            await Spell.CastSpell("Savage Blade", () => Actionmanager.LastSpell.Name == "Fast Blade" && Me.CurrentTarget.HasAura("Strength Down") && Me.CurrentTarget.HasAura("Goring Blade"));
            await Spell.CastSpell("Riot Blade", () => Me.CurrentTarget.HasAura("Strength Down") && Actionmanager.LastSpell.Name == "Fast Blade");
            await Spell.CastSpell("Fast Blade", () => true);
            return false;
        }
        #endregion

    }
}
