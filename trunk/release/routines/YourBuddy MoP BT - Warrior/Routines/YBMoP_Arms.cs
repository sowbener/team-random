using System.Windows.Forms;
using CommonBehaviors.Actions;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using YBMoP_BT_Warrior.Helpers;
using YBMoP_BT_Warrior.Managers;
using G = YBMoP_BT_Warrior.Routines.YBGlobal;
using Lua = YBMoP_BT_Warrior.Helpers.YBLua;
using S = YBMoP_BT_Warrior.Core.YBSpell;
using U = YBMoP_BT_Warrior.Core.YBUnit;
using I = YBMoP_BT_Warrior.Core.YBItem;
using SA = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsA;
using SG = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsG;

namespace YBMoP_BT_Warrior.Routines
{
    class YBArms
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeArms
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.CheckTreePerformance, YBLogger.TreePerformance("InitializeArms")),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.CheckAdvancedLogging, YBLogger.AdvancedLogging),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => SG.Instance.ModeChoice == YBEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SA.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => SA.Instance.CheckInterrupts && U.CanInterrupt, ArmsInterrupts()),
                                        new Decorator(ret => SA.Instance.CheckAoE && U.AttackableMeleeUnitsCount >= 2, ArmsMt()),
                                        ArmsSt())),
                        new Decorator(ret => SG.Instance.ModeChoice == YBEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SA.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => SA.Instance.CheckInterrupts && U.CanInterrupt, ArmsInterrupts()),
                                        new Decorator(ret => SA.Instance.CheckAoE && U.AttackableMeleeUnitsCount >= 2, ArmsMt()),
                                        ArmsSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite ArmsSt()
        {
            return new PrioritySelector(
                // SimulationCraft 520-8 (r16293)
                // heroic_strike,if=(debuff.colossus_smash.up&rage>=rage.max-40&target.health.pct>=20)|rage>=rage.max-15
                S.Cast("Heroic Strike", ret => U.AttackableMeleeUnitsCount <= 2 &&  (Lua.PlayerPower >= Me.MaxRage - 15 || (G.ColossusSmashAura && Lua.PlayerPower >= Me.MaxRage - 40 && G.NonExecuteCheck))),
                // mortal_strike
                S.Cast("Mortal Strike"),
                // dragon_roar,if=talent.dragon_roar.enabled&talent.bloodbath.enabled&buff.bloodbath.up&debuff.colossus_smash.down&target.health.pct>=20
                S.Cast("Dragon Roar", ret => G.DRTalent && G.BloodbathAura && !G.ColossusSmashAura && G.NonExecuteCheck),
                // storm_bolt,if=talent.storm_bolt.enabled&debuff.colossus_smash.up
                S.Cast("Storm Bolt", ret => G.SBTalent && G.ColossusSmashAura),
                // colossus_smash,if=debuff.colossus_smash.remains<1
                S.Cast("Colossus Smash", ret => !G.ColossusSmashAura || G.FadingCS1S),
                // execute,if=debuff.colossus_smash.up|buff.recklessness.up|rage>=rage.max-25
                S.Cast("Execute", ret => G.ColossusSmashAura || G.RecklessnessAura || Lua.PlayerPower >= Me.MaxRage - 25),
                // dragon_roar,if=talent.dragon_roar.enabled&((talent.bloodbath.enabled&buff.bloodbath.up&target.health.pct>=20)|(debuff.colossus_smash.down&target.health.pct<20))
                S.Cast("Dragon Roar", ret => G.DRTalent && ((G.BBTalent && G.BloodbathAura && G.NonExecuteCheck) || (!G.ColossusSmashAura && G.ExecuteCheck))),
                // slam,if=debuff.colossus_smash.up&(debuff.colossus_smash.remains<1|buff.recklessness.up)&target.health.pct>=20
                S.Cast("Slam", ret => G.ColossusSmashAura && (G.FadingCS1S || G.RecklessnessAura) && G.NonExecuteCheck),
                // overpower,if=buff.taste_for_blood.stack>=3&target.health.pct>=20
                S.Cast("Overpower", ret => G.TasteForBloodS3 && G.NonExecuteCheck),
                // slam,if=debuff.colossus_smash.up&debuff.colossus_smash.remains<2.5&target.health.pct>=20
                S.Cast("Slam", ret => G.ColossusSmashAura && G.FadingCS2S && G.NonExecuteCheck),
                // execute,if=buff.sudden_execute.down
                S.Cast("Execute", ret => G.SuddenExecAura),
                // overpower,if=target.health.pct>=20|buff.sudden_execute.up
                S.Cast("Overpower", ret => G.NonExecuteCheck || G.SuddenExecAura),
                // slam,if=rage>=40&target.health.pct>=20
                S.Cast("Slam", ret => Lua.PlayerPower >= 40 && G.NonExecuteCheck),
                // battle_shout
                S.Cast("Battle Shout"),
                // heroic_throw
                S.Cast("Heroic Throw")
                );
        }

        internal static Composite ArmsMt()
        {
            return new PrioritySelector(
                // http://www.icy-veins.com/arms-warrior-wow-pve-dps-rotation-cooldowns-abilities For AoE.
                new Decorator(ret => U.AttackableMeleeUnitsCount == 2,
                    new PrioritySelector(
                        S.Cast("Thunder Clap", ret => !G.DeepWoundsAura),
                        S.Cast("Sweeping Strikes", ret => G.DeepWoundsAura),
                        ArmsSt()
                        )),
                new Decorator(ret => U.AttackableMeleeUnitsCount == 3,
                    new PrioritySelector(
                        S.Cast("Thunder Clap", ret => !G.DeepWoundsAura),
                        S.Cast("Sweeping Strikes", ret => G.DeepWoundsAura),
                        S.Cast("Bladestorm", ret => G.BSTalent),
                        S.Cast("Dragon Roar", ret => G.DRTalent),
                        S.Cast("Shockwave", ret => G.SWTalent),
                        S.Cast("Cleave", ret => U.AttackableMeleeUnitsCount >= 3 && (Lua.PlayerPower >= Me.MaxRage - 15 || (G.ColossusSmashAura && Lua.PlayerPower >= Me.MaxRage - 40 && G.NonExecuteCheck))),
                        ArmsSt()
                        )),
                new Decorator(ret => U.AttackableMeleeUnitsCount >= 4,
                    new PrioritySelector(
                        S.Cast("Thunder Clap", ret => !G.DeepWoundsAura),
                        S.Cast("Bladestorm", ret => G.BSTalent),
                        S.Cast("Dragon Roar", ret => G.DRTalent),
                        S.Cast("Shockwave", ret => G.SWTalent),
                        S.Cast("Whirlwind"),
                        S.Cast("Mortal Strike"),
                        S.Cast("Colossus Smash", ret => !G.ColossusSmashAura || G.FadingCS1S),
                        S.Cast("Overpower")
                        ))
                );
        }

        internal static Composite ArmsDefensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite ArmsOffensive()
        {
            return new PrioritySelector(
                S.Cast("Berserker Rage", ret => G.BTCDOC && G.ColossusSmashAura && !G.EnrageAura),
                S.Cast("Avatar", ret => G.AVTalent && G.RecklessnessAura && G.SkullBannerAura),
                S.Cast("Bloodbath", ret => G.BBTalent),
                S.Cast("Recklessness", ret => G.SkullBannerAura),
                S.Cast("Skull Banner", ret => !G.SkullBannerAura)
                );
        }

        internal static Composite ArmsUtility()
        {
            return new PrioritySelector(
                S.Cast("Rallying Cry", ret => !G.RallyingCryAura),
                S.Cast("Shattering Throw", ret => SA.Instance.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3 || G.SBCD <= 3)),
                S.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SG.Instance.DemoBannerChoice == Keys.None && U.IsDoNotUseOnTgt)
                );
        }

        internal static Composite ArmsInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    S.Cast("Disrupting Shout", ret => G.PUCDOC)
                    ),
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    S.Cast("Pummel")
                    ));
        }
        #endregion
        }
}
