using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Cylindre
{
    public class Mesh
    {
        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<int> VertIndices { get; set; }
        public List<int> NormIndices { get; set; }

        public Mesh()
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            VertIndices = new List<int>();
            NormIndices = new List<int>();
        }

        public Mesh(int verticesCnt, int indicesCnt)
        {
            Vertices = new List<Vector3>(verticesCnt);
            Normals = new List<Vector3>(verticesCnt);
            VertIndices = new List<int>(indicesCnt);
            NormIndices = new List<int>(indicesCnt);
        }
    }

    public static class GeometryProvider
    {
        private static int GetMidpointIndex(Dictionary<Tuple<int, int>, int> midpointIndices, List<Vector3> vertices, int i0, int i1)
        {
            var edgeKey = Tuple.Create(Math.Min(i0, i1), Math.Max(i0, i1));

            int midpointIndex;

            if (!midpointIndices.TryGetValue(edgeKey, out midpointIndex))
            {
                var v0 = vertices[i0];
                var v1 = vertices[i1];

                var midpoint = (v0+v1) / 2f;

                if (vertices.Contains(midpoint))
                {
                    midpointIndex = vertices.IndexOf(midpoint);
                }
                else
                {
                    midpointIndex = vertices.Count;
                    vertices.Add(midpoint);
                    midpointIndices.Add(edgeKey, midpointIndex);
                }
            }

            return midpointIndex;
        }

        /// <remarks>
        ///      i0
        ///     /  \
        ///    m02-m01
        ///   /  \ /  \
        /// i2---m12---i1
        /// </remarks>
        public static void Subdivide(Mesh mesh, bool removeSourceTriangles)
        {
            var midpointIndices = new Dictionary<Tuple<int, int>, int>();

            var newIndices = new List<int>(mesh.VertIndices.Count * 4);

            if (!removeSourceTriangles)
            {
                newIndices.AddRange(mesh.VertIndices);
            }

            for (var i = 0; i < mesh.VertIndices.Count-2; i += 3)
            {
                var i0 = mesh.VertIndices[i];
                var i1 = mesh.VertIndices[i+1];
                var i2 = mesh.VertIndices[i+2];

                var m01 = GetMidpointIndex(midpointIndices, mesh.Vertices, i0, i1);
                var m12 = GetMidpointIndex(midpointIndices, mesh.Vertices, i1, i2);
                var m02 = GetMidpointIndex(midpointIndices, mesh.Vertices, i2, i0);

                newIndices.AddRange(
                    new[] {
                        i0,m01,m02,
                        i1,m12,m01,
                        i2,m02,m12,
                        m02,m01,m12
                    }
                );
            }

            mesh.VertIndices = newIndices;
        }
    }


    class Program
    {
        public static Mesh Octahedron()
        {
            Mesh mesh = new Mesh(6, 8*3);
            mesh.Vertices.AddRange(
                new[]
                {
                    new Vector3(0, 1, 0),
                    new Vector3(0, 0, 1),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 0, -1),
                    new Vector3(-1, 0, 0),
                    new Vector3(0, -1, 0),
                }
            );

            mesh.VertIndices.AddRange(
                new[]
                {
                    0, 2, 1,
                    0, 3, 2,
                    0, 4, 3,
                    0, 1, 4,
                    1, 2, 5,
                    2, 3, 5,
                    3, 4, 5,
                    4, 1, 5
                }
            );

            return mesh;
        }

        public static Mesh Icosahedron()
        {
            Mesh mesh = new Mesh(12, 20*3);

            var X = 0.525731112119133606f;
            var Z = 0.850650808352039932f;

            mesh.Vertices.AddRange(
                new[]
                {
                    new Vector3(-X, 0f, Z),
                    new Vector3(X, 0f, Z),
                    new Vector3(-X, 0f, -Z),
                    new Vector3(X, 0f, -Z),
                    new Vector3(0f, Z, X),
                    new Vector3(0f, Z, -X),
                    new Vector3(0f, -Z, X),
                    new Vector3(0f, -Z, -X),
                    new Vector3(Z, X, 0f),
                    new Vector3(-Z, X, 0f),
                    new Vector3(Z, -X, 0f),
                    new Vector3(-Z, -X, 0f)
                }
            );

            mesh.VertIndices.AddRange(
                new []
                {
                    0,1,4,
                    0,4,9,
                    9,4,5,
                    4,8,5,
                    4,1,8,
                    8,1,10,
                    8,10,3,
                    5,8,3,
                    5,3,2,
                    2,3,7,
                    7,3,10,
                    7,10,6,
                    7,6,11,
                    11,6,0,
                    0,6,1,
                    6,10,1,
                    9,11,0,
                    9,2,11,
                    9,5,2,
                    7,11,2
                }
            );

            return mesh;
        }

        static Mesh Geosphere(int detailLevel = 2)
        {
            // geosphere
            // Taken from http://gamedev.stackexchange.com/a/31312/86181
            Mesh geosphere = Icosahedron();
            //Mesh geosphere = Octahedron();

            for (var i = 0; i < detailLevel; i++)
            {
                GeometryProvider.Subdivide(geosphere, true);
            }

            // normalize vertices to "inflate" the icosahedron into a sphere.
            for (var i = 0; i < geosphere.Vertices.Count; i++)
            {
                geosphere.Vertices[i] = Vector3.Normalize(geosphere.Vertices[i]);
            }

            geosphere.Normals.AddRange(geosphere.Vertices);
            geosphere.NormIndices.AddRange(geosphere.VertIndices);

            return geosphere;
        }

        static Mesh Sphere(int detailLevel = 15)
        {
            // Compute n circle points
            int n = detailLevel * 2; // we only support even number of subdivision (for now).

            float angleInc = 360.0f / n;
            float currAngle = 0;

            float[] xPts = new float[n];
            float[] zPts = new float[n];

            Mesh sphere = new Mesh(n*n/2+1, n*n/2-n+2);

            for (int i = 0; i < n; i++)
            {
                float angle = DegToRad(currAngle);
                xPts[i] = (float)Math.Cos(angle);
                zPts[i] = (float)Math.Sin(angle);
                currAngle += angleInc;
            }

            // Top point
            sphere.Vertices.Add(new Vector3(0, 1, 0));

            // Compute the points along the y (up) axis
            angleInc = 360.0f/n;
            currAngle = angleInc+90;
            for (int i = 0; i < n/2-1; i++)
            {
                float angle = DegToRad(currAngle);
                float radius = (float)Math.Cos(angle);
                float y = (float)Math.Sin(angle);
                for (int j = 0; j < n; j++)
                {
                    sphere.Vertices.Add(new Vector3(xPts[j]*radius, y, zPts[j]*radius));
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
            sphere.VertIndices.AddRange(new[] {n*(n/2-2)+1, bottomIdx, n*(n/2-1)});

            sphere.Normals.AddRange(sphere.Vertices);
            sphere.NormIndices.AddRange(sphere.VertIndices);
            return sphere;
        }

        static Mesh Cylinder(int detailLevel = 15)
        {
            int n = detailLevel;
            Mesh cylinder = new Mesh(2*n+2, 4*n);

            float angleInc = 360.0f / n;
            float currAngle = 0;

            float[] xPts = new float[n];
            float[] zPts = new float[n];

            for (int i = 0; i < n; i++)
            {
                float angle = DegToRad(currAngle);
                xPts[i] = (float)Math.Cos(angle);
                zPts[i] = (float)Math.Sin(angle);
                currAngle += angleInc;
            }

            // Vertices for one face.
            var createVerts = new Action<float>(
                y =>
                {
                    cylinder.Vertices.Add(new Vector3(0, y, 0));

                    angleInc = 360.0f/n;
                    currAngle = angleInc+90;
                    for (int i = 0; i < n; i++)
                    {
                        cylinder.Vertices.Add(new Vector3(xPts[i], y, zPts[i]));
                        currAngle += angleInc;
                    }
                });
            createVerts(0);
            createVerts(1);

            // Normals
            cylinder.Normals.Add(new Vector3(0, -1, 0));

            angleInc = 360.0f/n;
            currAngle = angleInc+90;
            for (int i = 0; i < n; i++)
            {
                cylinder.Normals.Add(new Vector3(xPts[i], 0, zPts[i]));
                currAngle += angleInc;
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

        static void OutputObj(Mesh mesh, string fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            using (StreamWriter sr = new StreamWriter(fs))
            {
                foreach (var vertex in mesh.Vertices)
                {
                    sr.WriteLine("v {0:f5} {1:f5} {2:f5}", vertex.X, vertex.Y, vertex.Z);
                }

                foreach (var normal in mesh.Normals)
                {
                    sr.WriteLine("vn {0:f5} {1:f5} {2:f5}", normal.X, normal.Y, normal.Z);
                }

                for (int i = 0; i < mesh.VertIndices.Count; i+=3)
                {
                    sr.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
                        mesh.VertIndices[i]+1, mesh.NormIndices[i]+1,
                        mesh.VertIndices[i+1]+1, mesh.NormIndices[i+1]+1,
                        mesh.VertIndices[i+2]+1, mesh.NormIndices[i+2]+1);
                }
            }
        }

        static void Main(string[] args)
        {
            OutputObj(Geosphere(2), @"c:\dump\geosphere.obj");

            OutputObj(Sphere(5), @"c:\dump\sphere.obj");

            OutputObj(Cylinder(30), @"c:\dump\cylinder.obj");
        }

        static float DegToRad(float d)
        {
            return d * (float)Math.PI/180.0f;
        }
    }
}
