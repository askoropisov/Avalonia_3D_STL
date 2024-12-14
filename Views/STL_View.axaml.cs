using Avalonia.Controls;
using Avalonia_3D_STL.Services;

namespace Avalonia_3D_STL.Views;

public partial class STL_View : UserControl
{
    public STL_View()
    {
        InitializeComponent();

        glRenderer1.OnLoad += OnLoad;
        glRenderer1.OnUpdate += DrawingService.Update;
        glRenderer1.OnRender += DrawingService.Render;
        KeyDown += DrawingService.KeyReader;

        PointerPressed += DrawingService.PressMouseButton;
        PointerMoved += DrawingService.MoveMouseButton;
        PointerReleased += DrawingService.ReleasedMouseButton;
        PointerWheelChanged += DrawingService.WheelMouse;

        DrawingService.ClearChanged += DrawingService_ClearChanged;
    }

    private void OnLoad() => DrawingService.Load([glRenderer1]);


    private void DrawingService_ClearChanged(object? sender, System.EventArgs e)
    {
        if (glRenderer1.Parent is Panel panel)
        {
            var temp = glRenderer1;
            panel.Children.Remove(glRenderer1);
            panel.Children.Add(temp);
        }
    }
}