using System.IO;

using ExtremeSkins.Core.ExtremeNamePlate;
using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Core;

public sealed class ExtremeNamePlateExporter : ISkinExporter
{
    public string Author
    {
        init
        {
            this.author = value;
        }
    }

    public string AmongUsPath
    {
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            this.amongUsPath = Path.Combine(
                value, DataStructure.FolderName);
        }
    }

    public string LicenseFile
    {
        init
        {
            this.licenseFile = value;
        }
    }

    private string amongUsPath = string.Empty;
    private string licenseFile = string.Empty;
    private string author = string.Empty;

    private string imgName = string.Empty;
    private string imgFromPath = string.Empty;

    private string defaultExportPath => Path.Combine(
        IExporter.ExportDefaultPath, DataStructure.FolderName);

    public SameSkinCheckResult CheckSameSkin()
    {
        string imgPath = getExportImgPath(defaultExportPath);
        bool isExported = File.Exists(imgPath);
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            string imgExNPath = getExportImgPath(this.amongUsPath);
            if (File.Exists(imgExNPath) && !isExported)
            {
                return SameSkinCheckResult.ExistExS;
            }
        }

        return isExported ?
            SameSkinCheckResult.ExistMyExportedSkin :
            SameSkinCheckResult.No;
    }

    public void AddImage(string imgName, string basePath)
    {
        this.imgName = imgName;
        this.imgFromPath = basePath;
    }

    public void Export(bool _ = false)
    {
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            ExportTo(this.amongUsPath);
        }
        ExportTo(Path.Combine(
            IExporter.ExportDefaultPath, DataStructure.FolderName));
    }

    private void ExportTo(string targetPath)
    {
        string exportFolder = getExportFolderPath(targetPath);

        if (!Directory.Exists(exportFolder))
        {
            Directory.CreateDirectory(exportFolder);
        }

        File.Copy(this.imgFromPath, Path.Combine(exportFolder, this.imgName), true);

        ISkinExporter.ExportLicense(this.licenseFile, exportFolder);
    }

    private string getExportFolderPath(string targetPath) =>
        Path.Combine(targetPath, this.author);

    private string getExportImgPath(string targetPath) =>
        Path.Combine(targetPath, this.author, this.imgName);
}
