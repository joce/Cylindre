using System;
using System.Numerics;

namespace Cylindre
{
    public partial class Mesh
    {
        public static Mesh Cylinder()
        {
            return Cylinder(15);
        }

        public static Mesh Cylinder(int detailLevel)
        {
            int n = detailLevel;

            Vector2[] pts = MathHelpers.ComputeCirclePoints(n);
            Mesh cylinder = new Mesh(2*n+2, 4*n);

            // Vertices for one face.
            var createVerts = new Action<float>(
                y =>
                {
                    cylinder.Vertices.Add(new Vector3(0, y, 0));

                    for (int i = 0; i < n; i++)
                    {
                        cylinder.Vertices.Add(new Vector3(pts[i].X, y, pts[i].Y));
                    }
                });
            createVerts(0);
            createVerts(1);

            // Normals
            cylinder.Normals.Add(new Vector3(0, -1, 0));

            for (int i = 0; i < n; i++)
            {
                cylinder.Normals.Add(new Vector3(pts[i].X, 0, pts[i].Y));
            }
            cylinder.Normals.Add(new Vector3(0, 1, 0));

            // Bottom face
            for (int i = 1; i < n; i++)
            {
                cylinder.VertIndices.AddRange(new[] {0, i, i+1});
                cylinder.NormIndices.AddRange(new[] {0, 0, 0});
            }
            cylinder.VertIndices.AddRange(new[] {0, n, 1});
            cylinder.NormIndices.AddRange(new[] {0, 0, 0});

            // Top face
            for (int i = n+2; i < 2*n+1; i++)
            {
                cylinder.VertIndices.AddRange(new[] {n+1, i+1, i});
                cylinder.NormIndices.AddRange(new[] {n+1, n+1, n+1});
            }
            cylinder.VertIndices.AddRange(new[] {n+1, n+2, 2*n+1});
            cylinder.NormIndices.AddRange(new[] {n+1, n+1, n+1});

            // Sides
            for (int i = 1; i < n; i++)
            {
                cylinder.VertIndices.AddRange(new[] {i, i+n+2, i+1});
                cylinder.NormIndices.AddRange(new[] {i, i+1, i+1});

                cylinder.VertIndices.AddRange(new[] {i, i+n+1, i+n+2});
                cylinder.NormIndices.AddRange(new[] {i, i, i+1});
            }
            cylinder.VertIndices.AddRange(new[] {1, n, n+2});
            cylinder.NormIndices.AddRange(new[] {1, n, 1});

            cylinder.VertIndices.AddRange(new[] {n, 2*n+1, n+2});
            cylinder.NormIndices.AddRange(new[] {n, n, 1});

            return cylinder;
        }
    }
}
