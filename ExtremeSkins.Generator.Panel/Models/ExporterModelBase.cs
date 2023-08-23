using AnyAscii;
using ExtremeSkins.Core;
using ExtremeSkins.Generator.Core.Interface;

using System;
using System.IO;

using static ExtremeSkins.Generator.Panel.Interfaces.ICosmicModel;

namespace ExtremeSkins.Generator.Panel.Models;

public abstract class ExporterModelBase
{
    public string TranslatedAuthorName { get; private set; } = string.Empty;
    public string TranslatedSkinName { get; private set; } = string.Empty;

    public abstract void Export(bool isOverride);

    public abstract SameSkinCheckResult GetSameSkinCheckResult();


    protected (string, string) ReplaceAscii(string authorName, string skinName)
    {
        this.TranslatedSkinName = string.Empty;
        this.TranslatedAuthorName = string.Empty;

        if (TryReplaceAscii(authorName, out string asciiedAutherName))
        {
            this.TranslatedAuthorName = authorName;
            authorName = asciiedAutherName;
        }
        if (TryReplaceAscii(skinName, out string asciiedSkinName))
        {
            this.TranslatedSkinName = skinName;
            skinName = asciiedSkinName;
        }
        return (authorName, skinName);
    }

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
