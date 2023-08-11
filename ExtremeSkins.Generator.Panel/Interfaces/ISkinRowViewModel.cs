using Prism.Navigation;

using Reactive.Bindings;

using ExtremeSkins.Core;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface ISkinRowViewModel : IDestructible
{
    public ReactivePropertySlim<string> ImgPath { get; }
    public ReactivePropertySlim<bool> IsAnimation { get; }

    public ReactiveProperty<int> FrameCount { get; }

    public ReactiveCollection<IFileListItemViewModel> FileList { get; }

    public AnimationInfo.ImageSelection AnimationType { get; }
}
