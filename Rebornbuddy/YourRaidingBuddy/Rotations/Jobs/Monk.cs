using System.Threading.Tasks;
using System.Windows.Forms;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Interfaces.Settings;
using YourRaidingBuddy.Settings;
using HotkeyManager = YourRaidingBuddy.Managers.HotkeyManager;

namespace YourRaidingBuddy.Rotations
{
    public class Monk : Root
    {
        private static LocalPlayer Me
        {
            get { return Core.Player; }
        }

        public override ClassJobType[] Class
        {
            get
            {
                return new[]
                {
                    ClassJobType.Monk
                };
            }
        }

        public static SettingsG GeneralSettings
        {
            get { return InternalSettings.Instance.General; }
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
            return await MonkRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            return await MonkRotation();
        }

        public static async Task<bool> MonkRotation()
        {
            if (!Me.CurrentTarget.IsViable())
                await Spell.CastSpell("Demolish", () => Me.HasAura("Coeurl Form") && !Me.CurrentTarget.HasAura("Demolish", true, 5000));
            await Spell.CastSpell("Snap Punch", () => Me.HasAura("Coeurl Form"));
            await Spell.CastSpell("True Strike", () => Me.HasAura("Raptor Form") && Actionmanager.LastSpell.Name == "Bootshine");
            await Spell.CastSpell("Twin Strike", () => Me.HasAura("Raptor Form") && Actionmanager.LastSpell.Name == "Dragon Kick");
            await Spell.CastSpell("True Strike", () => Me.HasAura("Raptor Form"));
            await Spell.CastSpell("Bootshine", () => Me.HasAura("Opo Opo Form") && Me.CurrentTarget.HasAura("Twin Snake", true, 7000));
            await Spell.CastSpell("Dragon Kick", () => Me.HasAura("Opo Opo Form"));
            await Spell.CastSpell("Bootshine", () => true);

            return false;
        }
    }
}

#endregion