using System;
using System.Numerics;

namespace Cylindre
{
    public partial class Mesh
    {
        public static Mesh Sphere()
        {
            return Sphere(15);
        }

        public static Mesh Sphere(int detailLevel)
        {
            // Compute n circle points
            int n = detailLevel * 2; // we only support even number of subdivision (for now).

            Vector2[] pts = MathHelpers.ComputeCirclePoints(n);

            Mesh sphere = new Mesh(n*n/2+1, n*n/2-n+2);

            // Top point
            sphere.Vertices.Add(new Vector3(0, 1, 0));

            // Compute the points along the y (up) axis
            float angleInc = 360.0f/n;
            float currAngle = angleInc+90;
            for (int i = 0; i < n/2-1; i++)
            {
                float angle = MathHelpers.DegToRad(currAngle);
                float radius = (float)Math.Cos(angle);
                float y = (float)Math.Sin(angle);
                for (int j = 0; j < n; j++)
                {
                    sphere.Vertices.Add(new Vector3(pts[j].X*radius, y, pts[j].Y*radius));
                }
                currAngle += angleInc;
            }

            // Bottom point
            sphere.Vertices.Add(new Vector3(0, -1, 0));

            // Top row
            for (int i = 1; i < n; i++)
            {
                sphere.VertIndices.AddRange(new[] {0, i+1, i});
            }
            sphere.VertIndices.AddRange(new[] {0, 1, n});

            // Middle rows
            for (int c = 0; c < n/2-2; c++)
            {
                int row = c*n;
                for (int i = row+1; i < row+n; i++)
                {
                    sphere.VertIndices.AddRange(new[]
                    {
                        i+n, i,   i+1,
                        i+n, i+1, i+n+1
                    });
                }

                sphere.VertIndices.AddRange(new[]
                {
                    row+2*n, row+n, row+1,
                    row+2*n, row+1, row+n+1
                });
            }

            // Bottom row
            int bottomIdx = n*(n/2-1)+1;
            for (int i = n*(n/2-2)+1; i < bottomIdx; i++)
            {
                sphere.VertIndices.AddRange(new[] { i+1, bottomIdx, i });
            }
            sphere.VertIndices.AddRange(new[] {n*(n/2-2) + 1, bottomIdx, n*(n/2-1)});

            return sphere;
        }
    }
}
