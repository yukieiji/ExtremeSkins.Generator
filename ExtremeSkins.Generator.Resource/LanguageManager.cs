using System.Reflection;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace ExtremeSkins.Generator.Resource;

public static class LanguageManager
{
    private const string DefaultLocale = "ja-JP";
    private static ResourceDictionary Main;
    private static HashSet<string> supportLang => new HashSet<string>()
    {
        "ja-JP"
    };

    public static void Load(string locale)
    {
        if (!supportLang.Contains(locale))
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
