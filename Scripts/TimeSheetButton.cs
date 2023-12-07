using System.Collections.Generic;
using Godot;
using System;
using Godot.Collections;
using System.IO.Enumeration;
using System.Globalization;

public partial class TimeSheetButton : HSplitContainer
{
	public string FileName { get; set; }
	private string dateText;
	private Button editButton;

    public override void _Ready()
    {
		editButton = GetNode<Button>("Edit");

		string cleanFileName = FileName.Remove(FileName.Length - 5);
		dateText = DateOnly.Parse(cleanFileName).ToString();

		editButton.Text = dateText;
    }



#region Signals
	private void SwitchToFileEdit()
	{
		List<TimeSpanEntry> entries = new List<TimeSpanEntry>();
		string dataString;

		using(var file = FileAccess.Open($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{FileName}", FileAccess.ModeFlags.Read))
		{
			dataString = file.GetAsText(true);
		}

		string[] data = (string[])Json.ParseString(dataString);
		
		foreach(string jsonString in data)
		{
			Dictionary dict = (Dictionary)Json.ParseString(jsonString);
			entries.Add(new TimeSpanEntry(dict));
		}

		TimeSheet sheet = new TimeSheet(DateOnly.Parse(dateText), entries);
		Manager.Singleton.selectedSheet = sheet;
		
		Manager.Singleton.SwitchScene("TimeSheetEditor");
	}



    private void DeleteEntry()
	{
		DirAccess.RemoveAbsolute($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{FileName}");
		GD.Print(GetParent());
		QueueFree();
	}
#endregion
}
