using Silk.NET.Core.Native;
using System;

namespace Avalonia_3D_STL.Interfaces
{
    public delegate void DeltaAction(double deltaSeconds);

    public delegate void SizeAction(int width, int height);

    public interface IGraphicsHost<TContext> where TContext : NativeAPI
    {
        event Action? OnLoad;

        event Action? OnUnload;

        event DeltaAction? OnUpdate;

        event DeltaAction? OnRender;

        event SizeAction? OnResize;

        TContext GetContext();
    }

}
