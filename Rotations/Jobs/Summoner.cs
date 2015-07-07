using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.Helpers;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Books;
using System.Windows.Forms;
using System.Linq;
using YourRaidingBuddy.Interfaces.Settings;
using Buddy.Coroutines;

namespace YourRaidingBuddy.Rotations
{
    public class Summoner : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Summoner }; }
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
            return await Rotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            if (VariableBook.HkmMultiTarget) await Unleash();
            return await Rotation();
        }

        public static async Task<bool> Rotation()
        {
            await Spell.CastSpell("Aetherflow", Me, () => !Me.HasAura("Aethertrail Attunement"));
            await Spell.CastLocation("Shadowflare", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura(193) || !Me.HasAura("Shadow Flare"));
            if (!Me.CurrentTarget.HasAura("Bio II") && await Spell.ApplyCast("Bio II", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Bio II"))) return true;
            await Spell.NoneGcdCast("Painflare", Me.CurrentTarget, () => Me.HasAura("Aethertrail Attunement") && Me.CurrentTarget.HasAura("Bio II") && Me.Auras.GetAuraStacksById(807) == 3);
            await Spell.ApplyCast("Miasma", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Miasma"));
            await Spell.ApplyCast("Bio", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Bio"));
            await Spell.NoneGcdCast("Spur", Me.CurrentTarget, () => Me.CurrentTarget.HasAura("Bio") && Me.Auras.GetAuraStacksById(807) == 2);
            await Spell.NoneGcdCast("Fester", Me.CurrentTarget, () => Me.CurrentTarget.HasAura("Bio", false, 16500) && Me.Auras.GetAuraStacksById(807) == 1);
            if (Me.CurrentTarget.HasAura("Bio") && Me.CurrentTarget.HasAura("Bio II") && Me.CurrentTarget.HasAura("Miasma"))
            {
                await Spell.CastSpell("Ruin II", () => true);
                await Spell.NoneGcdCast("Rouse", Me, () => !Me.HasAura("Rouse"));
                await Spell.NoneGcdCast("Painflare", Me.CurrentTarget, () => Me.HasAura("Aethertrail Attunement"));
                await Spell.NoneGcdCast("Raging Strikes", Me, () => Me.HasAura("Aethertrail Attunement"));
                await Spell.NoneGcdCast("Dreadwyrm Trance", Me, () => Me.HasAura("Aethertrail Attunement"));
             }
            await Spell.CastSpell("Ruin III", () => Me.HasAura("Dreadwyrm Trance"));

            return false;
        }


        static bool triggerUnleash = false;
        public static async Task<bool> Unleash()
        {
            if (triggerUnleash && Actionmanager.CanCast("Unleash", Me))
            {
                await Spell.ApplyCast("Unleash", Me, () => true);
                triggerUnleash = false;
            }

            return false;
        }


        #endregion

    }
}