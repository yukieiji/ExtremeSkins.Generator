using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

using System.Windows;
using System.IO;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Event;

namespace ExtremeSkins.Generator.ViewModels;

// TODO: 出力したフォルダを出せるようにする
public sealed class MainWindowViewModel : BindableBase
{
    public string Title => "ExtremeSkins.Generator";

    private readonly IRegionManager regionManager;
    private readonly IEventAggregator ea;

    private readonly ICommonDialogService<FileDialogService.Result> commonDialogService;
    private readonly IWindowsDialogService windowDlgService;
    private readonly IOpenExplorerService openExplorerService;

    public DelegateCommand OpenExportedFolderCommand { get; private set; }

    public DelegateCommand<string> RadioCheckCommand { get; private set; }

    public string AmongUsPathText
    {
        get { return amongUsPathText; }
        set { SetProperty(ref amongUsPathText, value); }
    }
    private string amongUsPathText = string.Empty;
    private string amongUsFolderPath = string.Empty;

    public DelegateCommand SetAmongUsPathCommand { get; private set; }

    public MainWindowViewModel(
        IEventAggregator ea,
        IRegionManager regionManager,
        IDialogService dialogService,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowDlgService,
        IOpenExplorerService openExplorerService)
    {
        this.ea = ea;
        this.ea.GetEvent<AmongUsPathGetEvent>().Subscribe(GetAmongUsPath);

        this.regionManager = regionManager;
        this.commonDialogService = comDlgService;
        this.windowDlgService = windowDlgService;
        this.openExplorerService = openExplorerService;

        this.OpenExportedFolderCommand = new DelegateCommand(OpenExportedFolder);
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

    private void GetAmongUsPath()
    {
        this.ea.GetEvent<AmongUsPathSetEvent>().Publish(this.amongUsFolderPath);
    }

    private void OpenExportedFolder()
    {
        string curDirPath = Directory.GetCurrentDirectory();
        string exportedDir = Path.Combine(curDirPath, IExporter.ExportDefaultPath);
        if (!Directory.Exists(exportedDir))
        {
            Directory.CreateDirectory(exportedDir);
        }
        this.openExplorerService.Open(new OpenExplorerService.Setting
        {
            TargetPath = exportedDir,
        });
    }

    private void SetAmongUsPath()
    {
        var resource = Application.Current.MainWindow.Resources;

        var settings = new FileDialogService.Setting
        {
            Filter = "Among Us.exe |*.exe",
            Title = (string)resource["SetAmongUsPath"],
        };
        
        var result = this.commonDialogService.ShowDialog(settings);
        
        if (result.State != ICommonDialogResult.DialogShowState.Ok)
        {
            return;
        }

        this.AmongUsPathText = result.FileName;
        this.amongUsFolderPath = this.AmongUsPathText;
        if (Path.HasExtension(this.amongUsFolderPath))
        {
            this.amongUsFolderPath = Path.GetDirectoryName(this.amongUsFolderPath);
        }

        if (!File.Exists(Path.Combine(this.amongUsFolderPath, @"BepInEx/plugins/ExtremeSkins.dll")))
        {
            this.windowDlgService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotFindExS"],
                });
            return;
        }
        if (!File.Exists(Path.Combine(
                this.amongUsFolderPath,
                Config.Path)))
        {
            this.windowDlgService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotFindExSConfig"],
                });
            return;
        }

        this.ea.GetEvent<AmongUsPathSetEvent>().Publish(this.amongUsFolderPath);
    }
}
