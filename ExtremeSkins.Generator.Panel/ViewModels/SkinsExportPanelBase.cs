﻿using System.IO;
using System.Windows;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using AnyAscii;

using ExtremeSkins.Generator.Event;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Service.Interface;


namespace ExtremeSkins.Generator.Panel.ViewModels;

public abstract class SkinsExportPanelBase : BindableBase
{
    public enum DialogType
    {
        Png,
        Text,
    }

    public string SkinName
    {
        get { return this.skinName; }
        set { SetProperty(ref this.skinName, value); }
    }
    private string skinName = string.Empty;

    public string AutherName
    {
        get { return this.autherName; }
        set { SetProperty(ref this.autherName, value); }
    }
    private string autherName = string.Empty;

    public DelegateCommand ExportButtonCommand { get; set; }
    protected readonly IWindowsDialogService showMessageService;

    private IEventAggregator ea;
    private readonly ICommonDialogService<FileDialogService.Result> fileDialogService;

    protected string AmongUsPath = string.Empty;

    public SkinsExportPanelBase(
        IEventAggregator ea,
        IDialogService dialogService,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowsDialogService)
    {
        this.ea = ea;

        this.ea.GetEvent<AmongUsPathSetEvent>().Subscribe(SetAmongUsPath);
        this.ea.GetEvent<AmongUsPathGetEvent>().Publish();

        this.showMessageService = windowsDialogService;
        this.fileDialogService = comDlgService;
        this.ExportButtonCommand = new DelegateCommand(Export);
    }

    protected abstract void Export();

    protected string OpenDialogAndGetText(DialogType type)
    {

        var resource = Application.Current.MainWindow.Resources;

        string fileFilter = (string)resource[$"{type}Filter"];
        var settings = new FileDialogService.Setting
        {
            Filter = string.Format(type switch
            {
                DialogType.Png => "{0}(*.png)|*.png",
                DialogType.Text => "{0}(*.txt;*.md)|*.txt;*.md",
                _ => "Unknown",
            }, fileFilter),
            Title = (string)resource[$"{type}Title"],
        };

        var result = this.fileDialogService.ShowDialog(settings);

        return result.FileName;
    }

    private void SetAmongUsPath(string path)
    {
        this.AmongUsPath = path;
    }

    protected static bool TryReplaceAscii(string checkStr, out string replacedStr)
    {
        replacedStr = checkStr;
        bool isAscii = checkStr.IsAscii();
        if (!isAscii)
        {
            replacedStr = checkStr.Transliterate();
        }

        bool isReplace = false;
        char[] invalidch = Path.GetInvalidFileNameChars();
        foreach (char c in invalidch)
        {
            isReplace = TryReplecString(ref replacedStr, c) || isReplace;
        }
        isReplace = TryReplecString(ref replacedStr, '.') || isReplace;
        isReplace = TryReplecString(ref replacedStr, ' ') || isReplace;
        isReplace = TryReplecString(ref replacedStr, '}') || isReplace;
        isReplace = TryReplecString(ref replacedStr, ']') || isReplace;
        isReplace = TryReplecString(ref replacedStr, ')') || isReplace;
        isReplace = TryReplecString(ref replacedStr, '{', '_') || isReplace;
        isReplace = TryReplecString(ref replacedStr, '[', '_') || isReplace;
        isReplace = TryReplecString(ref replacedStr, '(', '_') || isReplace;

        return !isAscii || isReplace;
    }

    private static bool TryReplecString(ref string replacedStr, char c)
    {
        bool isReplace = false;
        if (replacedStr.Contains(c))
        {
            isReplace = true;
            replacedStr = replacedStr.Trim(c);
        }

        return isReplace;
    }
    private static bool TryReplecString(ref string replacedStr, char c, char targetC)
    {
        bool isReplace = false;
        if (replacedStr.Contains(c))
        {
            isReplace = true;
            replacedStr = replacedStr.Replace(c, targetC);
        }

        return isReplace;
    }
}
