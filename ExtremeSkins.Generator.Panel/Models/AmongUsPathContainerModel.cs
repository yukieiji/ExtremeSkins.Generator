using Reactive.Bindings;
using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class AmongUsPathContainerModel : IAmongUsPathContainerModel
{
    public ReactivePropertySlim<string> AmongUsPath { get; }
    public AmongUsPathContainerModel()
    {
        this.AmongUsPath = new ReactivePropertySlim<string>();
    }
}
