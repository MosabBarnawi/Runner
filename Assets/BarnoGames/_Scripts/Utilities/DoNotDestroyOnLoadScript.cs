using UnityEngine;
using UnityEngine.SceneManagement;

namespace BarnoGames.Runner2020
{
    public class DoNotDestroyOnLoadScript : MonoBehaviour
    {
        private void Awake() => DontDestroyOnLoad(gameObject);
    }
}