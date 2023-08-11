using Prism.Navigation;
using Reactive.Bindings;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface ISkinRowViewModel : IDestructible
{
    public ReactivePropertySlim<string> ImgPath { get; }
    public ReactivePropertySlim<bool> IsAnimation { get; }

    public ReactiveProperty<int> FrameCount { get; }

    public ReactiveCollection<IFileListItemViewModel> FileList { get; }
}
