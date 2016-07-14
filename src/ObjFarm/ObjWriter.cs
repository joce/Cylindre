using System;
using System.IO;
using System.Linq;

namespace ObjFarm
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

        public void OutputObj(IMesh mesh)
        {
            foreach (var vertex in mesh.Vertices)
            {
                m_StreamWriter.WriteLine("v {0:f5} {1:f5} {2:f5}", vertex.X, vertex.Y, vertex.Z);
            }

            foreach (var normal in mesh.Normals)
            {
                m_StreamWriter.WriteLine("vn {0:f5} {1:f5} {2:f5}", normal.X, normal.Y, normal.Z);
            }

            bool hasNormals = mesh.NormIndices.Any();

            using (var vertEnum = mesh.VertIndices.GetEnumerator())
            using (var normEnum = mesh.NormIndices.GetEnumerator())
            {
                int idx = 0;
                while (vertEnum.MoveNext())
                {
                    normEnum.MoveNext();
                    switch (idx % 3)
                    {
                        case 0:
                            m_StreamWriter.Write("f {0}{1} ", vertEnum.Current + m_VertOffset, hasNormals ? "//" + normEnum.Current + m_NormOffset : "");
                            break;
                        case 1:
                            m_StreamWriter.Write("{0}{1} ", vertEnum.Current + m_VertOffset, hasNormals ? "//" + normEnum.Current + m_NormOffset : "");
                            break;
                        case 2:
                            m_StreamWriter.WriteLine("{0}{1}", vertEnum.Current + m_VertOffset, hasNormals ? "//" + normEnum.Current + m_NormOffset : "");
                            break;
                    }

                    idx++;
                }
            }

            m_VertOffset += mesh.Vertices.Count();
            m_NormOffset += mesh.Normals.Count();
        }

        public void Dispose()
        {
            m_StreamWriter.Dispose();
            m_FileStream.Dispose();
        }
    }
}
