using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Core.Interface;

using SupportedLangs = ExtremeSkins.Core.CreatorMode.SupportedLangs;

namespace ExtremeSkins.Generator.Core;

public sealed class TranslationExporter : IExporter
{
    public string Locale { get; init; } = string.Empty;

    public string AmongUsPath
    {
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            this.amongUsPath = value;
        }
    }
    private string amongUsPath = string.Empty;
    private Dictionary<string, string> transData = new Dictionary<string, string>();

    
    private static Dictionary<SupportedLangs, string> supportLnag = new Dictionary<SupportedLangs, string>()
    {
        {SupportedLangs.English   , ""},
        {SupportedLangs.Latam     , ""},
        {SupportedLangs.Brazilian , ""},
        {SupportedLangs.Portuguese, ""},
        {SupportedLangs.Korean    , ""},
        {SupportedLangs.Russian   , ""},
        {SupportedLangs.Dutch     , ""},
        {SupportedLangs.Filipino  , ""},
        {SupportedLangs.French    , ""},
        {SupportedLangs.German    , ""},
        {SupportedLangs.Italian   , ""},
        {SupportedLangs.Japanese  , "ja-JP"},
        {SupportedLangs.Spanish   , ""},
        {SupportedLangs.SChinese  , ""},
        {SupportedLangs.TChinese  , ""},
        {SupportedLangs.Irish     , ""},
    };

    public void AddTransData(Dictionary<string, string> exportData)
    {
        this.transData = exportData;
    }

    public void Export()
    {
        if (this.transData.Count <= 0) { return; }

        if (!string.IsNullOrEmpty(this.amongUsPath))
        {
            CreatorMode.SetCreatorMode(this.amongUsPath, true);
            ExportTo(this.amongUsPath);
        }
        ExportTo(Path.Combine(Directory.GetCurrentDirectory(), IExporter.ExportDefaultPath));
    }
    private void ExportTo(string path)
    {
        string directoryFolder = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(directoryFolder)) { return; }

        List<string> writeStr = new List<string>();

        if (CreatorMode.IsExistTransFile(directoryFolder))
        {
            using (StreamReader csv = CreatorMode.GetTranslationReader(
                directoryFolder))
            {
                csv.ReadLine();
                while (!csv.EndOfStream)
                {
                    string line = csv.ReadLine();
                    if (!this.transData.Keys.Any(line.StartsWith))
                    {
                        writeStr.Add(csv.ReadLine());
                    }
                }
            }
        }

        foreach (var (transKey, trans) in this.transData)
        {
            StringBuilder builder = new StringBuilder(13);
            builder.Append(transKey).Append(CreatorMode.Comma);

            foreach (var local in supportLnag.Values)
            {
                builder.Append(
                    local == this.Locale ? trans : string.Empty).Append(
                    CreatorMode.Comma);
            }
            writeStr.Add(builder.ToString());
        }

        using StreamWriter transCsv = CreatorMode.CreateTranslationWriter(
            directoryFolder);

        foreach (string line in writeStr)
        {
            transCsv.WriteLine(line);
        }
    }
}
