using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using BarnoGames.Runner2020;

namespace BarnoGames.Runner2020.Subscriber
{
    public class Subscriber : MonoBehaviour
    {
        [SerializeField] private TEstEnum testEnum;

        #region Unity Callbacks

        void Start()
        {
            GameManager.SharedInstance.OnCollectedItem += ReciveMessage;
        }

        #region Incase of Disable Of Destroy
        private void OnDisable()
        {
            GameManager.SharedInstance.OnCollectedItem -= ReciveMessage;
        }
        private void OnDestroy()
        {
            GameManager.SharedInstance.OnCollectedItem -= ReciveMessage;
        }
        #endregion

        #endregion

        #region Private API
        private void ReciveMessage(object sender, GameManager.TestingM e)
        {
            Debug.Log("HRYY222 = " + e.counter);
            testEnum = e.estEnum;
            GameManager.SharedInstance.OnCollectedItem -= ReciveMessage;
        }

        private void ReciveMessage(object sender, System.EventArgs e)
        {
            Debug.Log("HRYY");
            GameManager.SharedInstance.OnCollectedItem -= ReciveMessage;
        }

        #endregion
    }
}