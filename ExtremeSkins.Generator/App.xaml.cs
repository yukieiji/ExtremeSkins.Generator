using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

using ExtremeSkins.Generator.Views;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Models;

using ExtremeSkins.Generator.Panel.Interfaces;
using ExtremeSkins.Generator.Panel.Models;

namespace ExtremeSkins.Generator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ICommonDialogService<FileDialogService.Result>, FileDialogService>();
        containerRegistry.RegisterSingleton<IWindowsDialogService, MessageShowService>();
        containerRegistry.RegisterSingleton<IOpenExplorerService, OpenExplorerService>();
        containerRegistry.RegisterSingleton<IAmongUsPathContainerModel, AmongUsPathContainerModel>();
        containerRegistry.RegisterSingleton<IMainWindowModel, MainWindowModel>();
    }
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<Panel.PanelModule>();
    }
}
