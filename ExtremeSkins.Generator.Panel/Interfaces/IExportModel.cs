using Reactive.Bindings;
namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IExportModel
{
    public enum ExportResult
    {
        Success,
        FrontImgMissing
    }

    public IAmongUsPathContainerModel AmongUsPathContainer { get; }

    public ReactivePropertySlim<string> SkinName { get; }
    public ReactivePropertySlim<string> AutherName { get; }

    public ExportResult Export();
}
