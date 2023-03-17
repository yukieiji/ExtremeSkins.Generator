namespace ExtremeSkins.Generator.Core.Interface;

public interface IInfoHasExporter<T> : IExporter
    where T : IInfo
{
    public T Info { init; }
}
