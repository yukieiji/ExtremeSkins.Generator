using System.Windows;

namespace ExtremeSkins.Generator.Service.Interface;

public interface IWindowsDialogService
{
    public MessageBoxResult Show(IMessageSetting setting);
}
