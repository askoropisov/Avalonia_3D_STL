using Avalonia;
using Avalonia.Input;
using Avalonia_3D_STL.Helpers;
using Avalonia_3D_STL.Models._3D;
using Avalonia_3D_STL.Models.Simple;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Avalonia_3D_STL.Services
{
    public static class DrawingService
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

        private static Renderer Renderer = null!;
        private static Camera Camera = null!;
        private static Vector4D<float> Color = new(1f, 0.57f, 0.0f, 0.5f);
        private static RenderPipeline SimplePipeline = null!;
        private static RenderPipeline SolidColorPipeline = null!;
        private static List<Figure> Meshes = new List<Figure>();
        private static Matrix4X4<float> Model = Matrix4X4<float>.Identity;

        private static Vector3D<float> cameraPosition = new Vector3D<float>(0.0f, 0.0f, 0.0f); // Начальная позиция камеры
        private static float CameraSpeed = 1f;
        private static float CurrentYaw = 0f;
        private static float CurrentPitch = 0f;

        private static Point StartPoint;
        private static bool IsRightMouseButton;
        private static bool IsMiddleMouseButton;

        public static event EventHandler ClearChanged;

        public static void Load(object[] args)
        {
            Renderer = (Renderer)args[0];

            using Models._3D.Shader vs1 = new(Renderer, ShaderType.VertexShader, File.ReadAllText("Resources/Shaders/Simple.vert"));
            using Models._3D.Shader fs1 = new(Renderer, ShaderType.FragmentShader, File.ReadAllText("Resources/Shaders/Simple.frag"));
            SimplePipeline = new RenderPipeline(Renderer, vs1, fs1);
            SolidColorPipeline = SimplePipeline;

            LoadFile();
        }

        public static void LoadFile()
        {
            STL_Reader.GetModel(out List<Vertex> vertices, out List<uint> indices);
            Meshes.Clear();
            Meshes.Add(new(Renderer, vertices, indices));
            StartPosition();
        }

        public static void LoadNewFile()
        {
            ClearChanged?.Invoke(Renderer, new EventArgs());
            LoadFile();
        }

        public static void StartPosition()
        {
            Camera = new Camera();
            CurrentPitch = 0f;
            CurrentYaw = 0f;
            CameraSpeed = 1f;
            StartPoint = new Point();
            float distance = Math.Max(STL_Reader.GetSizes().X, STL_Reader.GetSizes().Y);
            cameraPosition = STL_Reader.GetCenter() + new Vector3D<float>(0, 0, distance);
            Model = Matrix4X4.CreateFromYawPitchRoll(0f, 0f, 0f);
        }

        public static void Clear()
        {
            Meshes.Clear();
        }

        public static void SetColor(Vector4D<float> color)
        {
            Color = color;
        }

        public static void Update(double deltaSeconds)
        {
            Camera.Position = cameraPosition;
            Camera.Width = (int)Renderer.Bounds.Width;
            Camera.Height = (int)Renderer.Bounds.Height;
        }

        public static void Render(double deltaSeconds)
        {
            GL gl = Renderer.GetContext();
            gl.ClearColor(0.56f, 0.74f, 0.8f, 0.6f);
            gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

            Matrix4X4<float> m = Model * Matrix4X4.CreateTranslation(new Vector3D<float>(0.0f, 0.0f, 0.0f));

            foreach (Figure mesh in Meshes)
            {
                SolidColorPipeline.Bind();

                SolidColorPipeline.SetUniform(string.Empty, new UniTransforms()
                {
                    Model = m,
                    View = Camera.View,
                    Projection = Camera.Projection,
                    ObjectToWorld = m,
                    ObjectToClip = m * Camera.View * Camera.Projection,
                });

                SolidColorPipeline.SetUniform(string.Empty, new UniParameters()
                {
                    Color = Color
                });

                mesh.VertexAttributePointer((uint)SolidColorPipeline.GetAttribLocation("In_Position"), 3, nameof(Vertex.Position));
                mesh.VertexAttributePointer((uint)SolidColorPipeline.GetAttribLocation("In_Normal"), 3, nameof(Vertex.Normal));
                mesh.VertexAttributePointer((uint)SolidColorPipeline.GetAttribLocation("In_Color"), 4, nameof(Vertex.Color));

                mesh.Draw();

                SolidColorPipeline.Unbind();
            }
        }

        #region KeyController
        public static void KeyReader(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    cameraPosition.Y -= CameraSpeed * 10;
                    break;
                case Key.A:
                    cameraPosition.X += CameraSpeed * 10;
                    break;
                case Key.S:
                    cameraPosition.Y += CameraSpeed * 10;
                    break;
                case Key.D:
                    cameraPosition.X -= CameraSpeed * 10;
                    break;
            }
        }
        #endregion

        #region MouseController
        public static void PressMouseButton(object? sender, PointerPressedEventArgs e)
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

        public static void MoveMouseButton(object? sender, PointerEventArgs e)
        {
            var curPos = e.GetPosition(null);
            var P = curPos - StartPoint;
            var modelCenter = STL_Reader.GetCenter();

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

        public static void ReleasedMouseButton(object? sender, PointerReleasedEventArgs e)
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

        public static void WheelMouse(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y > 0) cameraPosition.Z -= CameraSpeed * 10;
            else cameraPosition.Z += CameraSpeed * 10;
        }
        #endregion
    }
}
