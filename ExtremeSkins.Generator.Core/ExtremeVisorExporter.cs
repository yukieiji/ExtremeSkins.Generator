using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Core;

public sealed class ExtremeVisorExporter : IInfoHasExporter<ExtremeVisorExporter.VisorInfo>
{
    public record VisorInfo(
        string Name, string Author,
        bool LeftIdle = false,
        bool Shader = false,
        bool BehindHat = false,
        string ComitHash = "") : IInfo(Name, Author);

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
            this.amongUsPath = Path.Combine(value, "ExtremeVisor");
        }
    }

    public string LicenseFile
    {
        init
        {
            this.licenseFile = value;
        }
    }

    private string amongUsPath = "";
    private string licenseFile = "";
    private VisorInfo info = new VisorInfo(string.Empty, string.Empty, false, false);

    private Dictionary<string, string> img = new Dictionary<string, string>();

    public void AddImage(string imgName, string basePath)
    {
        this.img.Add(imgName, basePath);
    }

    public void Export()
    {
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            ExportTo(Path.Combine(this.amongUsPath));
        }
        ExportTo(Path.Combine(IExporter.ExportDefaultPath, "ExtremeVisor"));
    }

    private void ExportTo(string targetPath)
    {
        string exportFolder = Path.Combine(targetPath, this.info.Name);

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

        File.WriteAllText(
            Path.Combine(exportFolder, "info.json"),
            JsonSerializer.Serialize(this.info, IExporter.Option));

        IExporter.ExportLicense(this.licenseFile, exportFolder);
    }
}
