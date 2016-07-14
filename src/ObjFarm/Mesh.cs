using System.Collections.Generic;
using System.Numerics;

namespace ObjFarm
{
    public partial class Mesh : IMesh
    {
        private List<Vector3> m_Vertices;
        private List<Vector3> m_Normals;
        private List<int> m_VertIndices;
        private List<int> m_NormIndices;

        public IEnumerable<Vector3> Vertices => m_Vertices;
        public IEnumerable<Vector3> Normals => m_Normals;
        public IEnumerable<int> VertIndices => m_VertIndices;
        public IEnumerable<int> NormIndices => m_NormIndices;

        public Mesh()
        {
            m_Vertices = new List<Vector3>();
            m_Normals = new List<Vector3>();
            m_VertIndices = new List<int>();
            m_NormIndices = new List<int>();
        }

        public Mesh(int verticesCnt, int indicesCnt)
        {
            m_Vertices = new List<Vector3>(verticesCnt);
            m_Normals = new List<Vector3>(verticesCnt);
            m_VertIndices = new List<int>(indicesCnt);
            m_NormIndices = new List<int>(indicesCnt);
        }
    }
}
