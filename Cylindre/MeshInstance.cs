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
            get
            {
                Matrix4x4 mat = new Matrix4x4();
                mat = Transform;
                mat.Translation = Vector3.Zero;
                return Mesh.Normals.Select(norm => Vector3.Transform(norm, mat));
            }
        }

        public IEnumerable<int> VertIndices => Mesh.VertIndices;
        public IEnumerable<int> NormIndices => Mesh.NormIndices;
    }
}
