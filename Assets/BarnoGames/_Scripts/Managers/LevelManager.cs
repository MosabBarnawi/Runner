using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BarnoGames.Runner2020
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;

        public Transform SpawnPosition => spawnPoint;
    }
}