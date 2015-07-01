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
            if (!Me.CurrentTarget.IsViable())
                return false;
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
            await Spell.CastSpell("Savage Blade", () => Actionmanager.LastSpell.Name == "Fast Blade");
            await Spell.CastSpell("Fast Blade", () => true);
            return false;
        }
        #endregion

    }
}
