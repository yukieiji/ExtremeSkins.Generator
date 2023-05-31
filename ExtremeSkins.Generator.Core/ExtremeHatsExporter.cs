﻿using System.Collections.Generic;
using System.IO;

using ExtremeSkins.Core;
using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Core;

public sealed class ExtremeHatsExporter  : IInfoHasExporter<HatInfo>
{
    public HatInfo Info
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
    private HatInfo info = new HatInfo(string.Empty, string.Empty, false, false);

    private Dictionary<string, string> img = new Dictionary<string, string>();

    private string defaultExportPath => Path.Combine(
        IExporter.ExportDefaultPath, DataStructure.FolderName);

    public SameSkinCheckResult CheckSameSkin()
    {
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            string imgExNPath = getExportFolderPath(this.amongUsPath);
            if (File.Exists(imgExNPath))
            {
                return SameSkinCheckResult.ExistExS;
            }
        }
        string imgPath = getExportFolderPath(defaultExportPath);

        return File.Exists(imgPath) ?
            SameSkinCheckResult.ExistMyExportedSkin :
            SameSkinCheckResult.No;
    }

    public void AddImage(string imgName, string basePath)
    {
        this.img.Add(imgName, basePath);
    }

    public void Export()
    {
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            ExportTo(this.amongUsPath);
        }
        ExportTo(defaultExportPath);
    }

    private void ExportTo(string targetPath)
    {
        string exportFolder = getExportFolderPath(targetPath);

        int counter = 0;
        while (Directory.Exists(exportFolder))
        {
            counter++;
            exportFolder = $"{exportFolder}_{counter}";
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
        Path.Combine(targetPath, this.info.Name);
}
