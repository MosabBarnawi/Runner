using UnityEngine;

namespace BarnoGames.Runner2020
{
    [System.Serializable] // TODO:: REMOVE THIS
    class RotationsContainer // mid be abole to do without this
    {
        public Vector3 IdelRotationAxis = new Vector3(1, 1, 0);
        public Vector3 MotionRotationAxis = new Vector3(1, 0, 5);
        public Vector3 ReturningSpeedRotation = new Vector3(-1, -1, 0);
    }
}
