using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    [System.Serializable]
    public struct Shard_TargetDetection
    {
        public Color DetectedObjectColor;
        public float SphereRadious;
        public float MaxDistance;
        public LayerMask TargetableLayerMask;
    }
}
