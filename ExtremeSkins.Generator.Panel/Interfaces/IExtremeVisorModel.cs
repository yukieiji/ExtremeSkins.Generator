using ExtremeSkins.Generator.Panel.Models;

using System.Collections.ObjectModel;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IExtremeVisorModel : ICosmicModel
{
    public ObservableCollection<SkinRowModel> Rows { get; }

    public SkinRowModel Idle { get; }
    public SkinRowModel LeftIdle { get; }
}
