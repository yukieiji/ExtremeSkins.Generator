
using Prism.Commands;
using Prism.Navigation;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System.Reactive.Disposables;

using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public abstract class NewSkinsExportPanelBase : BindableBase, IDestructible
{
    public ReactivePropertySlim<string> SkinName { get; set; }
    public ReactivePropertySlim<string> AutherName { get; set; }

    public DelegateCommand ExportButtonCommand { get; set; }

    protected CompositeDisposable Disposables = new CompositeDisposable();

    protected readonly IWindowsDialogService ShowMessageService;
    private IExportModel model;

    public NewSkinsExportPanelBase(
        IExportModel model,
        IWindowsDialogService windowsDialogService)
    {

        this.model = model;
        this.SkinName = this.model.SkinName
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.Disposables);
        this.AutherName = this.model.AutherName
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.Disposables);

        this.ShowMessageService = windowsDialogService;
        this.ExportButtonCommand = new DelegateCommand(Export);
    }

    protected abstract void Export();

    public void Destroy()
    {
        this.Disposables.Dispose();
    }
}
