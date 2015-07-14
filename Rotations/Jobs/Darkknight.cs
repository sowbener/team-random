// By HeinzSkies

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

        public static SettingsG GeneralSettings { get { return InternalSettings.Instance.General; } }
        public static DarkKnightSetting DarkKnightSettings { get { return InternalSettings.Instance.DarkKnight; } }

        public override void OnInitialize()
        {
        }

        public static async Task<bool> AutoMode()
        {
            if (!Me.CurrentTarget.IsViable())
            {
                return false;
            }
            Unit.UpdatePriorities(0, 5);
            return await AutoRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
            {
                return false;
            }
            Unit.UpdatePriorities(0, 5);
            return await HotkeyRotation();
        }

        public static async Task<bool> AutoRotation()
        {
            if (await Darkside()) return true;
            if (!IsEnmityKeyDown() && await Aoe(false)) return true;
            if (await DotRotation()) return true;
            if (await FinishedRotation()) return true;
            if (IsEnmityKeyDown())
            {
                if (await EnmityRotation()) return true;
            }
            else
            {
                if (await DpsRotation()) return true;
            }

            if (GeneralSettings.Cooldowns == CooldownUse.Always)
            {
                if (await OffGcdRotation()) return true;
                if (await MpGeneratorRotation()) return true;
            }

            return false;
        }

        public static async Task<bool> HotkeyRotation()
        {
            if (await Darkside()) return true;
            if (VariableBook.HkmMultiTarget)
            {
                if (!IsEnmityKeyDown() && await Aoe(true)) return true;
            }
            if (await DotRotation()) return true;
            if (await FinishedRotation()) return true;
            if (IsEnmityKeyDown())
            {
                if (await EnmityRotation()) return true;
            }
            else
            {
                if (await DpsRotation()) return true;
            }
            if (VariableBook.HkmCooldowns)
            {
                if (await OffGcdRotation()) return true;
                if (await MpGeneratorRotation()) return true;
            }

            return false;
        }

        private static bool IsEnmityKeyDown()
        {
            return YourRaidingBuddy.Managers.HotkeyManager.IsKeyDown(Keys.LShiftKey);
        }

        public static async Task<bool> Darkside()
        {
            if (await Spell.NoneGcdCast("Darkside", Me, () => DarkKnightSettings.UseDarkside && !Me.HasAura("Darkside") && Me.CurrentManaPercent > DarkKnightSettings.DarksideMpPercentage))
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
                        () => Actionmanager.LastSpell.Name == "Syphon Strike" &&
                            ((!ShouldApplyDelirium() &&
                              (Me.CurrentHealthPercent < DarkKnightSettings.SouleaterHpPercentage && Me.HasAura("Grit"))) || Me.HasAura("Dark Arts"))))
                return true;
            if (
                await
                    Spell.ApplyCast("Delirium", Me.CurrentTarget, () => Actionmanager.LastSpell.Name == "Syphon Strike"))
                return true;

            return await Spell.CastSpell("Power Slash", () => Actionmanager.LastSpell.Name == "Spinning Slash");
        }

        public static async Task<bool> DarkArts()
        {
            if (!DarkKnightSettings.UseDarkArts) return false;

            await
                Spell.NoneGcdCast("Dark Arts", Me,
                    () =>
                        Actionmanager.LastSpell.Name == "Syphon Strike" && Me.CurrentManaPercent > DarkKnightSettings.DarkArtsSouleaterHpPercentage &&
                        !ShouldApplyDelirium() && !Me.HasAura("Dark Arts"));
            await
                Spell.NoneGcdCast("Dark Arts", Me,
                    () => Actionmanager.LastSpell.Name == "Spinning Slash" && Me.CurrentManaPercent > DarkKnightSettings.DarkArtsPowerSlashHpPercentage && !Me.HasAura("Dark Arts"));

            return false;
        }

        public static bool ShouldApplyDelirium()
        {
            return !Me.CurrentTarget.HasAura("Delirium", false, 4000) && !Me.CurrentTarget.HasAura("Dragon Kick") && Me.ClassLevel >= 50 && DarkKnightSettings.UseDelirium;
        }

        public static async Task<bool> Aoe(bool force)
        {
            if (!force && !GeneralSettings.Aoe) return false;

            return
                await
                    Spell.CastSpell("Unleash", Me,
                        () =>
                            ((Me.CurrentManaPercent > 50 && VariableBook.HostileUnitsCount >= GeneralSettings.AoeCount) || Me.HasAura("Enhanced Unleash") || force));
        }

        public static async Task<bool> DotRotation()
        {
            if (await Spell.ApplyCast("Scourge",
                Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Scourge", true, 4000) &&
                                        Actionmanager.LastSpell.Name != "Syphon Strike" &&
                                        Actionmanager.LastSpell.Name != "Hard Slash" &&
                                        Actionmanager.LastSpell.Name != "Spinning Slash" &&
                                        (Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth > 3000)))
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

        public static bool TargetIsNear()
        {
            return Me.CurrentTarget.Distance(Me) < 5;
        }

        public static async Task<bool> OffGcdRotation()
        {
            // Defensive that keeps you from dying
            await Spell.NoneGcdCast("Convalescence", Me, () => DarkKnightSettings.UseConvalescence && Me.CurrentHealthPercent < DarkKnightSettings.ConvalescenceHpPercentage);
            await Spell.NoneGcdCast("Shadow Wall", Me, () => DarkKnightSettings.UseShadowWall && Me.CurrentHealthPercent < DarkKnightSettings.ShadowWallHpPercentage);
            await Spell.NoneGcdCast("Shadowskin", Me, () => DarkKnightSettings.UseShadowskin && Me.CurrentHealthPercent < DarkKnightSettings.ShadowskinHpPercentage);

            // Offensive by DPS
            await Spell.NoneGcdCast("Reprisal", Me.CurrentTarget, () => DarkKnightSettings.UseReprisal);
            await Spell.NoneGcdCast("Mercy Stroke", Me.CurrentTarget, () => DarkKnightSettings.UseMercyStroke && (Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealthPercent <= 20));
            await Spell.NoneGcdCast("Low Blow", Me.CurrentTarget, () => DarkKnightSettings.UseLowBlow);
            await Spell.CastLocation("Salted Earth", Me.CurrentTarget, () => DarkKnightSettings.UseSaltedEarth);

            // Defensive
            await Spell.NoneGcdCast("Bloodbath", Me, () => DarkKnightSettings.UseBloodbath && TargetIsNear() && Me.CurrentHealthPercent < DarkKnightSettings.BloodbathHpPercentage);
            await Spell.NoneGcdCast("Dark Dance", Me, () => DarkKnightSettings.UseDarkDance && Me.CurrentHealthPercent < DarkKnightSettings.DarkDanceHpPercentage);
            await Spell.NoneGcdCast("Foresight", Me, () => DarkKnightSettings.UseForesight && Me.CurrentHealthPercent < DarkKnightSettings.ForesightHpPercentage);

            return false;
        }

        public static async Task<bool> MpGeneratorRotation()
        {
            await
                Spell.NoneGcdCast("Blood Weapon", Me,
                    () => DarkKnightSettings.UseBloodWeapon && Me.CurrentManaPercent < DarkKnightSettings.BloodWeaponManaPercentage && TargetIsNear());
            await
                Spell.NoneGcdCast("Blood Price", Me,
                    () => DarkKnightSettings.UseBloodPrice && Me.CurrentManaPercent < DarkKnightSettings.BloodPriceManaPercentage && VariableBook.HostileUnitsTargettingMeCount > 0);
            await
                Spell.NoneGcdCast("Carve and Spit", Me.CurrentTarget,
                    () => !Me.HasAura("Dark Arts") && Actionmanager.LastSpell.Name != "Syphon Strike" && Actionmanager.LastSpell.Name != "Spinning Slash" && Me.CurrentManaPercent < 50);
            await
                Spell.NoneGcdCast("Sole Survivor", Me.CurrentTarget,
                    () => DarkKnightSettings.UseSoleSurvivor &&
                        (Me.CurrentManaPercent < DarkKnightSettings.SoleSurvivorManaPercentage || Me.CurrentHealthPercent < DarkKnightSettings.SoleSurvivorHpPercentage) &&
                        (Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth < 5000));

            return false;
        }
    }
}