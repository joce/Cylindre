﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Cylindre
{

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
        public static void Subdivide(List<Vector3> vectors, ref List<int> indices, bool removeSourceTriangles)
        {
            var midpointIndices = new Dictionary<Tuple<int, int>, int>();

            var newIndices = new List<int>(indices.Count * 4);

            if (!removeSourceTriangles)
            {
                newIndices.AddRange(indices);
            }

            for (var i = 0; i < indices.Count - 2; i += 3)
            {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var m01 = GetMidpointIndex(midpointIndices, vectors, i0, i1);
                var m12 = GetMidpointIndex(midpointIndices, vectors, i1, i2);
                var m02 = GetMidpointIndex(midpointIndices, vectors, i2, i0);

                newIndices.AddRange(
                    new[] {
                        i0,m01,m02,
                        i1,m12,m01,
                        i2,m02,m12,
                        m02,m01,m12
                    }
                );

            }

            //indices.Clear();
            //indices.AddRange(newIndices);
            indices = newIndices;
        }
    }


    class Program
    {

        public static void Icosahedron(List<Vector3> vertices, List<int> indices)
        {

            indices.AddRange(
                new []
                {
                    0,4,1,
                    0,9,4,
                    9,5,4,
                    4,5,8,
                    4,8,1,
                    8,10,1,
                    8,3,10,
                    5,3,8,
                    5,2,3,
                    2,7,3,
                    7,10,3,
                    7,6,10,
                    7,11,6,
                    11,0,6,
                    0,1,6,
                    6,1,10,
                    9,0,11,
                    9,11,2,
                    9,2,5,
                    7,2,11
                }
                .Select(i => i + vertices.Count)
            );

            var X = 0.525731112119133606f;
            var Z = 0.850650808352039932f;

            vertices.AddRange(
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
        }

        static void Geosphere(int detailLevel = 2)
        {
            // geosphere
            // Taken from http://gamedev.stackexchange.com/a/31312/86181
            var vectors = new List<Vector3>();
            var indices = new List<int>();
            Icosahedron(vectors, indices);

            for (var i = 0; i < detailLevel; i++)
            {
                GeometryProvider.Subdivide(vectors, ref indices, true);
            }

            // normalize vectors to "inflate" the icosahedron into a sphere.
            for (var i = 0; i < vectors.Count; i++)
            {
                vectors[i] = Vector3.Normalize(vectors[i]);
            }

            using (FileStream fs = File.Open(@"c:\dump\geosphere.obj", FileMode.Create))
            using (StreamWriter sr = new StreamWriter(fs))
            {
                foreach (var vector in vectors)
                {
                    sr.WriteLine("v {0:f5} {1:f5} {2:f5}", vector.X, vector.Y, vector.Z);
                    sr.WriteLine("vn {0:f5} {1:f5} {2:f5}", vector.X, vector.Y, vector.Z);
                }

                for (int i = 0; i < indices.Count; i+=3)
                {
                    sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", indices[i]+1, indices[i+2]+1, indices[i+1]+1);
                }
            }
        }

        static void Sphere(int detailLevel = 15)
        {
            // Compute n circle points
            int n = detailLevel * 2; // we only support even number of subdivision (for now).

            double angleInc = 360.0 / n;
            double currAngle = 0;

            double[] xPts = new double[n];
            double[] zPts = new double[n];

            for (int i = 0; i < n; i++)
            {
                double angle = DegToRad(currAngle);
                xPts[i] = Math.Cos(angle);
                zPts[i] = Math.Sin(angle);
                currAngle += angleInc;
            }

            using (FileStream fs = File.Open(@"c:\dump\sphere.obj", FileMode.Create))
            using (StreamWriter sr = new StreamWriter(fs))
            {
                // Top point
                sr.WriteLine("v 0 1 0");
                sr.WriteLine("vn 0 1 0");

                int cnt = 1;
                // Compute the points along the y (up) axis
                angleInc = 360.0/n;
                currAngle = angleInc + 90;
                for (int i = 0; i < n/2 - 1; i++)
                {
                    double angle = DegToRad(currAngle);
                    double radius = Math.Cos(angle);
                    double y = Math.Sin(angle);
                    for (int j = 0; j < n; j++)
                    {
                        sr.WriteLine("v {0:f5} {1:f5} {2:f5}", xPts[j]*radius, y, zPts[j]*radius);
                        sr.WriteLine("vn {0:f5} {1:f5} {2:f5}", xPts[j]*radius, y, zPts[j]*radius);
                        cnt++;
                    }
                    currAngle += angleInc;
                }

                // Bottom point
                sr.WriteLine("v 0 -1 0");
                sr.WriteLine("vn 0 -1 0");
                cnt++;

                Trace.WriteLine("Number of vertices = " + cnt);

                cnt = 0;

                // Top row
                for (int i = 2; i <= n; i++)
                {
                    sr.WriteLine("f 1//1 {0}//{0} {1}//{1}", i+1, i);
                    cnt++;
                }
                sr.WriteLine("f 1//1 2//2 {0}//{0}", n+1);
                cnt++;

                // Middle rows
                for (int c = 0; c < n/2-2; c++)
                {
                    int row = c*n;
                    // "normal" rows
                    sr.WriteLine("# row " + row);
                    for (int i = row+2; i <= row+n; i++)
                    {
                        sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", n + i, i, i + 1);
                        sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", n + i, i + 1, n + i + 1);
                        cnt++;
                    }

                    sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", row+2, row+2*n+1, row+n+1);
                    sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", row+2*n+1, row+2, row+n+2);
                    cnt++;
                }

                // Bottom row
                for (int i = n*(n/2-2)+2; i <= n*(n/2-1)+1; i++)
                {
                    sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", i+1, n*(n/2-1)+2, i);
                    cnt++;
                }

                sr.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", n*(n/2-2)+2, n*(n/2-1)+2, n*(n/2-1)+1);
                cnt++;
                Trace.WriteLine("Number of faces = " + cnt);
            }
        }

        static void Main(string[] args)
        {

            Geosphere();
            Sphere(16);

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

        static double DegToRad(double d)
        {
            return d * Math.PI/180.0;
        }
    }
}
