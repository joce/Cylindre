using System.Collections.Generic;
using System.Numerics;

namespace Cylindre
{
    public interface IMesh
    {
        IEnumerable<Vector3> Vertices { get; }
        IEnumerable<Vector3> Normals { get; }
        IEnumerable<int> VertIndices { get; }
        IEnumerable<int> NormIndices { get; }
    }
}
