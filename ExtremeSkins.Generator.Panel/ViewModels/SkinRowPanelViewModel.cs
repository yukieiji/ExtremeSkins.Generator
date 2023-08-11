using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

using System.Collections.Generic;
using System.Windows;

using System.Reactive.Disposables;

using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Core.Interface;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel.DataAnnotations;
using Prism.Ioc;
using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class SkinRowPanelViewModel : BindableBase, ISkinRowPanel
{
    public ReactivePropertySlim<string> TitleName { get; }
    public ReactivePropertySlim<string> ImgPath { get; }
    public ReactivePropertySlim<bool> IsAnimation { get; }

    [Range(1, 300)]
    public ReactiveProperty<int> FrameCount { get; }

    public DelegateCommand SelectFileCommand { get; private set; }
    public ReactiveCollection<IFileListItem> FileList { get; }

    private readonly ICommonDialogService<FileDialogService.Result> fileDialogService;
    private readonly IContainerProvider provider;

    private CompositeDisposable disposables = new CompositeDisposable();

    public SkinRowPanelViewModel(
        IContainerProvider provider,
        ICommonDialogService<FileDialogService.Result> comDlgService)
    {
        this.provider = provider;
        this.fileDialogService = comDlgService;
        this.SelectFileCommand = new DelegateCommand(this.SelectFile);


        this.TitleName   = new ReactivePropertySlim<string>("SelectImg").AddTo(this.disposables);
        this.ImgPath     = new ReactivePropertySlim<string>().AddTo(this.disposables);
        this.IsAnimation = new ReactivePropertySlim<bool>(false).AddTo(this.disposables);
        this.FrameCount  = new ReactiveProperty<int>(1).AddTo(this.disposables);
    }

    private void SelectFile()
    {
        this.ImgPath.Value = OpenFileSelectDialog();
    }

    private void AddFileItem()
    {
        string newImg = OpenFileSelectDialog();
        if (string.IsNullOrEmpty(newImg))
        {
            return;
        }
        var newItem = this.provider.Resolve<IFileListItem>();

    }

    private string OpenFileSelectDialog()
    {
        var resource = Application.Current.MainWindow.Resources;

        string fileFilter = (string)resource["PngFilter"];
        var settings = new FileDialogService.Setting
        {
            Filter = $"{fileFilter}(*.png)|*.png",
            Title = (string)resource[$"PngTitle"],
        };

        var result = this.fileDialogService.ShowDialog(settings);

        return result.FileName;
    }
}
