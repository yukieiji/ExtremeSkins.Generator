using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

using ExtremeSkins.Core;

namespace ExtremeSkins.Generator.Core.Interface;

public enum SameSkinCheckResult
{
    No,
    ExistExS,
    ExistMyExportedSkin
}

public interface ISkinExporter : IExporter
{
    protected static JsonSerializerOptions Option => new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    public string LicenseFile { init; }

    public SameSkinCheckResult CheckSameSkin();

    public void AddImage(string imgName, string basePath);

    public static void DeleteDirectory(string dirPath)
    {
        DeleteDirectory(new DirectoryInfo(dirPath));
    }

    /// ----------------------------------------------------------------------------
    /// <summary>
    ///     指定したディレクトリをすべて削除します。</summary>
    /// <param name="hDirectoryInfo">
    ///     削除するディレクトリの DirectoryInfo。</param>
    /// ----------------------------------------------------------------------------
    public static void DeleteDirectory(DirectoryInfo hDirectoryInfo)
    {
        // すべてのファイルの読み取り専用属性を解除する
        foreach (FileInfo cFileInfo in hDirectoryInfo.GetFiles())
        {
            if ((cFileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                cFileInfo.Attributes = FileAttributes.Normal;
            }
        }

        // サブディレクトリ内の読み取り専用属性を解除する (再帰)
        foreach (DirectoryInfo hDirInfo in hDirectoryInfo.GetDirectories())
        {
            DeleteDirectory(hDirInfo);
        }

        // このディレクトリの読み取り専用属性を解除する
        if ((hDirectoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        {
            hDirectoryInfo.Attributes = FileAttributes.Directory;
        }

        // このディレクトリを削除する
        hDirectoryInfo.Delete(true);
    }

    protected static void ExportLicense(string licensePath, string exportFolder)
    {
        string targetPath = Path.Combine(exportFolder, License.FileName);

        if (string.IsNullOrEmpty(licensePath) ||
            licensePath == targetPath)
        {
            return;
        }

        File.Copy(licensePath, targetPath, true);
    }
}
