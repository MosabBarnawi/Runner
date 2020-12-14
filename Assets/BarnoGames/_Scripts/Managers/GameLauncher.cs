using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class GameLauncher : MonoBehaviour
    {
        void Start()
        {
            SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        }
        public void TestLoadLevel()
        {
            FindObjectOfType<GameManager>().LoadSpecificLevel(2);
        }
    }
}