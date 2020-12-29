using BarnoGames.Runner2020;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Test
{
    public abstract class AbstractClassTest : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 5;

        private int health;

        private void Awake()
        {
            health = maxHealth;
        }

        public void TakeDamageeeee(int damage)
        {
            
        }

        public virtual void TakeDamage(int damage)
        {
            health -= damage;

            if (health <= 0)
                Destroy(gameObject);
        }
    }

    public class O1 : AbstractClassTest
    {
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

          
            /// DO STUFF
        }
    }

    public class Manager : MonoBehaviour
    {
        private O1 o1;

        private void Start()
        {
            o1.TakeDamage(1);
            o1.TakeDamageeeee(2);
        }
    }
}
