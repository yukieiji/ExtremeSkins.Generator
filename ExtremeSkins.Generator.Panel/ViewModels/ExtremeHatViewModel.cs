using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

using System.Collections.Generic;
using System.Windows;
using System.Reactive.Disposables;

using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class ExtremeHatViewModel : SkinsExportPanelBase
{
    public DelegateCommand<string> SelectFileCommand { get; private set; }

    public string FrontImagePath
    {
        get { return this.frontImagePath; }
        set { SetProperty(ref this.frontImagePath, value); }
    }
    private string frontImagePath;

    public string FrontFlipImagePath
    {
        get { return this.frontFlipImagePath; }
        set { SetProperty(ref this.frontFlipImagePath, value); }
    }
    private string frontFlipImagePath;

    public string BackImagePath
    {
        get { return this.backImagePath; }
        set { SetProperty(ref this.backImagePath, value); }
    }
    private string backImagePath;

    public string BackFlipImagePath
    {
        get { return this.backFlipImagePath; }
        set { SetProperty(ref this.backFlipImagePath, value); }
    }
    private string backFlipImagePath;

    public string ClimbImagePath
    {
        get { return this.climbImagePath; }
        set { SetProperty(ref this.climbImagePath, value); }
    }
    private string climbImagePath;

    public string LicensePath
    {
        get { return this.licensePath; }
        set { SetProperty(ref this.licensePath, value); }
    }
    private string licensePath;

    public bool IsBounce
    {
        get { return this.isBounce; }
        set { SetProperty(ref this.isBounce, value); }
    }
    private bool isBounce = false;

    public bool IsShader
    {
        get { return this.isShader; }
        set { SetProperty(ref this.isShader, value); }
    }
    private bool isShader = false;

    public ReadOnlyObservableCollection<SkinRowPanelViewModel> Rows { get; }
    private readonly IExtremeHatModel model;
    private CompositeDisposable disposables = new CompositeDisposable();

    public ExtremeHatViewModel(
        IExtremeHatModel model,
        IEventAggregator ea,
        IDialogService dialogService,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService) :
            base(ea, dialogService, comDlgService, windowsDialogService)
    {
        this.model = model;
        this.Rows = model.ImgRows
            .ToReadOnlyReactiveCollection(x => new SkinRowPanelViewModel(x, comDlgService))
            .AddTo(this.disposables);
        this.ExportButtonCommand = new DelegateCommand(() => this.model.Export());
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

        if (string.IsNullOrEmpty(this.FrontImagePath))
        {
            this.showMessageService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotEmptyFlontImage"],
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

        HatInfo hatInfo = new HatInfo(
            Name: skinName,
            Author: autherName,
            Bound: this.IsBounce,
            Shader: this.IsShader,
            Climb: !string.IsNullOrEmpty(this.ClimbImagePath),
            FrontFlip: !string.IsNullOrEmpty(this.FrontFlipImagePath),
            Back: !string.IsNullOrEmpty(this.BackImagePath),
            BackFlip: !string.IsNullOrEmpty(this.BackFlipImagePath)
        );

        ExtremeHatsExporter exporter = new ExtremeHatsExporter()
        {
            Info = hatInfo,
            AmongUsPath = this.AmongUsPath,
            LicenseFile = this.licensePath,
        };

        var sameSkinResult = exporter.CheckSameSkin();
        bool isOverride = false;
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

        exporter.AddImage(
            DataStructure.FrontImageName, this.FrontImagePath);
        if (hatInfo.FrontFlip)
        {
            exporter.AddImage(
                DataStructure.FrontFlipImageName, this.FrontFlipImagePath);
        }
        if (hatInfo.Back)
        {
            exporter.AddImage(
                DataStructure.BackImageName, this.BackImagePath);
        }
        if (hatInfo.BackFlip)
        {
            exporter.AddImage(
                DataStructure.BackFlipImageName, this.BackFlipImagePath);
        }
        if (hatInfo.Climb)
        {
            exporter.AddImage(
                DataStructure.ClimbImageName, this.ClimbImagePath);
        }

        try
        {
            exporter.Export(isOverride);
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
            case "Front":
                this.FrontImagePath = OpenDialogAndGetText(DialogType.Png);
                break;
            case "FrontFlip":
                this.FrontFlipImagePath = OpenDialogAndGetText(DialogType.Png);
                break;
            case "Back":
                this.BackImagePath = OpenDialogAndGetText(DialogType.Png);
                break;
            case "BackFlip":
                this.BackFlipImagePath = OpenDialogAndGetText(DialogType.Png);
                break;
            case "Climb":
                this.ClimbImagePath = OpenDialogAndGetText(DialogType.Png);
                break;
            case "License":
                this.LicensePath = OpenDialogAndGetText(DialogType.Text);
                break;
            default:
                break;
        }
    }
}
