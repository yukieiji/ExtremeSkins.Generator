using Reactive.Bindings;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IAmongUsPathContainerModel
{
    public ReactivePropertySlim<string> AmongUsExePath { get; }

    public string AmongUsFolderPath { get; }
}
