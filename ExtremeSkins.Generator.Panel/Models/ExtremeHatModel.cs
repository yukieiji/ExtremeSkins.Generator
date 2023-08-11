using ExtremeSkins.Core;
using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Panel.Interfaces;
using ExtremeSkins.Generator.Service;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeHatModel : ExportModelBase, IExtremeHatModel
{
    public SkinRowModel FrontRow { get; set; }
    public SkinRowModel FrontFlipRow { get; set; }

    public SkinRowModel BackRow { get; set; }
    public SkinRowModel BackFlipRow { get; set; }

    public SkinRowModel ClimbRow { get; set; }

    public ReactivePropertySlim<bool> IsBounce { get; set; }

    public ReactivePropertySlim<bool> IsShader { get; set; }

    public ReactivePropertySlim<string> LicencePath { get; set; }

    public override ExportResult Export()
    {
        return ExportResult.Success;

        /*
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

        else if (string.IsNullOrEmpty(this.SkinName))
        {
            this.showMessageService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotEmptySkin"],
                });
            return;
        }
        */

        if (string.IsNullOrEmpty(this.FrontRow.ImgPath.Value))
        {
            return ExportResult.FrontImgMissing;
        }

        Dictionary<string, string> replacedStr = new Dictionary<string, string>();

        /*
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
        */

        HatInfo hatInfo = new HatInfo(
            Name: string.Empty,
            Author: string.Empty,
            Bound: this.IsBounce.Value,
            Shader: this.IsShader.Value,
            Climb: !string.IsNullOrEmpty(this.ClimbRow.ImgPath.Value),
            FrontFlip: !string.IsNullOrEmpty(this.FrontFlipRow.ImgPath.Value),
            Back: !string.IsNullOrEmpty(this.BackRow.ImgPath.Value),
            BackFlip: !string.IsNullOrEmpty(this.BackFlipRow.ImgPath.Value)
        );
        /*
        ExtremeHatsExporter exporter = new ExtremeHatsExporter()
        {
            Info = hatInfo,
            AmongUsPath = this.AmongUsPath,
            LicenseFile = this.licensePath,
        };
        */
        /*
        var sameSkinResult = exporter.CheckSameSkin();
        bool isOverride = false;
        */
    }

    private void createExporter()
    {
        var frontAnimation = crateAnimationInfo(FrontRow);
        var frontFlipAnimation = crateAnimationInfo(FrontFlipRow);
        var backAnimation = crateAnimationInfo(BackRow);
        var backFlipAnimation = crateAnimationInfo(BackFlipRow);
        var climbAnimation = crateAnimationInfo(ClimbRow);

        var hatAnimation =
            frontAnimation == null &&
            frontFlipAnimation == null &&
            backAnimation == null &&
            backFlipAnimation == null &&
            climbAnimation == null ?
            null : new HatAnimation(frontAnimation, frontFlipAnimation, backAnimation, backFlipAnimation, climbAnimation);

        HatInfo hatInfo = new HatInfo(
            Name: string.Empty,
            Author: string.Empty,
            Bound: this.IsBounce.Value,
            Shader: this.IsShader.Value,
            Climb: !string.IsNullOrEmpty(this.ClimbRow.ImgPath.Value),
            FrontFlip: !string.IsNullOrEmpty(this.FrontFlipRow.ImgPath.Value),
            Back: !string.IsNullOrEmpty(this.BackRow.ImgPath.Value),
            BackFlip: !string.IsNullOrEmpty(this.BackFlipRow.ImgPath.Value),
            Animation: hatAnimation
        );
    }

    private static AnimationInfo? crateAnimationInfo(SkinRowModel rowModel)
    {
        uint frameCount = rowModel.FrameCount.Value;

        return
            rowModel.IsAnimation.Value &&
            rowModel.FileList.Count > 0 &&
            1 <= frameCount && frameCount <= 300 ?
                new AnimationInfo(
                    rowModel.Files, frameCount, rowModel.AnimationType) : null;
    }
}
