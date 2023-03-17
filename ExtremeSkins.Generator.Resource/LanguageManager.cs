using System;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;
using System.Windows.Markup;

namespace ExtremeSkins.Generator.Resource;

public static class LanguageManager
{
    private static ResourceDictionary Main;
    
    public static void Load(string locale)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        System.IO.Stream resourceStream = assembly.GetManifestResourceStream(
            $"ExtremeSkins.Generator.Resource.Language.{locale}.xaml");
        Main = (ResourceDictionary)XamlReader.Load(resourceStream);
    }

    public static ResourceDictionary Get()
    {
        if (Main is null)
        {
            Load("ja-jp");
        };

        return Main;
    }
}
