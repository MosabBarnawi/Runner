using System;

namespace BarnoGames.Runner2020
{
    [Serializable]
    public struct Shard_ThrowAttributes
    {
        public float Standard_ShardSpeed;
        public float Standard_RotationSpeed;
        public float SpeedToTarget;

        public Shard_ThrowAttributes(float standard_ShardSpeed, float standard_RotationSpeed, float travelToEnmeyTime)
        {
            Standard_ShardSpeed = standard_ShardSpeed;
            Standard_RotationSpeed = standard_RotationSpeed;
            SpeedToTarget = travelToEnmeyTime;
        }
    }
}
