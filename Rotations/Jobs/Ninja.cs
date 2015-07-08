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
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Rotations
{
    public class Ninja : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Ninja, ClassJobType.Rogue }; }
        }

        private const int MobHp = 0;
        private const int BuffHp = 0;
        public override void OnInitialize()
        {
            ;
        }

        #region NewRotation
        public static async Task<bool> AutoMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            return await NinjaRotation();
        }

        public static async Task<bool> HotkeyMode()
        {
            if (!Me.CurrentTarget.IsViable())
                return false;
            if (VariableBook.HkmMultiTarget) await NinjaAoE();
            return await NinjaRotation();
        }




        public static async Task<bool> NinjaRotation()
        {
            if (Managers.HotkeyManager.IsKeyDown(System.Windows.Forms.Keys.LShiftKey))
            {
                await EmergHuton();
            }
            await DoNinjutsu();
            await HutonRefresh();
            await DancingEdge();
            await Shadow();
            await SingleTarget();
            await NoneGCD();

            return false;
        }

        public static async Task<bool> NinjaAoE()
        {
            if (VariableBook.HostileUnitsCount >= 3 && Core.Me.CurrentTarget.CurrentHealthPercent >= 60)
            {
                await CastDoton();
            }
            if (VariableBook.HostileUnitsCount >= 5)
            {
                await Spell.CastSpell("Death Blossom", Me, () => true);
            }

            return false;
        }
        public static async Task<bool> EmergHuton()
        {
            await Spell.CastSpell("Armor Crush", () => Actionmanager.LastSpell.Name == "Gust Slash");
            await Spell.CastSpell("Gust Slash", () => Actionmanager.LastSpell.Name == "Spinning Edge");
            await Spell.CastSpell("Spinning Edge", () => true);
            return false;
        }
        public static async Task<bool> HutonRefresh()
        {
            if (!Me.HasAura("Duality") && !Core.Me.HasAura("Huton", true, 24000) && Actionmanager.LastSpell.Name == "Gust Slash")
            {
                return await Spell.CastSpell("Armor Crush", () => true);
            }

            return false;
        }

        public static async Task<bool> DancingEdge()
        {
            if (!Me.HasAura("Duality") && !Me.CurrentTarget.HasAura("Dancing Edge", true, 4000) && !Me.CurrentTarget.HasAura("Dancing Edge", false, 4000) && !Me.CurrentTarget.HasAura("Storm's Eye", false) && Actionmanager.LastSpell.Name == "Gust Slash")
            {
                return await Spell.CastSpell("Dancing Edge", () => true);
            }

            return false;
        }

        public static async Task<bool> Shadow()
        {

            if ((!Me.CurrentTarget.HasAura("Shadow Fang", true, 3000) || !Me.CurrentTarget.HasAura("Shadow Fang", true)) && (Me.CurrentTarget.HasAura("Dancing Edge") || Me.CurrentTarget.HasAura("Storm's Eye")) && Actionmanager.LastSpell.Name == "Spinning Edge" && Core.Me.CurrentTarget.CurrentHealth >= MobHp)
            {
                return await Spell.CastSpell("Shadow Fang", () => true);
            }
            return false;
        }


        public static async Task<bool> SingleTarget()
        {
            await Spell.CastSpell("Duality", Me, () => Actionmanager.LastSpell.Name == "Gust Slash" && Me.CurrentTarget.HasAura("Shadow Fang", true) && (Me.CurrentTarget.HasAura("Dancing Edge") || Me.CurrentTarget.HasAura("Storm's Eye")));
            await Spell.CastSpell("Aeolian Edge", () => Me.HasAura("Duality") || Actionmanager.LastSpell.Name == "Gust Slash");
            await Spell.CastSpell("Gust Slash", () => Actionmanager.LastSpell.Name == "Spinning Edge");
            await Spell.CastSpell("Mutilate", () => Me.CurrentTarget.HasAura(AuraBook.ShadowFang) && (!Me.CurrentTarget.HasAura(AuraBook.Mutilate, true, 4000) || !Me.CurrentTarget.HasAura(AuraBook.Mutilate)) &&
Core.Me.CurrentTarget.CurrentHealthPercent >= 25);
            await Spell.CastSpell("Spinning Edge", () => true);

            return false;
        }

        public static async Task<bool> NoneGCD()
        {
            await Spell.NoneGcdCast("Trick Attack", Me.CurrentTarget, () => Me.HasAura(AuraBook.Suiton) && Me.CurrentTarget.IsBehind);
            await Spell.NoneGcdCast("Internal Release", Me, () => !Me.HasAura(AuraBook.InternalRelease) && Core.Me.CurrentTarget.CurrentHealthPercent >= 25);
            await Spell.NoneGcdCast("Blood for Blood", Me, () => !Me.HasAura("Blood for Blood") && Core.Me.CurrentTarget.CurrentHealthPercent >= 25);
            await Spell.NoneGcdCast("Invigorate", Me, () => Me.CurrentTP < 550);
            await Spell.NoneGcdCast("Second Wind", Me, () => Me.CurrentHealthPercent <= 30);
            await Spell.NoneGcdCast("Jugulate", Me.CurrentTarget, () => Me.CurrentTarget.HasAura(AuraBook.Mutilate));
            await Spell.NoneGcdCast("Mug", Me.CurrentTarget, () => Me.CurrentTarget.HasAura(AuraBook.ShadowFang));

            await Spell.NoneGcdCast("Dream Within a Dream", Me.CurrentTarget, () => Me.CurrentTarget.HasAura("Vulnerability Up"));
            await Spell.NoneGcdCast("Assassinate", Me.CurrentTarget, () => true);

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


        private const int HutonRecast = 6000;
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

                    if (!(Kassatsu.Cooldown.TotalMilliseconds <= 0) || !Core.Player.HasTarget)
                        return false;

                    if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Kassatsu, null) && Me.CurrentTarget.HasAura("Shadow Fang") && Me.CurrentTarget.HasAura(492)))
                    {
                        Logger.Write("YourRaidingBuddy Casting " + "Raiton");
                        if (Me.CurrentTarget.HasAura(492)) await CastRaiton();
                    }


                    return false;

                }

                if (taCD.TotalSeconds >= 5)
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

        private static async Task CastHuton()
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


        private static async Task<bool> CastDoton()
        {

            if (Actionmanager.CanCastOrQueue(Jin, null))
            {
                if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Jin, null)))
                {
                    if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null)))
                    {
                        if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Chi, null)))
                        {
                            return await CastNinjutsu();
                        }
                    }
                }

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