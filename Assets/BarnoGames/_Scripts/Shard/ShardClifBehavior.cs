using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class ShardClifBehavior : MonoBehaviour
    {
        public float CliffHeight { get; set; }
        private IBoost iBoost;

        private BoxCollider boxCollider;
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            iBoost = PlayerInputControls.Player.GetComponent<IBoost>();
        }
        private void OnEnable() => boxCollider.enabled = true;

        private void OnDisable() => boxCollider.enabled = false;

        // TODO:: CHANGE TO COLLISION


        //private void OnCollisionEnter(Collision collision)
        //{
        //    Debug.Log(collision.ToString());
        //    if (collision.transform.GetComponent<IPlayerTAG>() != null)
        //    {
        //        //iBoost?.AddShardBoost(CliffHeight);
        //    }
        //}
        //private void OnCollisionEnter(Collision other)
        //{
        //          if (other.GetComponent<IPlayerTAG>() != null)
        //          {
        //              iBoost?.AddShardBoost(CliffHeight);
        //          }
        //}			
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IPlayerTAG>() != null)
            {
                iBoost?.AddShardBoost(CliffHeight);
            }
        }
    }
}