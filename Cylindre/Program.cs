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
                Mesh cylinder = Mesh.Cylinder(20);
                Mesh sphere = Mesh.Sphere(10);
                MeshInstance stem = new MeshInstance() {Mesh = cylinder};
                MeshInstance ball = new MeshInstance() {Mesh = sphere};
                Matrix4x4 stretch = Matrix4x4.CreateScale(new Vector3(1, 5, 1));
                stem.Transform = stretch;
                writer.OutputObj(ball);
                writer.OutputObj(stem);
                Matrix4x4 move = Matrix4x4.Identity;
                move.Translation = Vector3.UnitY * 5;
                ball.Transform = move;
                writer.OutputObj(ball);
            }
        }
    }
}
