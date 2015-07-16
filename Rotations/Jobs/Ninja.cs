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
        private static LocalPlayer Me
        {
            get { return Core.Player; }
        }

        public static SettingsG GeneralSettings
        {
            get { return InternalSettings.Instance.General; }
        }

        public static NinjaSetting NinjaSettings
        {
            get { return InternalSettings.Instance.Ninja; }
        }

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
            await Kassatsu();
            await EmergenHuton();
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

        public static async Task<bool> EmergenHuton()
        {
            if (!Me.HasAura("Duality") && !Core.Me.HasAura("Huton", true, NinjaSettings.EmergencyHutonClip) && Actionmanager.LastSpell.Name == "Gust Slash")
            {
                await Spell.CastSpell("Armor Crush", () => true);
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
            if (!Me.HasAura("Duality") && !Core.Me.HasAura("Huton", true, 40000) && Actionmanager.LastSpell.Name == "Gust Slash")
            {
                await DancingEdge();
                await Shadow();
                await Spell.CastSpell("Armor Crush", () => true);

            }


            return false;
        }

        public static async Task<bool> DancingEdge()
        {
            if (NinjaSettings.DancingEdge)
            {
                if (!Me.HasAura("Duality") && !Me.CurrentTarget.HasAura("Dancing Edge", true, NinjaSettings.DancingEdgeClip) && !Me.CurrentTarget.HasAura("Storm's Eye", false) && Actionmanager.LastSpell.Name == "Gust Slash")
                {
                    return await Spell.CastSpell("Dancing Edge", () => true);
                }
            }
            return false;
        }

        public static async Task<bool> Shadow()
        {

            if (!Me.CurrentTarget.HasAura("Shadow Fang", true, NinjaSettings.ShadowFangClip) && (Me.CurrentTarget.HasAura("Dancing Edge") || Me.CurrentTarget.HasAura("Storm's Eye")) && Actionmanager.LastSpell.Name == "Spinning Edge" && Core.Me.CurrentTarget.CurrentHealth >= MobHp)
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
            await Spell.CastSpell("Mutilate", () => Me.CurrentTarget.HasAura(AuraBook.ShadowFang) && !Me.CurrentTarget.HasAura(AuraBook.Mutilate, true, NinjaSettings.MutilationClip) && Core.Me.CurrentTarget.CurrentHealth >= MobHp);
            await Spell.CastSpell("Spinning Edge", () => true);

            return false;
        }

        public static async Task<bool> NoneGCD()
        {
            await Spell.NoneGcdCast("Trick Attack", Me.CurrentTarget, () => Me.HasAura("Suiton") && Me.CurrentTarget.IsBehind);
            await Spell.NoneGcdCast("Internal Release", Me, () => !Me.HasAura(AuraBook.InternalRelease) && Core.Me.CurrentTarget.CurrentHealth >= BuffHp);
            await Spell.NoneGcdCast("Blood for Blood", Me, () => Unit.CombatTime.ElapsedMilliseconds > 2000 && !Me.HasAura("Blood for Blood") && Core.Me.CurrentTarget.CurrentHealth >= BuffHp);
            await Spell.NoneGcdCast("Invigorate", Me, () => Me.CurrentTP < 550);
            await Spell.NoneGcdCast("Second Wind", Me, () => Me.CurrentHealthPercent <= 30);
            await Spell.NoneGcdCast("Jugulate", Me.CurrentTarget, () => Unit.CombatTime.ElapsedMilliseconds > 8000);
            await Spell.NoneGcdCast("Mug", Me.CurrentTarget, () => Unit.CombatTime.ElapsedMilliseconds > 4000);

            await Spell.NoneGcdCast("Dream Within a Dream", Me.CurrentTarget, () => Unit.CombatTime.ElapsedMilliseconds > 6000);
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

        //private static readonly SpellData Kassatsu = DataManager.GetSpellData(2264);

        private static readonly SpellData Trick_Attack = DataManager.GetSpellData(2258);
        private static readonly SpellData Sneak_Attack = DataManager.GetSpellData(2250);

        public static HashSet<uint> OverrideBackstabIds = new HashSet<uint>()
        {
            3240//Cloud of darkness
        };


        private const int HutonRecast = 6000;
        internal static async Task<bool> DoNinjutsu()
        {
            if (Core.Player.HasAura("Mudra")) return true;

            if (Actionmanager.CanCastOrQueue(Jin, null))
            {
                if (!Me.HasAura("Huton", true, HutonRecast))
                {
                    await CastHuton();
                }

                var taCD = Trick_Attack.Cooldown;

                if (taCD.TotalMilliseconds <= 1300)
                {
                    await CastSuiton();
                }
                if (Me.CurrentTarget.HasAura("Vulnerability Up"))
                {
                    await Kassatsu();
                }
                if (NinjaSettings.ShurikenAlways)
                {
                    await CastFuma();
                }
                else
                {
                    await CastRaiton();
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


        private static async Task<bool> Kassatsu()
        {
            if (NinjaSettings.Kassatsu)
            {
                if (Me.CurrentTarget.HasAura("Vulnerability Up"))
                {
                    await Spell.NoneGcdCast("Kassatsu", Me, () => true);
                }
            }
            return false;
        }
        private static async Task<bool> CastFuma()
        {
            if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null)))
            {
                return await CastNinjutsu();
            }
            return false;
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
            if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null)))
            {
                if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Chi, null)))
                {
                    if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Jin, null)))
                    {
                        return await CastNinjutsu();
                    }
                }
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