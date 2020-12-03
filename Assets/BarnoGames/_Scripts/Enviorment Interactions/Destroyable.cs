using UnityEngine;

namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public class Destroyable : MonoBehaviour
    {
        public void Smached()
        {
            Debug.Log("Broken");
            Destroy(gameObject);
        }
    }
}