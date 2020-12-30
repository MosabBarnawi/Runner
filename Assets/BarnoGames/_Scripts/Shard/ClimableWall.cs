using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class ClimableWall : MonoBehaviour, IClimableTAG
    {
        public Transform ExitPointTransform { get => _exitPointTransform; }
        [SerializeField] private Transform _exitPointTransform;
    }
}