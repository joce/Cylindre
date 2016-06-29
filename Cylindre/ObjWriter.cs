using System;
using System.IO;
using System.Linq;

namespace Cylindre
{
    public class ObjWriter :IDisposable
    {
        private int m_VertOffset = 1;
        private int m_NormOffset = 1;

        private readonly FileStream m_FileStream;
        private readonly StreamWriter m_StreamWriter;

        public ObjWriter(string fileName)
        {
            m_FileStream = File.Open(fileName, FileMode.Create);
            m_StreamWriter = new StreamWriter(m_FileStream);
        }

        public void OutputObj(Mesh mesh)
        {
            foreach (var vertex in mesh.Vertices)
            {
                m_StreamWriter.WriteLine("v {0:f5} {1:f5} {2:f5}", vertex.X, vertex.Y, vertex.Z);
            }

            foreach (var normal in mesh.Normals)
            {
                m_StreamWriter.WriteLine("vn {0:f5} {1:f5} {2:f5}", normal.X, normal.Y, normal.Z);
            }

            for (int i = 0; i < mesh.VertIndices.Count; i+=3)
            {
                m_StreamWriter.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
                    mesh.VertIndices[i]  + m_VertOffset, mesh.NormIndices[i]  + m_NormOffset,
                    mesh.VertIndices[i+1]+ m_VertOffset, mesh.NormIndices[i+1]+ m_NormOffset,
                    mesh.VertIndices[i+2]+ m_VertOffset, mesh.NormIndices[i+2]+ m_NormOffset);
            }

            m_VertOffset += mesh.Vertices.Count;
            m_NormOffset += mesh.Normals.Count;
        }

        public void Dispose()
        {
            m_StreamWriter.Dispose();
            m_FileStream.Dispose();
        }
    }
}
