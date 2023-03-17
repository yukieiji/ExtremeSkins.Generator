using System.Windows;

namespace ExtremeSkins.Generator.Service.Interface;

public interface IMessageSetting
{
    public string Title { get; set; }
    public string Message { get; set; }

    public MessageBoxButton Button { get; }
    public MessageBoxImage Icon { get; }
}
