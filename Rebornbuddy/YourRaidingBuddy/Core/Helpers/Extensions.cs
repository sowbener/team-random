using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Helpers
{
    public static class Extensions
    {

        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dic, Func<TValue, bool> predicate)
        {
            var keys = dic.Keys.Where(k => predicate(dic[k])).ToList();
            foreach (var key in keys)
            {
                dic.Remove(key);
            }
        }

        public static void UpdateDoubleCastDict(string spellName, GameObject unit)
        {
            if (unit == null)
                return;



            SpellData spellData;


            Actionmanager.CurrentActions.TryGetValue(spellName, out spellData);


            if (spellData == null)
                return;


            TimeSpan expir = DateTime.UtcNow.TimeOfDay + spellData.AdjustedCastTime + TimeSpan.FromSeconds(4);
            string key = DoubleCastKey(unit.ObjectId, spellName);
            if (DoubleCastPreventionDict.ContainsKey(key))
                DoubleCastPreventionDict[key] = expir;
            else
            DoubleCastPreventionDict.Add(key, expir);
        }

        public static void UpdateDoubleCastDict(uint spellName, GameObject unit)
        {
            if (unit == null)
                return;

            var castingspell = DataManager.GetSpellData(spellName);

            SpellData spellData;


            Actionmanager.CurrentActions.TryGetValue(spellName, out spellData);


            if (spellData == null)
                return;


            TimeSpan expir = DateTime.UtcNow.TimeOfDay + spellData.AdjustedCastTime + TimeSpan.FromSeconds(4);
            string key = DoubleCastKeyID(unit.ObjectId, spellName);
            if (DoubleCastPreventionDict.ContainsKey(key))
                DoubleCastPreventionDict[key] = expir;
            else
                DoubleCastPreventionDict.Add(key, expir);
        }

        public static string DoubleCastKey(uint guid, string spellName)
        {
            return guid.ToString("X") + "-" + spellName;
        }

        public static string DoubleCastKeyID(uint guid, uint spellName)
        {
            var castingspell = DataManager.GetSpellData(spellName);
            return guid.ToString("X") + "-" + castingspell;
        }

        public static string DoubleCastKey(GameObject unit, string spell)
        {
            return DoubleCastKey(unit.ObjectId, spell);
        }

        public static string DoubleCastKeyID(GameObject unit, uint spellName)
        {
            return DoubleCastKeyID(unit.ObjectId, spellName);
        }


        public static bool Contains(this Dictionary<string, TimeSpan> dict, GameObject unit, string spellName)
        {
            return dict.ContainsKey(unit == null ? DoubleCastKey(Core.Player.ObjectId, spellName) : DoubleCastKey(unit, spellName));
        }

        public static bool Contains(this Dictionary<string, TimeSpan> dict, GameObject unit, uint spellName)
        {
            return dict.ContainsKey(unit == null ? DoubleCastKeyID(Core.Player.ObjectId, spellName) : DoubleCastKeyID(unit, spellName));
        }

        public static bool ContainsAny(this Dictionary<string, TimeSpan> dict, GameObject unit,
             params string[] spellNames)
        {
            return unit == null ? spellNames.Any(s => dict.ContainsKey(DoubleCastKey(Core.Player.ObjectId, s))) : spellNames.Any(s => dict.ContainsKey(DoubleCastKey(unit, s)));
        }

        public static bool ContainsAll(this Dictionary<string, TimeSpan> dict, GameObject unit,
             params string[] spellNames)
        {
            return unit == null ? spellNames.Any(s => dict.ContainsKey(DoubleCastKey(Core.Player.ObjectId, s))) : spellNames.All(s => dict.ContainsKey(DoubleCastKey(unit, s)));
        }

        public static readonly Dictionary<string, TimeSpan> DoubleCastPreventionDict =
             new Dictionary<string, TimeSpan>();

        public static bool HasAura(this GameObject unit, string spellname, bool isMyAura = false, int msLeft = 0)
        {
            var unitasc = (unit as Character);
            if (unit == null || unitasc == null)
            {
                return false;
            }
            var auras = isMyAura
                ? unitasc.CharacterAuras.Where(r => r.CasterId == Core.Player.ObjectId && r.Name.Equals(spellname))
                : unitasc.CharacterAuras.Where(r => r.Name.Equals(spellname));
            auras = auras.Where(aura => aura.TimespanLeft.TotalMilliseconds >= msLeft);
            var e = auras as IList<Aura> ?? auras.ToList();

            foreach (var a in e)
            {
               Logger.WriteDebug("Aura " + a.Name + " - " + a.TimeLeft);
            }
            return e.Count > 0;
        }

        public static bool HasAura(this GameObject unit, int spellname, bool isMyAura = false, int msLeft = 0)
        {
            var unitasc = (unit as Character);
            if (unit == null || unitasc == null)
            {
                return false;
            }
            var auras = isMyAura
                ? unitasc.CharacterAuras.Where(r => r.CasterId == Core.Player.ObjectId && r.Id == spellname)
                : unitasc.CharacterAuras.Where(r => r.Id == spellname);
            auras = auras.Where(aura => aura.TimespanLeft.TotalMilliseconds >= msLeft);
            var e = auras as IList<Aura> ?? auras.ToList();

            foreach (var a in e)
            {
                Logger.WriteDebug("Aura " + a.Name + " - " + a.TimeLeft);
            }
            return e.Count > 0;
        }

        public static bool HasAuraFading(this GameObject unit, int spellname, bool isMyAura = false, int msLeft = 0)
        {
            var unitasc = (unit as Character);
            if (unit == null || unitasc == null)
            {
                return false;
            }
            var auras = isMyAura
                ? unitasc.CharacterAuras.Where(r => r.CasterId == Core.Player.ObjectId && r.Id == spellname)
                : unitasc.CharacterAuras.Where(r => r.Id == spellname);
            auras = auras.Where(aura => aura.TimespanLeft.TotalMilliseconds <= msLeft);
            var e = auras as IList<Aura> ?? auras.ToList();

            foreach (var a in e)
            {
                Logger.WriteDebug("Aura " + a.Name + " - " + a.TimeLeft);
            }
            return e.Count > 0;
        }

        public static bool HasAuraFading(this GameObject unit, string spellname, bool isMyAura = false, int msLeft = 0)
        {
            var unitasc = (unit as Character);
            if (unit == null || unitasc == null)
            {
                return false;
            }
            var auras = isMyAura
                ? unitasc.CharacterAuras.Where(r => r.CasterId == Core.Player.ObjectId && r.Name.Equals(spellname))
                : unitasc.CharacterAuras.Where(r => r.Name.Equals(spellname));
            auras = auras.Where(aura => aura.TimespanLeft.TotalMilliseconds <= msLeft);
            var e = auras as IList<Aura> ?? auras.ToList();

            foreach (var a in e)
            {
                Logger.WriteDebug("Aura " + a.Name + " - " + a.TimeLeft);
            }
            return e.Count > 0;
        }

        public static bool HasAura(this GameObject unit, List<string> spellnames, bool isMyAura = false, int msLeft = 0)
        {
            var unitasc = (unit as Character);
            if (unit == null || unitasc == null)
            {
                return false;
            }
            IEnumerable<Aura> auras;
            {
                auras = isMyAura
                    ? unitasc.CharacterAuras.Where(r => r.CasterId == Core.Player.ObjectId && spellnames.Any(n => !string.IsNullOrEmpty(n) && r.Name.ToLower().Equals(n)))
                    : unitasc.CharacterAuras.Where(r => spellnames.Any(n => !string.IsNullOrEmpty(n) && r.Name.ToLower().Equals(n)));
            }
            auras = auras.Where(aura => aura.TimespanLeft.TotalMilliseconds >= msLeft);
            var e = auras as IList<Aura> ?? auras.ToList();

            if (!InternalSettings.Instance.General.Debug) return e.Count > 0;
            foreach (var a in e)
            {
                Logger.WriteDebug("Aura " + a.Name + " - " + a.TimeLeft);
            }

            return e.Count > 0;
        }

        public static bool ShowPlayerNames = false;

        public static string SafeName(this GameObject obj)
        {
            if (obj.IsMe)
            {
                return "Me";
            }

            string name;
            if (obj as BattleCharacter != null)
            {
                if ((obj as BattleCharacter).CanAttack)
                {
                    name = "Enemy.";
                }
                else
                {
                    name = "Ally.";
                }
                name += ShowPlayerNames ? (obj).Name : ((BattleCharacter)obj).CurrentJob.ToString();
            }
            else
            {
                name = obj.Name;
            }

            return name + "." + obj.ObjectId;
        }

    }
}