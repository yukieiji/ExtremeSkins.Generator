using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeSkins.Generator.Panel.Models;

public abstract class ExportModelBase : BindableBase
{
    public enum ExportResult
    {
        Success,
        FrontImgMissing
    }

    public abstract ExportResult Export();
}
