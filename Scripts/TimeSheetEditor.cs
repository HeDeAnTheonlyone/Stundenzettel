using Godot;
using System;
using System.Collections.Generic;
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

		timeSheet = LoadTimeSheet();

		if (timeSheet.TimeSpanEntries.Count > 0)
		{
			Manager.Singleton.lastTimeStamp = timeSheet.TimeSpanEntries.Last().ToTime;
			PopulateEntryList(timeSheet.TimeSpanEntries);
		}
		else
			Manager.Singleton.lastTimeStamp = TimeOnly.Parse((string)Manager.Singleton.settingsData["startTime"]);

		date.Text = timeSheet.Date.ToString();
    }



	private TimeSheet LoadTimeSheet()
	{
		if (Manager.Singleton.selectedSheet == null)
			Manager.Singleton.selectedSheet = new TimeSheet
			(
				DateOnly.FromDateTime(DateTime.Today),
				new List<TimeSpanEntry>()
			);

		return Manager.Singleton.selectedSheet;
	}



	private void PopulateEntryList(List<TimeSpanEntry> entries)
	{
		foreach(TimeSpanEntry entry in entries)
		{
			var newEntry = timeSpanBlockButton.Instantiate() as TimeSpanBlockButton;
			newEntry.Entry = entry;
			timeSpanList.AddChild(newEntry);
		}
	}



	private void SaveTimeSheet(TimeSheet sheet)
	{
		string[] data = new string[timeSheet.TimeSpanEntries.Count];
		int pointer = 0;

		foreach(TimeSpanEntry entry in timeSheet.TimeSpanEntries)
		{
			data[pointer] = entry.ToJsonString();
			pointer++;
		}

		string dataString = Json.Stringify(data, "\t");

		Manager.Singleton.FixDocumentDirectory();

		using (var file = FileAccess.Open($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{sheet.Date.ToString("yyyy/MM/dd")}.json", FileAccess.ModeFlags.Write))
		{
			file.StoreString(dataString);
		}
	}



#region Signals
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
		SaveTimeSheet(timeSheet);

		Manager.Singleton.selectedSheet = null;
		Manager.Singleton.CallDeferred("SwitchScene", "MainMenu");
	}
#endregion
}
