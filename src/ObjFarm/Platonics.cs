using System.Numerics;

namespace ObjFarm
{
    public partial class Mesh
    {
        public static Mesh Tetrahedron()
        {
            return Tetrahedron(1);
        }

        public static Mesh Tetrahedron(float size)
        {
            Mesh mesh = new Mesh(4, 4*3);

            float dist = 0.577350269189626f * size;

            mesh.m_Vertices.AddRange(
                new[]
                {
                    new Vector3(dist, dist, dist),
                    new Vector3(dist, -dist, -dist),
                    new Vector3(-dist, dist, -dist),
                    new Vector3(-dist, -dist, dist),
                }
            );

            mesh.m_VertIndices.AddRange(
                new[]
                {
                    0, 1, 2,
                    0, 3, 1,
                    0, 2, 3,
                    1, 3, 2,
                }
            );

            return mesh;
        }

        public static Mesh Octahedron()
        {
            return Octahedron(1);
        }

        public static Mesh Octahedron(float size)
        {
            Mesh mesh = new Mesh(6, 8*3);

            float dist = 0.707106781186548f * size;

            mesh.m_Vertices.AddRange(
                new[]
                {
                    new Vector3(0, size, 0),
                    new Vector3(dist, 0, -dist),
                    new Vector3(-dist, 0, -dist),
                    new Vector3(-dist, 0, dist),
                    new Vector3(dist, 0, dist),
                    new Vector3(0, -size, 0),
                }
            );

            mesh.m_VertIndices.AddRange(
                new[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 4,
                    0, 4, 1,
                    1, 5, 2,
                    2, 5, 3,
                    3, 5, 4,
                    4, 5, 1
                }
            );

            return mesh;
        }

        public static Mesh Isocahedron()
        {
            return Isocahedron(1);
        }

        public static Mesh Isocahedron(float size)
        {
            Mesh mesh = new Mesh(12, 20*3);

            float x = 0.525731112119133606f * size;
            float z = 0.850650808352039932f * size;

            mesh.m_Vertices.AddRange(
                new[]
                {
                    new Vector3(-x, 0f, z),
                    new Vector3(x, 0f, z),
                    new Vector3(-x, 0f, -z),
                    new Vector3(x, 0f, -z),
                    new Vector3(0f, z, x),
                    new Vector3(0f, z, -x),
                    new Vector3(0f, -z, x),
                    new Vector3(0f, -z, -x),
                    new Vector3(z, x, 0f),
                    new Vector3(-z, x, 0f),
                    new Vector3(z, -x, 0f),
                    new Vector3(-z, -x, 0f)
                }
            );

            mesh.m_VertIndices.AddRange(
                new[]
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
    }
}
