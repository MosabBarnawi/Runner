using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class EndPointTrigger : MonoBehaviour
    {
        private bool triggered = false;
        private void EndLevel() => GameManager.SharedInstance.CompletedLevelState();

        private void OnTriggerEnter(Collider other)
        {
            if (triggered) return;

            if (other.GetComponent<PlayerTag>() != null)
            {
                EndLevel();
                triggered = true;
            }
        }
    }
}