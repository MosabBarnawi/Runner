using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class DestroyMeTimer : MonoBehaviour
    {
        [SerializeField] private float Timer = 2f;
        private void OnEnable() => Destroy(gameObject, Timer);
    }
}