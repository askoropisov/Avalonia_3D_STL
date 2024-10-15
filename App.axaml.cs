using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Avalonia_3D_STL.ViewModels;
using Avalonia_3D_STL.Views;
using DryIoc;
using Microsoft.Extensions.Configuration;
using Splat.DryIoc;
using Splat;
using ReactiveUI;
using Avalonia_3D_STL.Helpers;
using Avalonia_3D_STL.Factories;
using System;
using Avalonia_3D_STL.Interfaces;
using System.Linq;

namespace Avalonia_3D_STL;

public partial class App : Application
{
    protected IContainer? Container;
    protected IConfiguration? configuration;

    public override void Initialize()
    {
        InitializeDI();
        InitializeModules();
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeDI()
    {
        var container = new Container();

        container.Register<MainWindow>(Reuse.Singleton);

        container.Register<ViewModelFactory>(Reuse.Singleton);

        //container.Register(typeof(ITimerService), typeof(TimerService), Reuse.Singleton);

        //Services
        //container.RegisterMany<ConnectionService>(Reuse.Singleton);
        //container.RegisterMany<NavigationService>(Reuse.Singleton);


        //Singleton ViewModels

        container.Register<MainViewModel>(Reuse.Singleton);
        container.Register<STL_ViewModel>(Reuse.Singleton);


        //CreateNewStep singleton ViewModels



        //ViewModels


        //Singleton models


        var resolver = new DryIocDependencyResolver(container);
        Locator.SetLocator(resolver);
        container.RegisterInstance(resolver);

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.InitializeAvalonia();

        Container = container;
        ViewModelLocator.Container = Container;
    }

    public async void InitializeModules()
    {
        var desktopServices = Container.ResolveMany<IDesktopAppService>();

        foreach (var service in desktopServices)
        {
            try
            {
                Console.WriteLine($"Start service {service.GetType().Name}");
                await service.StartAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var args = Environment.GetCommandLineArgs();
            Avalonia.Controls.WindowState state = Avalonia.Controls.WindowState.Maximized;
            if (args.FirstOrDefault(a => a.ToUpper().Contains("FULLSCREEN")) != default)
            {
                state = Avalonia.Controls.WindowState.FullScreen;
            }

            var vm = Container?.Resolve<MainViewModel>();
            desktop.MainWindow = Container.Resolve<MainWindow>();
            desktop.MainWindow.DataContext = vm;
            desktop.MainWindow.WindowState = state;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
