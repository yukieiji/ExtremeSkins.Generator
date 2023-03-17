using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

using System.Windows;
using System.IO;

using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Event;

namespace ExtremeSkins.Generator.ViewModels;

// TODO: 出力したフォルダを出せるようにする
public sealed class MainWindowViewModel : BindableBase
{
    public string Title => "ExtremeSkins.Generator";

    private readonly IRegionManager regionManager;
    private readonly ICommonDialogService<FileDialogService.Result> commonDialogService;
    private readonly IWindowsDialogService windowDlgService;
    private readonly IEventAggregator ea;

    public DelegateCommand<string> RadioCheckCommand { get; private set; }

    public string AmongUsPathText
    {
        get { return amongUsPathText; }
        set { SetProperty(ref amongUsPathText, value); }
    }
    private string amongUsPathText;

    public DelegateCommand SetAmongUsPathCommand { get; private set; }

    public MainWindowViewModel(
        IEventAggregator ea,
        IRegionManager regionManager,
        IDialogService dialogService,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowDlgService)
    {
        this.ea = ea;
        this.regionManager = regionManager;
        this.commonDialogService = comDlgService;
        this.windowDlgService = windowDlgService;

        this.RadioCheckCommand = new DelegateCommand<string>(ChangeExportTarget);
        this.SetAmongUsPathCommand = new DelegateCommand(SetAmongUsPath);
    }

    private void ChangeExportTarget(string target)
    {
        if (string.IsNullOrEmpty(target))
        {
            return;
        }
        this.regionManager.RequestNavigate("ContentRegion", target);
    }

    private void SetAmongUsPath()
    {

        var resource = Application.Current.MainWindow.Resources;

        var settings = new FileDialogService.Setting
        {
            Filter = "Among Us.exe |*.exe",
            Title = (string)resource["SetAmongUsPath"],
        };
        
        bool result = this.commonDialogService.ShowDialog(settings);
        
        if (!result)
        {
            return;
        }

        string amongUsExePath = settings.Result.FileName;
        if (Path.HasExtension(amongUsExePath))
        {
            amongUsExePath = Path.GetDirectoryName(amongUsExePath);
        }

        if (!File.Exists(Path.Combine(amongUsExePath, @"BepInEx/plugins/ExtremeSkins.dll")))
        {
            this.windowDlgService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotFindExS"],
                });
            return;
        }
        if (!File.Exists(Path.Combine(amongUsExePath, @"BepInEx/config/me.yukieiji.extremeskins.cfg")))
        {
            this.windowDlgService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotFindExSConfig"],
                });
            return;
        }

        this.AmongUsPathText = amongUsExePath;
        this.ea.GetEvent<AmongUsPathSetEvent>().Publish(this.AmongUsPathText);
    }
}
