using System;
using System.Numerics;

namespace Cylindre
{
    public partial class Mesh
    {
        public static Mesh Cylinder()
        {
            return Cylinder(1, 1);
        }

        public static Mesh Cylinder(float radius, float height)
        {
            return Cylinder(radius, height, 15);
        }

        public static Mesh Cylinder(float radius, float height, int detailLevel)
        {
            int n = detailLevel;

            Vector2[] pts = MathHelpers.ComputeCirclePoints(n);
            Mesh cylinder = new Mesh(2*n+2, 12*n);

            // m_vertices for one face.
            var createVerts = new Action<float>(
                y =>
                {
                    cylinder.m_Vertices.Add(new Vector3(0, y, 0));

                    for (int i = 0; i < n; i++)
                    {
                        cylinder.m_Vertices.Add(new Vector3(pts[i].X * radius, y, pts[i].Y * radius));
                    }
                });
            createVerts(0);
            createVerts(height);

            // Bottom face
            for (int i = 1; i < n; i++)
            {
                cylinder.m_VertIndices.AddRange(new[] {0, i, i+1});
            }
            cylinder.m_VertIndices.AddRange(new[] {0, n, 1});

            // Top face
            for (int i = n+2; i < 2*n+1; i++)
            {
                cylinder.m_VertIndices.AddRange(new[] {n+1, i+1, i});
            }
            cylinder.m_VertIndices.AddRange(new[] {n+1, n+2, 2*n+1});

            // Sides
            for (int i = 1; i < n; i++)
            {
                cylinder.m_VertIndices.AddRange(new[] {i, i+n+2, i+1});
                cylinder.m_VertIndices.AddRange(new[] {i, i+n+1, i+n+2});
            }
            cylinder.m_VertIndices.AddRange(new[] {1, n, n+2});
            cylinder.m_VertIndices.AddRange(new[] {n, 2*n+1, n+2});

            return cylinder;
        }
    }
}
