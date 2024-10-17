using Avalonia_3D_STL.Helpers;
using Avalonia_3D_STL.Services;
using System.Threading.Tasks;

namespace Avalonia_3D_STL.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {

        public MenuViewModel()
        {

        }

        public async Task LoadSTL(string file)
        {
            STL_Reader.FileSTL = file;
            DrawingService.LoadFile();
        }

        public void Clear()
        {
            DrawingService.Clear();
        }

        public void StartPosition()
        {
            DrawingService.StartPosition();
        }
    }
}
