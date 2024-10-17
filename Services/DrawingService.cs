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
using System.Numerics;

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
        private STL_Reader reader = new STL_Reader();
        private RenderPipeline simplePipeline = null!;
        private RenderPipeline solidColorPipeline = null!;
        private List<Figure> Meshes = null!;
        private Matrix4X4<float> Model = Matrix4X4<float>.Identity;

        private Vector3D<float> cameraPosition = new Vector3D<float>(0.0f, 0.0f, 0.0f); // Начальная позиция камеры
        private float cameraSpeed = 1f;
        private float CurrentYaw = 0f;
        private float CurrentPitch = 0f;

        private Point StartPoint;
        private bool IsRightMouseButton;
        private bool IsMiddleMouseButton;


        public void Load(object[] args)
        {
            renderer = (Renderer)args[0];

            using Models._3D.Shader vs1 = new(renderer, ShaderType.VertexShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\Simple.vert"));
            using Models._3D.Shader fs1 = new(renderer, ShaderType.FragmentShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\Simple.frag"));
            simplePipeline = new RenderPipeline(renderer, vs1, fs1);

            using Models._3D.Shader vs2 = new(renderer, ShaderType.VertexShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\SolidColor.vert"));
            using Models._3D.Shader fs2 = new(renderer, ShaderType.FragmentShader, File.ReadAllText(@"C:\Code\3D\Avalonia_3D_STL\Assets\Shaders\SolidColor.frag"));

            solidColorPipeline = new RenderPipeline(renderer, vs2, fs2);

            reader.GetModel(out List<Vertex> vertices, out List<uint> indices);
            Meshes = [new(renderer, vertices, indices)];

            camera = new Camera();
            float distance = Math.Max(reader.GetSizes().X, reader.GetSizes().Y);
            cameraPosition = reader.GetCenter() + new Vector3D<float>(0, 0, distance);
            camera.Position = cameraPosition;
        }

        public void Update(double deltaSeconds)
        {
            camera.Position = cameraPosition;
            camera.Width = (int)renderer.Bounds.Width;
            camera.Height = (int)renderer.Bounds.Height;
        }

        public void Render(double deltaSeconds)
        {
            GL gl = renderer.GetContext();
            gl.ClearColor(0.56f, 0.74f, 0.8f, 0.6f);
            gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

            {
                Matrix4X4<float> m = Model * Matrix4X4.CreateTranslation(new Vector3D<float>(0.0f, 0.0f, 0.0f));

                foreach (Figure mesh in Meshes)
                {
                    solidColorPipeline.Bind();

                    solidColorPipeline.SetUniform(string.Empty, new UniTransforms()
                    {
                        Model = m,
                        View = camera.View,
                        Projection = camera.Projection,
                        ObjectToWorld = m,
                        ObjectToClip = m * camera.View * camera.Projection,
                    });

                    solidColorPipeline.SetUniform(string.Empty, new UniParameters()
                    {
                        Color = new(1f, 0.57f, 0.0f, 0.5f)
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

        #region MouseController
        public void PressMouseButton(object? sender, PointerPressedEventArgs e)
        {
            var button = e.GetCurrentPoint(null).Properties.PointerUpdateKind;

            switch (button)
            {
                case PointerUpdateKind.LeftButtonPressed:
                    break;
                case PointerUpdateKind.RightButtonPressed:
                    IsRightMouseButton = true;
                    StartPoint = e.GetPosition(null);
                    break;
                case PointerUpdateKind.MiddleButtonPressed:
                    IsMiddleMouseButton = true;
                    StartPoint = e.GetPosition(null);
                    break;
            }
        }

        public void MoveMouseButton(object? sender, PointerEventArgs e)
        {
            var curPos = e.GetPosition(null);
            var P = curPos - StartPoint;
            var modelCenter = reader.GetCenter();

            var distance = Vector3.Distance((Vector3)cameraPosition, (Vector3)modelCenter);
            float speedFactor = MathF.Log(distance + 1);

            if (IsRightMouseButton)
            {
                CurrentYaw += (float)P.X / 20000;
                CurrentPitch += (float)P.Y / 20000;

                //Ограничения вращения по осям (оборотам)
                //CurrentPitch = Math.Clamp(CurrentPitch, -MathF.PI, MathF.PI);
                //CurrentYaw = Math.Clamp(CurrentYaw, -MathF.PI, MathF.PI);

                var translationToOrigin = Matrix4X4.CreateTranslation(-modelCenter);
                var rotation = Matrix4X4.CreateFromYawPitchRoll(CurrentYaw, -CurrentPitch, 0f);
                var translationBack = Matrix4X4.CreateTranslation(modelCenter);

                Model = translationToOrigin * rotation * translationBack;
            }

            if (IsMiddleMouseButton)
            {
                cameraPosition.X -= (float)P.X * speedFactor / 12.5f;
                cameraPosition.Y += (float)P.Y * speedFactor / 12.5f;
                StartPoint = curPos;
            }
        }

        public void ReleasedMouseButton(object? sender, PointerReleasedEventArgs e)
        {
            switch (e.GetCurrentPoint(null).Properties.PointerUpdateKind)
            {
                case PointerUpdateKind.LeftButtonReleased:
                    break;
                case PointerUpdateKind.RightButtonReleased:
                    IsRightMouseButton = false;
                    break;
                case PointerUpdateKind.MiddleButtonReleased:
                    IsMiddleMouseButton = false;
                    break;
            }
        }

        public void WheelMouse(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y > 0) cameraPosition.Z -= cameraSpeed * 10;
            else cameraPosition.Z += cameraSpeed * 10;
        }
        #endregion
    }
}
