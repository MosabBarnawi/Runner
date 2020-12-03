using UnityEngine;

namespace BarnoGames.Runner2020
{
    class CurvePoint
    {
        public static Vector3 GetBQCPoint(Vector3 currentPosition, Vector3 curvePointEffector, Vector3 target, float time)
        {
            float u = 1 - time;
            float tt = time * time;
            float uu = u * u;
            Vector3 p = (uu * currentPosition) + (2 * u * time * curvePointEffector) + (tt * target);

            //Vector3 clamped = new Vector3(Mathf.Clamp(currentPosition.x, p.x, target.x), Mathf.Clamp(currentPosition.y, p.y, target.y), Mathf.Clamp(currentPosition.z, p.z, target.z));
            return p;
            //return clamped;
        }
    }
}
