using Godot;
using System;


public partial class Settings : CanvasLayer
{
	private Label version;
	private LineEdit workerName;
	private LineEdit startTime;



    public override void _Ready()
    {
		version = GetNode<Label>("Padding/SettingsList/Version/Input");
		workerName = GetNode<LineEdit>("Padding/SettingsList/WorkerName/Input");
		startTime = GetNode<LineEdit>("Padding/SettingsList/StartTime/Input");

		AssignSettingValues();
    }



	private void AssignSettingValues()
	{
		version.Text = (string)Manager.Instance.settingsData["version"];
		workerName.Text = (string)Manager.Instance.settingsData["workerName"];
		startTime.Text = (string)Manager.Instance.settingsData["startTime"];
	}


    #region Preview Triggered
    private void SetWorkerName(string name)
    {
		workerName.Text = name;
        Manager.Instance.settingsData["workerName"] = name;
    }

    private void SetStartTime() => SetStartTime(startTime.Text);
	private void SetStartTime(string timeText)
	{
		bool success = TimeOnly.TryParse(timeText, out TimeOnly parsedDay);

		if (success)
		{
			startTime.Text = timeText;
			Manager.Instance.settingsData["startTime"] = timeText;
		}
	}
    #endregion



    #region  Signals
    private void TriggerWorkerNamePreview() => Manager.Instance.OpenTextPreview(workerName.Text, Callable.From<string>(SetWorkerName));



    private void TriggerStartTimePreview() => Manager.Instance.OpenTextPreview(startTime.Text, Callable.From<string>(SetStartTime));



	private void ExitSettings()
	{
		SetStartTime();

		Manager.Instance.FixDocumentDirectory();
		var file = FileAccess.Open(Manager.Instance.settingsFilePath, FileAccess.ModeFlags.Write);
		Manager.Instance.SaveSettings(file);

		Manager.Instance.CallDeferred("SwitchScene", "MainMenu");
	}
#endregion
}
