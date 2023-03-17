namespace ExtremeSkins.Generator.Service.Interface;

public interface ICommonDialogSetting<T> where T : ICommonDialogResult
{
    public T Result { get; set; }
}