using UnityEngine;

namespace BarnoGames.Runner2020
{
    [System.Serializable] // TODO:: REMOVE THIS
    public struct ReturningCalculations
    {
        public int RandomCurvePointIndex { get; private set; }
        public float ReturningTime { get; set; }
        public Vector3 CurrentPosition { get; set; }

        public void GetRandomReturnPoint(int numberOfCuvePoints)
        {
            RandomCurvePointIndex = Random.Range(0, numberOfCuvePoints);
        }
    }
}
