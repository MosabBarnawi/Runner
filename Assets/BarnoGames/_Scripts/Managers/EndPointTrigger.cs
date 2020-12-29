using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class EndPointTrigger : MonoBehaviour
    {
        private bool triggered = false;
        private void EndLevel() => GameManager.SharedInstance.OnWinState();

        private void OnTriggerEnter(Collider other)
        {
            if (triggered) return;

            if (other.GetComponent<IPlayerTAG>() != null)
            {
                EndLevel();
                triggered = true;
            }
            else
            {
                Debug.LogError(other.gameObject.name);
            }
        }
    }
}