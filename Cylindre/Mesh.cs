using System.Collections.Generic;
using System.Numerics;

namespace Cylindre
{
    public partial class Mesh
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
}
