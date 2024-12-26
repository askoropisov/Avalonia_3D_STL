using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering.Composition;
using Avalonia.Utilities;
using Avalonia_3D_STL.Services;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Avalonia_3D_STL.Views;

public partial class STL_View : UserControl
{
    private bool _isInit;
    private double _currentScale;
    private double _lastScale = 1;

    public STL_View()
    {
        InitializeComponent();
        AddHandler(PointerPressedEvent, DrawingService.PressMouseButton, handledEventsToo: true);
        AddHandler(PointerReleasedEvent, DrawingService.ReleasedMouseButton, handledEventsToo: true);

        glRenderer1.OnLoad += OnLoad;
        glRenderer1.OnUpdate += DrawingService.Update;
        glRenderer1.OnRender += DrawingService.Render;
        KeyDown += DrawingService.KeyReader;

        //PointerPressed += DrawingService.PressMouseButton;
        PointerMoved += DrawingService.MoveMouseButton;
        //PointerReleased += DrawingService.ReleasedMouseButton;
        PointerWheelChanged += DrawingService.WheelMouse;

        DrawingService.ClearChanged += DrawingService_ClearChanged;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (_isInit)
        {
            return;
        }

        _isInit = true;

        var image = this.Get<Image>("PinchImage");
        SetPinchHandlers(image);
    }

    private void SetPinchHandlers(Control? control)
    {
        if (control == null)
        {
            return;
        }

        _currentScale = 1;
        Vector3D currentOffset = default;

        CompositionVisual? compositionVisual = null;

        void InitComposition(Control visual)
        {
            if (compositionVisual != null)
            {
                return;
            }

            compositionVisual = ElementComposition.GetElementVisual(visual);
        }

        control.LayoutUpdated += (s, e) =>
        {
            InitComposition(control!);
            if (compositionVisual != null)
            {
                compositionVisual.Scale = new(_currentScale, _currentScale, 1);

                if (currentOffset == default)
                {
                    currentOffset = compositionVisual.Offset;
                }
            }
        };

        control.AddHandler(Gestures.PinchEvent, (s, e) =>
        {
            InitComposition(control!);

            if (compositionVisual != null)
            {
                var scale = _currentScale * (float)e.Scale;

                if (scale <= 1 || scale < _lastScale)
                {
                    scale = 1;
                    compositionVisual.Offset = default;
                }

                compositionVisual.Scale = new(scale, scale, 1);

                e.Handled = true;
            }
        });

        control.AddHandler(Gestures.PinchEndedEvent, (s, e) =>
        {
            InitComposition(control!);

            if (compositionVisual != null)
            {
                _currentScale = compositionVisual.Scale.X;
                if (DrawingService.Option == "Zoom")
                {
                    if (_currentScale < _lastScale | _currentScale == 1)
                        DrawingService.Zoom(-1);
                    else
                        DrawingService.Zoom(1);

                    _lastScale = _currentScale;
                }
            }
        });
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
