using ExtremeSkins.Generator.Panel.Models;

using System.Threading.Tasks;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IExtremeVisorModel : IExportModel
{
    public SkinRowModel Idle { get; }
    public SkinRowModel LeftIdle { get; }

    public Task<bool> IsExVEnable();
}
