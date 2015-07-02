using System.Linq;
using System.Threading.Tasks;
using YourRaidingBuddy.Settings;
using System;
using System.Windows.Forms;
using System.Windows.Media;
using ff14bot.Enums;
using ff14bot.NeoProfiles;
using YourRaidingBuddy.Helpers;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.AClasses;
using Extensions = YourRaidingBuddy.Helpers.Extensions;
using ff14bot;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Interface.GUI;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy
{
    public abstract partial class Root : CombatRoutine
    {
        public override float PullRange
        {
            get { return 2.5f; }
        }
        public override string Name { get { return "YourRaidingBuddy" + Revision; } }
        new internal static readonly Version Revision = new Version(0, 0, 1);
        private Form _configForm;
        public override bool WantButton { get { return true; } }
        public static ClassJobType OldJob { get; set; }

        public override void OnButtonPress()
        {
            if (InternalSettings.Instance.General.X != 0 && InternalSettings.Instance.General.Y != 0)
                Overlay.Overlay.overlayLocation = new System.Drawing.Point(InternalSettings.Instance.General.X, InternalSettings.Instance.General.Y);

            if (_configForm == null || _configForm.IsDisposed || _configForm.Disposing)
                _configForm = new YrbGui();

            _configForm.ShowDialog();
        }
        private Buddy.Coroutines.Coroutine _avoidance;
        public virtual void OnPulse()
        {
            Unit.UpdatePriorities();
            Spell.SyncCombos();
            if(Core.Player.CurrentJob == ClassJobType.WhiteMage) HealTargeting.Instance.Pulse();
            if (Overlay.Overlay.isShown())
            {
                Overlay.Overlay.updateLabels();
            }
            Extensions.DoubleCastPreventionDict.RemoveAll(t => DateTime.UtcNow.TimeOfDay > t);
            //    Target.TargetChange();
            //    Movement.CheckStuckRunning();
            Spell.GcdPulse();
            if (InternalSettings.Instance.General.ShowSpellIds && (Core.Player.CurrentTarget as BattleCharacter) != null && (Core.Player.CurrentTarget as BattleCharacter).IsCasting && (Core.Player.CurrentTarget as BattleCharacter).SpellCastInfo != null)
                Logging.Write(Colors.OrangeRed, "YourRaidingBuddy "+(Core.Player.CurrentTarget as BattleCharacter).SpellCastInfo.Name +" Spell ID " + (Core.Player.CurrentTarget as BattleCharacter).SpellCastInfo.ActionId);
            if (!OldJob.Equals(Core.Player.CurrentJob))
            {
                OldJob = Core.Player.CurrentJob;
                Logging.Write(Colors.OrangeRed, "YourRaidingBuddy -->  Your job has been updated to " + OldJob + ". Rebuilding behaviors...");
                TreeHooks.Instance.ClearAll();
                OnInitialize();
            }
        }

        public static bool WantHealing = true;

        public override sealed void Pulse()
        {
            
            OnPulse();
        }
        public static bool Resting = false;        

        #region Hidden Overrides

        internal static Root Instance;

        public override void Initialize()
        {   
            OldJob = Core.Player.CurrentJob;
            VariableBook.Initialize();
            Overlay.Overlay.onLocationChanged += Overlay_onLocationChanged;
            /* Printing some Information */
            Logger.Write("------------------------------------------");
            Logger.Write(Name + " by Xcesius" + ".");
            Logger.Write("Internal Revision: " + Revision + ".");
            Logger.Write("Your Job is " + OldJob.ToString() + ".");
            Logger.Write("-------------------------------------------\r\n");

            Logging.WriteToFileSync(LogLevel.Normal, "(YourRaidingBuddy) Spell Queue: " + InternalSettings.Instance.General.SpellQueue);

            HookBehaviors();
        }

        public abstract void OnInitialize();

        public void Overlay_onLocationChanged(object sender, EventArgs e)
        {
            if (Overlay.Overlay.isShown())
            {
                InternalSettings.Instance.General.X = Overlay.Overlay.getLocation().X;
                InternalSettings.Instance.General.Y = Overlay.Overlay.getLocation().Y;
                InternalSettings.Instance.General.Save();
            }
        }

        private void OnInstanceOnHooksCleared(object s, EventArgs e)
        {
            RebuildBehaviors("Routines were reloaded, Instance On Hooks Cleared");
        }

        private void OnGameEventsOnMapChanged(object s, EventArgs e)
        {
            UpdateContext();
        }

        internal static void StopBot(string reason, string triggeredby)
        {
            TreeRoot.Stop(reason + " - Triggered by " + triggeredby);
        }

        private void OnRoutineManagerOnReloaded(object s, EventArgs e)
        {
            RebuildBehaviors("Routines were reloaded, Routine Manager");
        }

        private void OnOnGameContextChanged(object orig, GameContextEventArg ne)
        {
            RebuildBehaviors("Routines were reloaded, On Game Context Changed");
        }

        public override void ShutDown()
        {
            StopBehaviors("Bot Stopping", "Bot Stop");
            TreeHooks.Instance.ClearAll();
        }
        #endregion

        public static bool IsValid()
        {
            return Core.Player.IsValid && InternalSettings.Instance.General.Combat && !Core.Player.IsMounted && Core.IsInGame && !Core.Player.IsDead
                && !QuestLogManager.InCutscene && !ff14bot.RemoteWindows.Talk.DialogOpen;
        }
        public static bool ShouldPulse;
        public static async Task<bool> DefaultRestBehavior(float currentEnergy)
        {
            SpellData sprint;
            var sprintCooldown = !Actionmanager.CurrentActions.TryGetValue("Sprint", out sprint) ? 0 : sprint.Cooldown.TotalMilliseconds;
            if (Core.Player.CurrentHealthPercent < InternalSettings.Instance.General.RestHealthDone ||
                currentEnergy < InternalSettings.Instance.General.RestEnergyDone && Math.Abs(sprintCooldown) < 50)
            {
                Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Resting");
                if (MovementManager.IsMoving) Movement.StopMove();
                Resting = true;
                return true;
            }
            Resting = false;
            return false;
        }

        public static async Task<bool> CombatCompanion()
        {
            if (Core.Player.IsCasting && (Core.Player.SpellCastInfo == null || Core.Player.SpellCastInfo.ActionType == ActionType.Item || Core.Player.SpellCastInfo.ActionType == ActionType.Companion)) return true;
            if (!InternalSettings.Instance.General.CombatChocobo || Chocobo.Summoned || !Chocobo.CanSummon
                || InternalSettings.Instance.General.CombatChocoboExtraChecks && PartyManager.IsInParty && PartyManager.VisibleMembers.Any(bird => bird.GameObject.SummonerObjectId == Core.Player.ObjectId 
                    && bird.GameObject.NpcId == 0 && bird.GameObject.Type == GameObjectType.BattleNpc)) return false;
            if (!MovementManager.IsMoving) return SummonChocobo();
            Movement.StopMove();
            SummonChocobo();
            if (await Buddy.Coroutines.Coroutine.Wait(1500, () => Core.Player.IsCasting))
            {
                return await Buddy.Coroutines.Coroutine.Wait(1300, () => Chocobo.Summoned);
            }
            return false;
        }


        private static bool SummonChocobo()
        {
            Chocobo.Summon();
            return false;
        }
    }
}