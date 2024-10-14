using Avalonia_3D_STL.Interfaces;
using Silk.NET.OpenGLES;

namespace Avalonia_3D_STL.Models._3D
{
    public abstract class GraphicsResource(IGraphicsHost<GL> graphicsHost) : Disposable
    {
        protected IGraphicsHost<GL> GraphicsHost { get; } = graphicsHost;

        protected GL GL => GraphicsHost.GetContext();

        public uint Handle { get; protected set; }
    }
}
