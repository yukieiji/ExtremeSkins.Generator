using ExtremeSkins.Core;

namespace ExtremeSkins.Generator.Core.Interface;

public interface IInfoHasExporter<T> : ISkinExporter
    where T : InfoBase
{
    public string FolderName { get; }

    public T Info { init; }
}
