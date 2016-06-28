using System.IO;

namespace Cylindre
{
    public class ObjWriter
    {
        public void OutputObj(Mesh mesh, string fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            using (StreamWriter sr = new StreamWriter(fs))
            {
                foreach (var vertex in mesh.Vertices)
                {
                    sr.WriteLine("v {0:f5} {1:f5} {2:f5}", vertex.X, vertex.Y, vertex.Z);
                }

                foreach (var normal in mesh.Normals)
                {
                    sr.WriteLine("vn {0:f5} {1:f5} {2:f5}", normal.X, normal.Y, normal.Z);
                }

                for (int i = 0; i < mesh.VertIndices.Count; i+=3)
                {
                    sr.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
                        mesh.VertIndices[i]  +1, mesh.NormIndices[i]  +1,
                        mesh.VertIndices[i+1]+1, mesh.NormIndices[i+1]+1,
                        mesh.VertIndices[i+2]+1, mesh.NormIndices[i+2]+1);
                }
            }
        }
    }
}
