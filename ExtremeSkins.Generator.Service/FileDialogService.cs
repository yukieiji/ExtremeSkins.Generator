using Microsoft.Win32;

using ExtremeSkins.Generator.Service.Interface;
using static ExtremeSkins.Generator.Service.Interface.ICommonDialogResult;

namespace ExtremeSkins.Generator.Service;

public sealed class FileDialogService : ICommonDialogService<FileDialogService.Result>
{
    public sealed class Setting : ICommonDialogSetting
    {
        public string Filter { get; init; }
        public string Title { get; init; }
    }

    public sealed class Result : ICommonDialogResult
    {
        public DialogShowState State { get; set; }
        public string FileName { get; set; }

        public static Result CreateInvalidResult()
            => new Result()
            {
                State = DialogShowState.InvalidSetting,
                FileName = string.Empty,
            };
        public static Result CreateCancelResult()
            => new Result()
            {
                State = DialogShowState.Cancel,
                FileName = string.Empty,
            };
    }

    public Result ShowDialog(ICommonDialogSetting setting)
    {
        if (setting is not Setting dialogSetting)
        {
            return Result.CreateInvalidResult();
        }

        OpenFileDialog dlg = new OpenFileDialog()
        {
            Filter = dialogSetting.Filter,
            Title = dialogSetting.Title,
        };

        bool? result = dlg.ShowDialog();

        if (!result.HasValue || !result.Value)
        {
            return Result.CreateCancelResult();
        }

        return new Result()
        {
            State = DialogShowState.Ok,
            FileName = dlg.FileName,
        };
    }
}
