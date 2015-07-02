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

namespace YourRaidingBuddy.Rotations
{
    public class DarkKnight : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.DarkKnight }; }
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
            if (Managers.HotkeyManager.IsKeyDown(System.Windows.Forms.Keys.LShiftKey))
            {
                await PullSingle();
            }
            else
            {
                await PullMultiple();
            }
            await Unleash();
            await Plunge();
            await Darkside();
            await DotRotation();
            if (Managers.HotkeyManager.IsKeyDown(System.Windows.Forms.Keys.LShiftKey))
            {
                await EnmityRotation();
            }
            else
            {
                await DPSRotation();
            }
            await OffGcdRotation();
            await MPGeneratorRotation();

            return false;
        }

        public static async Task<bool> Darkside()
        {
            await Spell.NoneGcdCast("Darkside", Me, () => !Me.HasAura("Darkside") && Me.CurrentManaPercent > 90);

            return false;
        }

        public static async Task<bool> DPSRotation()
        {
            await DarkArts();

            await Spell.CastSpell("Souleater", () => Actionmanager.LastSpell.Name == "Syphon Strike" && Me.CurrentTarget.HasAura("Delirium", true, 4000));
            await Spell.CastSpell("Delirium", () => Actionmanager.LastSpell.Name == "Syphon Strike" && !Me.CurrentTarget.HasAura("Delirium", true, 4000));
            await Spell.CastSpell("Syphon Strike", () => Actionmanager.LastSpell.Name == "Hard Slash");
            await Spell.CastSpell("Hard Slash", () => true);

            return false;
        }

        public static async Task<bool> DarkArts()
        {
            await Spell.NoneGcdCast("Dark Arts", Me, () => Actionmanager.LastSpell.Name == "Syphon Strike" && Me.CurrentManaPercent > 90 && Me.CurrentTarget.HasAura("Delirium", true, 4000));

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

        static bool triggerPlunge = false;
        public static async Task<bool> Plunge()
        {
            if (triggerPlunge && Actionmanager.CanCast("Plunge", Me.CurrentTarget))
            {
                await Spell.NoneGcdCast("Plunge", Me.CurrentTarget, () => true);
                triggerPlunge = false;
            }

            return false;
        }

        public static async Task<bool> PullSingle()
        {
            if (Actionmanager.CanCast("Unmend", Me.CurrentTarget) && Me.CurrentTarget.Distance(Me) > 10)
            {
                await Spell.CastSpell("Unmend", () => true);

                triggerPlunge = true;
            }

            return false;
        }

        public static async Task<bool> PullMultiple()
        {
            if (Actionmanager.CanCast("Plunge", Me.CurrentTarget) && Me.CurrentTarget.Distance(Me) > 10)
            {
                await Spell.NoneGcdCast("Plunge", Me.CurrentTarget, () => true);
                triggerUnleash = true;
            }

            return false;
        }

        public static async Task<bool> DotRotation()
        {
            await Spell.ApplyCast("Scourge", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Scourge", true, 4000));

            return false;
        }

        // Currently without a way to detect enmity, we will use hotkey to trigger this.
        public static async Task<bool> EnmityRotation()
        {
            await Spell.CastSpell("Power Slash", () => Actionmanager.LastSpell.Name == "Spinning Slash");
            await Spell.CastSpell("Spinning Slash", () => Actionmanager.LastSpell.Name == "Hard Slash");
            await Spell.CastSpell("Hard Slash", () => true);
            return false;
        }

        public static async Task<bool> OffGcdRotation()
        {
            // Offensive
            await Spell.CastLocation("Salted Earth", Me.CurrentTarget, true);
            await Spell.NoneGcdCast("Reprisal", Me.CurrentTarget, () => true);
            await Spell.NoneGcdCast("Low Blow", Me.CurrentTarget, () => true);

            // Need to solve this problem for an increase in dps.
            // Leaving it like this allow it to be much more flexible.
            //await Spell.NoneGcdCast("Plunge", Me.CurrentTarget, () => Me.CurrentTarget.Distance(Me) > 10);

            // Defensive
            await Spell.NoneGcdCast("Dark Dance", Me, () => true);

            return false;
        }

        public static async Task<bool> MPGeneratorRotation()
        {
            await Spell.NoneGcdCast("Blood Weapon", Me, () => true);
            await Spell.NoneGcdCast("Blood Price", Me, () => true);
            await Spell.NoneGcdCast("Sole Survivor", Me, () => true);
            await Spell.NoneGcdCast("Carve and Spit", Me, () => true);

            return false;
        }

        #endregion

    }
}