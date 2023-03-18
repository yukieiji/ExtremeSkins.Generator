using System.IO;

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
            this.amongUsPath = Path.Combine(value, "ExtremeNamePlate");
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

    public void AddImage(string imgName, string basePath)
    {
        this.imgName = imgName;
        this.imgFromPath = basePath;
    }

    public void Export()
    {
        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            ExportTo(Path.Combine(this.amongUsPath));
        }
        ExportTo(Path.Combine(IExporter.ExportDefaultPath, "ExtremeNamePlate"));
    }

    private void ExportTo(string targetPath)
    {
        string exportFolder = Path.Combine(targetPath, this.author);

        if (!Directory.Exists(exportFolder))
        {
            Directory.CreateDirectory(exportFolder);
        }

        File.Copy(this.imgFromPath, Path.Combine(exportFolder, this.imgName));

        ISkinExporter.ExportLicense(this.licenseFile, exportFolder);
    }
}
