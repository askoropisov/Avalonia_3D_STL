using Avalonia_3D_STL.Services;
using System.Threading.Tasks;

namespace Avalonia_3D_STL.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly DrawingService _drawingService;

        public MenuViewModel(DrawingService drawingService)
        {
            _drawingService = drawingService;
        }

        public async Task LoadSTL(string file)
        {
            _drawingService.LoadFile();
        }

        public void Clear()
        {

        }

        public void StartPosition()
        {

        }
    }
}
