using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class LoadGame : MonoBehaviour
    {
        public void TestLoadLevel()
        {
            SceneManager.LoadScene(1);
            SceneManager.LoadScene(2);
        }
    }
}