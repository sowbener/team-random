using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Interfaces.Settings;
using YourRaidingBuddy.Books;

namespace YourRaidingBuddy.Helpers
{
    public static class Spell
    {
        public static bool RotationSwap, Opener;
        public static string LogFilter, NoneGcdCache, NoneGcdLast;
        public static GameObject NoneGcdCacheTarget, NoneGcdLastTarget;
        public static double QueueLogFilter;
        public static BagSlot Potion;
        public static SpellData GetSpellData(string spell)
        {
            SpellData data;
            Actionmanager.CurrentActions.TryGetValue(spell, out data);
            return data;
        }

        #region Healing stuff+

        internal static async Task<bool> CastHealing(uint datamanager, string spell, bool AoEHealing = false, int AmountUnits = 1, int HealthPercent = 99, int OverrideHealingPct = 10)
        {

            try
            {

                    if (Core.Me.IsCasting)
                        return false;

                int UnitCount = VariableBook.FriendlyUnitsCount;
                var castingspell = DataManager.GetSpellData(datamanager);
                var target = HealTargeting.Instance.FirstUnit;

                if (target == null)
                    return false;

                var targetHpPct = target.MaxHealth / target.CurrentHealth * 100;
                var targetPredictedHealing = HealPredict.HealPredicter.HealAmount(castingspell) + 30 / target.CurrentHealth * 100;

                #region AoE Healing

                if (AoEHealing == true && targetPredictedHealing - targetHpPct >= (1 * UnitCount * 100 + OverrideHealingPct) && target != null)
                    return false;

                if (AoEHealing == true && targetPredictedHealing - targetHpPct >= (1 * UnitCount * 100 + OverrideHealingPct) && target != null)
                {
                    Actionmanager.StopCasting();
                    return true;
                }

                if (AoEHealing == true && target.CurrentHealthPercent <= HealthPercent && UnitCount >= AmountUnits && target != null)
                {
                    await CastHeal(castingspell, target);
                    return true;
                }


                #endregion

                #region Single-Target Healing
                if (AoEHealing == false && targetPredictedHealing - targetHpPct >= (100 + OverrideHealingPct) && target != null)
                    return false;

                if (AoEHealing == false && targetPredictedHealing - targetHpPct >= (100 + OverrideHealingPct) && target != null)
                {
                    Actionmanager.StopCasting();
                    return true;
                }

                if (AoEHealing == false && target.CurrentHealthPercent <= HealthPercent && target != null)
                {
                    await CastHeal(castingspell, target);
                    return true;
                }
                #endregion

                return false;

                }
            catch (Exception ex)
            {
               if(InternalSettings.Instance.General.Debug) Logger.WriteDebug("CastBase.CastMultiDoT() Exception: {0}", ex); return false;
            }
        }
        #endregion

        #region Testing Selection of Units (Maybe multidotting in the future)
        internal static BattleCharacter SelectUnit()
        {
            try
            {
                var dotunitregular = Unit.FriendlyPriorities.FirstOrDefault(u =>
                    u.CurrentHealthPercent < 99); //&& (!u.HasAnyAura(Global.HashAuraList) || !Global.HashUnitList.Contains(u.Entry)));

                Logger.Write("Amount of Units We Can Select {0}", dotunitregular);
                return dotunitregular;
            }
            catch (Exception ex)
            {
                if(InternalSettings.Instance.General.Debug) Logger.WriteDebug("SelectMultiDoTUnit Exception: {0}", ex.ToString()); return null;
            }
        }
        #endregion

        #region PullCast

        public static async Task<bool> PullCast(string name)
        {
            return await PullCast(name, Core.Player.CurrentTarget);
        }
        public static async Task<bool> PullCast(string name, Func<bool> cond)
        {
            return await PullCast(name, Core.Player.CurrentTarget, cond);
        }
        public static async Task<bool> PullCast(string name, GameObject o)
        {
            return await PullCast(name, o);
        }
        public static async Task<bool> PullCast(string name, GameObject o, Func<bool> cond)
        {
            if (!InternalSettings.Instance.General.Movement || Core.Player.IsCasting || !cond()) return false;
            o = o ?? Core.Player.CurrentTarget;
            if (o == null) { return false; }
            var ck = Actionmanager.InSpellInRangeLOS(name, o);
            if (ck == SpellRangeCheck.ErrorNotInLineOfSight
                    || ck == SpellRangeCheck.ErrorNotInRange && !(MovementManager.IsMoving && Movement.IsFacingMovement(Core.Player, o)))
            {
                Movement.MoveTo(o);
                return true;
            }
            //if (ck == SpellRangeCheck.ErrorNotInFront) return await Movement.CheckFace(o);
            return await CastNew(name, o, cond);
        }

        #endregion

        #region combo
        public static bool ComboCountCheck;
        public static Combo LastCombo;
        public static Combo CachedCombo;
        public static TimeSpan ComboCountdown;
        public static Dictionary<uint, Combo> ComboSpells = new Dictionary<uint, Combo>
        {
            {75, Combo.TrueThrust},
            {78, Combo.VorpalThrust},
            {79, Combo.HeavyThrust},
            {81, Combo.ImpulseDrive},
            {87, Combo.Disembowel},
            {2240, Combo.SpinningEdge},
            {2242, Combo.GustSlash},
            {84, Combo.FullThrust},
            {88, Combo.ChaosThrust},
            {89, Combo.RingofThorns},
            {2251, Combo.DancingEdge},
            {2255, Combo.AeolianEdge},
            {2257, Combo.ShadowFang},
            {31, Combo.HeavySwing},
            {35, Combo.SkullSunder},
            {37, Combo.Maim},
            {42, Combo.StormsPath},
            {45, Combo.StormsEye},
            {47, Combo.ButchersBlock},
            {9, Combo.FastBlade},
            {11, Combo.SavageBlade},
            {15, Combo.RiotBlade},
            {21, Combo.RageofHalone},
            {154, Combo.Blizzard3},
        };

        public static void SyncCombos()
        {
            if (!ComboCountCheck) return;
            if (ComboCountdown > DateTime.UtcNow.TimeOfDay)
            {
                if (!Gcd || CachedCombo == Combo.Flushed) return;
                LastCombo = CachedCombo;
                CachedCombo = Combo.Flushed;
            }
            else
            {
                CachedCombo = Combo.Flushed;
                if (ComboSpells.ContainsKey(Actionmanager.LastSpellId))
                    LastCombo = ComboSpells[Actionmanager.LastSpellId];
                else
                {
                    ComboCountCheck = false;
                    if ((LastCombo | Combo.Finished) != Combo.Finished)
                        Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Clearing " + LastCombo + " because " + Actionmanager.LastSpell);
                    LastCombo = Combo.Finished;

                }
            }
        }
#endregion

        #region gcd

        public static TimeSpan GcdTimeSpan;
        public static double GcdTime;
        public static bool Gcd, UsedNoneGcd;

        public static void GcdPulse()
        {
            if (Core.Player.CurrentTarget != null && Movement.IsInSafeRange(Core.Player.CurrentTarget) && Core.Player.CurrentTarget.InLineOfSight())
            {
                if (!Extensions.DoubleCastPreventionDict.Contains(null, "gcd")) SetGcd();
                else GcdTime = (GcdTimeSpan - DateTime.UtcNow.TimeOfDay).TotalMilliseconds;
            }
            else GcdTime = 0;
            Gcd = GcdTime > InternalSettings.Instance.General.SpellQueue;
            if ((!UsedNoneGcd && NoneGcdCache == null) || !(GcdTime < InternalSettings.Instance.General.NoneGcdWindowEnd))
                return;
            NoneGcdLastTarget = NoneGcdCacheTarget;
            NoneGcdCacheTarget = null;
            NoneGcdLast = NoneGcdCache;
            NoneGcdCache = null;
            UsedNoneGcd = false;
        }
        public static bool InGcd()
        {
            return GcdTime > InternalSettings.Instance.General.SpellQueue;
        }
        public static bool NotCasting()
        {
            return !Core.Player.IsCasting;
        }
        public static bool InvalidOrOutGcdCon()
        {
            return (Core.Player.CurrentTarget as BattleCharacter) == null || (Core.Player.CurrentTarget as BattleCharacter).IsDead || GcdTime < 480;
        }
        public static void SetGcd()
        {
            switch (Core.Player.CurrentJob)
            {
                case ClassJobType.Ninja:
                    GcdTime = GcdMs("Spinning Edge");
                    break;
                case ClassJobType.Rogue:
                    GcdTime = GcdMs("Spinning Edge");
                    break;
                case ClassJobType.Dragoon:
                case ClassJobType.Lancer:
                    GcdTime = GcdMs("True Thrust");
                    break;
                case ClassJobType.Monk:
                case ClassJobType.Pugilist:
                    GcdTime = GcdMs("Bootshine");
                    break;
                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    GcdTime = GcdMs("Stone");
                    break;
                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    GcdTime = GcdMs("Heavy Swing");
                    break;
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    GcdTime = GcdMs("Fast Blade");
                    break;
                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    GcdTime = GcdMs("Heavy Shot");
                    break;
                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    GcdTime = GcdMs("Blizzard");
                    break;
                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                case ClassJobType.Scholar:
                    GcdTime = GcdMs("Ruin");
                    break;
            }
            if (!InGcd()) return;
            GcdTimeSpan = DateTime.UtcNow.TimeOfDay + TimeSpan.FromMilliseconds(GcdTime);
         //   Extensions.UpdateDoubleCastDictSpecific("gcd", null, (GcdTime - InternalSettings.Instance.General.SpellQueue)/1000);
        }
        public static double GcdMs(string spell)
        {
            var data = GetSpellData(spell);
            if (data == null) return 0;
            var cooldown = data.Cooldown.TotalMilliseconds;
            return (Math.Abs(cooldown - data.AdjustedCooldown.TotalMilliseconds) < 50
                || Math.Abs(cooldown - data.BaseCooldown.TotalMilliseconds) < 50) ? 0 : cooldown;
        }
        #endregion

        #region Apply Cast


        public static async Task<bool> ApplyCast(string spell, GameObject o, Func<bool> cond, int msLeft = 0, bool ignoreCanCast = false)
        {
            SpellData data;
            if (Actionmanager.CurrentActions.TryGetValue(spell, out data))
            {
                o = o ?? Core.Player.CurrentTarget;

                if (Extensions.DoubleCastPreventionDict.Contains(null, spell))
                    return false;

                if(o as Character != null && (o as Character).IsDead && o.Type != GameObjectType.Pc)
                    return false;

                if(!cond())
                    return false;

                if (o == null)
                    return false;

                if (!ignoreCanCast && !Actionmanager.CanCast(spell, o))
                    return false;

                if (Extensions.DoubleCastPreventionDict.Contains(o, spell))
                    return false;

                Logger.Write("Applying " + spell);
                if (Actionmanager.DoAction(spell, o))
                {
                    await SleepForLagDuration();
                    Extensions.UpdateDoubleCastDict(spell, o);
                    return true;
                }

                return true;
            }

            return false;

           }


        #endregion

        #region Cast - by name
        public static bool CanCast(SpellData data, GameObject o)
        {
            if (o == null || data == null) return false;
            var ck = Actionmanager.InSpellInRangeLOS(data, o);
            if (ck == SpellRangeCheck.ErrorNotInRange || ck == SpellRangeCheck.ErrorNotInLineOfSight)
            {
                Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + data.Name + " Failed: Range/LoS");
                return false;
            }
            if (ck == SpellRangeCheck.ErrorNotInFront)
            {
                if(InternalSettings.Instance.General.Facing)
                {
                    Logging.Write(Colors.Red, "[YourRaidingBuddy] Attempted to cast while not facing!");
                    Movement.Face(o);
                }
            }
            var cooldown = data.Cooldown.TotalMilliseconds;
            if (cooldown < InternalSettings.Instance.General.SpellQueue ||
                Math.Abs(cooldown - data.AdjustedCooldown.TotalMilliseconds) < 50
                || Math.Abs(cooldown - data.BaseCooldown.TotalMilliseconds) < 50)
            {
                if (InternalSettings.Instance.General.Dodge && Movement.Avoiding && cooldown < 10 && Movement.AvoidTill > DateTime.UtcNow.TimeOfDay && !Movement.IsFacingMovement(Core.Player, Movement.AvoidTo)) Movement.StopMove();
                return true;
            }
          //  if (cooldown > 2500) Extensions.UpdateDoubleCastDictSpecific(data.Name, null, (cooldown - InternalSettings.Instance.General.SpellQueue)/1000);
            if (InternalSettings.Instance.General.Debug)
                Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + data.Name + " CanCast Failed: " + cooldown);
            return false;
        }

        public static async Task<bool> CastSpell(string name, Func<bool> cond)
        {
            return await CastNew(name, Core.Player.CurrentTarget, cond);
        }

        public static async Task<bool> CastSpell(string name, GameObject o, Func<bool> cond)
        {
            return await CastNew(name, o, cond);
        }


        public static readonly Dictionary<string, DateTime> RecentSpell = new Dictionary<string, DateTime>();


        public static async Task<bool> SleepForLagDuration()
        {
            await Coroutine.Sleep(InternalSettings.Instance.General.PingMs * 2 + 50);
            return true;
        }

        internal static async Task<bool> CastHeal(SpellData Spell, BattleCharacter o)
        {

            if (o == null || !o.IsValid)
                return false;

            if (!Actionmanager.CanCast(Spell, o))
            {
                return false;
            }

            if (Spell.GroundTarget)
            {
                if (!Actionmanager.DoActionLocation(Spell.Id, o.Location))
                    return false;
            }

            if (Actionmanager.DoAction(Spell, o))
            {
                await SleepForLagDuration();
                Logging.Write(Colors.Orchid, "[YourRaidingBuddy] Casting {0}", Spell);
                if(InternalSettings.Instance.General.Debug) Logging.Write(Colors.AliceBlue, "[YourRaidingBuddy] Have Selected {0} because it needs healing {1}", HealTargeting.Instance.FirstUnit, HealTargeting.Instance.FirstUnit.CurrentHealthPercent);
                return true;
            }
            return true;
        }

        public static async Task<bool> CastNew(string name, GameObject o, Func<bool> cond, bool ignoreCanCast = false, bool lockDoubleCast = false, bool falseOnFailedDoAction = false)
        {

            SpellData data;
            if (Actionmanager.CurrentActions.TryGetValue(name, out data))
            {
                if (InternalSettings.Instance.General.Debug && data.Id == 125)
                {
                    if (o != null)
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) Cast Raise Attempt: " + o.ObjectId);
                    else
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) Cast Raise Attempt on null!");
                }

                o = o ?? Core.Player.CurrentTarget;
                if ((lockDoubleCast && Extensions.DoubleCastPreventionDict.Contains(o, name) || Extensions.DoubleCastPreventionDict.Contains(null, name))
                    || (o as Character) != null && (o as Character).IsDead && o.Type != GameObjectType.Pc
                    || !cond() || (!ignoreCanCast && !Actionmanager.CanCast(name, o))) //if (!ignoreCanCast && !Actionmanager.CanCast(spell, o))
                    return false;
                var castTime = data.AdjustedCastTime.TotalSeconds > 0;
                if (InternalSettings.Instance.General.Debug && InternalSettings.Instance.General.WriteSpellQueue &&
                    GcdTime < 500
                    && (!name.Equals(LogFilter) || QueueLogFilter < GcdTime))
                {
                    QueueLogFilter = GcdTime;
                    Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + GcdTime + " Spell Queue");
                }
                else if (QueueLogFilter > GcdTime) QueueLogFilter = GcdTime;
                Root.ShouldPulse = true;
                if (ComboSpells.ContainsKey(data.Id))
                {
                    if (LastCombo != ComboSpells[data.Id])
                    {
                        if (castTime)
                            ComboCountdown = DateTime.UtcNow.TimeOfDay +
                                             TimeSpan.FromSeconds(GcdTime / 1000 + 1 + data.AdjustedCastTime.TotalSeconds);
                        else
                            ComboCountdown = DateTime.UtcNow.TimeOfDay +
                                             TimeSpan.FromSeconds(GcdTime / 1000 + 1);
                        ComboCountCheck = true;
                        if (CachedCombo == Combo.Flushed)
                            CachedCombo = ComboSpells[data.Id];
                    }
                }
                // else if(!Gcd) {}  
                if (Actionmanager.DoAction(name, o))
                {
                    await SleepForLagDuration();
                    Logging.Write(Colors.Orchid, "[YourRaidingBuddy] Casting {0}", name);
                    return true;
                }
                if (lockDoubleCast)
                {
                    if (InternalSettings.Instance.General.Debug)
                    {
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + name + " DoAction Success, applying double cast prevention: " + (2.5 + data.AdjustedCastTime.TotalSeconds));
                    }
                        Extensions.UpdateDoubleCastDict(name, o);
               }
                return true;
            }
            return false;
        }

#pragma warning disable 1998
        public static async Task<bool> CastLocation(string name, GameObject o, Func<bool> cond, bool ignoreCanCast = false, bool lockDoubleCast = false)
#pragma warning restore 1998
        {
                SpellData data;
                if (Actionmanager.CurrentActions.TryGetValue(name, out data))
                {
                    o = o ?? Core.Player.CurrentTarget;
                    var loc = o.Location;
                    if ((Extensions.DoubleCastPreventionDict.Contains(o, name) || Extensions.DoubleCastPreventionDict.Contains(null, name))
                        || (o as Character) != null && (o as Character).IsDead && o.Type != GameObjectType.Pc
                        || !cond() || !ignoreCanCast && !CanCast(data, o)) return false;
                    var castTime = data.AdjustedCastTime.TotalSeconds > 0;
                    if (castTime && MovementManager.IsMoving && !Core.Player.HasAura("Swiftcast"))
                    {
                        if (!InternalSettings.Instance.General.AttemptCastWhileMoving)
                            return false; //BotManager.Current.IsAutonomous;
                        Movement.StopMove();
                    }

                    Root.ShouldPulse = true;
                    if (Actionmanager.DoActionLocation(name, loc))
                    {
                        Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Casting {0} at {1}", name, loc);
                        if (lockDoubleCast)
                        {
                            if (InternalSettings.Instance.General.Debug)
                                Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + name + " DoAction Success, applying double cast prevention.");
                                 Extensions.UpdateDoubleCastDict(name, o);
                    }
                }
                    else if (InternalSettings.Instance.General.Debug)
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + name + " DoAction Failed");
                    return true;
                }
                return false;
        }

        public static async Task<bool> ConfirmedCast(string name, GameObject o, bool cond = true, bool lockDoubleCast = false,bool spellData = false)
        {
            SpellData data;
            if (Actionmanager.CurrentActions.TryGetValue(name, out data))
            {

                if (InternalSettings.Instance.General.Debug && data.Id == 125)
                {
                    if (o != null)
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) ConfirmedCast Raise Attempt: " + o.ObjectId);
                    else
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) ConfirmedCast Raise Attempt on null!");
                }

                o = o ?? Core.Player.CurrentTarget;
                if (Extensions.DoubleCastPreventionDict.Contains(o, name) || Extensions.DoubleCastPreventionDict.Contains(null, name)
                    || (o as Character) != null && (o as Character).IsDead && o.Type != GameObjectType.Pc
                    || !cond || !CanCast(data, o)) return false;
                var castTime = data.AdjustedCastTime.TotalSeconds > 0;
                if (castTime && MovementManager.IsMoving && !Core.Player.HasAura("Swiftcast"))
                {
                    if (!InternalSettings.Instance.General.AttemptCastWhileMoving)
                        return false;
                    Movement.StopMove();
                }
                Root.ShouldPulse = true;
                if (!name.Equals(LogFilter))
                {
                    Logging.Write(Colors.Orchid, "[YourRaidingBuddy] Casting " + name);
                    LogFilter = name;
                }

                if (InternalSettings.Instance.General.Debug && InternalSettings.Instance.General.WriteSpellQueue && GcdTime < 500)
                    Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + GcdTime + " Looped DoAction Start");
                if (spellData)
                {
                    if (!await Coroutine.Wait(1200 + InternalSettings.Instance.General.SpellQueue, () => 
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        (Actionmanager.DoAction(name, o) || true) && Core.Player.IsCasting && Core.Player.CastingSpellId == data.Id))
                    {
                        if (InternalSettings.Instance.General.Debug)
                            Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + name + " Looped DoAction Failed");
                        return false;
                    }
                    if (lockDoubleCast)
                    { 
                 //   if (castTime) Extensions.UpdateDoubleCastDictSpecific(name, o, 2.5 + data.AdjustedCastTime.TotalSeconds);
                 //   else
                 //       Extensions.UpdateDoubleCastDictSpecific(name, o);
                    if (InternalSettings.Instance.General.Debug)
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + data.Name + " Applying Cast Prevention: " + (data.Cooldown.TotalMilliseconds - InternalSettings.Instance.General.SpellQueue));
 
                    }
                }
                else if (lockDoubleCast)
                {
                    if (!await Coroutine.Wait(500 + InternalSettings.Instance.General.SpellQueue, () => Actionmanager.DoAction(name, o)))
                    {
                        if (InternalSettings.Instance.General.Debug)
                            Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + name + " Looped DoAction Failed");
                        return false;
                    } 
               //     if (castTime) Extensions.UpdateDoubleCastDictSpecific(name, o, 2.5 + data.AdjustedCastTime.TotalSeconds);
               //     else
              //          Extensions.UpdateDoubleCastDictSpecific(name, o);
                    if (InternalSettings.Instance.General.Debug)
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + data.Name + " Applying Cast Prevention: " + (data.Cooldown.TotalMilliseconds - InternalSettings.Instance.General.SpellQueue));
                }
                else
                {
                    if (!await Coroutine.Wait(500 + InternalSettings.Instance.General.SpellQueue, () => Actionmanager.DoAction(name, o) && data.Cooldown.TotalMilliseconds > 500 + InternalSettings.Instance.General.SpellQueue || data.Cooldown.TotalMilliseconds > 500 + InternalSettings.Instance.General.SpellQueue))
                    {
                        if (InternalSettings.Instance.General.Debug)
                            Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + name + " Looped DoAction Failed");
                        return false;
                    }
              //      if (castTime) Extensions.UpdateDoubleCastDictSpecific(name, o, (data.Cooldown.TotalMilliseconds - InternalSettings.Instance.General.SpellQueue) / 1000 + data.AdjustedCastTime.TotalSeconds);
              //      else
             //       Extensions.UpdateDoubleCastDictSpecific(data.Name, null, (data.Cooldown.TotalMilliseconds - InternalSettings.Instance.General.SpellQueue) / 1000);
                    if (InternalSettings.Instance.General.Debug)
                        Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + data.Name + " Applying Cast Prevention: " + (data.Cooldown.TotalMilliseconds - InternalSettings.Instance.General.SpellQueue));
                }
                if (InternalSettings.Instance.General.Debug && InternalSettings.Instance.General.WriteSpellQueue && GcdTime < 500)
                    Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) " + GcdTime + " Looped Spell Queue");
                return true;
            }
            return false;
        }

        public static async Task<bool> CastHeal(string name, GameObject o, bool castLock)
        {
            if (await CastNew(name, o, () => true, false, false, castLock))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> BuffParty(string name, Func<bool> cond, bool castAll = true)
        {
            if (PartyManager.IsInParty)
            {
                SpellData data;
                if (!Actionmanager.CurrentActions.TryGetValue(name, out data)) return false;
                if (!cond()) return false;
                var target = PartyManager.VisibleMembers.FirstOrDefault(
        unit => unit.IsValid && unit.GameObject.Type == GameObjectType.Pc && (unit.GameObject as Character) != null &&  (unit.GameObject as Character).IsAlive && !unit.GameObject.HasAura(name));
                GameObject o;
                if (target != null) o = target.GameObject;
                else if (!Core.Player.HasAura(name)) o = Core.Player;
                else return false;
                if (Extensions.DoubleCastPreventionDict.Contains(castAll ? Core.Player : o, name)) return false;
                if (!CanCast(data, castAll ? Core.Player : o)) return false;
                var castTime = data.AdjustedCastTime.TotalSeconds > 0;
                if (castTime)
                {
                    if (!InternalSettings.Instance.General.AttemptCastWhileMoving && MovementManager.IsMoving)
                        return false;
                    Movement.StopMove();
                }
                if (!Actionmanager.DoAction(name, castAll ? Core.Player : o)) return false;
                if (!name.Equals(LogFilter))
                {
                    Logging.Write(Colors.Orchid, "[YourRaidingBuddy] Buffing " + name);
                    LogFilter = name;
                }
            //    Extensions.UpdateDoubleCastDictSpecific(name, castAll ? Core.Player : o, 12);
                return true;
            }

            if (!await CastNew(name, Core.Player, () => true && !Core.Player.HasAura(name), false, false, false)) return false;
        //    Extensions.UpdateDoubleCastDictSpecific(name, Core.Player, 12);
            return true;
        }

        public static async Task<bool> NoneGcdCast(string name, GameObject o, Func<bool> cond, bool force = false)

        {
            if (o == null || Extensions.DoubleCastPreventionDict.Contains(null, name)
                || !cond()) return false;
            await CastNew(name, o, cond, false, true);
            return true;
        }

        #endregion

    }
}
