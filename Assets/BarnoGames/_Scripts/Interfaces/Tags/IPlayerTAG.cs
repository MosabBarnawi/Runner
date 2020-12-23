using System;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    internal interface IPlayerTAG
    {
        void PlayerInWinState();
        void EnableReciveInputControls(bool enabled);
        void RespawnPlayer(Vector3 positionToSpawn);
        void PlayerIsFalling(Vector3 positionToSpawn, Action levelHasStartedCallback);
    }
}