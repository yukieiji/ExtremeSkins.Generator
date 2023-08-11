using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

using System.Reactive.Disposables;

using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class SkinRowPanelViewModel : BindableBase, ISkinRowViewModel
{
    public ReactivePropertySlim<string> ImgPath { get; }
    public ReactivePropertySlim<bool> IsAnimation { get; }

    [RegularExpression("[0-9]+", ErrorMessage = "数値を入力して下さい")]
    [Range(1, 300, ErrorMessage = "1～300までの整数の値を入力してください")]
    public ReactiveProperty<int> FrameCount { get; }

    public DelegateCommand SelectFileCommand { get; }
    public DelegateCommand AddAnimationFileCommand { get; }
    public DelegateCommand RemoveFileCommand { get; }

    public ReactiveCollection<IFileListItem> FileList { get; } = new ReactiveCollection<IFileListItem>();

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
        this.AddAnimationFileCommand = new DelegateCommand(this.AddFileItem);
        this.RemoveFileCommand = new DelegateCommand(() => this.FileList.Clear());

        this.ImgPath     = new ReactivePropertySlim<string>("").AddTo(this.disposables);
        this.IsAnimation = new ReactivePropertySlim<bool>(false).AddTo(this.disposables);

        this.FrameCount  = new ReactiveProperty<int>(1)
            .SetValidateAttribute(() => this.FrameCount)
            .AddTo(this.disposables);
    }

    public void Destroy()
    {
        this.disposables.Dispose();
    }

    private void AddFileItem()
    {
        string newImg = OpenFileSelectDialog();
        if (string.IsNullOrEmpty(newImg))
        {
            return;
        }
        var item = this.provider.Resolve<IFileListItem>();
        item.FilePath.Value = newImg;
        this.FileList.Add(item);
    }

    private void SelectFile()
    {
        this.ImgPath.Value = OpenFileSelectDialog();
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
