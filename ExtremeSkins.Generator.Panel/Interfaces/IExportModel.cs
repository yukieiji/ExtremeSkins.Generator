using AnyAscii;

using Reactive.Bindings;

using System;
using System.IO;
using System.Threading.Tasks;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Panel.Models;
using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IExportModel
{
    public record AnimationImgExportInfo(string FolderName, string[] FromImgPath, AnimationInfo? Info);

    public enum ExportStatus : byte
    {
        NoProblem,
        MissingAutherName,
        MissingSkinName,
        MissingFrontImg,
    }

    public IApiServerModel ApiServerModel { get; }

    public IAmongUsPathContainerModel AmongUsPathContainer { get; }

    public ReactivePropertySlim<string> SkinName { get; }
    public ReactivePropertySlim<string> AutherName { get; }

    public Task<bool> HotReloadCosmic();

    public string Export(bool isOverride);

    public ExportStatus GetCurrentExportStatus();

    public SameSkinCheckResult GetSameSkinStatus();

    protected static AnimationImgExportInfo CrateAnimationExportInfo(SkinRowModel rowModel)
    {
        uint frameCount = rowModel.FrameCount.Value;

        if (!rowModel.IsAnimation.Value ||
            rowModel.FileList.Count == 0 ||
            0 == frameCount ||
            frameCount > 300)
        {
            return new AnimationImgExportInfo(string.Empty, Array.Empty<string>(), null);
        }

        string[] files = rowModel.Files;
        string[] imgPath = new string[files.Length];

        string folderName = rowModel.RowName.Replace(".", "");

        for (int i = 0; i < files.Length; ++i)
        {
            string filePath = files[i];
            string fileName = Path.GetFileName(filePath);
            imgPath[i] = Path.Combine(folderName, $"{i}_{fileName}");
        }

        return new AnimationImgExportInfo(
            folderName, files, new AnimationInfo(imgPath, frameCount, rowModel.AnimationType));
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
