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
            return await HotkeyNinjaRotation();
        }

        public static async Task<bool> NinjaRotation()
        {
            if (await Posion()) return true;
            if (await Suiton()) return true;
            if (await DoNinjutsu()) return true;
            if (await Kassatsu()) return true;
            if (await EmergenHuton()) return true;
            if (await HutonRefresh()) return true;
            if (await DancingEdge()) return true;
            if (await ShadowFang()) return true;
            if (await AECombo()) return true;
            if (await TrickAttack()) return true;
            if (await Burst()) return true;
            if (await Interrupt()) return true;
            if (await NoneGCD()) return true;

            return false;
        }

        public static async Task<bool> HotkeyNinjaRotation()
        {
            if (await Posion()) return true;
            if (VariableBook.HkmMultiTarget)
            {
                if (await AoE()) return true;
            }
            if (VariableBook.HkmSpecialKey2)
            {
                if (await Suiton()) return true;
            }
            if (VariableBook.HkmSpecialKey)
            {
                if (await DoNinjutsu()) return true;
                if (await Kassatsu()) return true;
            }

            if (await EmergenHuton()) return true;
            if (await HutonRefresh()) return true;
            if (VariableBook.HkmSpecialKey3)
            {
                if (await DancingEdge()) return true;
            }
            if (await ShadowFang()) return true;
            if (await AECombo()) return true;
            if (await TrickAttack()) return true;
            if (VariableBook.HkmCooldowns)
            {
                if (await Burst()) return true;
            }
            if (VariableBook.HkmSpecialKey1)
            {
                if (await Interrupt()) return true;
            }
            if (await NoneGCD()) return true;

            return false;
        }

        public static async Task<bool> AoE()
        {
            if (NinjaSettings.AoEToggle)
            {
                if (Actionmanager.CanCastOrQueue(Kass, null) && VariableBook.HostileUnitsCount >= 3 && Core.Me.CurrentTarget.CurrentHealthPercent >= 60)
                {


                    if (await CastKaton()) return true;
                    if (await KatonKass()) return true;
                    if (await CastDoton()) return true;



                }
                if (Actionmanager.CanCastOrQueue(Kass, null) && VariableBook.HostileUnitsCount >= 3 && Core.Me.CurrentTarget.CurrentHealthPercent <= 60)
                {
                    await CastKaton();
                    await KatonKass();
                    await CastKaton();
                }

                if (VariableBook.HostileUnitsCount >= 3 && Core.Me.CurrentTarget.CurrentHealthPercent >= 60)
                {
                    await CastDoton();
                }
                if (VariableBook.HostileUnitsCount >= 3 && Core.Me.CurrentTarget.CurrentHealthPercent <= 60)
                {
                    await CastKaton();
                }
                if (VariableBook.HostileUnitsCount >= 5 && NinjaSettings.DeathBlossomToggle)
                {
                    await Spell.CastSpell("Death Blossom", Me, () => true);
                }
            }
            return false;
        }

        public static async Task<bool> Posion()
        {
            if (NinjaSettings.Wasp)
            {
                await Spell.CastSpell("Kiss of the Wasp", Me, () => !Core.Me.HasAura("Kiss of the Wasp"));
            }
            else
            {
                await Spell.CastSpell("Kiss of the Viper", Me, () => !Core.Me.HasAura("Kiss of the Viper"));
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
            if (!Me.HasAura("Duality") && !Core.Me.HasAura("Huton", true, 40000) && Actionmanager.LastSpell.Name == "Gust Slash" || (Me.CurrentTarget.IsFlanking && Actionmanager.LastSpell.Name == "Gust Slash"))
            {
                await DancingEdge();
                await ShadowFang();
                await Spell.CastSpell("Armor Crush", () => true);

            }


            return false;
        }

        public static async Task<bool> DancingEdge()
        {

            if (NinjaSettings.DancingEdge)
            {
                if (!Me.HasAura("Duality") && !Me.CurrentTarget.HasAura("Dancing Edge", true, NinjaSettings.DancingEdgeClip) && !Me.CurrentTarget.HasAura("Storm's Eye", false) && Actionmanager.LastSpell.Name == "Gust Slash" || (!Me.CurrentTarget.IsFlanking && !Me.CurrentTarget.IsBehind && Actionmanager.LastSpell.Name == "Gust Slash"))
                {
                    return await Spell.CastSpell("Dancing Edge", () => true);
                }
            }

            return false;
        }

        public static async Task<bool> ShadowFang()
        {

            if (!Me.CurrentTarget.HasAura("Shadow Fang", true, NinjaSettings.ShadowFangClip) && (Me.CurrentTarget.HasAura("Dancing Edge") || Me.CurrentTarget.HasAura("Storm's Eye")) && Actionmanager.LastSpell.Name == "Spinning Edge" && Core.Me.CurrentTarget.CurrentHealth >= NinjaSettings.MobHP)
            {
                return await Spell.CastSpell("Shadow Fang", () => true);
            }
            return false;
        }


        public static async Task<bool> AECombo()
        {
            await Spell.CastSpell("Duality", Me, () => Actionmanager.LastSpell.Name == "Gust Slash" && Me.CurrentTarget.IsBehind && (Me.CurrentTarget.HasAura("Dancing Edge") || Me.CurrentTarget.HasAura("Storm's Eye")));
            await Spell.CastSpell("Aeolian Edge", () => Me.HasAura("Duality") || Actionmanager.LastSpell.Name == "Gust Slash" && Me.CurrentTarget.IsBehind);
            await Spell.CastSpell("Gust Slash", () => Actionmanager.LastSpell.Name == "Spinning Edge");
            await Spell.CastSpell("Mutilate", () => Me.CurrentTarget.HasAura(AuraBook.ShadowFang) && !Me.CurrentTarget.HasAura(AuraBook.Mutilate, true, NinjaSettings.MutilationClip) && Core.Me.CurrentTarget.CurrentHealth >= NinjaSettings.MobHP);
            await Spell.CastSpell("Spinning Edge", () => true);

            return false;
        }

        public static async Task<bool> TrickAttack()
        {
            if (Spinning.Cooldown.TotalMilliseconds >= 1500)
            {
                await Spell.NoneGcdCast("Trick Attack", Me.CurrentTarget, () => Me.HasAura("Suiton") && Me.CurrentTarget.IsBehind);
            }
            return false;
        }

        public static async Task<bool> Suiton()
        {
            if (Actionmanager.CanCastOrQueue(Jin, null))
            {
                var taCD = Trick_Attack.Cooldown;
                if (taCD.TotalMilliseconds <= 1300)
                {
                    await CastSuiton();
                }
                if (Me.CurrentTarget.HasAura("Vulnerability Up") && taCD.TotalMilliseconds > 0)
                {
                    await Kassatsu();
                }
            }
            return false;
        }
        public static async Task<bool> Burst()
        {
            if (Spinning.Cooldown.TotalMilliseconds >= 1500)
            {
                await Spell.NoneGcdCast("Internal Release", Me, () => !Me.HasAura(AuraBook.InternalRelease) && Core.Me.CurrentTarget.CurrentHealth >= NinjaSettings.BuffHP);
                await Spell.NoneGcdCast("Blood for Blood", Me, () => !Me.HasAura("Blood for Blood") && Core.Me.CurrentTarget.CurrentHealth >= NinjaSettings.BuffHP);
                await Spell.NoneGcdCast("Mug", Me.CurrentTarget, () => true);
                await Spell.NoneGcdCast("Dream Within a Dream", Me.CurrentTarget, () => true);
            }
            return false;
        }



        public static async Task<bool> Interrupt()
        {
            await Spell.NoneGcdCast("Jugulate", Me.CurrentTarget, () => true);

            return false;
        }


        public static async Task<bool> NoneGCD()
        {
            await Spell.NoneGcdCast("Invigorate", Me, () => Me.CurrentTP < 550);
            await Spell.NoneGcdCast("Second Wind", Me, () => Me.CurrentHealthPercent <= 30);
            await Spell.NoneGcdCast("Assassinate", Me.CurrentTarget, () => true);

            return false;

        }
        #endregion

        #region Ninjatsu Copied From Kupo Credits Masta

        private static bool HasBleedingDebuff()
        {

            return Me.CurrentTarget.HasAura("Storm's Eye", false, 2000) || Me.CurrentTarget.HasAura("Dancing Edge");

        }



        private static readonly SpellData Kass = DataManager.GetSpellData(2264);
        private static readonly SpellData Jin = DataManager.GetSpellData(2263);
        private static readonly SpellData Chi = DataManager.GetSpellData(2261);
        private static readonly SpellData Ten = DataManager.GetSpellData(2259);
        private static readonly SpellData Ninjutsu = DataManager.GetSpellData(2260);
        private static readonly SpellData Jugulate = DataManager.GetSpellData(2251);
        private static readonly SpellData Spinning = DataManager.GetSpellData(2240);



        private static readonly SpellData Trick_Attack = DataManager.GetSpellData(2258);
        private static readonly SpellData Sneak_Attack = DataManager.GetSpellData(2250);
        private static readonly SpellData Katon = DataManager.GetSpellData(2266);
        private static readonly SpellData Doton = DataManager.GetSpellData(2270);

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
            if (Spinning.Cooldown.TotalMilliseconds >= 1500)
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
        }


        private static async Task<bool> KatonKass()
        {
            if (Spinning.Cooldown.TotalMilliseconds >= 1500)
            {
                if (NinjaSettings.KassatsuAoE)
                {
                    if (VariableBook.HostileUnitsCount >= 3 && Ninjutsu.Cooldown.TotalMilliseconds > 16000)
                    {
                        await Spell.NoneGcdCast("Kassatsu", Me, () => true);
                    }
                }
            }
            return false;
        }
        private static async Task<bool> Kassatsu()
        {
            if (Spinning.Cooldown.TotalMilliseconds >= 1500)
            {
                if (NinjaSettings.Kassatsu)
                {
                    if (Me.CurrentTarget.HasAura("Vulnerability Up") && Trick_Attack.Cooldown.TotalMilliseconds > 0)
                    {
                        await Spell.NoneGcdCast("Kassatsu", Me, () => true);
                    }
                }
            }
            return false;
        }
        private static async Task<bool> CastFuma()
        {
            if (Spinning.Cooldown.TotalMilliseconds >= 1500)
            {
                if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null)))
                {
                    return await CastNinjutsu();
                }
            }
            return false;
        }

        private static async Task<bool> CastKaton()
        {
            if (DataManager.GetSpellData(2240).Cooldown.TotalMilliseconds >= 1500)
            {
                if (Actionmanager.CanCastOrQueue(Jin, null))
                {
                    if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Chi, null)))
                    {
                        if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null)))
                        {
                            return await CastNinjutsu();
                        }
                    }
                }
            }
            return false;
        }
        private static async Task<bool> CastDoton()
        {
            if (DataManager.GetSpellData(2240).Cooldown.TotalMilliseconds >= 1500)
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
            }
            return false;
        }

        private static async Task<bool> CastRaiton()
        {
            if (Spinning.Cooldown.TotalMilliseconds >= 1500)
            {
                if (!await Coroutine.Wait(2000, () => Actionmanager.DoAction(Ten, null))) return false;
                if (await Coroutine.Wait(2000, () => Actionmanager.DoAction(Chi, null)))
                {
                    return await CastNinjutsu();
                }
            }
            return false;
        }

        private static async Task<bool> CastSuiton()
        {

            if (Core.Me.CurrentTarget.CurrentHealth >= NinjaSettings.SuitonHP)
            {
                if (Spinning.Cooldown.TotalMilliseconds >= 1500)
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