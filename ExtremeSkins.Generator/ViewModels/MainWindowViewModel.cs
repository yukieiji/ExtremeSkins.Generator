using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

using System.Windows;

using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Event;

namespace ExtremeSkins.Generator.ViewModels;

public sealed class MainWindowViewModel : BindableBase
{
    public string Title => "ExtremeSkins.Generator";

    private readonly IRegionManager regionManager;
    private readonly ICommonDialogService<FileDialogService.Result> commonDialogService;
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
        ICommonDialogService<FileDialogService.Result> comDlgService)
    {
        this.ea = ea;
        this.regionManager = regionManager;

        this.commonDialogService = comDlgService;

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

        var settings = new FileDialogService.Setting
        {
            Filter = "Among Us.exe |*.exe",
            Title = (string)Application.Current.MainWindow.Resources["SetAmongUsPath"],
        };
        
        bool result = this.commonDialogService.ShowDialog(settings);
        
        if (!result)
        {
            return;
        }

        this.AmongUsPathText = settings.Result.FileName;
        this.ea.GetEvent<AmongUsPathSetEvent>().Publish(this.AmongUsPathText);
    }
}
