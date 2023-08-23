using ExtremeSkins.Generator.Panel.Models;

using System.Collections.ObjectModel;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IExtremeHatModel : ICosmicModel
{
    public ObservableCollection<SkinRowModel> ImgRows { get; }
}
