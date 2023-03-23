namespace ExtremeSkins.Generator.Service.Interface;

public interface ICommonDialogService<T> where T : ICommonDialogResult
{
    /// <summary>コモンダイアログを表示します。</summary>
    /// <param name="settings">ダイアログと値を受け渡しするためのICommonDialogSettings。</param>
    /// <returns>trueが返るとOKボタン、falseが返るとキャンセルボタンが押されたことを表します。</returns>
    public T ShowDialog(ICommonDialogSetting settings);
}
