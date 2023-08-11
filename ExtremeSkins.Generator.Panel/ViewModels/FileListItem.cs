using Prism.Commands;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using System.Reactive.Disposables;

using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.ViewModels;

public sealed class FileListItem : BindableBase, IFileListItem
{
    public ReactivePropertySlim<string> FilePath { get; }
    public DelegateCommand RemoveSelf { get; }

    private CompositeDisposable disposables = new CompositeDisposable();

    public FileListItem()
    {
        this.FilePath = new ReactivePropertySlim<string>().AddTo(this.disposables);
        this.RemoveSelf = new DelegateCommand(this.Dispose);

    }
    public void Dispose()
    {
        this.disposables.Dispose();
    }
}
