using Avalonia_3D_STL.Interfaces;
using Silk.NET.OpenGLES;
using System;

namespace Avalonia_3D_STL.Models._3D
{
    public class Shader : GraphicsResource
    {
        public Shader(IGraphicsHost<GL> graphicsHost, ShaderType shaderType, string source) : base(graphicsHost)
        {
            Handle = GL.CreateShader(shaderType);

            GL.ShaderSource(Handle, source);
            GL.CompileShader(Handle);

            string error = GL.GetShaderInfoLog(Handle);

            if (!string.IsNullOrEmpty(error))
            {
                GL.DeleteShader(Handle);

                throw new Exception($"{shaderType}: {error}");
            }
        }

        protected override void Destroy(bool disposing = false)
        {
            GL.DeleteShader(Handle);
        }
    }
}
