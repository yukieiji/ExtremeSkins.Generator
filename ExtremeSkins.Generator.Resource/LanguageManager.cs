using System.Reflection;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

using SupportedLangs = ExtremeSkins.Core.CreatorMode.SupportedLangs;

namespace ExtremeSkins.Generator.Resource;

public static class LanguageManager
{
    private const string DefaultLocale = "ja-JP";
    private static ResourceDictionary Main;
    
    public static readonly Dictionary<SupportedLangs, string> SupportLang = 
        new Dictionary<SupportedLangs, string>()
    {
        {SupportedLangs.Japanese, "ja-JP"},
        {SupportedLangs.SChinese, "zh-CN"},
    };

    public static void Load(string locale)
    {
        if (!SupportLang.ContainsValue(locale))
        {
            locale = DefaultLocale;
        }

        Assembly assembly = Assembly.GetExecutingAssembly();
        System.IO.Stream resourceStream = assembly.GetManifestResourceStream(
            $"ExtremeSkins.Generator.Resource.Language.{locale}.xaml");
        Main = (ResourceDictionary)XamlReader.Load(resourceStream);
        Main["CurLocale"] = locale;
    }

    public static ResourceDictionary Get()
    {
        if (Main is null)
        {
            Load(DefaultLocale);
        };

        return Main;
    }
}
