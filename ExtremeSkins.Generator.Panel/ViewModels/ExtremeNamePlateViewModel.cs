using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

using System.Windows;

using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class ExtremeNamePlateViewModel : SkinsExportPanelBase
{
    public DelegateCommand<string> SelectFileCommand { get; private set; }

    public string ImagePath
    {
        get { return this.imagePath; }
        set { SetProperty(ref this.imagePath, value); }
    }
    private string imagePath;

    public string LicensePath
    {
        get { return this.licensePath; }
        set { SetProperty(ref this.licensePath, value); }
    }
    private string licensePath;


    public ExtremeNamePlateViewModel(
        IEventAggregator ea,
        IDialogService dialogService,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService) : 
            base(ea, dialogService, comDlgService, windowsDialogService)
    {
        this.SelectFileCommand = new DelegateCommand<string>(SetText);
    }

    protected override void Export()
    {
        var resource = Application.Current.MainWindow.Resources;

        if (string.IsNullOrEmpty(this.AutherName))
        {
            this.showMessageService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotEmptyAuther"],
                });
            return;
        }

        if (string.IsNullOrEmpty(this.SkinName))
        {
            this.showMessageService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotEmptySkin"],
                });
            return;
        }

        if (string.IsNullOrEmpty(this.ImagePath))
        {
            this.showMessageService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotEmptyImage"],
                });
            return;
        }

        // TODO: 強制的にローマ字化
        ExtremeNamePlateExporter exporter = new ExtremeNamePlateExporter()
        {
            AmongUsPath = this.AmongUsPath,
            LicenseFile = this.licensePath,
        };

        exporter.AddImage($"{this.SkinName}.png", this.ImagePath);

        try
        {
            exporter.Export();

            this.showMessageService.Show(
                new MessageShowService.InfoMessageSetting()
                {
                    Title = (string)resource["Success"],
                    Message = (string)resource["ExportSuccess"],
                });
        }
        catch (System.Exception ex)
        {
            this.showMessageService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = $"{(string)resource["ExportError"]}\\{ex.Message}",
                });
        }
    }

    private void SetText(string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            return;
        }

        switch (type)
        {
            case "Image":
                this.ImagePath = OpenDialogAndGetText(DialogType.Png);
                break;
            case "License":
                this.LicensePath = OpenDialogAndGetText(DialogType.Text);
                break;
            default:
                break;
        }
    }
}
