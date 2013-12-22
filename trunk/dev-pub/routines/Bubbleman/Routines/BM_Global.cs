using Bubbleman.Core;
using Bubbleman.Helpers;
using Bubbleman.Managers;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using KP = Bubbleman.Managers.BMHotKeyManager.KeyboardPolling;
using SG = Bubbleman.Interfaces.Settings.BMSettings;
using SH = Bubbleman.Interfaces.Settings.BMSettingsH;
using Spell = Bubbleman.Core.BMSpell;
using T = Bubbleman.Managers.BMTalentManager;


namespace Bubbleman.Routines
{
   public class BMGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
         internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                new Decorator(ret => BMUnit.DefaultBuffCheck && (SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat,
                new PrioritySelector(
                Spell.CastRaidBuff("Legacy of the White Tiger", ret => Me.Specialization == WoWSpec.PaladinRetribution && !Me.HasAnyAura(LegacyoftheWhiteTiger)),
                Spell.Cast("Stance of the Sturdy Ox", ret => Me.Specialization == WoWSpec.PaladinProtection),
                Spell.CastRaidBuff("Legacy of the Emperor", ret => !Me.HasAnyAura(LegacyoftheEmperor))
               )));
            }
        }


        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
               // new Action(delegate { BMSpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { BMUnit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; }),
                new Action(delegate { BMUnit.GetNearbyAggroUnitsCount(); return RunStatus.Failure; })
                );
        }

        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

        #region DevastatingAbilities

       internal static readonly HashSet<int> DevastatingAbilities = new HashSet<int>
       {

           143502, // Execute General Nazgrim HC 

       };


        #endregion

        #region InterruptList

        internal static Random _random = new Random();

        internal static readonly HashSet<int> LegacyoftheEmperor = new HashSet<int>
       {

           117666, // Legacy of the Emperor
           115921, // Legacy of the emperor
           1126, // Mark of the Wild
           20217, // Blessing of Kings
           90363, // Embrace of the Shale Spider	

       };

        internal static readonly HashSet<int> LegacyoftheWhiteTiger = new HashSet<int>
       {

         116781, // Legacy of the White Tiger
         17007, // Leader of the Pack
         1459, // Arcane Brilliance
         61316, // Dalaran Brilliance
         24604, // Furious Howl
         90309, // Terrifying Roar	
         126373, // Fearless Roar
         126309, // Still Water
       };

        internal static readonly HashSet<int> AgilityProcList = new HashSet<int>
        {

         126489, // Searing Words
         126554, // Bottle of Infinite Stars
         128984, // Relic of Xuen
         126707, // Tyrannical
         126690, // Call of Conquest
         146312, // Celestial Trinket
         126649, // Terror in the Mists
         138737, // Blades
         138756, // Blades
         138938, // Juju Madness
         138699, // Valor Trinket
         148896, // Sigil of Rampage
         146310, // Ticking Ebon Detonator
         146308, // Assurance of Consequence
         148903, // Haromm's Talisman

        };

        internal static readonly HashSet<int> InterruptListMoP = new HashSet<int>
       {
           136797, // Dino-Mending (ToT)
           136587, // Venom Bolt Volley (ToT)
           61909, // Fireball (ToT)
           136189, // Sandbolt (ToT)
           144583, // Wulf Healing (SoO)
           144018, // Corruption (SoO)
           144649, // Hurl Corruption (SoO)
           143473, // Empowered Chain Heal (SoO)
           145555, // Empowered Chain Heal (SoO)
           143432, // Arcane Shock (SoO)
           143431, // Magistrike (SoO)
           145230, // Forbidden Magic (SoO)
           144922, // Harden Flesh
           143958, // Corruption Shock
           145631, // Corruption Chain
           144923, // Earthen Shard
           144379, // Mocking Blast (Manifestation of Pride)
           144468, // Inspiring Song (Chi Ji Timeless Isle)
           146728, // Chain-Heal (SoO)
           146757, // Chain-Heal (SoO)
           143423, // Sha Sear
           123654, // Ming (5man HC instance)
           121284, // Wing Leader (5man HC instance)
           117833, // Crazy Thought (MSV)
           117628, // Shadow Blast (MSV)
           125877, // Dispatch (HoF)
           124077, // Dispatch (HoF)


       };

        internal static readonly HashSet<int> InterruptListTBA = new HashSet<int>
        {

        };

        #endregion

        #endregion

    }
}
