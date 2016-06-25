﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Cylindre
{
    public class Mesh
    {
        public List<Vector3> Vertices { get; set; }
        public List<int> Indices { get; set; }

        public Mesh()
        {
            Vertices = new List<Vector3>();
            Indices = new List<int>();
        }

        public Mesh(int verticesCnt, int indicesCnt)
        {
            Vertices = new List<Vector3>(verticesCnt);
            Indices = new List<int>(indicesCnt);
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

                var midpoint = (v0 + v1) / 2f;

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

            var newIndices = new List<int>(mesh.Indices.Count * 4);

            if (!removeSourceTriangles)
            {
                newIndices.AddRange(mesh.Indices);
            }

            for (var i = 0; i < mesh.Indices.Count - 2; i += 3)
            {
                var i0 = mesh.Indices[i];
                var i1 = mesh.Indices[i + 1];
                var i2 = mesh.Indices[i + 2];

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

            mesh.Indices = newIndices;
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

            mesh.Indices.AddRange(
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

            mesh.Indices.AddRange(
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

            Mesh sphere = new Mesh(n*n/2 + 1, n*n/2 - n + 2);

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
            currAngle = angleInc + 90;
            for (int i = 0; i < n/2 - 1; i++)
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
                sphere.Indices.AddRange(new[] {0, i+1, i});
            }
            sphere.Indices.AddRange(new[] {0, 1, n});

            // Middle rows
            for (int c = 0; c < n/2-2; c++)
            {
                int row = c*n;
                for (int i = row+1; i < row+n; i++)
                {
                    sphere.Indices.AddRange(new[]
                    {
                        i+n, i,   i+1,
                        i+n, i+1, i+n+1
                    });
                }

                sphere.Indices.AddRange(new[]
                {
                    row+2*n, row+n, row+1,
                    row+2*n, row+1, row+n+1
                });
            }

            // Bottom row
            int bottomIdx = n*(n/2 - 1) + 1;
            for (int i = n*(n/2-2)+1; i < bottomIdx; i++)
            {
                sphere.Indices.AddRange(new[] { i+1, bottomIdx, i });
            }
            sphere.Indices.AddRange(new[] { n*(n/2-2)+1, bottomIdx, n*(n/2-1) });

            return sphere;
        }

        static void OutputObj(Mesh mesh, string fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            using (StreamWriter sr = new StreamWriter(fs))
            {
                foreach (var vertex in mesh.Vertices)
                {
                    sr.WriteLine("v {0:f5} {1:f5} {2:f5}", vertex.X, vertex.Y, vertex.Z);
                    sr.WriteLine("vn {0:f5} {1:f5} {2:f5}", vertex.X, vertex.Y, vertex.Z);
                }

                for (int i = 0; i < mesh.Indices.Count; i+=3)
                {
                    sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", mesh.Indices[i]+1, mesh.Indices[i+1]+1, mesh.Indices[i+2]+1);
                }
            }
        }

        static void Main(string[] args)
        {
            Mesh geosphere = Geosphere(2);
            OutputObj(geosphere, @"c:\dump\geosphere.obj");
            Mesh sphere = Sphere(5);
            OutputObj(sphere, @"c:\dump\sphere.obj");

#if false // Cylindre
    // Compute n circle points
            int n = 25;

            double angleInc = 360.0 / n;
            double currAngle = 0;

            double[] xPts = new double[n];
            double[] yPts = new double[n];

            for (int i = 0; i < n; i++)
            {
                double angle = DegToRad(currAngle);
                xPts[i] = Math.Cos(angle);
                yPts[i] = Math.Sin(angle);
                currAngle += angleInc;
            }

            using (FileStream fs = File.Open(@"c:\dump\cylindre.obj", FileMode.Create))
            using (StreamWriter sr = new StreamWriter(fs))
            {
                sr.WriteLine("g blah\n");
                sr.WriteLine("v {0:f5} {1:f5} {2:f5}", 0, 10, 0);

                for (int i = 0; i < n; i++)
                {
                    sr.WriteLine("v {0:f5} {1:f5} {2:f5}", xPts[i]*5, 10, yPts[i]*5);
                }
                sr.WriteLine();

                sr.WriteLine("vn 0 1 0");


                for (int i = 1; i < n ; i++)
                {
                    sr.WriteLine("f {0}//1 {1}//1 {2}//1", 1, i+1, i+2);
                }
                sr.WriteLine("f {0}//1 {1}//1 {2}//1", 1, n+1, 2);
            }
#endif
        }

        static float DegToRad(float d)
        {
            return d * (float)Math.PI/180.0f;
        }
    }
}
