using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class StartLevelFinishedAnimation : MonoBehaviour
    {
        
        public void StartLevel()
        {
            PlayerInputControls.PlayerScript.StartLevel();
            //Debug.LogWarning("This is Empty");
        }
    }
}