using ExtremeSkins.Generator.Panel.Models;

using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IExtremeHatModel : ICosmicModel
{
    public ObservableCollection<SkinRowModel> ImgRows { get; }

    public Task<bool> IsExHEnable();
}
