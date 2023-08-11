using Prism.Commands;
using Reactive.Bindings;

using System;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IFileListItemViewModel : IDisposable
{
    public ReactivePropertySlim<string> FilePath { get; }

    public DelegateCommand RemoveSelf { get; set; }
}
