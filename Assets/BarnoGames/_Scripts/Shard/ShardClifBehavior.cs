using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class ShardClifBehavior : MonoBehaviour
    {
        public Vector3 exitPosition { get; set; }
        private IBoost iBoost;

        private BoxCollider boxCollider;
        private void Awake() => boxCollider = GetComponent<BoxCollider>();
        private void OnEnable() => boxCollider.enabled = true;

        private void Start() => iBoost = PlayerInputControls.Player.GetComponent<IBoost>();

        private void OnDisable() => boxCollider.enabled = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IPlayerTAG>() != null)
            {
                iBoost?.TeleportToPosition(exitPosition);
                Instantiate(GameManager.SharedInstance.TeleportFX, other.transform.position, GameManager.SharedInstance.TeleportFX.transform.rotation);
            }
        }
    }
}