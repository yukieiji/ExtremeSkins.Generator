using System.Collections.Generic;
using System.IO;
using System.Text;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Core.Interface;

using SupportedLangs = ExtremeSkins.Core.CreatorMode.SupportedLangs;

namespace ExtremeSkins.Generator.Core;

public sealed class TranslationExporter : IExporter
{
    private const string comma = ",";

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
    private List<string> writeLineData = new List<string>();

    
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
        foreach (var (transKey, trans) in exportData)
        {
            StringBuilder builder = new StringBuilder(13);
            builder.Append(transKey).Append(comma);

            foreach (var local in supportLnag.Values)
            {
                builder.Append(
                    local == this.Locale ? trans : string.Empty).Append(comma);
            }
            this.writeLineData.Add(builder.ToString());
        }
    }

    public void Export()
    {
        if (this.writeLineData.Count == 0) { return; }

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

        using StreamWriter transCsv = CreatorMode.GetTranslationWriter(directoryFolder);

        foreach (string line in this.writeLineData)
        {
            transCsv.WriteLine(line);
        }
    }
}
