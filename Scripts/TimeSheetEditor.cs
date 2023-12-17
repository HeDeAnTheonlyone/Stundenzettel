using Godot;
using System;
using System.Linq;

public partial class TimeSheetEditor : CanvasLayer
{
	private PackedScene timeSpanBlockButton = GD.Load("res://Objects/TimeSpanBlockButton.tscn") as PackedScene;
	private LineEdit date;
	private VBoxContainer timeSpanList;
	private TimeSheet timeSheet;



	public override void _Ready()
    {
		date = GetNode<LineEdit>("TitlePadding/Date");
		timeSpanList = GetNode<VBoxContainer>("Padding/Splitter/ScrollList/TimeSpanList");

		timeSheet = FileManager.LoadSelectedTimeSheet();

		if (timeSheet.TimeSpanEntries.Count > 0)
		{
			Manager.Singleton.lastTimeStamp = timeSheet.TimeSpanEntries.Last().ToTime;
			timeSpanList.PopulateList(timeSheet.TimeSpanEntries, timeSpanBlockButton);
		}
		else
			Manager.Singleton.lastTimeStamp = TimeOnly.Parse((string)Manager.Singleton.settingsData["startTime"]);

		date.Text = timeSheet.Date.ToString();
    }



    #region Signals
    private void SetDate() => SetDate(date.Text);
    private void SetDate(string dateText)
	{
		try
		{
			timeSheet.Date = DateOnly.Parse(dateText);
		}
		catch
		{
			date.Text = timeSheet.Date.ToString();
		}
	}



    private void AddTimeSpan()
	{
		var newEntry = timeSpanBlockButton.Instantiate() as TimeSpanBlockButton;
		timeSpanList.AddChild(newEntry);
	}



	private void SwitchToMainMenu()
	{
		if (timeSheet.TimeSpanEntries.Count > 0)
			FileManager.SaveTimeSheet(timeSheet);

		Manager.Singleton.selectedSheet = null;
		Manager.Singleton.CallDeferred("SwitchScene", "MainMenu");
	}
#endregion
}
