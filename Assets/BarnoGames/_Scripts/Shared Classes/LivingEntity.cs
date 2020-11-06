using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IDamagable
{
    public Action OnDeath;
    public Action OnAddHealth;
    public Action OnTakeHit;

    [Header("Health")]
    public float StartingHealth;
    //protected float _maxHealth;
    //protected float _health;
    //protected bool dead;
    private float _health;
    private bool dead;

    public bool isDead 
    {
        get { return dead; } 
        set { isDead = value; } 
    }

    #region Unity Callbacks
    protected virtual void Start()
    {
        if (StartingHealth <= 0)
        {
            //Debug.LogFormat("{0}, Dose not Have A Starting Health Value {1}", gameObject.name, StartiingHealth);
            _health = 10f;
        }
        else _health = StartingHealth;
    }
    #endregion

    #region Contracts
    public void TakeDamage( float damage )
    {
        Debug.LogFormat("{0} , Taken {1} Damage" , gameObject.name , damage);
        _health -= damage;

        if (_health <= 0 && !dead)
            Die();
    }

    public void TakeHit( float damage , RaycastHit hit )
    {
        Debug.LogFormat("{0} , Taken {1} Damage" , gameObject.name , damage + " + Raycast");
        _health -= damage;

        OnTakeHit?.Invoke();

        if (_health <= 0 && !dead)
            Die();
    }

    public void AddHealth( float healthValue )
    {
        _health += healthValue;
        Debug.LogFormat("{0} , Added {1} HP" , gameObject.name , healthValue);

        if (OnAddHealth != null) OnAddHealth();
        else Debug.Log("No On Add Health");
    }
    #endregion

    #region Private API
    private void Die()
    {
        dead = true;

        if (OnDeath != null)
        {
            OnDeath();
        }
        else Debug.LogAssertion("No OnDeath Function");
    }
    #endregion
}
