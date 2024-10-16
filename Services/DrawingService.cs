using Avalonia;
using Avalonia.Input;
using Avalonia_3D_STL.Helpers;
using Avalonia_3D_STL.Interfaces;
using Avalonia_3D_STL.Models._3D;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System;
using System.Collections.Generic;
using System.IO;

namespace Avalonia_3D_STL.Services
{
    public class SimpleDrawingService : IDrawingService
    {
        #region Uniforms
        private struct UniTransforms
        {
            public Matrix4X4<float> Model;

            public Matrix4X4<float> View;

            public Matrix4X4<float> Projection;

            public Matrix4X4<float> ObjectToWorld;

            public Matrix4X4<float> ObjectToClip;

            public Matrix4X4<float> WorldToObject;
        }

        private struct UniParameters
        {
            public Vector4D<float> Color;
        }
        #endregion

        private Renderer renderer = null!;
        private Camera camera = null!;

        private Vector3D<float> cameraPosition = new Vector3D<float>(50.0f, 0.0f, 120.0f); // Начальная позиция камеры
        private float cameraDistance = 50.0f; // Начальное расстояние до модели
        private float cameraSpeed = 1f; // Скорость перемещения камеры

        #region Pipelines
        private RenderPipeline simplePipeline = null!;
        private RenderPipeline solidColorPipeline = null!;
        #endregion

        #region Meshes
        private List<Figure> Meshes = null!;
        #endregion

        private Matrix4X4<float> model = Matrix4X4<float>.Identity;
        private Vector4D<float> color = new(1f, 0.57f, 0.0f, 0.5f);

        public void Load(object[] args)
        {
            STL_Reader reader = new STL_Reader();
            renderer = (Renderer)args[0];
            camera = new Camera();
            camera.Position = cameraPosition;
            camera.Fov = 250f;

            using Models._3D.Shader vs1 = new(renderer, ShaderType.VertexShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\Simple.vert"));
            using Models._3D.Shader fs1 = new(renderer, ShaderType.FragmentShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\Simple.frag"));
            simplePipeline = new RenderPipeline(renderer, vs1, fs1);

            using Models._3D.Shader vs2 = new(renderer, ShaderType.VertexShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\SolidColor.vert"));
            using Models._3D.Shader fs2 = new(renderer, ShaderType.FragmentShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\SolidColor.frag"));

            solidColorPipeline = new RenderPipeline(renderer, vs2, fs2);

            reader.GetModel(out List<Vertex> vertices, out List<uint> indices);
            Meshes = [new(renderer, vertices, indices)];
        }

        public void Update(double deltaSeconds)
        {
            camera.Position = cameraPosition;

            //Обновление и вращение модели по времени deltaSec
            //model = Matrix4X4.CreateFromAxisAngle(new Vector3D<float>(0.0f, 0.0f, 0.0f), 0);

            camera.Width = (int)renderer.Bounds.Width;
            camera.Height = (int)renderer.Bounds.Height;
        }

        public void Render(double deltaSeconds)
        {
            GL gl = renderer.GetContext();

            gl.ClearColor(0.56f, 0.74f, 0.8f, 0.6f);
            gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

            {
                Matrix4X4<float> m = model * Matrix4X4.CreateTranslation(new Vector3D<float>(0.0f, 0.0f, 0.0f));

                foreach (Figure mesh in Meshes)
                {
                    solidColorPipeline.Bind();

                    solidColorPipeline.SetUniform(string.Empty, new UniTransforms()
                    {
                        Model = m,
                        View = camera.View,
                        //Projection = camera.Projection,
                        ObjectToWorld = m,
                        ObjectToClip = m * camera.View * camera.Projection,
                    });

                    solidColorPipeline.SetUniform(string.Empty, new UniParameters()
                    {
                        Color = color
                    });

                    mesh.VertexAttributePointer((uint)solidColorPipeline.GetAttribLocation("In_Position"), 3, nameof(Vertex.Position));
                    mesh.VertexAttributePointer((uint)solidColorPipeline.GetAttribLocation("In_Normal"), 3, nameof(Vertex.Normal));
                    mesh.VertexAttributePointer((uint)solidColorPipeline.GetAttribLocation("In_Color"), 4, nameof(Vertex.Color));

                    mesh.Draw();

                    solidColorPipeline.Unbind();
                }
            }
        }

        public void KeyReader(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    cameraPosition.Y -= cameraSpeed * 10;
                    break;
                case Key.A:
                    cameraPosition.X += cameraSpeed * 10;
                    break;
                case Key.S:
                    cameraPosition.Y += cameraSpeed * 10;
                    break;
                case Key.D:
                    cameraPosition.X -= cameraSpeed * 10;
                    break;
            }
        }

        private Point StartPoint { get; set; }

        private bool IsRightMouseButton { get; set; }
        private bool IsMiddleMouseButton { get; set; }

        public void PressMouseButton(object? sender, PointerPressedEventArgs e)
        {
            var button = e.GetCurrentPoint(null).Properties.PointerUpdateKind;

            switch (button)
            {
                case PointerUpdateKind.LeftButtonPressed:
                    break;
                case PointerUpdateKind.RightButtonPressed:
                    IsRightMouseButton = true;
                    break;
                case PointerUpdateKind.MiddleButtonPressed:
                    IsMiddleMouseButton = true;
                    break;
                default:
                    break;
            }

            StartPoint = e.GetPosition(null);
        }

        public void MoveMouseButton(object? sender, PointerEventArgs e)
        {

            if (IsRightMouseButton)
            {
                var curPos = e.GetCurrentPoint(null).Position;
                var P = curPos - StartPoint;
                model = Matrix4X4.CreateFromYawPitchRoll((float)P.X / 100, (float)P.Y / 100, 0f);
            }

            if (IsMiddleMouseButton)
            {
                var curPos = e.GetCurrentPoint(null).Position;
                var P = curPos - StartPoint;

                if (curPos.X > StartPoint.X)
                {
                    var pos = P.X >= 0 ? (float)P.X * cameraSpeed / 100 : -(float)P.X * cameraSpeed / 100;
                    cameraPosition.X -= pos;
                }
                else if (curPos.X < StartPoint.X)
                {
                    var pos = P.X >= 0 ? (float)P.X * cameraSpeed / 100 : -(float)P.X * cameraSpeed / 100;
                    cameraPosition.X += pos;
                }

                if (curPos.Y > StartPoint.Y)
                {
                    var pos = P.Y >= 0 ? (float)P.Y * cameraSpeed / 100 : -(float)P.Y * cameraSpeed / 100;
                    cameraPosition.Y += pos;
                }
                else if (curPos.Y < StartPoint.Y)
                {
                    var pos = P.Y >= 0 ? (float)P.Y * cameraSpeed / 100 : -(float)P.Y * cameraSpeed / 100;
                    cameraPosition.Y -= pos;
                }
            }
        }

        public void ReleasedMouseButton(object? sender, PointerReleasedEventArgs e)
        {

            var button = e.GetCurrentPoint(null).Properties.PointerUpdateKind;

            switch (button)
            {
                case PointerUpdateKind.LeftButtonReleased:
                    break;
                case PointerUpdateKind.RightButtonReleased:
                    IsRightMouseButton = false;
                    break;
                case PointerUpdateKind.MiddleButtonReleased:
                    IsMiddleMouseButton = false;
                    break;
                default:
                    break;
            }
        }

        public void WheelMouse(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y > 0) cameraPosition.Z -= cameraSpeed * 10;
            else cameraPosition.Z += cameraSpeed * 10;
        }
    }
}
