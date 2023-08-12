using System.Windows;

using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System.Reactive.Disposables;

using ExtremeSkins.Generator.Event;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public abstract class NewSkinsExportPanelBase : BindableBase, IDestructible
{
    public ReactivePropertySlim<string> SkinName { get; set; }
    public ReactivePropertySlim<string> AutherName { get; set; }

    public DelegateCommand ExportButtonCommand { get; set; }

    protected CompositeDisposable Disposables = new CompositeDisposable();

    protected readonly IWindowsDialogService showMessageService;

    private IEventAggregator ea;

    protected string AmongUsPath = string.Empty;

    public NewSkinsExportPanelBase(
        IExportModel model,
        IEventAggregator ea,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService)
    {
        this.ea = ea;

        this.ea.GetEvent<AmongUsPathSetEvent>().Subscribe(SetAmongUsPath);
        this.ea.GetEvent<AmongUsPathGetEvent>().Publish();

        this.SkinName = model.SkinName
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.Disposables);
        this.AutherName = model.AutherName
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.Disposables);

        this.showMessageService = windowsDialogService;
        this.ExportButtonCommand = new DelegateCommand(Export);
    }

    protected abstract void Export();

    public void Destroy()
    {
        this.Disposables.Dispose();
    }

    private void SetAmongUsPath(string path)
    {
        this.AmongUsPath = path;
    }

}
