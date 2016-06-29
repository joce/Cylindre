using System.Numerics;

namespace Cylindre
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ObjWriter writer = new ObjWriter(@"c:\dump\geosphere.obj"))
                writer.OutputObj(Mesh.Geosphere());
            using (ObjWriter writer = new ObjWriter(@"c:\dump\sphere.obj"))
                writer.OutputObj(Mesh.Sphere());
            using (ObjWriter writer = new ObjWriter(@"c:\dump\cylinder.obj"))
                writer.OutputObj(Mesh.Cylinder());

            using (ObjWriter writer = new ObjWriter(@"c:\dump\mix.obj"))
            {
                writer.OutputObj(Mesh.Cylinder(30));
                writer.OutputObj(Mesh.Sphere());
            }
        }
    }
}
