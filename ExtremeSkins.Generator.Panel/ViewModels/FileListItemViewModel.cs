using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System.Reactive.Disposables;

using ExtremeSkins.Generator.Panel.Interfaces;
using Prism.Commands;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class FileListItemViewModel : IFileListItemViewModel
{
    public ReactivePropertySlim<string> FilePath { get; }
    public DelegateCommand RemoveSelf { get; set; }

    private CompositeDisposable disposables = new CompositeDisposable();

    public FileListItemViewModel()
    {
        this.FilePath = new ReactivePropertySlim<string>().AddTo(this.disposables);
    }

    public void Dispose()
    {
        this.disposables.Dispose();
    }
}
