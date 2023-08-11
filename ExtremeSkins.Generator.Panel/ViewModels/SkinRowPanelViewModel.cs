using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

using System.Reactive.Disposables;

using ExtremeSkins.Core;
using ExtremeSkins.Generator.Service.Interface;
using ExtremeSkins.Generator.Service;
using ExtremeSkins.Generator.Panel.Interfaces;
using ExtremeSkins.Generator.Panel.Models;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class SkinRowPanelViewModel : BindableBase, ISkinRowViewModel
{
    public ReactivePropertySlim<string> ImgPath { get; }
    public ReactivePropertySlim<bool> IsAnimation { get; }
    public ReadOnlyReactiveCollection<IFileListItemViewModel> FileList { get; }

    [RegularExpression("[0-9]+", ErrorMessage = "数値を入力して下さい")]
    [Range(1, 300, ErrorMessage = "1～300までの整数の値を入力してください")]
    public ReactiveProperty<int> FrameCount { get; }

    public DelegateCommand<string> RadioCheckCommand { get; }
    public DelegateCommand SelectFileCommand { get; }
    public DelegateCommand AddAnimationFileCommand { get; }
    public DelegateCommand RemoveFileCommand { get; }

    private readonly ICommonDialogService<FileDialogService.Result> fileDialogService;

    private CompositeDisposable disposables = new CompositeDisposable();

    private readonly SkinRowModel model;

    public SkinRowPanelViewModel(
        SkinRowModel model,
        ICommonDialogService<FileDialogService.Result> comDlgService)
    {
        this.fileDialogService = comDlgService;
        this.model = model;

        this.SelectFileCommand = new DelegateCommand(this.SelectFile);
        this.AddAnimationFileCommand = new DelegateCommand(this.AddFileItem);
        this.RemoveFileCommand = new DelegateCommand(this.model.ClearFileList);
        this.RadioCheckCommand = new DelegateCommand<string>(this.model.ChangeImageSelection);

        this.ImgPath = this.model.ImgPath
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.disposables);
        this.IsAnimation = this.model.IsAnimation
            .ToReactivePropertySlimAsSynchronized(x => x.Value)
            .AddTo(this.disposables);

        this.FrameCount = model.FrameCount
            .ToReactivePropertyAsSynchronized(
                x => x.Value,
                ignoreValidationErrorValue: true)
            .AddTo(this.disposables);
        this.FileList = this.model.FileList
            .ToReadOnlyReactiveCollection(x => (IFileListItemViewModel)new FileListItemViewModel(model, x))
            .AddTo(this.disposables);
    }

    public void Destroy()
    {
        this.disposables.Dispose();
    }

    private void AddFileItem()
    {
        string newImg = OpenFileSelectDialog();
        this.model.AddFile(newImg);
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
