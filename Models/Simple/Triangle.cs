using Silk.NET.Maths;

namespace Avalonia_3D_STL.Models.Simple
{
    public class Triangle
    {
        public Vector3D<float>[] Position = new Vector3D<float>[3];
        public Vector3D<float> Normal = new Vector3D<float>();
    }
}
