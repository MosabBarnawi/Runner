using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class Clollectable : MonoBehaviour
    {
        [SerializeField] private int value = 5;
        private void SharedInstance_Collect()
        {
            Debug.Log("Pickup");
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<IPlayerTAG>() != null)
            {
                GameManager.SharedInstance.AddPoints(value, SharedInstance_Collect);
            }
        }
    }
}