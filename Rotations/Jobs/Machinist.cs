using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Settings;
using Clio.Utilities;
using TreeSharp;
using System.Collections.Generic;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Rotations
{
    public class Machinist : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter

        public static Vector3 RookPosition = new Vector3(0f, 0f, 2.5f);

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Machinist }; }
        }

        public override void OnInitialize()
        {
            ;
        }

        #region NewRotation
        public static async Task<bool> AutoMode()
        {
            //Combat
            if (!Me.CurrentTarget.IsViable())
                return false;
            return await MachinistRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            //Combat
            if (!Me.CurrentTarget.IsViable())
                return false;

            return await MachinistRotation();
        }

        public static async Task<bool> MachinistOpener()
        {
            return false;
        }

        public static async Task<bool> MachinistRotation()
        {
            //Low Health Finisher
            await Spell.CastSpell("Heartbreak", Me.CurrentTarget, () => true);
            await Spell.CastSpell("Blank", Me.CurrentTarget, () => true);

            //Turret Control
           // if (Core.Player.InCombat && Core.Player.Pet == null)
            //    Actionmanager.DoActionLocation(2864, (Core.Target.Location + RookPosition)); //Summon Turret

            //DeBuff, Dots & Buffs
                //WeaponSkill
                await Spell.CastSpell("Hot Shot", Me.CurrentTarget, () => !Me.HasAura("Hot Shot", true, 2000) && !Me.HasAura("Ammunition Loaded"));
                    //Activate while GCD is in Progress
                    await Spell.CastSpell("Wildfire", Me.CurrentTarget, () => Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpDeBuff);
                    await Spell.CastSpell("Hawk's Eye", Me, () => Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && InternalSettings.Instance.Machinist.EnableAutoBuffs);
                    await Spell.CastSpell("Blood for Blood", Me, () => Me.HasAura("Hawk's Eye") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && InternalSettings.Instance.Machinist.EnableAutoBuffs);
                //WeaponSkill
                await Spell.CastSpell("Lead Shot", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Lead Shot", true, 500) && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpDeBuff);
                    //Activate while GCD is in Progress
                    await Spell.CastSpell("Raging Strikes", Me, () => Me.HasAura("Blood for Blood") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && InternalSettings.Instance.Machinist.EnableAutoBuffs);
                    await Spell.CastSpell("Quelling Strikes", Me, () => Me.HasAura("Hawk's Eye") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && InternalSettings.Instance.Machinist.EnableAutoBuffs);
                    await Spell.CastSpell("Invigorate", Me, () => Core.Player.CurrentTP < InternalSettings.Instance.Machinist.UseInvigorateTP);

            //Rotation
            await Spell.CastSpell("Split Shot", Me.CurrentTarget, () => !Me.HasAura("Enhanced Slug Shot") && !Me.HasAura("Cleaner Shot"));
            await Spell.CastSpell("Reload", Me, () => Me.HasAura("Hawk's Eye") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && InternalSettings.Instance.Machinist.EnableAutoBuffs);
            await Spell.CastSpell("Rapid Fire", Me, () => !Me.HasAura("Enhanced Slug Shot") && !Me.HasAura("Cleaner Shot"));
            await Spell.CastSpell("Slug Shot", Me.CurrentTarget, () => Me.HasAura("Enhanced Slug Shot"));
            //await Spell.CastSpell("Head Graze", Me.CurrentTarget, () => true);
            await Spell.CastSpell("Quick Reload", Me, () => true);
            await Spell.CastSpell("Rapid Fire", Me, () => !Me.HasAura("Enhanced Slug Shot") && !Me.HasAura("Cleaner Shot"));
            await Spell.CastSpell("Reassemble", Me, () => Me.HasAura("Cleaner Shot") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff);
            await Spell.CastSpell("Clean Shot", Me.CurrentTarget, () => Me.HasAura("Cleaner Shot"));
            return false;
        }
        #endregion
    }
}
