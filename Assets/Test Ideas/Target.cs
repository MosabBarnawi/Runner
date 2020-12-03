using UnityEngine;

namespace BarnoGames.Runner2020.Target
{
    public class Target : MonoBehaviour
    {
        private Transform _transform;
        private AttackControls iAttack;

        private void Start()
        {
            _transform = GetComponent<Transform>();
            iAttack = FindObjectOfType<AttackControls>();
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag(TAGS.TAG_PLAYER))
            //{
            //    Debug.Log("In");
            //    iAttack.GoToPosition(transform, DestroyMe);
            //}
        }

        private void OnTriggerExit(Collider other)
        {
            //if (other.CompareTag(TAGS.TAG_PLAYER))
            //{
            //    DestroyMe(); // BETTER WAY SO PLAYER CANNOT FLY BACK
            //}

            Debug.Log("Out");
        }

        private void DestroyMe() => Destroy(gameObject);
    }
}