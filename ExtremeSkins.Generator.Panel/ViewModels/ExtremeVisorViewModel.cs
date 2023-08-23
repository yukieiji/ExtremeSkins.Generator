using System.Collections.ObjectModel;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Panel.Interfaces;


namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class ExtremeVisorViewModel : NewSkinsExportPanelBase
{
    public ReadOnlyObservableCollection<SkinRowPanelViewModel> Rows { get; }
    private readonly IExtremeVisorModel model;

    public ExtremeVisorViewModel(
        IExtremeVisorModel model,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService) :
            base(model, windowsDialogService, "ExHNotFoundError")
    {
        this.model = model;
        this.Rows = this.model.Rows
            .ToReadOnlyReactiveCollection(x => new SkinRowPanelViewModel(x, comDlgService))
            .AddTo(this.Disposables);

    }
}
