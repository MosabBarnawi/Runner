using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    [CreateAssetMenu(fileName = "New Level Asset Settings", menuName = "Barno Utils/Level/Level Asset")]
    public class LevelAsset : ScriptableObject
    {
        [SerializeField] private int levelIndex;
        [SerializeField] private Transform spawnPoint;
        public Transform SpawnPosition => spawnPoint;
        public int GetLevelIndex => levelIndex;

        //private void OnEnable()
        //{
        //    //TODO ADD SELF TO LEVEL MANAGER
        //}
    }
}