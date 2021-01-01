using System.Collections.Generic;
using System.Threading.Tasks;
using BarnoGames.Utilities;
using BarnoUtils;
using System.Linq;
using System.Text;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    public enum GameState
    {
        None,

        Init,
        Booted,

        LevelRestart,
        LevelReady,

        PlayerRestarting,
        PlayerDeath,

        InGame,
        Paused,
        ShowingAD,
        WinState,
    }

    public abstract class GameStateManager : MonoBehaviour
    {
        public GameState CurrentGameState { get; private set; }

        private Action BootedToGameAction;
        private Action LevelRestartAction;
        private Action LevelReadyActions;
        private Action PlayerRestartingActions;
        private Action PlayerDeathActions;
        private Action InGameActions;
        private Action PausedActions;
        private Action ShowingADActions;
        private Action WinStateActions;

        /// <param name="action">Register Action to a Gamestate.</param>
        public void RegisterGameState(Action action, GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.Booted:
                    BootedToGameAction += action;
                    break;
                case GameState.LevelRestart:
                    LevelRestartAction += action;
                    break;
                case GameState.LevelReady:
                    LevelReadyActions += action;
                    break;
                case GameState.PlayerRestarting:
                    PlayerRestartingActions += action;
                    break;
                case GameState.PlayerDeath:
                    PlayerDeathActions += action;
                    break;
                case GameState.InGame:
                    InGameActions += action;
                    break;
                case GameState.Paused:
                    PausedActions += action;
                    break;
                case GameState.ShowingAD:
                    ShowingADActions += action;
                    break;
                case GameState.WinState:
                    WinStateActions += action;
                    break;
                default:
                    break;
            }
        }

        /// <param name="action">Removes Completly.</param>
        public void UnRegisterGameState(Action action)
        {
            BootedToGameAction -= action;
            LevelRestartAction -= action;
            LevelReadyActions -= action;
            PlayerRestartingActions -= action;
            PlayerDeathActions -= action;
            InGameActions -= action;
            PausedActions -= action;
            ShowingADActions -= action;
            WinStateActions -= action;
        }

        private void SetCurrentGameState(GameState newGameState)
        {
            CurrentGameState = newGameState;
            BarnoDebug.Log("Current Game status", $"= {CurrentGameState}", BarnoColor.Green);
        }

        public virtual void OnInitilization()
        {
            SetCurrentGameState(GameState.Init);
        }

        public virtual void OnBooted()
        { 
            SetCurrentGameState(GameState.Booted);
            BootedToGameAction?.Invoke();
        }

        public virtual void OnLevelRestart()
        {
            SetCurrentGameState(GameState.LevelRestart);
            LevelRestartAction?.Invoke(); 
        }

        public virtual void OnLevelReady() 
        {
            SetCurrentGameState(GameState.LevelReady);
            LevelReadyActions?.Invoke(); 
        }

        public virtual void OnPlayerRestarting() 
        {
            SetCurrentGameState(GameState.PlayerRestarting);
            PlayerRestartingActions?.Invoke(); 
        }
        public virtual void OnPlayerDeath() 
        {
            SetCurrentGameState(GameState.PlayerDeath);
            PlayerDeathActions?.Invoke(); 
        }

        public virtual void OnInGame()
        {
            SetCurrentGameState(GameState.InGame);
            InGameActions?.Invoke();
        }
        public virtual void OnPaused()
        {
            SetCurrentGameState(GameState.Paused);
            PausedActions?.Invoke();
        }
        public virtual void OnShowingAD() 
        {
            SetCurrentGameState(GameState.ShowingAD);
            ShowingADActions?.Invoke();
        }
        public virtual void OnWinState()
        {
            SetCurrentGameState(GameState.WinState);
            WinStateActions?.Invoke(); 
        }
    }
}
