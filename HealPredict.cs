using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;

namespace YourRaidingBuddy.HealPredict
{
    internal class HealPredicter : ITargetingProvider
    {
        private static Dictionary<uint, int> _spells;

        public static Dictionary<uint, int> Spells
        {
            get { return _spells ?? (_spells = LoadSpells(Core.Me.CurrentJob)); }
            set { _spells = value; }
        }

        public List<BattleCharacter> GetObjectsByWeight()
        {
            return PartyManager.VisibleMembers
                .Select(c => c.GameObject as BattleCharacter)
                .Where(c => c != null && c.CurrentHealth < c.MaxHealth && !c.IsDead)
                .OrderByDescending(GetScoreForMember)
                .ToList();
        }

        private double GetScoreForMember(BattleCharacter c)
        {
            throw new NotImplementedException();
        }

        #region Load Spells

        private static Dictionary<uint, int> LoadSpells(ClassJobType jobType)
        {
            switch (jobType)
            {
                case ClassJobType.Scholar:
                    return LoadScholarSpells();

                case ClassJobType.WhiteMage:
                    return LoadWhiteMageSpells();

                case ClassJobType.Conjurer:
                    return LoadWhiteMageSpells();

                default:
                    return _spells = new Dictionary<uint, int>();

            }
        }

        internal static double HealAmount(SpellData spell)
        {
            var me = Core.Player.Stats;
            var Me = Core.Player;
            var weaponDamage = InventoryManager.EquippedItems.Where(x => x.Slot == (ushort)EquipmentSlot.MainHand).Select(z => z.Item).First().Attributes[ItemAttribute.Magic_Damage];
            var spellPotency = Spells[spell.Id];
            var multiplier = Me.ClassLevel < 20 ? 1 : Me.ClassLevel < 40 ? 1.1 : 1.3;
            return ((spellPotency / 300D) * ((weaponDamage) * (0.0114 * me.Mind + 0.00145 * me.Determination + 0.3736) + (0.156 * me.Mind) + (0.11 * me.Determination) + (0.00018316 * me.Mind * me.Determination)));
        }
        /// <summary>
        /// Dictionary of SpellData.Id, Potency
        /// </summary>
        private static Dictionary<uint, int> LoadScholarSpells()
        {
            return new Dictionary<uint, int>
            {
                { 190, 300 },   //Physick
                { 185, 300 }    //Adloquium

            };
        }
        /// <summary>
        /// Dictionary of SpellData.Id, Potency
        /// </summary>
        private static Dictionary<uint, int> LoadWhiteMageSpells()
        {
            return new Dictionary<uint, int>
            {
                {120, 400},     //Cure
                {124, 300}      //Medica
            };
        }

        #endregion
    }
}