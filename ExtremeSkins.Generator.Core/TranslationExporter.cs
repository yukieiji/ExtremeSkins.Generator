using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Core.Interface;

using LangMng = ExtremeSkins.Generator.Resource.LanguageManager;
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

    public void AddTransData(Dictionary<string, string> exportData)
    {
        foreach (var (transKey, trans) in exportData)
        {
            StringBuilder builder = new StringBuilder(13);
            builder.Append(transKey).Append(comma);

            foreach (var local in Enum.GetValues<SupportedLangs>())
            {
                builder.Append(
                    LangMng.SupportLang.TryGetValue(local, out string localStr) &&
                    localStr == this.Locale ? trans : string.Empty).Append(comma);
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
        ExportTo(Path.Combine(Directory.GetCurrentDirectory(), 
            IExporter.ExportDefaultPath));
    }
    private void ExportTo(string path)
    {
        if (string.IsNullOrEmpty(path)) { return; }

        using StreamWriter transCsv = CreatorMode.GetTranslationWriter(
            path);

        foreach (string line in this.writeLineData)
        {
            transCsv.WriteLine(line);
        }
    }
}
