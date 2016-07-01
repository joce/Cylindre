using System;
using System.Collections.Generic;
using System.Numerics;

namespace Cylindre
{
    public partial class Mesh
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
        private static void Subdivide(Mesh mesh, bool removeSourceTriangles)
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

        public static Mesh Geosphere()
        {
            return Geosphere(2);
        }

        public static Mesh Geosphere(int detailLevel)
        {
            // geosphere
            // Taken from http://gamedev.stackexchange.com/a/31312/86181
            Mesh geosphere = Isocahedron();
            //Mesh geosphere = Octahedron();
            //Mesh geosphere = Tetrahedron();

            for (var i = 0; i < detailLevel; i++)
            {
                Subdivide(geosphere, true);
            }

            // normalize vertices to "inflate" the icosahedron into a sphere.
            for (var i = 0; i < geosphere.Vertices.Count; i++)
            {
                geosphere.Vertices[i] = Vector3.Normalize(geosphere.Vertices[i]);
            }

            return geosphere;
        }
    }
}
