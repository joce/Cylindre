using System.Numerics;

namespace Cylindre
{
    public partial class Mesh
    {
        public static Mesh Tetrahedron()
        {
            Mesh mesh = new Mesh(4, 4*3);

            mesh.Vertices.AddRange(
                new[]
                {
                    new Vector3(1, 1, 1),
                    new Vector3(1, -1, -1),
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, -1, 1),
                }
            );

            mesh.VertIndices.AddRange(
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
