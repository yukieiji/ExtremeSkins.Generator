using Prism.Mvvm;

using Reactive.Bindings;

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;

using static ExtremeSkins.Generator.Panel.Interfaces.IExportModel;


namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeHatModel : BindableBase, IExtremeHatModel
{
    public IAmongUsPathContainerModel AmongUsPathContainer { get; }
    public ReactivePropertySlim<string> SkinName { get; }
    public ReactivePropertySlim<string> AutherName { get; }
    public ReactivePropertySlim<bool> IsBounce { get; }
    public ReactivePropertySlim<bool> IsShader { get; }
    public ReactivePropertySlim<string> LicencePath { get; }
    public ObservableCollection<SkinRowModel> ImgRows { get; }

    private ExtremeHatsExporter? exporter = null;
    private TranslationExporter? transDataExporter = null;

    public ExtremeHatModel(IAmongUsPathContainerModel model)
    {
        this.AmongUsPathContainer = model;
        this.SkinName = new ReactivePropertySlim<string>("");
        this.AutherName = new ReactivePropertySlim<string>("");
        this.LicencePath = new ReactivePropertySlim<string>("");
        this.IsBounce = new ReactivePropertySlim<bool>(false);
        this.IsShader = new ReactivePropertySlim<bool>(false);


        ImgRows = new ObservableCollection<SkinRowModel>()
        {
            new SkinRowModel("ExH.SelectFrontImage"),
            new SkinRowModel("ExH.SelectFrontFlipImage"),
            new SkinRowModel("ExH.SelectBackImage"),
            new SkinRowModel("ExH.SelectBackFlipImage"),
            new SkinRowModel("ExH.SelectClimbImage"),
        };
    }

    public string Export(bool isOverride)
    {
        try
        {
            if (this.exporter == null)
            {
                this.CreateExporter();
            }
            this.exporter!.Export(isOverride);
            this.transDataExporter?.Export();
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public ExportStatus GetCurrentExportStatus()
    {
        if (string.IsNullOrEmpty(this.AutherName.Value))
        {
            return ExportStatus.MissingAutherName;
        }
        else if (string.IsNullOrEmpty(this.SkinName.Value))
        {
            return ExportStatus.MissingSkinName;
        }
        else if (string.IsNullOrEmpty(GetImgRow("ExH.SelectFrontImage").ImgPath.Value))
        {
            return ExportStatus.MissingFrontImg;
        }
        else
        {
            return ExportStatus.NoProblem;
        }
    }

    public SameSkinCheckResult GetSameSkinStatus()
    {
        this.CreateExporter();
        return this.exporter!.CheckSameSkin();
    }

    private void CreateExporter()
    {

        this.transDataExporter = null;

        var frontRow = GetImgRow("ExH.SelectFrontImage");
        var frontFlipRow = GetImgRow("ExH.SelectFrontFlipImage");
        var backRow = GetImgRow("ExH.SelectBackImage");
        var backFlipRow = GetImgRow("ExH.SelectBackFlipImage");
        var climbRow = GetImgRow("ExH.SelectClimbImage");

        var frontAnimationExportInfo = CrateAnimationExportInfo(frontRow);
        var frontFlipAnimationExportInfo = CrateAnimationExportInfo(frontFlipRow);
        var backAnimationExportInfo = CrateAnimationExportInfo(backRow);
        var backFlipAnimationExportInfo = CrateAnimationExportInfo(backFlipRow);
        var climbAnimationExportInfo = CrateAnimationExportInfo(climbRow);

        var hatAnimation =
            frontAnimationExportInfo.Info == null &&
            frontFlipAnimationExportInfo.Info == null &&
            backAnimationExportInfo.Info == null &&
            backFlipAnimationExportInfo.Info == null &&
            climbAnimationExportInfo.Info == null ?
            null : new HatAnimation(
                frontAnimationExportInfo.Info,
                frontFlipAnimationExportInfo.Info,
                backAnimationExportInfo.Info,
                backFlipAnimationExportInfo.Info,
                climbAnimationExportInfo.Info);

        Dictionary<string, string> replacedStr = new Dictionary<string, string>();

        string autherName = this.AutherName.Value;
        if (TryReplaceAscii(autherName, out string asciiedAutherName))
        {
            replacedStr.Add(asciiedAutherName, autherName);
            autherName = asciiedAutherName;
        }
        string skinName = this.SkinName.Value;
        if (TryReplaceAscii(skinName, out string asciiedSkinName))
        {
            replacedStr.Add(asciiedSkinName, skinName);
            skinName = asciiedSkinName;
        }

        HatInfo hatInfo = new HatInfo(
            Name: skinName,
            Author: autherName,
            Bound: this.IsBounce.Value,
            Shader: this.IsShader.Value,
            Climb: !string.IsNullOrEmpty(climbRow.ImgPath.Value),
            FrontFlip: !string.IsNullOrEmpty(frontFlipRow.ImgPath.Value),
            Back: !string.IsNullOrEmpty(backRow.ImgPath.Value),
            BackFlip: !string.IsNullOrEmpty(backFlipRow.ImgPath.Value),
            Animation: hatAnimation
        );

        string amongUsPath = this.AmongUsPathContainer.AmongUsPath.Value;

        this.exporter = new ExtremeHatsExporter()
        {
            Info = hatInfo,
            AmongUsPath = amongUsPath,
            LicenseFile = this.LicencePath.Value,
        };

        this.exporter.AddImage(
            DataStructure.FrontImageName, frontRow.ImgPath.Value);
        if (hatInfo.FrontFlip)
        {
            this.exporter.AddImage(
                DataStructure.FrontFlipImageName, frontFlipRow.ImgPath.Value);
        }
        if (hatInfo.Back)
        {
            this.exporter.AddImage(
                DataStructure.BackImageName, backRow.ImgPath.Value);
        }
        if (hatInfo.BackFlip)
        {
            this.exporter.AddImage(
                DataStructure.BackFlipImageName, backFlipRow.ImgPath.Value);
        }
        if (hatInfo.Climb)
        {
            this.exporter.AddImage(
                DataStructure.ClimbImageName, climbRow.ImgPath.Value);
        }
        if (hatInfo.Animation != null)
        {
            if (hatInfo.Animation.Front != null &&
                frontAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, frontAnimationExportInfo);
            }
            if (hatInfo.Animation.FrontFlip != null &&
                frontFlipAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, frontFlipAnimationExportInfo);
            }
            if (hatInfo.Animation.Back != null &&
                backAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, backAnimationExportInfo);
            }
            if (hatInfo.Animation.BackFlip != null &&
                backFlipAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, backFlipAnimationExportInfo);
            }
            if (hatInfo.Animation.Climb != null &&
                climbAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, climbAnimationExportInfo);
            }
        }

        if (replacedStr.Count != 0)
        {
            this.transDataExporter = new TranslationExporter()
            {
                Locale = (string)Application.Current.MainWindow.Resources["CurLocale"],
                AmongUsPath = amongUsPath,
            };
            this.transDataExporter.AddTransData(replacedStr);
        }
    }

    private SkinRowModel GetImgRow(string key)
        => this.ImgRows.Where(x => x.RowName == key).First();

    private static void AddAnimationImg(ExtremeHatsExporter exporter, AnimationImgExportInfo info)
    {
        exporter.AddFolder(info.FolderName);
        for (int i = 0; i < info.FromImgPath.Length; ++i)
        {
            string toPath = info.Info!.Img[i];
            string fromPath = info.FromImgPath[i];
            exporter.AddImage(toPath, fromPath);
        }
    }
}
