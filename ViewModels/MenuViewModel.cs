using Avalonia.Media;
using Avalonia_3D_STL.Helpers;
using Avalonia_3D_STL.Services;
using ReactiveUI;
using Silk.NET.Maths;
using System.Threading.Tasks;

namespace Avalonia_3D_STL.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {

        public MenuViewModel()
        {
            Drag();
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                this.RaiseAndSetIfChanged(ref _color, value);

                Vector4D<float> newColor = new Vector4D<float>((float)_color.R / 255, (float)_color.G / 255, (float)_color.B / 255, (float)_color.A / 255);
                DrawingService.SetColor(newColor);
            }
        }
        
        private string _selectedOption;
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedOption, value);
            }
        }
        
        public void Drag()
        {
            SelectedOption = "Drag";
            DrawingService.SetOption(SelectedOption);
        }

        public void Rotate()
        {
            SelectedOption = "Rotate";
            DrawingService.SetOption(SelectedOption);
        }
        
        public void Zoom()
        {
            SelectedOption = "Zoom";
            DrawingService.SetOption(SelectedOption);
        }

        public async void ZoomIn()
        {
            DrawingService.Zoom(1);
        }
        public async void ZoomOut()
        {
            DrawingService.Zoom(-1);
        }

        public async Task LoadSTL(string file)
        {
            STL_Reader.FileSTL = file;
            DrawingService.LoadNewFile();
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
