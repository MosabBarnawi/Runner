using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    [CreateAssetMenu(fileName = "New Shard Behavior Settings", menuName = "Barno Utils/Wepons/Shard Behavior")]

    class ShardAttributesScriptable : ScriptableObject
    {
        [Header("Abilities")]
        [Tooltip("Boost after Help Wall Jump form Shard")]
        public float SharedBoostSpeed = 20f;

        [Space(10)]
        public float HitAmount = 5f;

        [Header("OTHER")]

        [Space(10)]
        public RotationsContainer[] Rotations = new RotationsContainer[2];

        [Space(10)]
        public Shard_TargetDetection TargetDetection;

        [Space(10)]
        [SerializeField]
        private Shard_ThrowAttributes throwAttributes = new Shard_ThrowAttributes(40f, 5, 3f);

        [Space(10)]
        [SerializeField]
        private Shard_RepellAttributes repellAttributes = new Shard_RepellAttributes(1.5f);

        [Space(10)]
        [SerializeField]
        private Shard_ReturnAttributes returnAttributes = new Shard_ReturnAttributes();

        #region Properties
        public Shard_RepellAttributes RepellAttributes { get => repellAttributes; }
        public Shard_ThrowAttributes ThrowAttributes { get => throwAttributes; }
        public Shard_ReturnAttributes ReturnAttributes { get => returnAttributes; }
        #endregion
    }
}
