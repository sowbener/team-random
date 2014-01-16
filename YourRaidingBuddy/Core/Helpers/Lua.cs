using System;
using System.Linq;
using System.Text;
using YourBuddy.Core.Utilities;
using YourBuddy.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Action = Styx.TreeSharp.Action;
using Lua = YourBuddy.Core.Helpers.LuaClass;

// Credits for this code go out to the PureRotation Team!
namespace YourBuddy.Core.Helpers
{
    internal static class LuaClass
    {
        #region Start AutoAttack
        internal static Composite StartAutoAttack
        {
            get
            {
                return new Action(ret =>
                    {
                        if (!StyxWoW.Me.IsAutoAttacking)
                            Styx.WoWInternals.Lua.DoString("StartAttack()");
                        return RunStatus.Failure;
                    });
            }
        }
        #endregion

        #region Run Macro text
        public static string RealLuaEscape(string luastring)
        {
            var bytes = Encoding.UTF8.GetBytes(luastring);
            return bytes.Aggregate(String.Empty, (current, b) => current + ("\\" + b));
        }

        public static Composite RunMacroText(string macro, CanRunDecoratorDelegate cond)
        {
            return new Decorator(
                       cond,
                       new PrioritySelector(
                           new Action(a => Styx.WoWInternals.Lua.DoString("RunMacroText(\"" + RealLuaEscape(macro) + "\")"))));
        }
        #endregion

        #region PlayerPower Rage
        public static uint PlayerPower
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Styx.WoWInternals.Lua.GetReturnVal<uint>("return UnitPower(\"player\");", 0);
                    }
                }
                catch { Logger.DiagLogPu("Yb: Lua Failed in PlayerPower"); return StyxWoW.Me.CurrentPower; }
            }
        }

        public static uint PlayerPowerMax
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Styx.WoWInternals.Lua.GetReturnVal<uint>("return UnitPowerMax(\"player\",1);", 0);
                    }
                }
                catch
                {
                    Logger.DiagLogPu("Yb: Lua Failed in PlayerPowerMax"); return 0;
                }
            }
        }
        #endregion

        #region PlayerPower Rage

        public static double Vengeance(int spellid)
        {
            try
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    var lua = String.Format("local vName,veng; vName=GetSpellInfo({0}); veng=select(15, UnitAura(\"player\", vName)); if veng==nil then return 0 else return veng end", spellid);
                    var t = Double.Parse(Styx.WoWInternals.Lua.GetReturnValues(lua)[0]);
                    return Math.Abs(t);
                }
            }
            catch
            {
                Logger.CombatLogFb(" Lua Failed in Vengeance");
                return 0;
            }
        }

        //local stagger = select(14, UnitAura("player", "moderate stagger", nil, "HARMFUL"));
        public static double PurifyingBrew(int spellid)
        {
            try
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    var lua = String.Format("local vName,brew; vName=GetSpellInfo({0}); brew=select(15, UnitDebuff(\"player\", vName, nil)); if brew==nil then return 0 else return brew end", spellid);
                    var t = Double.Parse(Styx.WoWInternals.Lua.GetReturnValues(lua)[0]);
                    return Math.Abs(t);
                }
            }
            catch
            {
                Logger.CombatLogFb(" Lua Failed in Purifying Brew");
                return 0;
            }
        }



        public static double LuaGetSpellCharges()
        {
            return Styx.WoWInternals.Lua.GetReturnVal<int>("return GetSpellCharges(115399)", 0);
        }

        public static double GetSpellRechargeTime(int spellId, int rechargeTime)
        {
            try
            {
                // currentCharges, maxCharges, timeLastCast, cooldownDuration = GetSpellCharges(spellId or spellName)
                double val = Double.Parse(Styx.WoWInternals.Lua.GetReturnValues("local x=select(3, GetSpellCharges(" + spellId + ")); if x==nil then return 999 else return GetTime()-x end")[0]);

                if (val > 0)
                {
                    return (rechargeTime - val);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                Logger.CombatLogFb("Lua failed in GetSpellRechargeTime");
                return 999;
            }
        }

        public static double PlayerChi
        {
            get
            {
                //return Me.CurrentChi;
                try
                {
                    // return Me.CurrentChi;
                    //using (Memory.AcquireFrame())
                    {
                        return Styx.WoWInternals.Lua.GetReturnVal<int>("return UnitPower(\"player\", SPELL_POWER_CHI);", 0);
                    }
                }
                catch
                {
                    Logger.DiagLogFb("Failed in Me.CurrentChi");
                    return 0;
                }
            }
        }


        #endregion

        public static double PlayerComboPts
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Styx.WoWInternals.Lua.GetReturnVal<int>("return GetComboPoints(\"player\");", 0);
                    }
                }
                catch
                {
                    //  Logger.FailLog(" Lua Failed in PlayerComboPts");
                    return 0;
                }
            }
        }

        #region Taken From SuperBad //YouKnowILoveYouNavi <3

        public static double LuaGetComboPoints()
        {
            return Styx.WoWInternals.Lua.GetReturnVal<int>("return GetComboPoints(\"player\",\"target\");", 0);
        }

        public static double LuaGetEnergyRegen()
        {
            return Styx.WoWInternals.Lua.GetReturnVal<float>("return GetPowerRegen()", 1);
        }



        public static double RJWOK()
        {
            double playerEnergy;
            double ER_Rate;
            double RJWCD;
            double RJWISOK;

            playerEnergy = Lua.PlayerPower;
            ER_Rate = Lua.LuaGetEnergyRegen();
            RJWCD = CooldownTracker.GetSpellCooldown(116847).TotalSeconds;

            RJWISOK = playerEnergy + (ER_Rate * RJWCD);

            return RJWISOK;

        }


        public static double BlackoutKickOK()
        {

            //actions.single_target+=/blackout_kick,if=energy+energy.regen*cooldown.rising_sun_kick.remains>=40

            double playerEnergy;
            double ER_Rate;
            double RSKCooldown;
            double BoKOK;

            playerEnergy = Lua.PlayerPower;
            ER_Rate = Lua.LuaGetEnergyRegen();
            RSKCooldown = CooldownTracker.GetSpellCooldown(107428).TotalSeconds;

            BoKOK = playerEnergy + ER_Rate * RSKCooldown;

            return BoKOK;
        }

        public static double JabOK()
        {
            double playerEnergy;
            double ER_Rate;
            double KSCD;
            double JabbingIsOK;

            playerEnergy = Lua.PlayerPower;
            ER_Rate = Lua.LuaGetEnergyRegen();
            KSCD = CooldownTracker.GetSpellCooldown(121253).TotalSeconds;

            JabbingIsOK = (playerEnergy - 40) + (KSCD * ER_Rate);

            return JabbingIsOK;

        }

        public static double TimeToEnergyCap()
        {

            double timetoEnergyCap;
            double playerEnergy;
            double ER_Rate;

            playerEnergy = Lua.PlayerPower; // current Energy 
            ER_Rate = Lua.LuaGetEnergyRegen();
            timetoEnergyCap = (100 - playerEnergy) * (1.0 / ER_Rate); // math 

            return timetoEnergyCap;
        }

        #endregion

        #region Disable Click-To-Move
        public static void DisableClickToMove()
        {
            if (InternalSettings.Instance.General.CheckDisableClickToMove)
            {
                Styx.WoWInternals.Lua.DoString("SetCVar('autoInteract', '0')");
            }
        }

        public static void EnableClickToMove()
        {
            if (InternalSettings.Instance.General.CheckDisableClickToMove)
            {
                Styx.WoWInternals.Lua.DoString("SetCVar('autoInteract', '1')");
            }
        }
        #endregion

        #region Other LUA
        internal static uint GetFps()
        {
            try
            {
                return (uint)Styx.WoWInternals.Lua.GetReturnVal<float>("return GetFramerate()", 0);
            }
            catch (Exception ex)
            {
                Logger.DiagLogWh("Yb: {0} - GetFps()", ex);
            }
            return 0;
        }
        #endregion
    }
}
