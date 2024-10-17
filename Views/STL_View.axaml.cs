using Avalonia.Controls;
using Avalonia_3D_STL.Services;

namespace Avalonia_3D_STL.Views;

public partial class STL_View : UserControl
{
    public STL_View()
    {
        InitializeComponent();

        glRenderer1.OnLoad += () => { DrawingService.Load([glRenderer1]); };
        glRenderer1.OnUpdate += DrawingService.Update;
        glRenderer1.OnRender += DrawingService.Render;
        KeyDown += DrawingService.KeyReader;

        PointerPressed += DrawingService.PressMouseButton;
        PointerMoved += DrawingService.MoveMouseButton;
        PointerReleased += DrawingService.ReleasedMouseButton;
        PointerWheelChanged += DrawingService.WheelMouse;
    }
}