using Prism.Mvvm;
using Reactive.Bindings;

using System;
using ExtremeSkins.Core;
using System.Linq;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class SkinRowModel : BindableBase
{
    public sealed record FilePath(string Path, Guid id);

    public ReactivePropertySlim<string> ImgPath { get; set; }
    public ReactivePropertySlim<bool> IsAnimation { get; set;  }

    public ReactiveProperty<uint> FrameCount { get; set; }
    public AnimationInfo.ImageSelection AnimationType { get; set; } = AnimationInfo.ImageSelection.Sequential;

    public string[] Files => FileList.Select(x => x.Path).ToArray();
    public ReactiveCollection<FilePath> FileList { get; } = new ReactiveCollection<FilePath>();

    public SkinRowModel()
    {
        this.ImgPath = new ReactivePropertySlim<string>("");
        this.IsAnimation = new ReactivePropertySlim<bool>(false);
        this.FrameCount = new ReactiveProperty<uint>(1);
    }

    public void AddFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        this.FileList.Add(new FilePath(path, Guid.NewGuid()));
    }

    public void RemoveFile(FilePath file)
    {
        this.FileList.Remove(file);
    }

    public void ClearFileList()
    {
        this.FileList.Clear();
    }

    public void ChangeImageSelection(string parameter)
    {
        if (string.IsNullOrEmpty(parameter))
        {
            return;
        }

        switch (parameter)
        {
            case "Sequential":
                this.AnimationType = AnimationInfo.ImageSelection.Sequential;
                break;
            case "Random":
                this.AnimationType = AnimationInfo.ImageSelection.Random;
                break;
            default:
                break;
        }
    }
}
