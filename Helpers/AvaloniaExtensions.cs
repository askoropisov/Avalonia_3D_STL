using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;

namespace Avalonia_3D_STL.Helpers
{
    public static class AvaloniaExtensions
    {
        public static void InitializeAvalonia(this IMutableDependencyResolver resolver)
        {
            resolver.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            resolver.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
        }
    }
}
