using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;

namespace YourRaidingBuddy.Helpers
{
    public class Enmity
    {
        private readonly FfxivMemory _memory;

        private Enmity()
        {
            var p = FfxivProcessHelper.GetFfxivProcess();

            if ((_memory == null && p != null) ||
                (_memory != null && p != null && p.Id != _memory.Process.Id))
            {
                _memory = new FfxivMemory(p);
            }
            else if (_memory != null && p == null)
            {
                _memory.Dispose();
                _memory = null;
            }
        }

        private static readonly Lazy<Enmity> instance = new Lazy<Enmity>(() => new Enmity());
        public static Enmity Instance
        {
            get { return instance.Value; }
        }

        /*
        Every condition below ignore tanks.
        Tries to get to the top of the enmity list.
        Tries to get at least enmityGap to the next party member.x
        */
        public bool NeedToGenerateEnmity(uint enmityGap = 10000)
        {
            var enmityList = Instance.GetEnmityEntryList();
            var enmityEntries = enmityList as IList<EnmityEntry> ?? enmityList.ToList();
            if (enmityEntries.Count <= 1) return false;
            var firstOnList = enmityEntries.First();
            var secondWithoutTankOnList = enmityEntries.First(e => e.BattleCharacter.IsMe == false && !IsTank(e.BattleCharacter));
            return !IsTank(firstOnList.BattleCharacter) ||
                   secondWithoutTankOnList != null && firstOnList.BattleCharacter.IsMe && firstOnList.Enmity - secondWithoutTankOnList.Enmity < enmityGap;
        }

        private static bool IsTank(Character c)
        {
            return c.CurrentJob == ClassJobType.Marauder || c.CurrentJob == ClassJobType.Warrior ||
                   c.CurrentJob == ClassJobType.Gladiator || c.CurrentJob == ClassJobType.Paladin ||
                   c.CurrentJob == ClassJobType.DarkKnight;
        }

        public IEnumerable<AggroEntry> GetAggroList()
        {
            return _memory.GetAggroList().Select(r => new AggroEntry(r));
        }

        public BattleCharacter GetAnchorCombatant()
        {
            return GetBattleCharacterWithId(_memory.GetAnchorCombatant().Id);
        }

        public IEnumerable<EnmityEntry> GetEnmityEntryList()
        {
            return _memory.GetEnmityEntryList().Select(r => new EnmityEntry(r));
        }

        public BattleCharacter GetFocusCombatant()
        {
            return GetBattleCharacterWithId(_memory.GetFocusCombatant().Id);
        }

        public BattleCharacter GetHoverCombatant()
        {
            return GetBattleCharacterWithId(_memory.GetHoverCombatant().Id);
        }

        public BattleCharacter GetTargetCombatant()
        {
            return GetBattleCharacterWithId(_memory.GetTargetCombatant().Id);
        }

        private static BattleCharacter GetBattleCharacterWithId(uint id)
        {
            return GameObjectManager.GetObjectsOfType<BattleCharacter>().First(a => a.ObjectId == id);
        }

        public class EnmityEntry
        {
            public BattleCharacter BattleCharacter;
            public uint Enmity;
            public int HateRate;

            public EnmityEntry(Helpers.EnmityEntry enmityEntry)
            {
                if (enmityEntry.IsMe)
                {
                    BattleCharacter = Core.Me;
                }
                else
                {
                    BattleCharacter = GetBattleCharacterWithId(enmityEntry.Id);
                }

                Enmity = enmityEntry.Enmity;
                HateRate = enmityEntry.HateRate;
            }
        }

        public class AggroEntry
        {
            public BattleCharacter BattleCharacter;
            public int HateRate;
            public int Order;
            public EnmityEntry Target;

            public AggroEntry(Helpers.AggroEntry aggroEntry)
            {
                BattleCharacter = GetBattleCharacterWithId(aggroEntry.Id);
                HateRate = aggroEntry.HateRate;
                Order = aggroEntry.Order;
                Target = new EnmityEntry(aggroEntry.Target);
            }
        }
    }
}