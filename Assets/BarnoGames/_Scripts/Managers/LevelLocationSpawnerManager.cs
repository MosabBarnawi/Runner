using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BarnoGames.Runner2020
{
    public class LevelLocationSpawnerManager : MonoBehaviour // TODO:: CHANGE TO SCRIPTABLE OBJECT AND LET LEVEL MANAGER HANDLE THE SPAWN POSITIONS INSTEAD
    {
        [SerializeField] private Transform spawnPoint;

        public Transform SpawnPosition => spawnPoint;
    }
}