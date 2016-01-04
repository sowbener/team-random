using ff14bot.Behavior;
using YourRaidingBuddy.Helpers;
using TreeSharp;
using System;
using YourRaidingBuddy.Managers;
using YourRaidingBuddy.Interfaces.Settings;
using ff14bot;

namespace YourRaidingBuddy
{
    /// <summary>
    ///     http://www.thebuddyforum.com/honorbuddy-forum/community-developer-forum/101361-reference-treehooks.html3
    ///     http://www.thebuddyforum.com/honorbuddy-forum/community-developer-forum/114540-answers-treesharp-questions.html
    /// </summary>
    public abstract partial class Root
    {
        private Composite _combatBehavior;
        private Composite _combatBuffsBehavior;
        private Composite _healBehavior;
        private Composite _preCombatBuffsBehavior;
        private Composite _pullBehavior;
        private Composite _restBehavior;

        /// <summary>
        ///     CombatBehaviour Implementation.
        /// </summary>
        public override Composite CombatBehavior
        {
            get { return _combatBehavior ?? (_combatBehavior = CreateCombat()); }
        }

        public override Composite CombatBuffBehavior
        {
            get { return _combatBuffsBehavior ?? (_combatBuffsBehavior = CreateCombatBuff()); }
        }

        public override Composite HealBehavior
        {
            get { return _healBehavior ?? (_healBehavior = CreateHealBehavior()); }
        }

        public override Composite PreCombatBuffBehavior
        {
            get { return _preCombatBuffsBehavior ?? (_preCombatBuffsBehavior = CreatePreCombat()); }
        }

        public override Composite PullBehavior
        {
            get { return _pullBehavior ?? (_pullBehavior = CreatePullBehavior()); }
        }

        public override Composite RestBehavior
        {
            get { return _restBehavior ?? (_restBehavior = CreateRestBehavior()); }
        }
        /// <summary>
        ///     Combat Rotations will be plugged into this hook (Replace).
        /// </summary>
        protected virtual Composite CreateCombat()
        {
            return new HookExecutor("YRB_Combat_Selector", "CombatBehaviorHook",
                new ActionAlwaysFail());
        }

        /// <summary>
        ///     Combat Rotations Buff will be plugged into this hook (Replace).
        /// </summary>
        /// 
        protected virtual Composite CreateCombatBuff()
        {
            return new HookExecutor("YRB_CombatBuff_Selector", "CombatBuffBehaviorHook",
                new ActionAlwaysFail());
        }

        /// <summary>
        ///     PreCombat Rotations will be plugged into this hook (Replace).
        /// </summary>
        protected virtual Composite CreatePreCombat()
        {
            return new HookExecutor("YRB_Pre_Combat_Selector", "PreCombatBehaviorHook",
                new ActionAlwaysFail());
        }

        /// <summary>
        ///     Pull Rotations will be plugged into this hook (Replace).
        /// </summary>
        protected virtual Composite CreatePullBehavior()
        {
            return new HookExecutor("YRB_Pull_Selector", "PullBehaviorHook",
                new ActionAlwaysFail());
        }

        /// <summary>
        ///     Rest Rotations will be plugged into this hook (Replace).
        /// </summary>
        protected virtual Composite CreateRestBehavior()
        {
            return new HookExecutor("YRB_Rest_Selector", "RestBehaviorHook",
                new ActionAlwaysFail());
        }

        /// <summary>
        ///    Heal Behavior will be plugged into this hook (Replace).
        /// </summary>
        protected virtual Composite CreateHealBehavior()
        {
            return new HookExecutor("YRB_Heal_Selector", "HealBehaviorHook",
                new ActionAlwaysFail());
        }


        /// <summary>
        ///     Replaces existing hooks with the corresponding hooks for your Hotkeymode, Rotation and Specialization.
        ///     Also initializing required functions for the routine.
        /// </summary>
        /// <param name="reinitialized">Used to see if its triggered by RebuildBehaviors or HookBehaviors</param>
        /// 
                #region HookStart
        internal static void HookBehaviors(bool reinitialized = false)
        {
            try
            {
                /* Clearing all existing hooks */
                // TreeHooks.Instance.ClearAll();

                /* Starting various functions */
                if (reinitialized)
                {
                    Interface.Overlay.Overlay.RebuildOverlay(true);
                    HotkeyManager.Initialize(true);
                    //       Logger.PrintInformation();;
                }
                else
                {
                    HotkeyManager.Initialize();
                    //     Logger.PrintInformation();
                }
                #endregion

                #region Ninja Hooks
                // Ninja Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Ninja)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Ninja", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Ninja (RebuildBehaviors())."
                        : "Hooks initialized for Ninja (HookBehaviors()).");
                }

                #endregion

                #region Bard Hooks
                // Ninja Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Bard)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Bard.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Bard.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Bard", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Bard (RebuildBehaviors())."
                        : "Hooks initialized for Bard (HookBehaviors()).");
                }

                #endregion

                #region Dragoon Hooks
                // Dragoon Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Dragoon)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Dragoon.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Dragoon.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Dragoon", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Dragoon (RebuildBehaviors())."
                        : "Hooks initialized for Dragoon (HookBehaviors()).");
                }

                #endregion

                #region Paladin Hook
                // Paladin Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Paladin)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Paladin.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Paladin.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Paladin", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Paladin (RebuildBehaviors())."
                        : "Hooks initialized for Paladin (HookBehaviors()).");
                }

                #endregion

                #region DarkKnight Hook
                // Deathknight Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.DarkKnight)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.DarkKnight.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.DarkKnight.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize DarkKnight", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for DarkKnight (RebuildBehaviors())."
                        : "Hooks initialized for DarkKnight (HookBehaviors()).");
                }

                #endregion

                #region Warrior Hook
                // Warrior Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Warrior)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Warrior.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Warrior.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize DarkKnight", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for DarkKnight (RebuildBehaviors())."
                        : "Hooks initialized for DarkKnight (HookBehaviors()).");
                }

                #endregion

                #region Monk Hook
                // Monk Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Monk)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Monk.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Monk.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Monk", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Monk (RebuildBehaviors())."
                        : "Hooks initialized for Monk (HookBehaviors()).");
                }

                #endregion

                #region Summoner Hook
                // Summoner Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Summoner)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Summoner.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Summoner.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize DarkKnight", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for DarkKnight (RebuildBehaviors())."
                        : "Hooks initialized for DarkKnight (HookBehaviors()).");
                }

                #endregion

                #region Pugilist Hooks
                // Ninja Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Pugilist)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Pugilist.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                //    TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.HkMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Pugilist", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Pugilist (RebuildBehaviors())."
                        : "Hooks initialized for Pugilist (HookBehaviors()).");
                }

                #endregion

                #region Machinist Hooks
                // Ninja Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Machinist)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Machinist.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Machinist.HotkeyMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Machinist", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Machinist (RebuildBehaviors())."
                        : "Hooks initialized for Machinist (HookBehaviors()).");
                }

                #endregion

                #region Conjurer Hooks
                // Ninja Job
                if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Conjurer)
                {
                    //      if (InternalSettings.Instance.General.ArmsRotVersion == Enums.ArmsRotationVersion.Normal)
                    {
                        switch (InternalSettings.Instance.Hotkeys.HotkeyModeSelection)
                        {
                            case HotkeyMode.Automatic:
                                TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Conjurer.AutoMode()));
                                TreeHooks.Instance.ReplaceHook("YRB_Heal_Selector", new ActionRunCoroutine(ctx => Rotations.Conjurer.AutoMode()));
                                //  TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildPreCombatBuffs()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_CombatBuff_Selector", new ActionRunCoroutine(ctx => Rotations.Ninja.NinjaBuildCombatBuffs()));
                                break;

                            case HotkeyMode.SemiHotkeyMode:
                                //   TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.SemiHkMode()));
                                //   TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            case HotkeyMode.HotkeyMode:
                                //    TreeHooks.Instance.ReplaceHook("YRB_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.HkMode()));
                                //    TreeHooks.Instance.ReplaceHook("YRB_Pre_Combat_Selector", new ActionRunCoroutine(ctx => Enhancement.PreCombat()));
                                break;

                            default:
                                StopBehaviors("Unable to intialize Pugilist", "HookBehaviors");
                                break;
                        }
                    }

                    Logger.WriteDebug(reinitialized
                        ? "Hooks reinitialized for Pugilist (RebuildBehaviors())."
                        : "Hooks initialized for Pugilist (HookBehaviors()).");
                }

                #endregion

                #region End Hooks
                Logger.WriteDebug(reinitialized
                    ? "Done with reinitializing the Routine."
                    : "Done with initializing the Routine.", false);
            }
            catch (Exception ex)
            {
                StopBehaviors(ex.ToString(), "Exception in HookBehaviors()");
            }
        }
        #endregion


        /// <summary>
        ///     Void to be used when character changes - Eg: Talents, Specialization.
        /// </summary>
        /// <param name="triggeredby">string - name of process which triggered it</param>
        internal static void RebuildBehaviors(string triggeredby)
        {
            Logger.Write("RebuildBehaviors is triggered by {0}.", triggeredby);

            //    MyToonGuid = Me.Guid;

            HookBehaviors();
        }

        /// <summary>
        ///     Global void used for all stopbot functions - Like shutdown or exceptions.
        /// </summary>
        /// <param name="reason">string - reason</param>
        /// <param name="triggeredby">string - name of process which triggered it</param>
        internal static void StopBehaviors(string reason, string triggeredby)
        {
            /* Cleaning up for a clean shutdown */
            if (Overlay.Overlay.isShown())
            {
                InternalSettings.Instance.General.X = Overlay.Overlay.getLocation().X;
                InternalSettings.Instance.General.Y = Overlay.Overlay.getLocation().Y;
                Overlay.Overlay.closeOverlay();
                InternalSettings.Instance.General.Save();
            }

            /* Cleaning up for a clean shutdown */
            HotkeyManager.Stop();

            /* Cleaning up Overlay */
            Interface.Overlay.Overlay.Terminate();

            /* Stopping the TreeRoot */
            StopBot(reason, triggeredby);
        }

    }
}