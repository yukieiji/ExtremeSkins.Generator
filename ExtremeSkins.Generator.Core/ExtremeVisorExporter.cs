using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using ExtremeSkins.Core;
using ExtremeSkins.Core.ExtremeVisor;
using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Core;

public sealed class ExtremeVisorExporter : IInfoHasExporter<VisorInfo>
{

    public VisorInfo Info
    {
        init
        {
            this.info = value;
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
    private VisorInfo info = new VisorInfo(string.Empty, string.Empty, false, false);

    private Dictionary<string, string> img = new Dictionary<string, string>();

    private string defaultExportPath => Path.Combine(
        IExporter.ExportDefaultPath, DataStructure.FolderName);

    public SameSkinCheckResult CheckSameSkin()
    {
        string imgPath = getExportFolderPath(defaultExportPath);
        bool isExported = File.Exists(imgPath);
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            string imgExNPath = getExportFolderPath(this.amongUsPath);
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
        this.img.Add(imgName, basePath);
    }

    public void Export(bool isOverride)
    {
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            ExportTo(this.amongUsPath, isOverride);
        }
        ExportTo(defaultExportPath, isOverride);
    }

    private void ExportTo(string targetPath, bool isOverride)
    {
        string exportFolder = getExportFolderPath(targetPath);

        int counter = 0;

        bool isExist = Directory.Exists(exportFolder);

        if (!isOverride && isExist)
        {
            return;
        }
        else if (isOverride && isExist)
        {
            ISkinExporter.DeleteDirectory(exportFolder);
        }
        else
        {
            while (Directory.Exists(exportFolder))
            {
                counter++;
                exportFolder = $"{exportFolder}_{counter}";
            }
        }

        Directory.CreateDirectory(exportFolder);

        foreach (var (imgName, copyFromPath) in this.img)
        {
            string imgCopyPath = Path.Combine(exportFolder, imgName);
            File.Copy(copyFromPath, imgCopyPath);
        }

        InfoBase.ExportToJson(this.info, exportFolder);
        ISkinExporter.ExportLicense(this.licenseFile, exportFolder);
    }

    private string getExportFolderPath(string targetPath) =>
        Path.Combine(targetPath, $"{this.info.Name}_{this.info.Author}");
}
