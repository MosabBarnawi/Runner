using UnityEngine;

namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public class Destroyable : MonoBehaviour, IDestroyable
    {
        public void Smached()
        {
            Debug.Log("Broken");
            Destroy(gameObject);
        }
    }
}