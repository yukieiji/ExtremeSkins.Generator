using Prism.Ioc;
using Prism.Modularity;

using ExtremeSkins.Generator.Panel.Views;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Panel.Interfaces;
using ExtremeSkins.Generator.Panel.ViewModels;
using ExtremeSkins.Generator.Panel.Models;

namespace ExtremeSkins.Generator.Panel;

public sealed class PanelModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {

    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry
            .RegisterSingleton<IExtremeHatModel  , ExtremeHatModel>()
            .RegisterSingleton<IExtremeVisorModel, ExtremeVisorModel>()
            .RegisterSingleton<ICommonDialogService<FileDialogService.Result>, FileDialogService>()
            .RegisterSingleton<IWindowsDialogService, MessageShowService>()
            .RegisterSingleton<IApiServerModel, ApiServerModel>();

        containerRegistry.RegisterForNavigation<ExtremeHat>();
        containerRegistry.RegisterForNavigation<ExtremeVisor>();
        containerRegistry.RegisterForNavigation<ExtremeNamePlate>();
    }
}