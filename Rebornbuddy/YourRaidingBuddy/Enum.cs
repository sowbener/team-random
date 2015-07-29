using System;
using System.Windows.Input;

namespace YourRaidingBuddy
{
        [Flags]
        public enum GameContext
        {
            None = 0,
            Normal = 0x1,
            Instances = 0x2,
            PvP = 0x4,

            All = Normal | Instances | PvP,
        }

        [Flags]
        public enum ForceGameContext
        {
            None = 0,
            Normal = 0x1,
            Instances = 0x2,
        }


    [Flags]
    public enum ModifierKeyss
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8,
        NoRepeat = 16384
    }
    [Flags]
        public enum BehaviorType
        {
            Rest = 0x1,
            PreCombatBuffs = 0x2,
            Pull = 0x4,
            Heal = 0x8,
            CombatBuffs = 0x10,
            Combat = 0x20,

            Initialize = 0x40, 

            // this is no guarantee that the bot is in combat
            InCombat = Heal | CombatBuffs | Combat,
            // this is no guarantee that the bot is out of combat
            OutOfCombat = Rest | PreCombatBuffs,

            All = Rest | PreCombatBuffs | Pull | Heal | CombatBuffs | Combat,
        }

    internal enum HotkeyMode
    {
        Automatic,
        HotkeyMode,
        SemiHotkeyMode
    }
    internal enum SemiHotkeyMode
    {

        AoE,
        Cooldown

    }

    [Flags]
    internal enum KeyStates
    {
        None = 0,
        Down = 1,
    }

    [Flags]
    public enum Combo
    {
        Flushed = 0,
      //Rogue
        SpinningEdge = 1 << 1,
        GustSlash = 1 << 2,
        DancingEdge = 1 << 3,
        AeolianEdge = 1 << 4,
        ShadowFang = 1 << 5,
      //Lancer
        TrueThrust = 1 << 6,
        VorpalThrust = 1 << 7,
        HeavyThrust = 1 << 8,
        ImpulseDrive = 1 << 9,
        Disembowel = 1 << 10,
        FullThrust = 1 << 11,
        ChaosThrust = 1 << 12,
        RingofThorns = 1 << 13,
      //Marauder
        HeavySwing = 1 << 14,
        SkullSunder = 1 << 15,
        Maim = 1 << 16,
        ButchersBlock = 1 << 17,
        StormsPath = 1 << 18,
        StormsEye = 1 << 19,
      //Gladiator
        FastBlade = 1 << 20,
        SavageBlade = 1 << 21,
        RiotBlade = 1 << 22,
        RageofHalone = 1 << 23,
      //Thaumaturge
        Blizzard3 = 1 << 24,

        Finished = DancingEdge | AeolianEdge | ShadowFang | FullThrust | ChaosThrust | RingofThorns
            | ButchersBlock | StormsPath | StormsEye | RiotBlade | RageofHalone | Blizzard3,
    }

    [Flags]
    public enum ModifierKey
    {
        None = ModifierKeys.None,
        Shift = ModifierKeys.Shift,
        Alt = ModifierKeys.Alt,
        Control = ModifierKeys.Control,

        ShiftAlt = Shift | Alt,
        ShiftControl = Shift | Control,
        AltControl = Alt | Control,
        All = Alt | Control | Shift,


    }
    public enum CooldownUse
        {
            Never = 0,
            Always = 0x1,
   //         ByFocus= 0x2,
   //         BySpecial = 0x4
            
        }

    public enum RogueOpeners
    {
        DancingEdgeShadowFangMutilate,
        DancingEdgeMutilateShadowFang,
        MutilateDancingEdgeShadowFang,
        MutilateShadowFangDancingEdge,
        ShadowFangDancingEdgeMutilate,
        ShadowFangMutilateDancingEdge
    }
    public enum PallyOaths
    {
        Auto,
        Rotation,
        Shield,
        Sword,
    }

    [Flags]
    public enum AoeType
    {
        None = 0,
        StripRegularFrontal = 1 << 0,
        StripLongFrontal = 1 << 1,
        StripWideFrontal = 1 << 2,
        ConeVeryNarrowFrontal = 1 << 3,
        ConeNarrowFrontal = 1 << 4,
        ConeNarrowBehind = 1 << 5,
        ConeWideFrontal = 1 << 6,
        CircleTargetLarge = 1 << 7,
        CircleTargetLargePersistent = 1 << 8,
        CircleTargetSmall = 1 << 9,
        CircleTargetSmallPersistent = 1 << 10,
        CircleMobLarge = 1 << 11,
        CircleMobLargePersistent = 1 << 12,
        CircleMobSmall = 1 << 13,
        CircleMobSmallPersistent = 1 << 14,

        Persistent = CircleTargetLargePersistent | CircleTargetSmallPersistent | CircleMobLargePersistent | CircleMobSmallPersistent,
        FlankingSafe = StripRegularFrontal | StripLongFrontal | ConeVeryNarrowFrontal | ConeNarrowFrontal, // | StripWideFrontal?
        Circle = CircleTargetLarge | CircleTargetLargePersistent | CircleTargetSmall | CircleTargetSmallPersistent
        | CircleMobLarge | CircleMobLargePersistent | CircleMobSmall | CircleMobSmallPersistent,
    }
}
