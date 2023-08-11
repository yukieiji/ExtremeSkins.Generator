using ExtremeSkins.Core;
using ExtremeSkins.Generator.Panel.Interfaces;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class SkinRowModel : BindableBase
{
    public sealed record FilePath(string Path, Guid id);

    public ReactivePropertySlim<string> ImgPath { get; set; }
    public ReactivePropertySlim<bool> IsAnimation { get; set;  }

    public ReactiveProperty<int> FrameCount { get; set; }
    public AnimationInfo.ImageSelection AnimationType { get; set; } = AnimationInfo.ImageSelection.Sequential;

    public ReactiveCollection<FilePath> FileList { get; } = new ReactiveCollection<FilePath>();


    private readonly IContainerProvider provider;

    public SkinRowModel(IContainerProvider provider)
    {
        this.ImgPath = new ReactivePropertySlim<string>("");
        this.IsAnimation = new ReactivePropertySlim<bool>(false);
        this.FrameCount = new ReactiveProperty<int>(1);

        this.provider = provider;
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
