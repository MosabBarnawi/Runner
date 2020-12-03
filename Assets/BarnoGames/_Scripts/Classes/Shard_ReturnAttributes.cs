using UnityEngine;

namespace BarnoGames.Runner2020
{
    [System.Serializable]
    public class Shard_ReturnAttributes
    {
        [Tooltip("Normal Retrun Speed")]
        public float ReturnSpeed = 1f;

        [Tooltip("Retrun Speed After UnStcuking from Wall")]
        public float ReturnBoostSpeed = 1f;
    }
}
