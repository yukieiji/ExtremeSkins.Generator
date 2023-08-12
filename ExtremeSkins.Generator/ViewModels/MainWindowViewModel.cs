using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Mvvm;
using Prism.Regions;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System.Windows;
using System.IO;
using System.Reactive.Disposables;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Event;
using ExtremeSkins.Generator.Models;

namespace ExtremeSkins.Generator.ViewModels;

// TODO: 出力したフォルダを出せるようにする
public sealed class MainWindowViewModel : BindableBase, IDestructible
{
    public string Title => "ExtremeSkins.Generator";

    private readonly IRegionManager regionManager;
    private readonly IEventAggregator ea;

    private readonly ICommonDialogService<FileDialogService.Result> commonDialogService;
    private readonly IWindowsDialogService windowDlgService;
    private readonly IOpenExplorerService openExplorerService;
    private readonly IMainWindowModel model;

    public DelegateCommand OpenExportedFolderCommand { get; private set; }
    public DelegateCommand ExportZipFolderCommand { get; private set; }

    public DelegateCommand<string> RadioCheckCommand { get; private set; }

    public ReactivePropertySlim<string> AmongUsPathText { get; }
    private string amongUsFolderPath = string.Empty;

    public DelegateCommand SetAmongUsPathCommand { get; private set; }

    private CompositeDisposable disposables = new CompositeDisposable();

    public MainWindowViewModel(
        IMainWindowModel model,
        IEventAggregator ea,
        IRegionManager regionManager,
        ICommonDialogService<FileDialogService.Result> comDlgService,
        IWindowsDialogService windowDlgService,
        IOpenExplorerService openExplorerService)
    {
        this.model = model;
        this.ea = ea;
        this.ea.GetEvent<AmongUsPathGetEvent>().Subscribe(GetAmongUsPath);

        this.regionManager = regionManager;
        this.commonDialogService = comDlgService;
        this.windowDlgService = windowDlgService;
        this.openExplorerService = openExplorerService;

        this.OpenExportedFolderCommand = new DelegateCommand(OpenExportedFolder);
        this.RadioCheckCommand = new DelegateCommand<string>(ChangeExportTarget);
        this.SetAmongUsPathCommand = new DelegateCommand(SetAmongUsPath);

        this.ExportZipFolderCommand = new DelegateCommand(ExportZipFile);

        this.AmongUsPathText = model.AmongUsPathContainer.AmongUsExePath
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.disposables);
    }

    public void Destroy()
    {
        this.disposables.Dispose();
    }

    private void ExportZipFile()
    {
        var resource = Application.Current.MainWindow.Resources;

        string fileName = this.model.ExportToZip();
        if (string.IsNullOrEmpty(fileName))
        {
            this.windowDlgService.Show(
                new MessageShowService.ErrorMessageSetting()
                {
                    Title = (string)resource["Error"],
                    Message = (string)resource["CannotExportToZip"],
                });

        }
        else
        {
            var showResult = this.windowDlgService.Show(
                new MessageShowService.InfoMessageSetting()
                {
                    Title = (string)resource["Success"],
                    Message = (string)resource["SuccessExportToZip"],
                });
            if (showResult == MessageBoxResult.OK)
            {
                System.Diagnostics.Process.Start(
                    "EXPLORER.EXE", $@"/select, ""{fileName}""");
            }
        }
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

        this.amongUsFolderPath = result.FileName;
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
        this.AmongUsPathText.Value = result.FileName;
        this.ea.GetEvent<AmongUsPathSetEvent>().Publish(this.amongUsFolderPath);
    }
}
