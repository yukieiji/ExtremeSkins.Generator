using Prism.Events;

using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class ExtremeHatViewModel : NewSkinsExportPanelBase
{
    public ReadOnlyObservableCollection<SkinRowPanelViewModel> Rows { get; }
    private readonly IExtremeHatModel model;

    public ExtremeHatViewModel(
        IExtremeHatModel model,
        IEventAggregator ea,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService) :
            base(model, ea, comDlgService, windowsDialogService)
    {
        this.model = model;
        this.Rows = model.ImgRows
            .ToReadOnlyReactiveCollection(x => new SkinRowPanelViewModel(x, comDlgService))
            .AddTo(this.Disposables);
    }

    protected override void Export()
    {
        throw new System.NotImplementedException();
    }
}
