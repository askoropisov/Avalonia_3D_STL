using Avalonia_3D_STL.Models.Simple;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Avalonia_3D_STL.Helpers
{
    public static unsafe class STL_Reader
    {
        private static float MaxX { get; set; } = 0;
        private static float MaxY { get; set; } = 0;
        private static float MaxZ { get; set; } = 0;
        private static float MinX { get; set; } = 99999999;
        private static float MinY { get; set; } = 99999999;
        private static float MinZ { get; set; } = 99999999;

        public static string? FileSTL { get; set; } = string.Empty;

        public static void GetModel(out List<Vertex> vertices, out List<uint> indices, float size = 0.5f)
        {
            if (string.IsNullOrEmpty(FileSTL)) FileSTL = "Resources/Test2.STL";

            var parser = new StlBinaryParser();
            var triangles = parser.Parse(FileSTL);
            vertices = new List<Vertex>();

            foreach (var triangle in triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    var vert = new Vertex();
                    vert.Position = triangle.Position[i];
                    vert.Normal = triangle.Normal;
                    vertices.Add(vert);

                    //Плохая реализация, нужно исправить
                    MaxX = Math.Max(MaxX, triangle.Position[i].X);
                    MaxY = Math.Max(MaxY, triangle.Position[i].Y);
                    MaxZ = Math.Max(MaxZ, triangle.Position[i].Z);
                    MinX = Math.Min(MinX, triangle.Position[i].X);
                    MinY = Math.Min(MinY, triangle.Position[i].Y);
                    MinZ = Math.Min(MinZ, triangle.Position[i].Z);
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

        public static Vector3D<float> GetCenter()
        {
            Vector3D<float> center = new Vector3D<float>();

            center.X = (MaxX + MinX) / 2;
            center.Y = (MaxY + MinY) / 2;
            center.Z = (MaxZ + MinZ) / 2;

            return center;
        }

        public static Vector3D<float> GetSizes()
        {
            Vector3D<float> sizes = new Vector3D<float>();

            sizes.X = (MaxX - MinX);
            sizes.Y = (MaxY - MinY);
            sizes.Z = (MaxZ - MinZ);

            return sizes;
        }

        public class StlBinaryParser
        {
            //Добавить проверку на существование файла
            public List<Triangle> Parse(string filePath)
            {
                if (!File.Exists(filePath))
                {
                    throw new Exception("File not existed");
                }

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

                        // Пропускаем атрибуты (2 байта) - данные о цвете треугольника
                        reader.ReadUInt16();

                        triangles.Add(triangle);
                    }
                }

                return triangles;
            }
        }
    }
}
