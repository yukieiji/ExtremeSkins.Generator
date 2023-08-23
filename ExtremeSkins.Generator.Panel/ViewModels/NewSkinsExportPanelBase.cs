
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
    private string hotReloadErrorKey = string.Empty;

    public NewSkinsExportPanelBase(
        ICosmicModel model,
        IWindowsDialogService windowsDialogService,
        string hotReloadErrorKey)
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
        this.hotReloadErrorKey = hotReloadErrorKey;
    }

    private void Export()
    {
        var resource = Application.Current.MainWindow.Resources;

        ICosmicModel.ExportStatus status = this.model.GetCurrentExportStatus();

        switch (status)
        {
            case ICosmicModel.ExportStatus.MissingAutherName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyAuther"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingSkinName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptySkin"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingFrontImg:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyFlontImage"],
                    });
                return;
            default:
                break;
        }

        var sameSkinStatus = this.model.GetSameSkinStatus();

        bool isOverride = false;
        switch (sameSkinStatus)
        {
            case SameSkinCheckResult.No:
                break;
            case SameSkinCheckResult.ExistExS:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotExportSameSkin"],
                    });
                return;
            case SameSkinCheckResult.ExistMyExportedSkin:
                var result = this.ShowMessageService.Show(
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

        string errorMessage = this.model.Export(isOverride);

        if (string.IsNullOrEmpty(errorMessage))
        {
            string messageKey =
                string.IsNullOrEmpty(this.model.AmongUsPathContainer.AmongUsFolderPath) ?
                "ExportSuccess" : "ExportSuccessWithInstall";

            this.ShowMessageService.Show(
                new MessageShowService.InfoMessageSetting()
                {
                    Title = (string)resource["Success"],
                    Message = (string)resource[messageKey],
                });
        }
        else
        {
            this.ShowMessageService.Show(
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

        ICosmicModel.ExportStatus status = this.model.GetCurrentExportStatus();

        switch (status)
        {
            case ICosmicModel.ExportStatus.MissingAutherName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyAuther"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingSkinName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptySkin"],
                    });
                return;
            case ICosmicModel.ExportStatus.MissingFrontImg:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyFlontImage"],
                    });
                return;
            default:
                break;
        }

        var sameSkinStatus = this.model.GetSameSkinStatus();

        bool isOverride = false;
        switch (sameSkinStatus)
        {
            case SameSkinCheckResult.No:
                break;
            case SameSkinCheckResult.ExistMyExportedSkin:
                var existResult = this.ShowMessageService.Show(
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

        bool isEnaleHat = await this.model.IsModuleEnable();

        if (!isEnaleHat)
        {
            this.ShowMessageService.Show(
               new MessageShowService.ErrorMessageSetting()
               {
                   Title = (string)resource["Error"],
                   Message = $"{(string)resource[this.hotReloadErrorKey]}",
               });
            return;
        }

        string errorMessage = this.model.Export(isOverride);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            this.ShowMessageService.Show(
               new MessageShowService.ErrorMessageSetting()
               {
                   Title = (string)resource["Error"],
                   Message = $"{(string)resource["ExportError"]}\\{errorMessage}",
               });
            return;
        }

        bool result = await this.model.HotReloadCosmic();
        if (result)
        {
            this.ShowMessageService.Show(
                new MessageShowService.InfoMessageSetting()
                {
                    Title = (string)resource["Success"],
                    Message = (string)resource["HotReloadSuccess"],
                });
        }
        else
        {
            this.ShowMessageService.Show(
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
