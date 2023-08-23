using System.Windows;

using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Core.Interface;

using static ExtremeSkins.Generator.Panel.Interfaces.ICosmicModel;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeHatExporterModel : ExporterModelBase
{
    public string AutherName => this.info.Author;
    public string FolderName => this.hatsExporter.FolderName;

    private readonly HatInfo info;
    private readonly ExtremeHatsExporter hatsExporter;
    private readonly TranslationExporter? transExporter;

    public ExtremeHatExporterModel(
        string auPath,
        string authorName,
        string skinName,
        string licencePath,
        bool isBounce,
        bool isShader,
        SkinRowModel frontRow,
        SkinRowModel frontFlipRow,
        SkinRowModel backRow,
        SkinRowModel backFlipRow,
        SkinRowModel climbRow)
    {

        (authorName, skinName) = ReplaceAscii(authorName, skinName);

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

        this.info = new HatInfo(
            Name: skinName,
            Author: authorName,
            Bound: isBounce,
            Shader: isShader,
            Climb: !string.IsNullOrEmpty(climbRow.ImgPath.Value),
            FrontFlip: !string.IsNullOrEmpty(frontFlipRow.ImgPath.Value),
            Back: !string.IsNullOrEmpty(backRow.ImgPath.Value),
            BackFlip: !string.IsNullOrEmpty(backFlipRow.ImgPath.Value),
            Animation: hatAnimation
        );

        this.hatsExporter = new ExtremeHatsExporter()
        {
            Info = this.info,
            AmongUsPath = auPath,
            LicenseFile = licencePath,
        };

        this.hatsExporter.AddImage(
            DataStructure.FrontImageName, frontRow.ImgPath.Value);
        if (this.info.FrontFlip)
        {
            this.hatsExporter.AddImage(
                DataStructure.FrontFlipImageName, frontFlipRow.ImgPath.Value);
        }
        if (this.info.Back)
        {
            this.hatsExporter.AddImage(
                DataStructure.BackImageName, backRow.ImgPath.Value);
        }
        if (this.info.BackFlip)
        {
            this.hatsExporter.AddImage(
                DataStructure.BackFlipImageName, backFlipRow.ImgPath.Value);
        }
        if (this.info.Climb)
        {
            this.hatsExporter.AddImage(
                DataStructure.ClimbImageName, climbRow.ImgPath.Value);
        }
        if (this.info.Animation != null)
        {
            if (this.info.Animation.Front != null &&
                frontAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.hatsExporter, frontAnimationExportInfo);
            }
            if (this.info.Animation.FrontFlip != null &&
                frontFlipAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.hatsExporter, frontFlipAnimationExportInfo);
            }
            if (this.info.Animation.Back != null &&
                backAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.hatsExporter, backAnimationExportInfo);
            }
            if (this.info.Animation.BackFlip != null &&
                backFlipAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.hatsExporter, backFlipAnimationExportInfo);
            }
            if (this.info.Animation.Climb != null &&
                climbAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.hatsExporter, climbAnimationExportInfo);
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
        this.hatsExporter!.Export(isOverride);
        this.transExporter?.Export();
    }

    public override SameSkinCheckResult GetSameSkinCheckResult() => this.hatsExporter.CheckSameSkin();

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
