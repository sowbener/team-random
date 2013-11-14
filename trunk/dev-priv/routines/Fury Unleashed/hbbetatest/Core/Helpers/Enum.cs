﻿using System;

namespace FuryUnleashed.Core.Helpers
{
    internal static class Enum
    {
        #region CBOItem
        public class FuCboItem
        {
            public readonly int E;
            private readonly string _s;

            public FuCboItem(int pe, string ps)
            {
                E = pe;
                _s = ps;
            }

            public override string ToString()
            {
                return _s;
            }
        }
        #endregion

        #region General ENUMS
        public enum AbilityTrigger
        {
            Never,
            Always,
            OnBossDummy,
            OnBlTwHr
        }

        public enum BarrierBlock
        {
            None,
            ShieldBlock,
            ShieldBarrier
        }

        public enum Interrupts
        {
            Constant,
            RandomTimed
        }

        public enum Hamstring
        {
            Never,
            Always,
            AddList
        }

        [Flags]
        public enum KeyStates
        {
            None = 0,
            Down = 1,
        }

        [Flags]
        internal enum LogCategory
        {
            None = 0,
            Performance
        }

        public enum Mode
        {
            Auto,
            Hotkey,
            SemiHotkey
        }

        public enum MsrTrigger
        {
            Never,
            Always,
            OnBossDummy
        }

        public enum Shouts
        {
            BattleShout,
            CommandingShout,
            None
        }

        public enum SunderArmor
        {
            Never,
            Always,
            OnBossDummy
        }

        public enum SunderStacks
        {
            OneStack,
            TwoStacks,
            ThreeStacks
        }

        public enum SvnUrl
        {
            Release,
            Development
        }

        public enum VcTrigger
        {
            Never,
            Always,
            OnVictoriousProc,
            OnT15Proc,
        }

        public enum VigilanceTrigger
        {
            Never,
            OnTank,
            OnRaidMember
        }

        public enum WarriorTalents
        {
            Juggernaut = 1,
            DoubleTime,
            Warbringer,
            EnragedRegeneration,
            SecondWind,
            ImpendingVictory,
            StaggeringShout,
            PiercingHowl,
            DisruptingShout,
            Bladestorm,
            Shockwave,
            DragonRoar,
            MassSpellReflection,
            Safeguard,
            Vigilance,
            Avatar,
            Bloodbath,
            StormBolt
        }

        public enum RotationVersion
        {
            Development,
            PvP,
            Release
        }
        #endregion
    }
}
