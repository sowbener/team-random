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
using TreeSharp;
using System.Collections.Generic;
using YourRaidingBuddy.Books;

namespace YourRaidingBuddy.Rotations
{
    public class Conjurer : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter
        
        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Conjurer }; }
        }

        public override void OnInitialize()
        {
            ;
        }

        #region NewRotation
        public static async Task<bool> AutoMode()
        {
            return await BasicHealing();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
           // if (VariableBook.HkmMultiTarget) await NinjaAoE();
            return await BasicHealing();
        }


        public static async Task<bool> BasicHealing()
        {
            await Spell.CastHealing(124, "Medica", true, 2, 80, 10);
            await Spell.CastHealing(120, "Cure", false, 1, 90, 5);
            return false;
        }
        #endregion

        #region Ninjatsu Copied From Kupo Credits Masta

        private static bool HasBleedingDebuff()
        {

            return Me.CurrentTarget.HasAura("Storm's Eye", false, 2000) || Me.CurrentTarget.HasAura("Dancing Edge");

        }




        private static readonly SpellData Jin = DataManager.GetSpellData(2263);
        private static readonly SpellData Chi = DataManager.GetSpellData(2261);
        private static readonly SpellData Ten = DataManager.GetSpellData(2259);
        private static readonly SpellData Ninjutsu = DataManager.GetSpellData(2260);
        private static readonly SpellData Jugulate = DataManager.GetSpellData(2251);

        private static readonly SpellData Kassatsu = DataManager.GetSpellData(2264);

        private static readonly SpellData Trick_Attack = DataManager.GetSpellData(2258);
        private static readonly SpellData Sneak_Attack = DataManager.GetSpellData(2250);

        public static HashSet<uint> OverrideBackstabIds = new HashSet<uint>()
        {
            3240//Cloud of darkness
        };


        private const int HutonRecast = 20000;
        internal static async Task<bool> DoNinjutsu()
        {


            //Exit early if player was inputting something
            if (Core.Player.HasAura("Mudra"))
                return true;

            if (Actionmanager.CanCastOrQueue(Jin, null))
            {
                if (!Core.Player.HasAura("Huton", true, HutonRecast))
                {
                    Logger.Write("YourRaidingBuddy Casting " + "Huton" + "Chi Combo!");
                    await CastHuton();
                    return false;
                }

                var curTarget = Core.Target as BattleCharacter;
                if (curTarget == null)
                    return false;

             //  if (curTarget.TimeToDeath() <= 3)
             //       return false;

                //Suiton
                var taCD = Trick_Attack.Cooldown;
                //We can start casting suiton before trick attack is ready cause its going to take some time
                if (taCD.TotalMilliseconds <= 1300)
                {
                    if (!await CastSuiton())
                        return false;

                    if (!await CastTrickAttack())
                        return false;

                    if (!(Kassatsu.Cooldown.TotalMilliseconds <= 0) || !Core.Player.HasTarget)
                        return false;

                    if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Kassatsu, null) && Me.CurrentTarget.HasAura("Shadow Fang") && Me.CurrentTarget.HasAura(492)))
                    {
                        Logger.Write("YourRaidingBuddy Casting " + "Raiton");
                        if (Me.CurrentTarget.HasAura(492)) await CastRaiton();
                    }


                    return false;

                }

                if (taCD.TotalSeconds >= 20)
                {
                    await CastRaiton();
                }


                return false;
            }



            if (Actionmanager.CanCastOrQueue(Chi, null) && Me.CurrentTarget.HasAura("Shadow Fang"))
            {
                Logger.Write("YourRaidingBuddy Casting " + "Raiton");
                await CastRaiton();
                return false;
            }

            if (Actionmanager.CanCastOrQueue(Ten, null))
            {
                Logger.Write("YourRaidingBuddy Casting " + "Ten" + "Ninjutsu!");
                
                {
                    await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null));
                    await CastNinjutsu();
                    return false;
                }
            }




            return false;
        }

        private static  async Task CastHuton()
        {
            if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Jin, null)))
            {
                if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Chi, null)))
                {
                    if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null)))
                    {
                        await CastNinjutsu();
                    }
                }
            }
        }


        private static async Task<bool> CastTrickAttack()
        {

            while (Core.Player.HasAura("Suiton"))
            {
                if (Core.Player.HasTarget)
                {
                    if (OverrideBackstabIds.Contains(Core.Target.NpcId) || Core.Target.IsBehind)
                    {
                        await Spell.CastSpell("Spinning Edge", () => Me.HasAura("Suiton") && !Me.CurrentTarget.HasAura("Vulnerability Up") && Spell.LastCombo == Combo.Flushed);
                        await Spell.ApplyCast("Trick Attack", Me.CurrentTarget, () => !Me.CurrentTarget.HasAura("Vulnerability Up") && Spell.LastCombo == Combo.SpinningEdge); //Actionmanager.DoAction(Trick_Attack, Core.Target);
                    }
                    else if (BotManager.Current.IsAutonomous)
                    {
                        await Spell.ApplyCast("Sneak Attack", Me.CurrentTarget, () => true); //Actionmanager.DoAction(Sneak_Attack, Core.Target);
                    }
                }
                if (!Core.Player.InCombat)
                    return false;

                await Coroutine.Yield();
            }

            if (!BotManager.Current.IsAutonomous)
            {
                return await Coroutine.Wait(2000, () => Core.Target != null && Core.Target.IsValid && Core.Target.HasAura("Vulnerability Up"));
            }

            return false;
        }


        private static async Task<bool> CastRaiton()
        {
            if (!await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null))) return false;
            if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Chi, null)))
            {
                return await CastNinjutsu();
            }
            return false;
        }

        private static async Task<bool> CastSuiton()
        {
            if (!await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null))) return false;
            if (!await Coroutine.Wait(2000, () => Actionmanager.DoAction(Chi, null))) return false;
            if (!await Coroutine.Wait(2000, () => Actionmanager.DoAction(Jin, null))) return false;


            if (await CastNinjutsu())
            {
                return await Coroutine.Wait(2000, () => Core.Player.HasAura("Suiton"));
            }

            return false;
        }


        private static async Task<bool> CastNinjutsu()
        {
            if (await Coroutine.Wait(2000, () => Core.Player.HasAura("Mudra")))
            {
                bool possibly = false;
                while (Core.Player.HasAura("Mudra"))
                {
                    if (Core.Player.HasTarget)
                    {
                        if (Actionmanager.DoAction(Ninjutsu, Core.Target))
                        {
                            possibly = true;
                        }
                    }
                    if (!Core.Player.InCombat)
                        return false;

                    await Coroutine.Yield();
                }
                await Coroutine.Wait(2000, () => Ninjutsu.Cooldown.TotalSeconds > 10);
                return possibly;
            }
            return false;
        }

        #endregion
    }
}
