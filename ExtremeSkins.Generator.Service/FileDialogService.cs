using Microsoft.Win32;

using ExtremeSkins.Generator.Service.Interface;

namespace ExtremeSkins.Generator.Service;

public sealed class FileDialogService : ICommonDialogService<FileDialogService.Result>
{
    public sealed class Setting : ICommonDialogSetting<Result>
    {
        public string Filter { get; init; }
        public string Title { get; init; }
        public Result Result { get; set; }
    }

    public sealed class Result : ICommonDialogResult
    {
        public string FileName { get; set; }
    }

    public bool ShowDialog(ICommonDialogSetting<Result> setting)
    {
        if (setting is not Setting dialogSetting)
        {
            return false;
        }

        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = dialogSetting.Filter;
        dlg.Title = dialogSetting.Title;

        bool? result = dlg.ShowDialog();

        if (!result.HasValue || !result.Value)
        {
            return false;
        }

        setting.Result = new Result()
        {
            FileName = dlg.FileName,
        };

        return true;
    }
}
