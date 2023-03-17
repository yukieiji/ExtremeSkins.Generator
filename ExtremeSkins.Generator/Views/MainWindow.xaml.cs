
using System.Windows;
using ExtremeSkins.Generator.Resource;

namespace ExtremeSkins.Generator.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Resources.MergedDictionaries.Add(LanguageManager.Get());
    }
}
