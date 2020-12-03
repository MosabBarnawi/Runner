using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class ClimableWall : MonoBehaviour, IClimableTAG
    {
        [SerializeField] public float Height = 50f;
        public Vector3 CliffCenter;

        private void Awake()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            CliffCenter.x = boxCollider.size.x / 2;
            CliffCenter.y = boxCollider.size.y / 2;
            CliffCenter.z = boxCollider.size.z / 2;
        }
    }
}