using ExtremeSkins.Generator.Panel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IExtremeHatModel : IExportModel
{
    public ObservableCollection<SkinRowModel> ImgRows { get; }
}
