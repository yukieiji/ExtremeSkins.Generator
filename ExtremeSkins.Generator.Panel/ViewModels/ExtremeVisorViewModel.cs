using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Panel.Interfaces;


namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class ExtremeVisorViewModel : NewSkinsExportPanelBase
{
    public ExtremeVisorViewModel(
        IExtremeVisorModel model,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService) :
            base(model, windowsDialogService, comDlgService, "ExHNotFoundError")
    {
    }
}
