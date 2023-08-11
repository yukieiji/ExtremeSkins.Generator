using Prism.Commands;
using Reactive.Bindings;

using System;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IFileListItem : IDisposable
{
    public ReactivePropertySlim<string> FilePath { get; }

    public DelegateCommand RemoveSelf { get; }
}
