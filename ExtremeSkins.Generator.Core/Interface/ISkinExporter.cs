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

    protected static void ExportLicense(string licensePath, string exportFolder)
    {
        if (string.IsNullOrEmpty(licensePath))
        {
            return;
        }

        File.Copy(licensePath,
            Path.Combine(exportFolder, License.FileName));
    }
}
