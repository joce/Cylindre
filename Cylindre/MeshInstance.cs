using System.Collections.Generic;
using System.Numerics;

namespace Cylindre
{
    public class MeshInstance
    {
        public Mesh Mesh { get; set; }
        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;
        public IEnumerable<Vector3> Vertices { get; }
        public IEnumerable<Vector3> Normals { get; }
    }
}
