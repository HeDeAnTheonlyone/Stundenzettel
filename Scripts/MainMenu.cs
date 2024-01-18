using Godot;

public partial class MainMenu : CanvasLayer
{
    private void SwitchToNewFile() => Manager.Instance.SwitchScene("TimeSheetEditor");



    private void SwitchToFileSelection() => Manager.Instance.SwitchScene("TimeSheetSelector");



    private void SwitchToSettings() => Manager.Instance.SwitchScene("Settings");



    private void QuitApp() => GetTree().Quit();
}
