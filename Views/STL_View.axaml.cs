using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia_3D_STL.Interfaces;
using Avalonia_3D_STL.Services;

namespace Avalonia_3D_STL;

public partial class STL_View : UserControl
{
    private readonly IDrawingService _drawingService1;

    public STL_View()
    {
        _drawingService1 = new SimpleDrawingService();

        InitializeComponent();

        glRenderer1.OnLoad += () => { _drawingService1.Load([glRenderer1]); };
        glRenderer1.OnUpdate += _drawingService1.Update;
        glRenderer1.OnRender += _drawingService1.Render;
        KeyDown += _drawingService1.KeyReader;

        PointerPressed += _drawingService1.PressMouseButton;
        PointerMoved += _drawingService1.MoveMouseButton;
        PointerReleased += _drawingService1.ReleasedMouseButton;
        PointerWheelChanged += _drawingService1.WheelMouse;
    }
}