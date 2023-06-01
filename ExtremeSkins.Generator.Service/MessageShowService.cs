using System.Windows;

using ExtremeSkins.Generator.Service.Interface;

namespace ExtremeSkins.Generator.Service;

public sealed class MessageShowService : IWindowsDialogService
{
    public class InfoMessageSetting : IMessageSetting
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageBoxButton Button => MessageBoxButton.OK;
        public MessageBoxImage Icon => MessageBoxImage.Information;
    }

    public class ErrorMessageSetting : IMessageSetting
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageBoxButton Button => MessageBoxButton.OK;
        public MessageBoxImage Icon => MessageBoxImage.Warning;
    }

    public class CheckMessageSetting : IMessageSetting
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageBoxButton Button => MessageBoxButton.YesNoCancel;
        public MessageBoxImage Icon => MessageBoxImage.Information;
    }

    public MessageBoxResult Show(IMessageSetting setting)
    {
        return MessageBox.Show(
            setting.Message, setting.Title,
            setting.Button, setting.Icon);
    }
}
