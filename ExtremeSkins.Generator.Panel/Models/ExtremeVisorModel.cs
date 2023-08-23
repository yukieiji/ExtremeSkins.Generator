using Prism.Mvvm;

using Reactive.Bindings;

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using ExtremeSkins.Core.API;
using ExtremeSkins.Core.ExtremeVisor;
using ExtremeSkins.Generator.Core;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;

using static ExtremeSkins.Generator.Panel.Interfaces.ICosmicModel;

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

    public ObservableCollection<SkinRowModel> ImgRows { get; }

    private SkinRowModel idle { get; }
    private SkinRowModel leftIdle { get; }

    private ExtremeVisorExporterModel? exporter = null;

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

        idle = new SkinRowModel("ExV.SelectFrontImage");
        leftIdle = new SkinRowModel("ExV.SelectFrontFlipImage");

        ImgRows = new ObservableCollection<SkinRowModel>()
        {
            idle, leftIdle,
        };
    }

    public string Export(bool isOverride)
    {
        try
        {
            if (this.exporter == null)
            {
                this.exporter = this.CreateExporter();
            }
            this.exporter.Export(isOverride);
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
        else if (string.IsNullOrEmpty(this.idle.ImgPath.Value))
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
        this.exporter = this.CreateExporter();
        return this.exporter.GetSameSkinCheckResult();
    }

    public async Task<bool> IsModuleEnable()
    {
        try
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

            return status.Module.ExtremeVisor == ModuleStatus.Arrive;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HotReloadCosmic()
    {
        string outputParentPath = Path.GetFullPath(
            Path.Combine(IExporter.ExportDefaultPath, DataStructure.FolderName));
        NewCosmicData newHatData = new NewCosmicData(
            outputParentPath,
            this.exporter!.FolderName,
            this.exporter!.AutherName,
            this.exporter!.TranslatedSkinName,
            this.exporter!.TranslatedAuthorName);

        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };
        try
        {
            try
            {
                var postRespons = await this.ApiServerModel.PostAsync("visor/", newHatData, options);
                return postRespons != null && postRespons.IsSuccessStatusCode;
            }
            catch
            {
                var respons = await this.ApiServerModel.PutAsync("visor/", newHatData);
                return respons != null && respons.IsSuccessStatusCode;
            }
        }
        catch
        {
            return false;
        }
    }

    private ExtremeVisorExporterModel CreateExporter(bool exportWithAu = true)
    {
        string amongUsPath = exportWithAu ? this.AmongUsPathContainer.AmongUsFolderPath : string.Empty;

        return new ExtremeVisorExporterModel(
            amongUsPath,
            this.AutherName.Value,
            this.SkinName.Value,
            this.LicencePath.Value,
            this.IsBehindHat.Value,
            this.IsShader.Value,
            this.idle,
            this.leftIdle);
    }
}
