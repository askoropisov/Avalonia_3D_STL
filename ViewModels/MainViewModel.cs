using Avalonia_3D_STL.Factories;

namespace Avalonia_3D_STL.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ViewModelFactory _viewModelFactory;

    public MainViewModel(ViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;

        STL = _viewModelFactory.Create<STL_ViewModel>();
        Menu = _viewModelFactory.Create<MenuViewModel>();
    }

    public STL_ViewModel STL { get; set; }
    public MenuViewModel Menu { get; set; }
}
