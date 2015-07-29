using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using YourRaidingBuddy;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Interfaces.Settings;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using Clio.Common;
using Clio.Utilities;
using TreeSharp;
using System.Collections.Generic;
using Action = TreeSharp.Action;

namespace YourRaidingBuddy.Rotations
{
    public class Machinist : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } }

        public override float PullRange
        {
            get { return 20f; }
        }

        //Variables
        private static readonly SpellData Hawk_Eye = DataManager.GetSpellData(99);
        //private const bool InternalSettings.Instance.Machinist.EnableAutoBuffs = true;
        //private const bool InternalSettings.Instance.Machinist.EnablePotions = false;
        //private const bool InternalSettings.Instance.Machinist.EnableStuns = true;
        //private const bool InternalSettings.Instance.Machinist.EnableAutoSummon = true;
        //private const int InternalSettings.Instance.Machinist.MinHpBuff = 15000;
        //private const int InternalSettings.Instance.Machinist.MinHpDeBuff = 10000;
        //private const int InternalSettings.Instance.Machinist.UseInvigorateTP = 500;
        //private const int InternalSettings.Instance.Machinist.AoEMonster = 4;
        //private const String InternalSettings.Instance.Machinist.PotName = "X-Potion of Dexterity";
        public static Vector3 RookPosition = new Vector3(0.3f, 0f, .5f);


        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Machinist }; }
        }

        public override void OnInitialize()
        {
            //Summon Companion
            if (Core.Player.Pet == null && VariableBook.HostileUnitsCount < 2 && InternalSettings.Instance.Machinist.EnableAutoSummon && Me.CurrentTarget != null)
                Actionmanager.DoActionLocation(2864, (Core.Target.Location + RookPosition));

            if (Core.Player.Pet == null && VariableBook.HostileUnitsCount > 2 && InternalSettings.Instance.Machinist.EnableAutoSummon && Me.CurrentTarget != null)
                Actionmanager.DoActionLocation(2865, (Core.Target.Location + RookPosition));
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
            //Combat
            if (!Me.CurrentTarget.IsViable())
                return false;
            return await MachinistRotation();
        }

        public static async Task<bool> MachinistRotation()
        {
            var GBCD = Hawk_Eye.Cooldown;
            var pot = InventoryManager.FilledSlots.FirstOrDefault(a => a.Name.Equals(InternalSettings.Instance.Machinist.PotName) && a.IsHighQuality);

            //Turned it off for manual control.
            //await Spell.CastSpell("Gauss Barrel", Me, () => !Me.HasAura("Gauss Barrel"));

            //Summon Companion
            if (Core.Player.Pet == null && VariableBook.HostileUnitsCount < 2 && InternalSettings.Instance.Machinist.EnableAutoSummon && Me.CurrentTarget != null)
                Actionmanager.DoActionLocation(2864, (Core.Target.Location + RookPosition));

            if (Core.Player.Pet == null && VariableBook.HostileUnitsCount > 2 && InternalSettings.Instance.Machinist.EnableAutoSummon && Me.CurrentTarget != null)
                Actionmanager.DoActionLocation(2865, (Core.Target.Location + RookPosition));

            //Invigorate when TP Lower than the value of InternalSettings.Instance.Machinist.UseInvigorateTP
            await Spell.ApplyCast("Invigorate", Me, () => Me.CurrentTP <= InternalSettings.Instance.Machinist.UseInvigorateTP && Me.CurrentTarget != null);

            //Hypercharge will go off if mob has more HP than the variable InternalSettings.Instance.Machinist.MinHpBuff
            if (Core.Player.ClassLevel >= 58 && Core.Player.InCombat && Core.Player.Pet != null && Me.CurrentTarget != null)
                await Spell.ApplyCast("Hypercharge", Me, () => Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && Me.CurrentTarget != null);

            //AOE Component. Will go off if 5 or more targets present.
            if (VariableBook.HostileUnitsCount >= InternalSettings.Instance.Machinist.AoEMonster && Me.CurrentTP >= 200 && Me.CurrentTarget != null)
            {
                await Spell.ApplyCast("Invigorate", Me, () => Me.CurrentTP <= InternalSettings.Instance.Machinist.UseInvigorateTP && Me.CurrentTarget != null);

                if (Core.Me.CurrentTarget.CurrentHealthPercent >= 30 && Me.CurrentTarget != null)
                    await Spell.CastSpell("Spread Shot", Me.CurrentTarget, () => Me.CurrentTarget != null);
                else
                    await Spell.CastSpell("Grenado Shot", Me.CurrentTarget, () => Me.CurrentTarget != null);
            }

            //Buffs will go off if mob has more HP than the variable InternalSettings.Instance.Machinist.MinHpBuff.
            await Spell.ApplyCast("Blood for Blood", Me, () => GBCD.TotalMilliseconds <= 0 && Me.HasAura("Gauss Barrel") && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && InternalSettings.Instance.Machinist.EnableAutoBuffs && Me.CurrentTarget != null);

            //Gauss Round will always go off when ready and Gauss Barrel is up.
            await Spell.CastSpell("Gauss Round", Me.CurrentTarget, () => Me.HasAura("Gauss Barrel") && Me.CurrentTarget != null);

            //Hot Shot will got out when 3 or less seconds on timer. Wont go off if Ammunition Buff is Up or you're moving. 
            if (!MovementManager.IsMoving)
                await Spell.ApplyCast("Hot Shot", Me.CurrentTarget, () => !Me.HasAura("Ammunition Loaded") && !Me.HasAura("Hot Shot", true, 3000) && Me.CurrentTarget != null);
            else
                await Spell.CastSpell("Feint", Me.CurrentTarget, () => Me.CurrentTarget != null); //Feint is instant and will be used when moving.

            await Spell.ApplyCast("Raging Strikes", Me, () => Me.HasAura("Blood for Blood") && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && Me.CurrentTarget != null && InternalSettings.Instance.Machinist.EnableAutoBuffs);
            await Spell.ApplyCast("Hawk's Eye", Me, () => Me.HasAura("Gauss Barrel") && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && Me.CurrentTarget != null && InternalSettings.Instance.Machinist.EnableAutoBuffs);

            if (!Me.HasAura("Reassembled") && !MovementManager.IsMoving)
                await Spell.ApplyCast("Lead Shot", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Lead Shot", true, 5000) && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpDeBuff);
            else
                await Spell.CastSpell("Feint", Me.CurrentTarget, () => Me.CurrentTarget != null); //Feint is instant and will be used when moving.

            await Spell.ApplyCast("Rapid Fire", Me, () => Me.HasAura("Gauss Barrel") && Me.HasAura("Blood for Blood") && Me.CurrentTarget != null);
            await Spell.ApplyCast("Reload", Me, () => Me.HasAura("Gauss Barrel") && Me.HasAura("Hawk's Eye") && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && Me.CurrentTarget != null && InternalSettings.Instance.Machinist.EnableAutoBuffs);

            if (!Me.HasAura("Gauss Barrel") | !MovementManager.IsMoving | Me.HasAura("Rapid Fire"))
                await Spell.CastSpell("Split Shot", Me.CurrentTarget, () => !Me.HasAura("Reassembled") && !Me.HasAura("Enhanced Slug Shot") && !Me.HasAura("Cleaner Shot") && Me.CurrentTarget != null);
            else
                await Spell.CastSpell("Feint", Me.CurrentTarget, () => Me.CurrentTarget != null);//Feint is instant and will be used when moving.

            await Spell.ApplyCast("Wildfire", Me.CurrentTarget, () => Me.HasAura("Hawk's Eye") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpDeBuff);

            if (Me.HasAura("Enhanced Slug Shot") | !MovementManager.IsMoving | Me.HasAura("Rapid Fire"))
                await Spell.CastSpell("Slug Shot", Me.CurrentTarget, () => !Me.HasAura("Reassembled") && Me.HasAura("Enhanced Slug Shot") && Me.CurrentTarget != null);
            else
                await Spell.CastSpell("Feint", Me.CurrentTarget, () => Me.CurrentTarget != null);//Feint is instant and will be used when moving.

            //Uses potion on Variable 'InternalSettings.Instance.Machinist.PotName'
            if (Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && Me.HasAura("Gauss Barrel") && Me.HasAura("Hawk's Eye") && Me.CurrentTarget != null && InternalSettings.Instance.Machinist.EnablePotions)
            {
                pot.UseItem(Core.Me);
            }

            await Spell.ApplyCast("Reassemble", Me, () => Me.HasAura("Gauss Barrel") && Me.HasAura("Cleaner Shot") && Me.HasAura("Hawk's Eye") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpBuff && InternalSettings.Instance.Machinist.EnableAutoBuffs);

            if (Me.HasAura("Cleaner Shot") | !MovementManager.IsMoving | Me.HasAura("Rapid Fire"))
                await Spell.CastSpell("Clean Shot", Me.CurrentTarget, () => Me.HasAura("Cleaner Shot") && Me.CurrentTarget != null);
            else
                await Spell.CastSpell("Feint", Me.CurrentTarget, () => Me.CurrentTarget != null);//Feint is instant and will be used when moving.

            await Spell.ApplyCast("Quick Reload", Me, () => InternalSettings.Instance.Machinist.EnableAutoBuffs && Me.CurrentTarget != null);
            await Spell.ApplyCast("Ricochet", Me.CurrentTarget, () => !Me.HasAura("Reassembled") && Me.HasAura("Gauss Barrel") && Me.HasAura("Hawk's Eye") && Me.CurrentTarget != null && Me.CurrentTarget.CurrentHealth >= InternalSettings.Instance.Machinist.MinHpDeBuff);

            //Silence will go off if enabled on the variable 'InternalSettings.Instance.Machinist.EnableStuns'
            await Spell.CastSpell("Head Graze", Me.CurrentTarget, () => Me.CurrentTarget != null && InternalSettings.Instance.Machinist.EnableStuns);

            //These will go off whenever off Cooldown.
            await Spell.CastSpell("Blank", Me.CurrentTarget, () => Me.CurrentTarget != null);
            await Spell.CastSpell("Heartbreak", Me.CurrentTarget, () => Me.CurrentTarget != null);

            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            return false;
        }

        #endregion
    }

}
