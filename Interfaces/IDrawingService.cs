using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_3D_STL.Interfaces
{
    public interface IDrawingService
    {
        void Load(object[] args);

        void Update(double deltaSeconds);

        void Render(double deltaSeconds);
        void KeyReader(object? sender, KeyEventArgs e);
        void PressMouseButton(object? sender, PointerPressedEventArgs e);
        void MoveMouseButton(object? sender, PointerEventArgs e);
        void ReleasedMouseButton(object? sender, PointerReleasedEventArgs e);
        void WheelMouse(object? sender, PointerWheelEventArgs e);
    }
}
