using Prism.Mvvm;

using Reactive.Bindings;
using System.Collections.ObjectModel;

using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeHatModel : BindableBase, IExtremeHatModel
{
    public string AmongUsPath { private get; set; }

    public ReactivePropertySlim<string> SkinName { get; }
    public ReactivePropertySlim<string> AutherName { get; }
    public ReactivePropertySlim<bool> IsBounce { get; }
    public ReactivePropertySlim<bool> IsShader { get; }
    public ReactivePropertySlim<string> LicencePath { get; }
    public ObservableCollection<SkinRowModel> ImgRows { get; }

    public ExtremeHatModel()
    {
        this.AmongUsPath = string.Empty;
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

    public IExportModel.ExportResult Export()
    {
        return IExportModel.ExportResult.Success;
    }

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

if (string.IsNullOrEmpty(this.FrontRow.ImgPath.Value))
{
   return ExportResult.FrontImgMissing;
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
*/
}
