using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ExtremeSkins.Generator.Core.Interface;

public interface IExporter
{
    public const string ExportDefaultPath = "outputs";

    protected static JsonSerializerOptions Option => new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    public string AmongUsPath { init; }
    public string LicenseFile { init; }

    public void AddImage(string imgName, string basePath);
    public void Export();

    protected static void ExportLicense(string licensePath, string exportFolder)
    {
        if (string.IsNullOrEmpty(licensePath))
        {
            return;
        }

        File.Copy(licensePath,
            Path.Combine(exportFolder, "LICENSE.md"));
    }
}
