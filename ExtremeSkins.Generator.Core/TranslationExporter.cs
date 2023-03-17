using System.Collections.Generic;
using System.IO;

namespace ExtremeSkins.Generator.Core;

public sealed class TranslationExporter
{
    public string AmongUsPath
    {
        init
        {
            this.transPath = Path.Combine(value, @"CreatorMode/translation.csv");
            this.configPath = Path.Combine(value, @"BepInEx/config/me.yukieiji.extremeskins.cfg");
        }
    }
    private string transPath;
    private string configPath;

    public void AddTransData(Dictionary<string, string> exportData)
    {

    }
}
