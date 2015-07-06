// By HeinzSkies

using System.Threading.Tasks;
using System.Windows.Forms;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Helpers;
using HotkeyManager = YourRaidingBuddy.Managers.HotkeyManager;

namespace YourRaidingBuddy.Rotations
{
    public class DarkKnight : Root
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
                    ClassJobType.DarkKnight
                };
            }
        }

        public override void OnInitialize()
        {
            ;
        }

        public static async Task<bool> AutoMode()
        {
            if (!Me.CurrentTarget.IsViable())
            {
                return false;
            }
            Unit.UpdatePriorities(0, 15);
            return await AutoRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
            {
                return false;
            }
            Unit.UpdatePriorities(0, 15);
            return await HotkeyRotation();
        }

        public static async Task<bool> AutoRotation()
        {
            if (await Darkside()) return true;
            if (await Aoe()) return true;
            if (await DotRotation()) return true;
            if (await FinishedRotation()) return true;
            if (HotkeyManager.IsKeyDown(Keys.LShiftKey))
            {
                if (await EnmityRotation()) return true;
            }
            else
            {
                if (await DpsRotation()) return true;
            }
            if (await OffGcdRotation()) return true;
            if (await MpGeneratorRotation()) return true;

            return false;
        }

        public static async Task<bool> HotkeyRotation()
        {
            if (await Darkside()) return true;
            if (await Aoe()) return true;
            if (await DotRotation()) return true;
            if (await FinishedRotation()) return true;
            if (HotkeyManager.IsKeyDown(Keys.LShiftKey))
            {
                if (await EnmityRotation()) return true;
            }
            else
            {
                if (await DpsRotation()) return true;
            }
            if (await OffGcdRotation()) return true;
            if (await MpGeneratorRotation()) return true;

            return false;
        }

        public static async Task<bool> Darkside()
        {
            if (await Spell.NoneGcdCast("Darkside", Me, () => !Me.HasAura("Darkside") && Me.CurrentManaPercent > 90))
                return true;

            return false;
        }

        public static async Task<bool> DpsRotation()
        {
            if (await Spell.CastSpell("Syphon Strike", () => Actionmanager.LastSpell.Name == "Hard Slash")) return true;
            if (await Spell.CastSpell("Hard Slash", () => true)) return true;

            return false;
        }

        // We always want to finished out combo.
        public static async Task<bool> FinishedRotation()
        {
            await DarkArts();

            if (
                await
                    Spell.CastSpell("Souleater",
                        () =>
                            Actionmanager.LastSpell.Name == "Syphon Strike" &&
                            (((Me.CurrentTarget.HasAura("Delirium", true, 4000) ||
                               Me.CurrentTarget.HasAura("Dragon Kick", true, 4000)) &&
                              (Me.CurrentHealthPercent < 70 && Me.HasAura("Grit"))) || Me.HasAura("Dark Arts"))))
                return true;
            if (
                await
                    Spell.ApplyCast("Delirium", Me.CurrentTarget, () => Actionmanager.LastSpell.Name == "Syphon Strike"))
                return true;

            return await Spell.CastSpell("Power Slash", () => Actionmanager.LastSpell.Name == "Spinning Slash");
        }

        public static async Task<bool> DarkArts()
        {
            // Using it at Mana > 70% means that we never interrupt more important stuff like Power Slash or Unleash. Which both trigger at 50%.
            await
                Spell.NoneGcdCast("Dark Arts", Me,
                    () =>
                        Actionmanager.LastSpell.Name == "Syphon Strike" && Me.CurrentManaPercent > 70 &&
                        (Me.CurrentTarget.HasAura("Delirium", true, 4000) ||
                         Me.CurrentTarget.HasAura("Dragon Kick", true, 4000)));
            await
                Spell.NoneGcdCast("Dark Arts", Me,
                    () => Actionmanager.LastSpell.Name == "Spinning Slash" && Me.CurrentManaPercent > 50);

            return false;
        }

        public static async Task<bool> Aoe()
        {
            return
                await
                    Spell.CastSpell("Unleash", Me,
                        () =>
                            ((Me.CurrentManaPercent > 50 && VariableBook.HostileUnitsCount > 2) || Me.HasAura("Unleash")) &&
                            Actionmanager.LastSpell.Name != "Syphon Strike" &&
                            Actionmanager.LastSpell.Name != "Hard Slash" &&
                            Actionmanager.LastSpell.Name != "Spinning Slash");
        }

        public static async Task<bool> DotRotation()
        {
            if (await Spell.ApplyCast("Scourge",
                Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Scourge", true, 4000) &&
                                        Actionmanager.LastSpell.Name != "Syphon Strike" &&
                                        Actionmanager.LastSpell.Name != "Hard Slash" &&
                                        Actionmanager.LastSpell.Name != "Spinning Slash" &&
                                        Me.CurrentTarget.CurrentHealth > 3000))
                return true;

            return false;
        }

        // Currently without a way to detect enmity, we will use hotkey to trigger this.
        public static async Task<bool> EnmityRotation()
        {
            if (await Spell.CastSpell("Spinning Slash", () => Actionmanager.LastSpell.Name == "Hard Slash"))
                return true;
            if (await Spell.CastSpell("Hard Slash", () => true)) return true;
            return false;
        }

        public static async Task<bool> OffGcdRotation()
        {
            // Offensive
            await Spell.CastLocation("Salted Earth", Me.CurrentTarget, () => true);
            await Spell.NoneGcdCast("Reprisal", Me.CurrentTarget, () => true);
            await Spell.NoneGcdCast("Low Blow", Me.CurrentTarget, () => true);

            // Need to solve this problem for an increase in dps.
            // Leaving it like this allow it to be much more flexible.
            //await Spell.NoneGcdCast("Plunge", Me.CurrentTarget, () => Me.CurrentTarget.Distance(Me) > 10);

            // Defensive
            await Spell.NoneGcdCast("Dark Dance", Me, () => Me.CurrentTarget.Distance(Me) < 3);

            return false;
        }

        public static async Task<bool> MpGeneratorRotation()
        {
            await
                Spell.NoneGcdCast("Blood Weapon", Me,
                    () => Me.CurrentManaPercent < 90 && Me.CurrentTarget.Distance(Me) < 3);
            await
                Spell.NoneGcdCast("Blood Price", Me,
                    () => Me.CurrentManaPercent < 70 && VariableBook.HostileUnitsTargettingMeCount > 0);
            await
                Spell.NoneGcdCast("Carve and Spit", Me.CurrentTarget,
                    () => !Me.HasAura("Dark Arts") && Me.CurrentManaPercent < 50 && Me.CurrentTarget.Distance(Me) < 3);
            await
                Spell.NoneGcdCast("Sole Survivor", Me.CurrentTarget,
                    () =>
                        (Me.CurrentManaPercent < 70 || Me.CurrentHealthPercent < 70) &&
                        Me.CurrentTarget.Distance(Me) < 3 && Me.CurrentTarget.CurrentHealth < 5000);

            return false;
        }
    }
}