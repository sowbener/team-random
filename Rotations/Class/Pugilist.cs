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

namespace YourRaidingBuddy.Rotations
{
    public class Pugilist : Root
    {
        private static LocalPlayer Me { get { return Core.Player; } } //Core.Player.CurrentTarget as BattleCharacter

        public override ClassJobType[] Class
        {
            get { return new[] { ClassJobType.Pugilist }; }
        }

        public override void OnInitialize()
        {
            ;
        }

        #region NewRotation
        public static async Task<bool> AutoMode()
        {

            return await PugRotation();
        }


        public static async Task<bool> PugRotation()
        {
            await Spell.CastSpell("True Strike", () => Me.HasAura(108));
            await Spell.CastSpell("Bootshine", () => true);

            return false;
        }
        #endregion

        
    }
}
