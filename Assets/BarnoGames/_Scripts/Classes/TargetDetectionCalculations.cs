using UnityEngine;

namespace BarnoGames.Runner2020.Calculations
{
    [System.Serializable] // TODO:: REMOVE THIS
    public struct TargetDetectionCalculations
    {
        public float TravelToTargetTime { get; set; }
        public ITargetable Targetable;
        //public ITargetable[] targetablesBuffer;
        public float CurrentHitDistance;
        public Vector3 Origin;
        public Vector3 Direction;
        public Vector3 TargetPositon { get => Targetable.GetPosition(); }
        public Vector3 RadndomDistanceBetweenShardAndTarget { get; set; }
    }
}
