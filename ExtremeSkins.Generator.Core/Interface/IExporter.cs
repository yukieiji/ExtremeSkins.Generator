namespace ExtremeSkins.Generator.Core.Interface;

public interface IExporter
{
    public string AmongUsPath { init; }

    public const string ExportDefaultPath = "outputs";

    public void Export(bool isOverride);
}
