using Avalonia_3D_STL.Models._3D;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_3D_STL.Helpers
{
    public static unsafe class STL_Reader
    {
        public static void GetModel(out List<Vertex> vertices, out List<uint> indices, float size = 0.5f)
        {

            var parser = new StlBinaryParser();
            //var triangles = parser.Parse("C:\\Code\\3D\\OpenGLTest\\Assets\\Test.STL");
            var triangles = parser.Parse(Path.Combine(AppContext.BaseDirectory, "Assets", "Test.STL"));
            vertices = new List<Vertex>();

            foreach (var triangle in triangles)
            {

                //MinY = Math.Min(MinY, triangle.Position[0].Y);
                //MaxY = Math.Max(MaxY, triangle.Position[0].Y);

                for (int i = 0; i < 3; i++)
                {
                    var vert = new Vertex();
                    vert.Position = triangle.Position[i];
                    vert.Normal = triangle.Normal;
                    vertices.Add(vert);
                }
            }

            indices = vertices.Select((a, b) => (uint)b).ToList();
        }

        public static void GetCanvas(out List<Vertex> vertices, out List<uint> indices)
        {
            vertices =
            [
                new(new(-1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
                new(new(-1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(0.0f, 0.0f)),
                new(new(1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
                new(new(1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
                new(new(1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(1.0f, 1.0f)),
                new(new(-1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f))
            ];

            indices = vertices.Select((a, b) => (uint)b).ToList();
        }

        public class StlBinaryParser
        {
            public List<Triangle> Parse(string filePath)
            {
                var triangles = new List<Triangle>();

                using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    // Пропускаем заголовок (80 байт)
                    reader.ReadBytes(80);

                    // Читаем количество треугольников
                    uint triangleCount = reader.ReadUInt32();

                    for (uint i = 0; i < triangleCount; i++)
                    {
                        var triangle = new Triangle();

                        // Читаем нормаль
                        triangle.Normal.X = reader.ReadSingle();
                        triangle.Normal.Y = reader.ReadSingle();
                        triangle.Normal.Z = reader.ReadSingle();

                        // Читаем вершины
                        for (int j = 0; j < 3; j++)
                        {
                            triangle.Position[j].X = reader.ReadSingle();
                            triangle.Position[j].Y = reader.ReadSingle();
                            triangle.Position[j].Z = reader.ReadSingle();
                        }

                        // Пропускаем атрибуты (2 байта)
                        reader.ReadUInt16();

                        triangles.Add(triangle);
                    }
                }

                return triangles;
            }
        }

        public class Triangle
        {
            public Vector3D<float>[] Position = new Vector3D<float>[3];
            public Vector3D<float> Normal = new Vector3D<float>();
        }
    }
}
