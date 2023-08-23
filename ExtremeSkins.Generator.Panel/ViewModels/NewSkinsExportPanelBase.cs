
using Prism.Commands;
using Prism.Navigation;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System.Reactive.Disposables;

using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Service;
using System.Windows;
using System.Collections.ObjectModel;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public abstract class NewSkinsExportPanelBase : BindableBase, IDestructible
{
    public ReadOnlyObservableCollection<SkinRowPanelViewModel> Rows { get; }

    public ReactivePropertySlim<string> SkinName { get; set; }
    public ReactivePropertySlim<string> AutherName { get; set; }

    public DelegateCommand ExportButtonCommand { get; set; }
    public DelegateCommand HotReloadButtonCommand { get; set; }

    protected CompositeDisposable Disposables = new CompositeDisposable();

    protected readonly ICosmicModel Model;

    private readonly IWindowsDialogService showMessageService;
    private readonly string hotReloadErrorKey = string.Empty;

    public NewSkinsExportPanelBase(
        ICosmicModel model,
        IWindowsDialogService windowsDialogService,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        string hotReloadErrorKey)
    {
        this.Model = model;

        this.Rows = model.ImgRows
            .ToReadOnlyReactiveCollection(x => new SkinRowPanelViewModel(x, comDlgService))
            .AddTo(this.Disposables);
        this.SkinName = this.Model.SkinName
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.Disposables);
        this.AutherName = this.Model.AutherName
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.Disposables);

        this.showMessageService = windowsDialogService;
        this.ExportButtonCommand = new DelegateCommand(Export);
        this.HotReloadButtonCommand = new DelegateCommand(HotReload);
        this.hotReloadErrorKey = hotReloadErrorKey;
    }

    private void Export()
    {
        var resource = Application.Current.MainWindow.Resources;

        ICosmicModel.ExportStatus status = this.Model.GetCurrentExportStatus();

        switch (status)
        {
            case ICosmicModel.ExportStatus.MissingAutherName:
                this.showMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyAuther"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingSkinName:
                this.showMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptySkin"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingFrontImg:
                this.showMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyFlontImage"],
                    });
                return;
            default:
                break;
        }

        var sameSkinStatus = this.Model.GetSameSkinStatus();

        bool isOverride = false;
        switch (sameSkinStatus)
        {
            case SameSkinCheckResult.No:
                break;
            case SameSkinCheckResult.ExistExS:
                this.showMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotExportSameSkin"],
                    });
                return;
            case SameSkinCheckResult.ExistMyExportedSkin:
                var result = this.showMessageService.Show(
                    new MessageShowService.CheckMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["IsOverrideMessage"],
                    });
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        isOverride = true;
                        break;
                    case MessageBoxResult.No:
                        isOverride = false;
                        break;
                    default:
                        return;
                }
                break;
            default:
                return;
        }

        string errorMessage = this.Model.Export(isOverride);

        if (string.IsNullOrEmpty(errorMessage))
        {
            string messageKey =
                string.IsNullOrEmpty(this.Model.AmongUsPathContainer.AmongUsFolderPath) ?
                "ExportSuccess" : "ExportSuccessWithInstall";

            this.showMessageService.Show(
                new MessageShowService.InfoMessageSetting()
                {
                    Title = (string)resource["Success"],
                    Message = (string)resource[messageKey],
                });
        }
        else
        {
            this.showMessageService.Show(
               new MessageShowService.ErrorMessageSetting()
               {
                   Title = (string)resource["Error"],
                   Message = $"{(string)resource["ExportError"]}\\{errorMessage}",
               });
        }
    }

    private async void HotReload()
    {
        var resource = Application.Current.MainWindow.Resources;

        ICosmicModel.ExportStatus status = this.Model.GetCurrentExportStatus();

        switch (status)
        {
            case ICosmicModel.ExportStatus.MissingAutherName:
                this.showMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyAuther"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingSkinName:
                this.showMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptySkin"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingFrontImg:
                this.showMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyFlontImage"],
                    });
                return;
            default:
                break;
        }

        var sameSkinStatus = this.Model.GetSameSkinStatus();

        bool isOverride = false;
        switch (sameSkinStatus)
        {
            case SameSkinCheckResult.No:
                break;
            case SameSkinCheckResult.ExistMyExportedSkin:
                var existResult = this.showMessageService.Show(
                    new MessageShowService.CheckMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["IsOverrideMessage"],
                    });
                switch (existResult)
                {
                    case MessageBoxResult.Yes:
                        isOverride = true;
                        break;
                    case MessageBoxResult.No:
                        isOverride = false;
                        break;
                    default:
                        return;
                }
                break;
            default:
                return;
        }

        bool isEnaleHat = await this.Model.IsModuleEnable();

        if (!isEnaleHat)
        {
            this.showMessageService.Show(
               new MessageShowService.ErrorMessageSetting()
               {
                   Title = (string)resource["Error"],
                   Message = $"{(string)resource[this.hotReloadErrorKey]}",
               });
            return;
        }

        string errorMessage = this.Model.Export(isOverride);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            this.showMessageService.Show(
               new MessageShowService.ErrorMessageSetting()
               {
                   Title = (string)resource["Error"],
                   Message = $"{(string)resource["ExportError"]}\\{errorMessage}",
               });
            return;
        }

        bool result = await this.Model.HotReloadCosmic();
        if (result)
        {
            this.showMessageService.Show(
                new MessageShowService.InfoMessageSetting()
                {
                    Title = (string)resource["Success"],
                    Message = (string)resource["HotReloadSuccess"],
                });
        }
        else
        {
            this.showMessageService.Show(
               new MessageShowService.ErrorMessageSetting()
               {
                   Title = (string)resource["Error"],
                   Message = $"{(string)resource["HotReloadError"]}",
               });
        }
    }

    public void Destroy()
    {
        this.Disposables.Dispose();
    }
}
