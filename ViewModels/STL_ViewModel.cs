using Avalonia_3D_STL.Services;
using ReactiveUI;

namespace Avalonia_3D_STL.ViewModels
{
    public class STL_ViewModel : ViewModelBase
    {
        public STL_ViewModel()
        {
            DrawingService.ZoomPanelChanged += OnZoomPanelChanged;
        }

        private void OnZoomPanelChanged(object? sender, bool e)
        {
            ZoomVisible = e;
        }

        private bool _zoomVisible = false;
        public bool ZoomVisible
        {
            get => _zoomVisible;
            set => this.RaiseAndSetIfChanged(ref _zoomVisible, value);
        }
    }
}
