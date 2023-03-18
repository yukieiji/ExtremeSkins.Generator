using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExtremeSkins.Generator.Service.Interface;

namespace ExtremeSkins.Generator.Service;

public sealed class OpenExplorerService : IOpenExplorerService
{
    public class Setting : IOpenExplorerSetting
    {
        public string Arg { get; init; } = string.Empty;
        public string TargetPath { get; init; } = string.Empty;
    }

    public void Open(IOpenExplorerSetting setting)
    {
        if (setting is not Setting openSetting ||
            string.IsNullOrEmpty(openSetting.TargetPath))
        {
            return;
        }

        string targetPath = openSetting.TargetPath;
        string arg = openSetting.Arg;

        string processArg = 
            string.IsNullOrEmpty(arg) ? 
            $@"""{targetPath}""" :
            $@"{arg}, ""{targetPath}""";

        System.Diagnostics.Process.Start("EXPLORER.EXE", processArg);
    }
}
