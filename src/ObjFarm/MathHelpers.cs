using System;
using System.Numerics;

namespace ObjFarm
{
    public static class MathHelpers
    {
        public static float DegToRad(float d)
        {
            return d * (float)Math.PI/180.0f;
        }

        public static Vector2[] ComputeCirclePoints(int n)
        {
            float angleInc = 360.0f / n;
            float currAngle = 0;

            Vector2[] pts = new Vector2[n];

            for (int i = 0; i < n; i++)
            {
                float angle = DegToRad(currAngle);
                pts[i] = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                currAngle += angleInc;
            }

            return pts;
        }
    }
}
