using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cylindre
{
    public static class MathHelpers
    {
        public static float DegToRad(float d)
        {
            return d * (float)Math.PI/180.0f;
        }
    }
}
