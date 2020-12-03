using System;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    [Serializable]
    public struct Shard_RepellAttributes
    {
        public float RepellTime;
        public float FallSpeedAfterRepellSpeed;
        public float MinTimeCatch; //Must Be Below Repell Time and Above Zero
        public float MaxTimeCatch;
        public float RepellSpeed;

        #region Unity Validation
        private void OnValidate()
        {
            if (MinTimeCatch < 0)
            {
                Debug.LogError("Invalid Input --  Cannot be below Zero");
                MinTimeCatch = 0.5f;
            }

            if (MaxTimeCatch > RepellTime)
            {
                Debug.LogError("Invalid Input -- Greater tha Repell Time");
                MaxTimeCatch = RepellTime;
            }
        }
        #endregion

        public Shard_RepellAttributes(float repellTime) : this(repellTime, 5f, 0.5f, 1.2f, 5f) { }

        public Shard_RepellAttributes(float repellTime, float fallSpeedAfterRepellSpeed, float minTimeCatch, float maxTimeCatch, float repellSpeed)
        {
            RepellTime = repellTime;
            FallSpeedAfterRepellSpeed = fallSpeedAfterRepellSpeed;
            MinTimeCatch = minTimeCatch;
            MaxTimeCatch = maxTimeCatch;
            RepellSpeed = repellSpeed;
        }
    }
}
