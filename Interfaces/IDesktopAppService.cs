using System.Threading.Tasks;

namespace Avalonia_3D_STL.Interfaces
{
    public interface IDesktopAppService
    {
        Task StartAsync();

        Task StopAsync();
    }
}
