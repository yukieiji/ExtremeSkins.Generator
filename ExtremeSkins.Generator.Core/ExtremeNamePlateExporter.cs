using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Core;

public sealed class ExtremeNamePlateExporter : IExporter
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

    private string amongUsPath = "";
    private string licenseFile = "";
    private string author = "";

    private string imgName = "";
    private string imgFromPath = "";

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

        IExporter.ExportLicense(this.licenseFile, exportFolder);
    }
}
