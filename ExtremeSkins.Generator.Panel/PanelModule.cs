using Prism.Ioc;
using Prism.Modularity;

using ExtremeSkins.Generator.Panel.Views;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;

namespace ExtremeSkins.Generator.Panel;

public sealed class PanelModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {

    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<
            ICommonDialogService<FileDialogService.Result>, FileDialogService>();
        containerRegistry.RegisterSingleton<
            IWindowsDialogService, MessageShowService>();

        containerRegistry.RegisterForNavigation<ExtremeHat>();
        containerRegistry.RegisterForNavigation<ExtremeVisor>();
        containerRegistry.RegisterForNavigation<ExtremeNamePlate>();
    }
}