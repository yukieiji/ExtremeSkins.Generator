﻿using Prism.Mvvm;

using Reactive.Bindings;

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using ExtremeSkins.Core.ExtremeVisor;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;

using static ExtremeSkins.Generator.Panel.Interfaces.IExportModel;

using ExtremeSkins.Core.API;
using System.Net.Http.Json;
using System.IO;


namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeVisorModel : BindableBase, IExtremeVisorModel
{
    public IApiServerModel ApiServerModel { get; set; }
    public IAmongUsPathContainerModel AmongUsPathContainer { get; }
    public ReactivePropertySlim<string> SkinName { get; }
    public ReactivePropertySlim<string> AutherName { get; }
    public ReactivePropertySlim<bool> IsBehindHat { get; }
    public ReactivePropertySlim<bool> IsShader { get; }
    public ReactivePropertySlim<string> LicencePath { get; }

    public SkinRowModel Idle { get; }
    public SkinRowModel LeftIdle { get; }

    private ExtremeVisorExporter? exporter = null;
    private TranslationExporter? transDataExporter = null;

    public ExtremeVisorModel(
        IApiServerModel apiServerModel,
        IAmongUsPathContainerModel auModel)
    {
        this.ApiServerModel = apiServerModel;
        this.AmongUsPathContainer = auModel;
        this.SkinName = new ReactivePropertySlim<string>("");
        this.AutherName = new ReactivePropertySlim<string>("");
        this.LicencePath = new ReactivePropertySlim<string>("");
        this.IsBehindHat = new ReactivePropertySlim<bool>(false);
        this.IsShader = new ReactivePropertySlim<bool>(false);

        Idle = new SkinRowModel("ExV.SelectFrontImage");
        LeftIdle = new SkinRowModel("ExV.SelectFrontFlipImage");
    }

    public string Export(bool isOverride)
    {
        try
        {
            if (this.exporter == null)
            {
                this.CreateExporter();
            }
            this.exporter!.Export(isOverride);
            this.transDataExporter?.Export();
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public ExportStatus GetCurrentExportStatus()
    {
        if (string.IsNullOrEmpty(this.AutherName.Value))
        {
            return ExportStatus.MissingAutherName;
        }
        else if (string.IsNullOrEmpty(this.SkinName.Value))
        {
            return ExportStatus.MissingSkinName;
        }
        else if (string.IsNullOrEmpty(GetImgRow("ExH.SelectFrontImage").ImgPath.Value))
        {
            return ExportStatus.MissingFrontImg;
        }
        else
        {
            return ExportStatus.NoProblem;
        }
    }

    public SameSkinCheckResult GetSameSkinStatus()
    {
        this.CreateExporter();
        return this.exporter!.CheckSameSkin();
    }

    public async Task<bool> IsExHEnable()
    {
        var respons = await this.ApiServerModel.GetAmongUsStatusAsync();
        if (respons == null || !respons.IsSuccessStatusCode)
        {
            return false;
        }
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = { new JsonStringEnumConverter() },
        };
        StatusData status = await respons.Content.ReadFromJsonAsync<StatusData>(options);
        if (status.Status == ExSStatus.Booting)
        {
            return false;
        }

        return status.Module.ExtremeHat == ModuleStatus.Arrive;
    }

    public async Task<bool> HotReloadCosmic()
    {
        string outputParentPath = Path.GetFullPath(
            Path.Combine(IExporter.ExportDefaultPath, DataStructure.FolderName));
        InfoData newHatData = new InfoData(outputParentPath, this.exporter!.FolderName);

        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        var postRespons = await this.ApiServerModel.PostAsync("hat/", newHatData, options);
        if (postRespons != null && postRespons.IsSuccessStatusCode)
        {
            return true;
        }

        var respons = await this.ApiServerModel.PutAsync("hat/", newHatData);
        return respons != null && respons.IsSuccessStatusCode;
    }
    private void CreateExporter(bool exportWithAu=true)
    {

        this.transDataExporter = null;

        var frontRow = GetImgRow("ExH.SelectFrontImage");
        var frontFlipRow = GetImgRow("ExH.SelectFrontFlipImage");
        var backRow = GetImgRow("ExH.SelectBackImage");
        var backFlipRow = GetImgRow("ExH.SelectBackFlipImage");
        var climbRow = GetImgRow("ExH.SelectClimbImage");

        var frontAnimationExportInfo = CrateAnimationExportInfo(frontRow);
        var frontFlipAnimationExportInfo = CrateAnimationExportInfo(frontFlipRow);
        var backAnimationExportInfo = CrateAnimationExportInfo(backRow);
        var backFlipAnimationExportInfo = CrateAnimationExportInfo(backFlipRow);
        var climbAnimationExportInfo = CrateAnimationExportInfo(climbRow);

        var hatAnimation =
            frontAnimationExportInfo.Info == null &&
            frontFlipAnimationExportInfo.Info == null &&
            backAnimationExportInfo.Info == null &&
            backFlipAnimationExportInfo.Info == null &&
            climbAnimationExportInfo.Info == null ?
            null : new HatAnimation(
                frontAnimationExportInfo.Info,
                frontFlipAnimationExportInfo.Info,
                backAnimationExportInfo.Info,
                backFlipAnimationExportInfo.Info,
                climbAnimationExportInfo.Info);

        Dictionary<string, string> replacedStr = new Dictionary<string, string>();

        string autherName = this.AutherName.Value;
        if (TryReplaceAscii(autherName, out string asciiedAutherName))
        {
            replacedStr.Add(asciiedAutherName, autherName);
            autherName = asciiedAutherName;
        }
        string skinName = this.SkinName.Value;
        if (TryReplaceAscii(skinName, out string asciiedSkinName))
        {
            replacedStr.Add(asciiedSkinName, skinName);
            skinName = asciiedSkinName;
        }

        VisorInfo hatInfo = new VisorInfo(
            Name: skinName,
            Author: autherName,
            BehindHat: this.IsBehindHat.Value,
            Shader: this.IsShader.Value,
            LeftIdle: !string.IsNullOrEmpty(frontFlipRow.ImgPath.Value),
            Animation: hatAnimation
        );

        string amongUsPath = exportWithAu ? this.AmongUsPathContainer.AmongUsFolderPath : string.Empty;

        this.exporter = new ExtremeHatsExporter()
        {
            Info = hatInfo,
            AmongUsPath = amongUsPath,
            LicenseFile = this.LicencePath.Value,
        };

        this.exporter.AddImage(
            DataStructure.FrontImageName, frontRow.ImgPath.Value);
        if (hatInfo.FrontFlip)
        {
            this.exporter.AddImage(
                DataStructure.FrontFlipImageName, frontFlipRow.ImgPath.Value);
        }
        if (hatInfo.Back)
        {
            this.exporter.AddImage(
                DataStructure.BackImageName, backRow.ImgPath.Value);
        }
        if (hatInfo.BackFlip)
        {
            this.exporter.AddImage(
                DataStructure.BackFlipImageName, backFlipRow.ImgPath.Value);
        }
        if (hatInfo.Climb)
        {
            this.exporter.AddImage(
                DataStructure.ClimbImageName, climbRow.ImgPath.Value);
        }
        if (hatInfo.Animation != null)
        {
            if (hatInfo.Animation.Front != null &&
                frontAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, frontAnimationExportInfo);
            }
            if (hatInfo.Animation.FrontFlip != null &&
                frontFlipAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, frontFlipAnimationExportInfo);
            }
            if (hatInfo.Animation.Back != null &&
                backAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, backAnimationExportInfo);
            }
            if (hatInfo.Animation.BackFlip != null &&
                backFlipAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, backFlipAnimationExportInfo);
            }
            if (hatInfo.Animation.Climb != null &&
                climbAnimationExportInfo.Info != null)
            {
                AddAnimationImg(this.exporter, climbAnimationExportInfo);
            }
        }

        if (replacedStr.Count != 0)
        {
            this.transDataExporter = new TranslationExporter()
            {
                Locale = (string)Application.Current.MainWindow.Resources["CurLocale"],
                AmongUsPath = amongUsPath,
            };
            this.transDataExporter.AddTransData(replacedStr);
        }
    }

    private SkinRowModel GetImgRow(string key)
        => this.ImgRows.Where(x => x.RowName == key).First();

    private static void AddAnimationImg(ExtremeHatsExporter exporter, AnimationImgExportInfo info)
    {
        exporter.AddFolder(info.FolderName);
        for (int i = 0; i < info.FromImgPath.Length; ++i)
        {
            string toPath = info.Info!.Img[i];
            string fromPath = info.FromImgPath[i];
            exporter.AddImage(toPath, fromPath);
        }
    }

    public Task<bool> IsExVEnable()
    {
        throw new NotImplementedException();
    }
}