using System.Windows;

using ExtremeSkins.Core.ExtremeVisor;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Core.Interface;

using static ExtremeSkins.Generator.Panel.Interfaces.ICosmicModel;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeVisorExporterModel : ExporterModelBase
{
    public string AutherName => this.info.Author;
    public string FolderName => this.visorExporter.FolderName;
    public string SkinName => this.info.Name;

    private readonly VisorInfo info;
    private readonly ExtremeVisorExporter visorExporter;
    private readonly TranslationExporter? transExporter;

    public ExtremeVisorExporterModel(
        string auPath,
        string authorName,
        string skinName,
        string licencePath,
        bool isBehindHat,
        bool isShader,
        SkinRowModel idleRow,
        SkinRowModel leftIdleRow)
    {

        (authorName, skinName) = ReplaceAscii(authorName, skinName);

        var frontAnimationExportInfo = CrateAnimationExportInfo(idleRow);
        var frontFlipAnimationExportInfo = CrateAnimationExportInfo(leftIdleRow);

        var visorAnimation =
            frontAnimationExportInfo.Info == null &&
            frontFlipAnimationExportInfo.Info == null ?
            null : new VisorAnimation(
                frontAnimationExportInfo.Info,
                frontFlipAnimationExportInfo.Info);

        this.info = new VisorInfo(
            Name: skinName,
            Author: authorName,
            Shader: isShader,
            BehindHat: isBehindHat,
            LeftIdle: !string.IsNullOrEmpty(leftIdleRow.ImgPath.Value),
            Animation: visorAnimation
        );

        this.visorExporter = new ExtremeVisorExporter()
        {
            Info = this.info,
            AmongUsPath = auPath,
            LicenseFile = licencePath,
        };

        this.visorExporter.AddImage(
            DataStructure.IdleImageName, idleRow.ImgPath.Value);
        if (this.info.LeftIdle)
        {
            this.visorExporter.AddImage(
                DataStructure.FlipIdleImageName, leftIdleRow.ImgPath.Value);
        }
        if (this.info.Animation != null)
        {
            if (this.info.Animation.Idle != null &&
                frontAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.visorExporter, frontAnimationExportInfo);
            }
            if (this.info.Animation.LeftIdle != null &&
                frontFlipAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.visorExporter, frontFlipAnimationExportInfo);
            }
        }

        if (!string.IsNullOrEmpty(this.TranslatedSkinName) ||
            !string.IsNullOrEmpty(this.TranslatedAuthorName))
        {
            this.transExporter = new TranslationExporter()
            {
                Locale = (string)Application.Current.MainWindow.Resources["CurLocale"],
                AmongUsPath = auPath,
            };
            this.transExporter.AddTransData(skinName, this.TranslatedSkinName);
            this.transExporter.AddTransData(authorName, this.TranslatedAuthorName);
        }
    }


    public override void Export(bool isOverride)
    {
        this.visorExporter!.Export(isOverride);
        this.transExporter?.Export();
    }

    public override SameSkinCheckResult GetSameSkinCheckResult() => this.visorExporter.CheckSameSkin();

    private static void AddAnimationImg(
        ExtremeVisorExporter exporter, AnimationImgExportInfo info)
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
