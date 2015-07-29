using System;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy
{
    #region Nested type: GameContextEventArg

    public class GameContextEventArg : EventArgs
    {
        public readonly GameContext CurrentContext;
        public readonly GameContext PreviousContext;

        public GameContextEventArg(GameContext currentContext, GameContext prevContext)
        {
            CurrentContext = currentContext;
            PreviousContext = prevContext;
        }
    }

    #endregion

    public partial class Root
    {
        public event EventHandler<GameContextEventArg> OnGameContextChanged;
        private GameContext _lastContext = GameContext.None;
        private uint _lastMapId;
        public GameContext CurrentContext { get; set; }
        internal void UpdateContextStateValues()
        {
            //if (CurrentContext == GameContext.Instances || CurrentContext == GameContext.PvP){WantHealing = Healers.Contains(Core.Player.CurrentJob);}
            
        }

        internal void UpdateContext()
        {
            DetermineCurrentContext();
            //Logging.Write("Old context:{0} CurrentContext:{1}", _lastContext, CurrentContext);
            // Can't update the context when it doesn't exist.
            if (CurrentContext == GameContext.None)
                return;


            if (CurrentContext != _lastContext)
            {
                Logging.Write("(YourRaidingBuddy) Old context:{0} CurrentContext:{1}", _lastContext, CurrentContext);
                // store values that require scanning lists
                UpdateContextStateValues();
                //DescribeContext();
                if (OnGameContextChanged != null)
                {
                    try
                    {
                        OnGameContextChanged(this, new GameContextEventArg(CurrentContext, _lastContext));
                    }
// ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                        // Eat any exceptions thrown.
                    }
                }


                _lastContext = CurrentContext;
                _lastMapId = WorldManager.ZoneId;
            }
            else if (_lastMapId != WorldManager.ZoneId)
            {
                //DescribeContext();
                _lastMapId = WorldManager.ZoneId;
            }
        }

        private void DetermineCurrentContext()
        {
            CurrentContext = _DetermineCurrentContext();
        }

        private GameContext _DetermineCurrentContext()
        {
            if (!Core.IsInGame)
                return GameContext.None;

            if ((GameContext)InternalSettings.Instance.General.ForcedContext != GameContext.None)
                return (GameContext)InternalSettings.Instance.General.ForcedContext;

            if (WorldManager.ZoneId == 337 || WorldManager.ZoneId == 336 || WorldManager.ZoneId == 175)
            {
                return GameContext.PvP;
            }

            if (PartyManager.IsInParty && DutyManager.InInstance)
            {
                return GameContext.Instances;
            }

            return GameContext.Normal;
        }
    }
}