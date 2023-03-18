using System.Collections.Generic;
using System.IO;
using System.Text;

using ExtremeSkins.Generator.Core.Interface;

namespace ExtremeSkins.Generator.Core;

public sealed class TranslationExporter : IExporter
{
    public const string ExSConfigPath = @"BepInEx/config/me.yukieiji.extremeskins.cfg";

    private const string translaterPath = @"CreatorMode/translation.csv";
    private const string comma = ",";

    private const string creatorStr = "CreatorMode = false";
    private const string replacedCreatorModeStr = "CreatorMode = true";

    public string Locale { get; init; } = string.Empty;

    public string AmongUsPath
    {
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            this.transPath = Path.Combine(value, translaterPath);
            this.configPath = Path.Combine(value, @"BepInEx/config/me.yukieiji.extremeskins.cfg");
        }
    }
    private string transPath = string.Empty;
    private string configPath = string.Empty;
    private List<string> writeLineData = new List<string>();

    public enum SupportedLangs
    {
        English,
        Latam,
        Brazilian,
        Portuguese,
        Korean,
        Russian,
        Dutch,
        Filipino,
        French,
        German,
        Italian,
        Japanese,
        Spanish,
        SChinese,
        TChinese,
        Irish
    }

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

        if (!string.IsNullOrEmpty(this.transPath) &&
            !string.IsNullOrEmpty(this.configPath))
        {
            ReplaceConfigValue(this.configPath);
            ExportTo(Path.Combine(this.transPath));
        }
        ExportTo(Path.Combine(IExporter.ExportDefaultPath, translaterPath));
    }
    private void ExportTo(string path)
    {
        string? directoryFolder = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(directoryFolder)) { return; }

        if (!Directory.Exists(directoryFolder))
        {
            Directory.CreateDirectory(directoryFolder);
        }
        bool isFileExist = File.Exists(path);

        using StreamWriter transCsv = new StreamWriter(
            path, isFileExist, new UTF8Encoding(true));

        if (!isFileExist)
        {
            List<string> langList = new List<string>();

            foreach (SupportedLangs enumValue in supportLnag.Keys)
            {
                langList.Add(enumValue.ToString());
            }

            transCsv.WriteLine(
                string.Format(
                    "{1}{0}{2}",
                    comma,
                    "TransKey",
                    string.Join(comma, langList)));
        }

        foreach (string line in this.writeLineData)
        {
            transCsv.WriteLine(line);
        }
    }

    private void ReplaceConfigValue(string cfgFile)
    {
        string text;
        using (var cfg = new StreamReader(cfgFile, new UTF8Encoding(true)))
        {
            text = cfg.ReadToEnd();
        }
        text = text.Replace(creatorStr, replacedCreatorModeStr);

        using var newCfg = new StreamWriter(cfgFile, false, new UTF8Encoding(true));
        newCfg.Write(text);
    }
}
