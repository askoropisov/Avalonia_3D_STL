using Avalonia_3D_STL.Interfaces;
using Silk.NET.OpenGLES;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Avalonia_3D_STL.Models._3D
{
    public unsafe class Figure : GraphicsResource
    {
        public Figure(IGraphicsHost<GL> graphicsHost, List<Vertex> vertices, List<uint> indices) : base(graphicsHost)
        {
            Handle = GL.GenVertexArray();
            ArrayBuffer = GL.GenBuffer();
            IndexBuffer = GL.GenBuffer();
            IndexLength = indices.Count;
            GL.BindVertexArray(Handle);
            GL.BindBuffer(GLEnum.ArrayBuffer, ArrayBuffer);
            GL.BufferData<Vertex>(GLEnum.ArrayBuffer, (uint)(vertices.Count * sizeof(Vertex)), vertices.ToArray(), GLEnum.StaticDraw);

            GL.BindBuffer(GLEnum.ElementArrayBuffer, IndexBuffer);
            GL.BufferData<uint>(GLEnum.ElementArrayBuffer, (uint)(indices.Count * sizeof(uint)), indices.ToArray(), GLEnum.StaticDraw);

            GL.BindVertexArray(0);
        }

        public uint ArrayBuffer { get; }

        public uint IndexBuffer { get; }

        public int IndexLength { get; }

        protected override void Destroy(bool disposing = false)
        {
            GL.DeleteVertexArray(Handle);
            GL.DeleteBuffer(ArrayBuffer);
            GL.DeleteBuffer(IndexBuffer);
        }

        public void VertexAttributePointer(uint index, int size, string fieldName)
        {
            GL.BindVertexArray(Handle);

            GL.BindBuffer(GLEnum.ArrayBuffer, ArrayBuffer);
            GL.VertexAttribPointer(index, size, GLEnum.Float, false, (uint)sizeof(Vertex), (void*)Marshal.OffsetOf<Vertex>(fieldName));
            GL.EnableVertexAttribArray(index);
            GL.BindBuffer(GLEnum.ArrayBuffer, 0);

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(Handle);
            GL.DrawElements(GLEnum.Triangles, (uint)IndexLength, GLEnum.UnsignedInt, null);
            GL.BindVertexArray(0);
        }
    }
}
