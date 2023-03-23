namespace ExtremeSkins.Generator.Service.Interface;

public interface ICommonDialogResult
{
    public enum DialogShowState
    {
        InvalidSetting,
        Ok,
        Cancel,
    }
    public DialogShowState State { get; set; }
}
