using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Cylindre
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjWriter writer = new ObjWriter();
            writer.OutputObj(Mesh.Geosphere(), @"c:\dump\geosphere.obj");
            writer.OutputObj(Mesh.Sphere(), @"c:\dump\sphere.obj");
            writer.OutputObj(Mesh.Cylinder(), @"c:\dump\cylinder.obj");
        }
    }
}
