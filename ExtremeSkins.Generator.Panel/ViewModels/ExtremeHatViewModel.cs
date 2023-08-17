using System.Collections.ObjectModel;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Panel.Interfaces;
using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class ExtremeHatViewModel : NewSkinsExportPanelBase
{
    public ReadOnlyObservableCollection<SkinRowPanelViewModel> Rows { get; }
    private readonly IExtremeHatModel model;

    public ExtremeHatViewModel(
        IExtremeHatModel model,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService) :
            base(model, windowsDialogService)
    {
        this.model = model;
        this.Rows = model.ImgRows
            .ToReadOnlyReactiveCollection(x => new SkinRowPanelViewModel(x, comDlgService))
            .AddTo(this.Disposables);
    }

    protected override void Export()
    {
        var resource = Application.Current.MainWindow.Resources;

        IExportModel.ExportStatus status = this.model.GetCurrentExportStatus();

        switch (status)
        {
            case IExportModel.ExportStatus.MissingAutherName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyAuther"],
                    });
                return;
            case IExportModel.ExportStatus.MissingSkinName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptySkin"],
                    });
                return;
            case IExportModel.ExportStatus.MissingFrontImg:
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

    protected override async void HotReload()
    {
        var resource = Application.Current.MainWindow.Resources;

        IExportModel.ExportStatus status = this.model.GetCurrentExportStatus();

        switch (status)
        {
            case IExportModel.ExportStatus.MissingAutherName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptyAuther"],
                    });
                return;
            case IExportModel.ExportStatus.MissingSkinName:
                this.ShowMessageService.Show(
                    new MessageShowService.ErrorMessageSetting()
                    {
                        Title = (string)resource["Error"],
                        Message = (string)resource["CannotEmptySkin"],
                    });
                return;
            case IExportModel.ExportStatus.MissingFrontImg:
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

        bool isEnaleHat = await this.model.IsExHEnable();

        if (!isEnaleHat)
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
                    Message = (string)resource["ExportSuccess"],
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
}
