using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControls : MonoBehaviour, IAttack
{
    #region Unity CallBacks
    void Start()
    {

    }

    void Update()
    {

    }

    #endregion

    #region
    public void Attack()
    {
        Debug.Log("Attacked");
    }
    #endregion
}
