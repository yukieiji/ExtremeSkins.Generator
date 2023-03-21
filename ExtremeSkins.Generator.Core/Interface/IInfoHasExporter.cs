using ExtremeSkins.Core;

namespace ExtremeSkins.Generator.Core.Interface;

public interface IInfoHasExporter<T> : ISkinExporter
    where T : InfoBase
{
    public T Info { init; }
}
