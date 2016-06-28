using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Cylindre
{
    public class MeshInstance
    {
        public Mesh Mesh { get; set; }
        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;

        public IEnumerable<Vector3> Vertices
        {
            get { return Mesh.Vertices.Select(vert => Vector3.Transform(vert, Transform)); }
        }

        public IEnumerable<Vector3> Normals
        {
            get { return Mesh.Normals.Select(norm => Vector3.Transform(norm, Transform)); }
        }
    }
}
