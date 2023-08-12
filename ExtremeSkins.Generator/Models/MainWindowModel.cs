using System;
using System.IO.Compression;
using System.IO;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;

using ExHData = ExtremeSkins.Core.ExtremeHats.DataStructure;
using ExNData = ExtremeSkins.Core.ExtremeNamePlate.DataStructure;
using ExVData = ExtremeSkins.Core.ExtremeHats.DataStructure;

namespace ExtremeSkins.Generator.Models;

public sealed class MainWindowModel : IMainWindowModel
{
    public IAmongUsPathContainerModel AmongUsPathContainer { get; }

    public MainWindowModel(IAmongUsPathContainerModel amongUsPathContainer)
    {
        AmongUsPathContainer = amongUsPathContainer;
    }

    public string ExportToZip()
    {
        string exportFolder = IExporter.ExportDefaultPath;
        if (!Directory.Exists(exportFolder))
        {
            return "";
        }

        string fileName = $"output_{DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss")}.zip";

        using var zip = ZipFile.Open(fileName, ZipArchiveMode.Update);

        string creatorModePath = CreatorMode.CreatorModeFolder;
        string creatorModeFolderPath = Path.Combine(exportFolder, creatorModePath);

        if (Directory.Exists(creatorModeFolderPath))
        {
            string folderPath = $"{creatorModePath}/";
            string csvFileName = CreatorMode.TranslationCsvFile;
            zip.CreateEntry(folderPath);
            zip.CreateEntryFromFile(
                Path.Combine(creatorModeFolderPath, csvFileName),
                $"{folderPath}{csvFileName}");
        }
        addRecursivelyAllFolder(zip, ExHData.FolderName);
        addRecursivelyAllFolder(zip, ExVData.FolderName);
        addRecursivelyAllFolder(zip, ExNData.FolderName);

        return fileName;
    }

    private static void addRecursivelyAllFolder(ZipArchive zip, string folderName)
    {
        string exportFolder = IExporter.ExportDefaultPath;
        string folderPath = Path.Combine(exportFolder, folderName);
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        string zipFolderPath = $"{folderPath}/";
        zip.CreateEntry(zipFolderPath);

        var skinDirInfo = new DirectoryInfo(folderPath);
        foreach (var dirInfo in skinDirInfo.GetDirectories())
        {
            string name = dirInfo.Name;
            string namePath = $"{zipFolderPath}{name}";

            foreach (var file in dirInfo.GetFiles())
            {
                zip.CreateEntryFromFile(
                    file.FullName,
                    $"{namePath}/{file.Name}");
            }
        }
    }
}
