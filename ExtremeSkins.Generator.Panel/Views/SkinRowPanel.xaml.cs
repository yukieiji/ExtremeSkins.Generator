using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtremeSkins.Generator.Panel.Views;

/// <summary>
/// SkinRowPanel.xaml の相互作用ロジック
/// </summary>
public partial class SkinRowPanel : UserControl
{

    public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
        "TitleText", typeof(string), typeof(SkinRowPanel), new PropertyMetadata(default(string)));

    public string TitleText
    {
        get { return (string)GetValue(TitleTextProperty); }
        set { SetValue(TitleTextProperty, value); }
    }

    public SkinRowPanel()
    {
        InitializeComponent();
    }
}
