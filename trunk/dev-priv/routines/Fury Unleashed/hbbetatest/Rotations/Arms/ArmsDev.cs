using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using AG = FuryUnleashed.Rotations.Arms.ArmsGlobal;
using Enum = FuryUnleashed.Core.Helpers.Enum;
using G = FuryUnleashed.Rotations.Global;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Rotations.Arms
{
    class ArmsDev
    {
        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        internal static Composite DevArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                new Decorator(ret => AG.StanceDanceUsage, Dev_ArmsStanceDance()),
                                Dev_ArmsNonGcdUtility(),
                                Dev_ArmsRacials(),
                                Dev_ArmsOffensive(),
                                Item.CreateItemBehaviour(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => AG.MultiTargetUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                new Decorator(ret => AG.StanceDanceUsage, Dev_ArmsStanceDance()),
                                Dev_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => AG.MultiTargetUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                new Decorator(ret => AG.StanceDanceUsage, Dev_ArmsStanceDance()),
                                Dev_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => AG.MultiTargetUsage && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_ArmsSt())
                                        ))))));
            }
        }

        // http://www.icy-veins.com/arms-warrior-wow-pve-dps-rotation-cooldowns-abilities
        internal static Composite Dev_ArmsSt()
        {
            return new PrioritySelector(
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        new Decorator(ret => !G.Tier16TwoPieceBonus,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower >= Lua.PlayerPowerMax - 10, true),
                                Spell.Cast(SpellBook.ColossusSmash),
                                Spell.Cast(SpellBook.MortalStrike),

                                Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityUsage),
                                Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityUsage),
                                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && !G.ReadinessAura && AG.Tier6AbilityUsage),

                                Spell.Cast(SpellBook.Overpower),
                                Spell.Cast(SpellBook.Slam, ret => Lua.PlayerPower >= 80 && (G.TasteForBloodS1 || G.TasteForBloodS2)),
                                new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me))),
                                Spell.Cast(SpellBook.HeroicThrow, ret => AG.HeroicThrowUsage))),
                        new Decorator(ret => G.Tier16TwoPieceBonus,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower >= Lua.PlayerPowerMax - 10, true),
                                Spell.Cast(SpellBook.ColossusSmash),
                                Spell.Cast(SpellBook.MortalStrike),

                                Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityUsage),
                                Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityUsage),
                                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && !G.ReadinessAura && AG.Tier6AbilityUsage),

                                Spell.Cast(SpellBook.Overpower, ret => G.TasteForBloodS4 || G.TasteForBloodS5),
                                Spell.Cast(SpellBook.Slam, ret => Lua.PlayerPower >= 80 && (G.TasteForBloodS1 || G.TasteForBloodS2 || G.TasteForBloodS3)),
                                new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me))),
                                Spell.Cast(SpellBook.HeroicThrow, ret => AG.HeroicThrowUsage)
                                )))),
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        new Decorator(ret => !G.Tier16TwoPieceBonus,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.MortalStrike),

                                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityUsage),

                                Spell.Cast(SpellBook.Slam, ret => Lua.PlayerPower >= 40 || G.RecklessnessAura),
                                Spell.Cast(SpellBook.Overpower))),
                        new Decorator(ret => G.Tier16TwoPieceBonus,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower >= Lua.PlayerPowerMax - 10 && !G.RecklessnessAura, true),
                                Spell.Cast(SpellBook.MortalStrike),

                                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityUsage),

                                Spell.Cast(SpellBook.Slam, ret => G.RecklessnessAura),
                                Spell.Cast(SpellBook.Overpower, ret => G.TasteForBloodS4 || G.TasteForBloodS5),
                                Spell.Cast(SpellBook.Slam, ret => G.TasteForBloodS1 || G.TasteForBloodS2 || G.TasteForBloodS3)
                                )))));
        }

        // This non CS window rotation is solely to preserve rage for the Colossus Smash Window.
        internal static Composite Dev_ArmsExec()
        {
            return new PrioritySelector(
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.MortalStrike),

                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityUsage),

                        Spell.Cast(SpellBook.Execute, ret => Lua.PlayerPower > 80),
                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.DragonRoar, ret => AG.Tier4AbilityUsage),
                        Spell.Cast(SpellBook.ImpendingVictory, ret => G.ImpendingVictoryTalent && AG.RotationalImpendingVictoryUsage),
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me)))
                        )),
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.MortalStrike),
                        Spell.Cast(SpellBook.Execute),
                        Spell.Cast(SpellBook.Overpower))));
        }

        internal static Composite Dev_ArmsMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.SweepingStrikes),
                        Spell.MultiDoT(SpellBook.MortalStrike, AuraBook.DeepWounds),

                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityAoEUsage),

                        Dev_ArmsSt())),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.SweepingStrikes),
                        new ThrottlePasses(1, TimeSpan.FromSeconds(15), RunStatus.Failure,
                            Spell.Cast(SpellBook.ThunderClap)),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.MultiDoT(SpellBook.MortalStrike, AuraBook.DeepWounds),

                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityAoEUsage),

                        new Decorator(ret => IS.Instance.Arms.CheckSmartAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam, ret => G.SlamViable),
                                Spell.Cast(SpellBook.Whirlwind, ret => G.WhirlwindViable))),

                        new Decorator(ret => !IS.Instance.Arms.CheckSmartAoE,
                            Spell.Cast(SpellBook.Slam)),

                        new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me))),
                        Spell.Cast(SpellBook.Overpower)
                        )));
        }

        internal static Composite Dev_ArmsStanceDance()
        {
            return new Decorator(ret => AG.StanceDanceUsage,
                G.StanceDanceLogic());
        }

        internal static Composite Dev_ArmsOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.BerserkerRage, ret => G.ColossusSmashAura && (!G.EnrageAura || G.FadingEnrage(500)) && AG.BerserkerRageUsage, true),
                Spell.Cast(SpellBook.Bloodbath, ret => G.BloodbathTalent && AG.Tier6AbilityUsage, true),

                Spell.Cast(SpellBook.Recklessness, ret => AG.RecklessnessUsage, true),
                Spell.Cast(SpellBook.Avatar, ret => G.AvatarTalent && G.RecklessnessAura && AG.Tier6AbilityUsage, true),
                Spell.Cast(SpellBook.SkullBanner, ret => !G.SkullBannerAura && G.RecklessnessAura && AG.SkullBannerUsage, true)
                );
        }

        internal static Composite Dev_ArmsGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_ArmsRacials()
        {
            return new Decorator(ret => AG.RacialUsage,
                Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell())));
        }

        internal static Composite Dev_ArmsDefensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_ArmsNonGcdUtility()
        {
            return new PrioritySelector(
                );
        }
    }
}
