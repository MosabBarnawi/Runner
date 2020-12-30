using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class ShardClifBehavior : MonoBehaviour
    {
        public Vector3 ExitPosition { get; set; }
        [SerializeField] MainPlayerScript mainPlayer;

        private BoxCollider boxCollider;
        private void Awake() => boxCollider = GetComponent<BoxCollider>();
        private void OnEnable() => boxCollider.enabled = true;

        private void Start()
        {
            if (mainPlayer == null) Debug.Log("Mian Player Script Not Assigned");
        }

        private void OnDisable() => boxCollider.enabled = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<MainPlayerScript>() != null)
            {
                mainPlayer.TeleportPlayer(ExitPosition);
                Instantiate(GameManager.SharedInstance.TeleportFX, other.transform.position, GameManager.SharedInstance.TeleportFX.transform.rotation);
            }
        }
    }
}