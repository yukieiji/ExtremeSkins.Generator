using Prism.Mvvm;

using Reactive.Bindings;

using System;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using ExtremeSkins.Core.API;
using ExtremeSkins.Core.ExtremeHats;
using ExtremeSkins.Generator.Core.Interface;
using ExtremeSkins.Generator.Panel.Interfaces;

using static ExtremeSkins.Generator.Panel.Interfaces.ICosmicModel;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ExtremeHatModel : BindableBase, IExtremeHatModel
{
    public IApiServerModel ApiServerModel { get; set; }
    public IAmongUsPathContainerModel AmongUsPathContainer { get; }
    public ReactivePropertySlim<string> SkinName { get; }
    public ReactivePropertySlim<string> AutherName { get; }
    public ReactivePropertySlim<bool> IsBounce { get; }
    public ReactivePropertySlim<bool> IsShader { get; }
    public ReactivePropertySlim<string> LicencePath { get; }
    public ObservableCollection<SkinRowModel> ImgRows { get; }

    private ExtremeHatExporterModel? exporter = null;

    public ExtremeHatModel(
        IApiServerModel apiServerModel,
        IAmongUsPathContainerModel auModel)
    {
        this.ApiServerModel = apiServerModel;
        this.AmongUsPathContainer = auModel;
        this.SkinName = new ReactivePropertySlim<string>("");
        this.AutherName = new ReactivePropertySlim<string>("");
        this.LicencePath = new ReactivePropertySlim<string>("");
        this.IsBounce = new ReactivePropertySlim<bool>(false);
        this.IsShader = new ReactivePropertySlim<bool>(false);


        ImgRows = new ObservableCollection<SkinRowModel>()
        {
            new SkinRowModel("ExH.SelectFrontImage"),
            new SkinRowModel("ExH.SelectFrontFlipImage"),
            new SkinRowModel("ExH.SelectBackImage"),
            new SkinRowModel("ExH.SelectBackFlipImage"),
            new SkinRowModel("ExH.SelectClimbImage"),
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
        this.exporter = this.CreateExporter();
        return this.exporter.GetSameSkinCheckResult();
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

        var postRespons = await this.ApiServerModel.PostAsync("hat/", newHatData, options);
        if (postRespons != null && postRespons.IsSuccessStatusCode)
        {
            return true;
        }

        var respons = await this.ApiServerModel.PutAsync("hat/", newHatData);
        return respons != null && respons.IsSuccessStatusCode;
    }

    private ExtremeHatExporterModel CreateExporter(bool exportWithAu = true)
    {
        string amongUsPath = exportWithAu ? this.AmongUsPathContainer.AmongUsFolderPath : string.Empty;

        var frontRow = GetImgRow("ExH.SelectFrontImage");
        var frontFlipRow = GetImgRow("ExH.SelectFrontFlipImage");
        var backRow = GetImgRow("ExH.SelectBackImage");
        var backFlipRow = GetImgRow("ExH.SelectBackFlipImage");
        var climbRow = GetImgRow("ExH.SelectClimbImage");


        return new ExtremeHatExporterModel(
            amongUsPath,
            this.AutherName.Value,
            this.SkinName.Value,
            this.LicencePath.Value,
            this.IsBounce.Value,
            this.IsShader.Value,
            frontRow,
            frontFlipRow,
            backRow,
            backFlipRow,
            climbRow);
    }

    private SkinRowModel GetImgRow(string key)
        => this.ImgRows.Where(x => x.RowName == key).First();

}
