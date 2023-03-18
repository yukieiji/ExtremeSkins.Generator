namespace ExtremeSkins.Generator.Core.Interface;

public interface IInfoHasExporter<T> : ISkinExporter
    where T : IInfo
{
    public T Info { init; }
}
