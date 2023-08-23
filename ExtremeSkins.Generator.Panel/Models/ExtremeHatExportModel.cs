using System.IO;
using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeHatExportModel
{
    public string DefaultExportFolderPath =>
        Path.GetFullPath(
            Path.Combine(IExporter.ExportDefaultPath, DataStructure.FolderName));

}
