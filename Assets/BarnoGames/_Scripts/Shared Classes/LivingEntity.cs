using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    public abstract class LivingEntity : MonoBehaviour, IDamagable
    {
        protected Action OnDeath;
        protected Action OnAddHealth;
        protected Action OnTakeHit;

        [Space(10)]
        [SerializeField] private LivingEntityHealth entityHealth;

        #region Properties
        public bool IsDead
        {
            get => entityHealth.isDead;
            protected set => entityHealth.isDead = value;
        }

        #endregion

        #region Unity Callbacks
        protected virtual void Awake() { }

        protected virtual void Start() => entityHealth.Health = entityHealth.StartingHealth <= 0 ? entityHealth.Health = 10f : entityHealth.StartingHealth;
        #endregion

        #region IDamagable Interface
        public void TakeDamage(float damage)
        {
            Debug.LogFormat("{0} , Taken {1} Damage", gameObject.name, damage);
            entityHealth.Health -= damage;

            OnTakeHit?.Invoke();

            if (entityHealth.Health <= 0 && !IsDead)
                Die();
        }

        public void TakeHit(float damage, RaycastHit hit)
        {
            Debug.LogFormat("{0} , Taken {1} Damage", gameObject.name, damage + " + Raycast");
            entityHealth.Health -= damage;

            OnTakeHit?.Invoke();

            if (entityHealth.Health <= 0 && !IsDead)
                Die();
        }

        #endregion

        #region Public API
        public void AddHealth(float healthValue)
        {
            entityHealth.Health += healthValue;
            Debug.LogFormat("{0} , Added {1} HP", gameObject.name, healthValue);

            if (OnAddHealth != null) OnAddHealth();
            else Debug.Log("No On Add Health");
        }

        #endregion

        #region Private API
        private void Die()
        {
            IsDead = true;

            if (OnDeath != null)
            {
                OnDeath();
            }
            else Debug.LogAssertion("No OnDeath Function");
        }
        #endregion
    }

}
