using System.IO;

using Reactive.Bindings;
using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class AmongUsPathContainerModel : IAmongUsPathContainerModel
{
    public ReactivePropertySlim<string> AmongUsExePath { get; }

    public string AmongUsFolderPath
    {
        get
        {
            string value = this.AmongUsExePath.Value;
            if (Path.HasExtension(value))
            {
                string? dirName = Path.GetDirectoryName(value);
                if (string.IsNullOrEmpty(dirName))
                {
                    return value;
                }
                value = dirName;
            }
            return value;
        }
    }

    public AmongUsPathContainerModel()
    {
        this.AmongUsExePath = new ReactivePropertySlim<string>();
    }
}
