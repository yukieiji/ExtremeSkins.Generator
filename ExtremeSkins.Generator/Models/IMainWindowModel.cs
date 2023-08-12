using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Models;

public interface IMainWindowModel
{
    public IAmongUsPathContainerModel AmongUsPathContainer { get; }

    public bool ExportToZip();
}
