using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

using System.Windows;

using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using System.Collections.Generic;

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

        Dictionary<string, string> replacedStr = new Dictionary<string, string>();
        string autherName = this.AutherName;
        if (TryReplaceAscii(autherName, out string asciiedAutherName))
        {
            replacedStr.Add(asciiedAutherName, autherName);
            autherName = asciiedAutherName;
        }
        string skinName = this.SkinName;
        if (TryReplaceAscii(skinName, out string asciiedSkinName))
        {
            replacedStr.Add(asciiedSkinName, skinName);
            skinName = asciiedSkinName;
        }
        ExtremeNamePlateExporter exporter = new ExtremeNamePlateExporter()
        {
            Author = autherName,
            AmongUsPath = this.AmongUsPath,
            LicenseFile = this.licensePath,
        };

        exporter.AddImage($"{skinName}.png", this.ImagePath);

        var sameSkinResult = exporter.CheckSameSkin();
        switch (sameSkinResult)
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
                        break;
                    case MessageBoxResult.No:
                        int index = 0;
                        string newSkinName;
                        do
                        {
                            newSkinName = $"{skinName}_{index}";
                            exporter.AddImage($"{newSkinName}.png", this.ImagePath);
                            index++;
                        }
                        while (exporter.CheckSameSkin() != SameSkinCheckResult.No);
                        replacedStr[newSkinName] = this.SkinName;
                        break;
                    default:
                        return;
                }
                break;
            default:
                return;
        }

        if (replacedStr.Count != 0)
        {
            var transExporter = new TranslationExporter()
            {
                Locale = (string)resource["CurLocale"],
                AmongUsPath = this.AmongUsPath,
            };
            transExporter.AddTransData(replacedStr);
            transExporter.Export();
        }

        try
        {
            exporter.Export();

            string messageKey =
                string.IsNullOrEmpty(this.AmongUsPath) ?
                "ExportSuccess" : "ExportSuccessWithInstall";

            this.showMessageService.Show(
                new MessageShowService.InfoMessageSetting()
                {
                    Title = (string)resource["Success"],
                    Message = (string)resource[messageKey],
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
