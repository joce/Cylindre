using System;
using System.Numerics;

namespace ObjFarm
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

            using (ObjWriter writer = new ObjWriter(@"c:\dump\tetra.obj"))
                writer.OutputObj(Mesh.Tetrahedron());

            using (ObjWriter writer = new ObjWriter(@"c:\dump\octa.obj"))
                writer.OutputObj(Mesh.Octahedron());

            using (ObjWriter writer = new ObjWriter(@"c:\dump\iso.obj"))
                writer.OutputObj(Mesh.Isocahedron());

            using (ObjWriter writer = new ObjWriter(@"c:\dump\mix.obj"))
            {
                Mesh cylinder = Mesh.Cylinder(1, 1, 20);
                Mesh sphere = Mesh.Sphere(1, 10);
                MeshInstance stem = new MeshInstance {Mesh = cylinder};
                MeshInstance ball = new MeshInstance {Mesh = sphere};
                Matrix4x4 stretch = Matrix4x4.CreateScale(new Vector3(1, 5, 1));
                stem.Transform = stretch;
                writer.OutputObj(ball);
                writer.OutputObj(stem);
                Matrix4x4 move = Matrix4x4.Identity;
                move.Translation = Vector3.UnitY * 5;
                ball.Transform = move;
                writer.OutputObj(ball);

                Matrix4x4 rot = Matrix4x4.CreateRotationZ((float)Math.PI/2);
                stem.Transform = stem.Transform * rot;
                writer.OutputObj(stem);

                ball.Transform = ball.Transform * rot;

                writer.OutputObj(ball);
            }
        }
    }
}
