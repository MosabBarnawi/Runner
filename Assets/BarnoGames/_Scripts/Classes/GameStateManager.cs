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

        LoadingSavedData,
        Init,
        Booted,

        LevelTransition,
        LevelRestart,
        LevelLoading,
        LevelFinishedLoading,
        LevelReady,

        PlayerSpawning,
        PlayerRestarting,
        PlayerReady,
        PlayerDeath,

        InGame,
        Paused,
        ShowingAD,
        WinState,
    }

    public abstract class GameStateManager : MonoBehaviour
    {
        public GameState CurrentGameState { get; private set; }

        private Action LoadSaveDataActions;  // TODO:: REMOVE THIS
        private Action InitilizationActions; // TODO:: REMOVE THIS
        private Action BootedToGameAction;
        private Action LevelTransitionActions;
        private Action LevelRestartAction;
        private Action LevelLoadingActions;
        private Action LevelFinishedLoadingActions;
        private Action LevelReadyActions;
        private Action PlayerSpawningActions;
        private Action PlayerReadyActions;
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
                case GameState.LoadingSavedData:
                    LoadSaveDataActions += action;
                    break;
                case GameState.Init:
                    InitilizationActions += action;
                    break;
                case GameState.Booted:
                    BootedToGameAction += action;
                    break;
                case GameState.LevelTransition:
                    LevelTransitionActions += action;
                    break;
                case GameState.LevelRestart:
                    LevelRestartAction += action;
                    break;
                case GameState.LevelLoading:
                    LevelLoadingActions += action;
                    break;
                case GameState.LevelFinishedLoading:
                    LevelFinishedLoadingActions += action;
                    break;
                case GameState.LevelReady:
                    LevelReadyActions += action;
                    break;
                case GameState.PlayerSpawning:
                    PlayerSpawningActions += action;
                    break;
                case GameState.PlayerReady:
                    PlayerReadyActions += action;
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
            LoadSaveDataActions -= action;
            InitilizationActions -= action;
            BootedToGameAction -= action;
            LevelTransitionActions -= action;
            LevelRestartAction -= action;
            LevelLoadingActions -= action;
            LevelFinishedLoadingActions -= action;
            LevelReadyActions -= action;
            PlayerSpawningActions -= action;
            PlayerReadyActions -= action;
            PlayerRestartingActions -= action;
            PlayerDeathActions -= action;
            InGameActions -= action;
            PausedActions -= action;
            ShowingADActions -= action;
            WinStateActions -= action;
        }

        /// <param name="action">Removes From Selected Game State.</param>
        public void UnRegisterGameState(Action action, GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.LoadingSavedData:
                    LoadSaveDataActions -= action;
                    break;
                case GameState.Init:
                    InitilizationActions -= action;
                    break;
                case GameState.Booted:
                    BootedToGameAction -= action;
                    break;
                case GameState.LevelTransition:
                    LevelTransitionActions -= action;
                    break;
                case GameState.LevelRestart:
                    LevelRestartAction -= action;
                    break;
                case GameState.LevelLoading:
                    LevelLoadingActions -= action;
                    break;
                case GameState.LevelFinishedLoading:
                    LevelFinishedLoadingActions -= action;
                    break;
                case GameState.LevelReady:
                    LevelReadyActions -= action;
                    break;
                case GameState.PlayerSpawning:
                    PlayerSpawningActions -= action;
                    break;
                case GameState.PlayerReady:
                    PlayerReadyActions -= action;
                    break;
                case GameState.PlayerRestarting:
                    PlayerRestartingActions -= action;
                    break;
                case GameState.PlayerDeath:
                    PlayerDeathActions -= action;
                    break;
                case GameState.InGame:
                    InGameActions -= action;
                    break;
                case GameState.Paused:
                    PausedActions -= action;
                    break;
                case GameState.ShowingAD:
                    ShowingADActions -= action;
                    break;
                case GameState.WinState:
                    WinStateActions -= action;
                    break;
                default:
                    break;
            }
        }

        private void SetCurrentGameState(GameState newGameState)
        {
            CurrentGameState = newGameState;
            BarnoDebug.Log("Current Game status", $"= {CurrentGameState}", BarnoColor.Green);
        }

        //private void GameSateChange(GameState gameState)
        //{
        //    switch (gameState)
        //    {
        //        case GameState.None:
        //            break;
        //        case GameState.LoadingSavedData:
        //            LoadSaveDataActions.Invoke();
        //            break;
        //        case GameState.Init:
        //            InitilizationActions.Invoke();
        //            break;
        //        case GameState.LevelTransition:
        //            LevelTransitionActions.Invoke();
        //            break;
        //        case GameState.LevelRestart:
        //            LevelRestartAction.Invoke();
        //            break;
        //        case GameState.LevelLoading:
        //            LevelLoadingActions.Invoke();
        //            break;
        //        case GameState.LevelFinishedLoading:
        //            LevelFinishedLoadingActions.Invoke();
        //            break;
        //        case GameState.LevelReady:
        //            LevelReadyActions.Invoke();
        //            break;
        //        case GameState.PlayerSpawning:
        //            PlayerSpawningActions.Invoke();
        //            break;
        //        case GameState.PlayerReady:
        //            PlayerReadyActions.Invoke();
        //            break;
        //        case GameState.PlayerRestarting:
        //            PlayerRestartingActions.Invoke();
        //            break;
        //        case GameState.PlayerDeath:
        //            PlayerDeathActions.Invoke();
        //            break;
        //        case GameState.InGame:
        //            InGameActions.Invoke();
        //            break;
        //        case GameState.Paused:
        //            PausedActions.Invoke();
        //            break;
        //        case GameState.ShowingAD:
        //            ShowingADActions.Invoke();
        //            break;
        //        case GameState.WinState:
        //            WinStateActions.Invoke();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public virtual void OnLoadSaveData()
        {
            SetCurrentGameState(GameState.LoadingSavedData);
            LoadSaveDataActions?.Invoke();
        }
        public virtual void OnInitilization()
        {
            SetCurrentGameState(GameState.Init);
            InitilizationActions?.Invoke();
        }

        public virtual void OnBooted()
        { 
            SetCurrentGameState(GameState.Booted);
            BootedToGameAction?.Invoke();
        }

        public virtual void OnLevelTransition() { }
        public virtual void OnLevelRestart() { LevelRestartAction?.Invoke(); }
        public virtual void OnLevelLoading() 
        {
            SetCurrentGameState(GameState.LevelLoading);
            LevelLoadingActions?.Invoke(); 
        }
        public virtual void OnLevelFinishedLoading() { }
        public virtual void OnLevelReady() 
        {
            SetCurrentGameState(GameState.LevelReady);
            LevelReadyActions.Invoke(); 
        }
        public virtual void OnPlayerSpawning() { }
        public virtual void OnPlayerReady() { PlayerReadyActions?.Invoke(); }
        public virtual void OnPlayerRestarting() { PlayerRestartingActions?.Invoke(); }
        public virtual void OnPlayerDeath() { PlayerDeathActions?.Invoke(); }

        public virtual void OnInGame()
        {
            InGameActions?.Invoke();
            SetCurrentGameState(GameState.InGame);
        }
        public virtual void OnPaused()
        {
            InGameActions?.Invoke();
            SetCurrentGameState(GameState.Paused);
        }
        public virtual void OnShowingAD() { }
        public virtual void OnWinState() { WinStateActions?.Invoke(); }
    }
}
