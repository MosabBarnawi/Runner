using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public enum GameState
    {
        None,

        LoadingSavedData,
        Init,

        LevelTransition,
        LevelLoading,
        LevelFinishedLoading,
        LevelReady,

        PlayerSpawning,
        PlayerRestarting,
        PlayerDeath,

        InGame,
        Paused,
        ShowingAD,
        WinState,
    }

    public abstract class GameStateManager : MonoBehaviour
    {
        //private GameState CurrentGameState;

        private Action OnLoadSaveData;
        private Action OnInitilization;
        private Action OnLevelTransition;
        private Action OnLevelLoading;
        private Action OnLevelFinishedLoading;
        private Action OnLevelReady;
        private Action OnPlayerSpawning;
        private Action OnPlayerRestarting;
        private Action OnPlayerDeath;
        private Action OnInGame;
        private Action OnPaused;
        private Action OnShowingAD;
        private Action OnWinStateActions;

        public void RegisterGameState(Action action, GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.LoadingSavedData:
                    OnLoadSaveData += action;
                    break;
                case GameState.Init:
                    OnInitilization += action;
                    break;
                case GameState.LevelTransition:
                    OnLevelTransition += action;
                    break;
                case GameState.LevelLoading:
                    OnLevelLoading += action;
                    break;
                case GameState.LevelFinishedLoading:
                    OnLevelFinishedLoading += action;
                    break;
                case GameState.LevelReady:
                    OnLevelReady += action;
                    break;
                case GameState.PlayerSpawning:
                    OnPlayerSpawning += action;
                    break;
                case GameState.PlayerRestarting:
                    OnPlayerRestarting += action;
                    break;
                case GameState.PlayerDeath:
                    OnPlayerDeath += action;
                    break;
                case GameState.InGame:
                    OnInGame += action;
                    break;
                case GameState.Paused:
                    OnPaused += action;
                    break;
                case GameState.ShowingAD:
                    OnShowingAD += action;
                    break;
                case GameState.WinState:
                    OnWinStateActions += action;
                    break;
                default:
                    break;
            }
        }

        /// <param name="action">Removes Completly.</param>
        public void UnRegisterGameState(Action action)
        {
            OnLoadSaveData -= action;
            OnInitilization -= action;
            OnLevelTransition -= action;
            OnLevelLoading -= action;
            OnLevelFinishedLoading -= action;
            OnLevelReady -= action;
            OnPlayerSpawning -= action;
            OnPlayerRestarting -= action;
            OnPlayerDeath -= action;
            OnInGame -= action;
            OnPaused -= action;
            OnShowingAD -= action;
            OnWinStateActions -= action;
        }

        /// <param name="action">Removes From Selected Game State.</param>
        public void UnRegisterGameState(Action action, GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.LoadingSavedData:
                    OnLoadSaveData -= action;
                    break;
                case GameState.Init:
                    OnInitilization -= action;
                    break;
                case GameState.LevelTransition:
                    OnLevelTransition -= action;
                    break;
                case GameState.LevelLoading:
                    OnLevelLoading -= action;
                    break;
                case GameState.LevelFinishedLoading:
                    OnLevelFinishedLoading -= action;
                    break;
                case GameState.LevelReady:
                    OnLevelReady -= action;
                    break;
                case GameState.PlayerSpawning:
                    OnPlayerSpawning -= action;
                    break;
                case GameState.PlayerRestarting:
                    OnPlayerRestarting -= action;
                    break;
                case GameState.PlayerDeath:
                    OnPlayerDeath -= action;
                    break; 
                case GameState.InGame:
                    OnInGame -= action;
                    break;
                case GameState.Paused:
                    OnPaused -= action;
                    break;
                case GameState.ShowingAD:
                    OnShowingAD -= action;
                    break;
                case GameState.WinState:
                    OnWinStateActions -= action;
                    break;
                default:
                    break;
            }
        }

        protected void GameSateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.LoadingSavedData:
                    OnLoadSaveData.Invoke();
                    break;
                case GameState.Init:
                    OnInitilization.Invoke();
                    break;
                case GameState.LevelTransition:
                    OnLevelTransition.Invoke();
                    break;
                case GameState.LevelLoading:
                    OnLevelLoading.Invoke();
                    break;
                case GameState.LevelFinishedLoading:
                    OnLevelFinishedLoading.Invoke();
                    break;
                case GameState.LevelReady:
                    OnLevelReady.Invoke();
                    break;
                case GameState.PlayerSpawning:
                    OnPlayerSpawning.Invoke();
                    break;
                case GameState.PlayerRestarting:
                    OnPlayerRestarting.Invoke();
                    break;
                case GameState.PlayerDeath:
                    OnPlayerDeath.Invoke();
                    break;
                case GameState.InGame:
                    OnInGame.Invoke();
                    break;
                case GameState.Paused:
                    OnPaused.Invoke();
                    break;
                case GameState.ShowingAD:
                    OnShowingAD.Invoke();
                    break;
                case GameState.WinState:
                    OnWinStateActions.Invoke();
                    break;
                default:
                    break;
            }
        }

        public virtual void OnLoadedSaveData()
        {
            Debug.Log("Heyyyyyyy");
        }

    }
}
