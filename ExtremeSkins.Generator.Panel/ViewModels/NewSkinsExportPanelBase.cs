
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
    public DelegateCommand HotReloadButtonCommand { get; set; }

    protected CompositeDisposable Disposables = new CompositeDisposable();

    protected readonly IWindowsDialogService ShowMessageService;
    private ICosmicModel model;

    public NewSkinsExportPanelBase(
        ICosmicModel model,
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
        this.HotReloadButtonCommand = new DelegateCommand(HotReload);
    }

    protected abstract void Export();
    protected abstract void HotReload();

    public void Destroy()
    {
        this.Disposables.Dispose();
    }
}
