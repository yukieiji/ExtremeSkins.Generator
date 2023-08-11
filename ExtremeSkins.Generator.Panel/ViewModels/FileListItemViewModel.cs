using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

using ExtremeSkins.Generator.Panel.Interfaces;
using Prism.Commands;
using ExtremeSkins.Generator.Panel.Models;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class FileListItemViewModel : IFileListItemViewModel
{
    public ReactivePropertySlim<string> FilePath { get; }
    public DelegateCommand RemoveSelf { get; }

    private CompositeDisposable disposables = new CompositeDisposable();

    public FileListItemViewModel(SkinRowModel model, KeyValuePair<Guid, string> fileData)
    {
        this.FilePath = new ReactivePropertySlim<string>(fileData.Value).AddTo(this.disposables);
        this.RemoveSelf = new DelegateCommand(() => model.RemoveFile(fileData));
    }

    public void Dispose()
    {
        this.disposables.Dispose();
    }
}
