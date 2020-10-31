using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int TEXT_VALUE;
    public static GameManager SharedInstance;

    #region Unity Callbacks
    void Awake()
    {
        if (SharedInstance == null)
        {
            DontDestroyOnLoad(this);
            SharedInstance = this;
        }
        else if (SharedInstance != this)
        {
            Destroy(this);
        }
    }
    #endregion
}
