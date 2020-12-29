using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager SharedInstance;

        #region Unity Callbacks
        void Awake()
        {
            if (SharedInstance == null)
            {
                SharedInstance = this;
            }
            else if (SharedInstance != this)
            {
                Destroy(this);
            }
        }
        #endregion

        public void ShowAD()
        {
            Debug.Log("Ads Manager :: Displayed Ad");
        }
    }
}