using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class TimeSheetEditor : CanvasLayer
{
	private PackedScene timeSpanBlockButton = GD.Load<PackedScene>("res://Objects/TimeSpanBlockButton.tscn");
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
			Manager.Instance.lastTimeStamp = timeSheet.TimeSpanEntries.Last().ToTime;
			timeSpanList.PopulateList(timeSheet.TimeSpanEntries, timeSpanBlockButton);
		}
		else
			Manager.Instance.lastTimeStamp = TimeOnly.Parse((string)Manager.Instance.settingsData["startTime"]);

		date.Text = timeSheet.Date.ToString();
    }



    public void UpdateOrder(TimeSpanBlockButton moveButton)
	{
		float yPos = moveButton.Position.Y;

		moveButton.ResetPos();

		if (yPos < 0)
		{
			timeSpanList.MoveChild(moveButton, 0);
			return;
		}

		int newIndex = moveButton.GetIndex();
		Array<Node> buttons = timeSpanList.GetChildren();

		foreach (TimeSpanBlockButton button in buttons)
		{
			if (yPos > button.Position.Y)
				newIndex = button.GetIndex();
			else
			{
				if (button == moveButton)
					return;

				break;
			}
		}

		timeSheet.TimeSpanEntries.Remove(moveButton.entry);
		timeSheet.TimeSpanEntries.Insert(newIndex, moveButton.entry);

		timeSpanList.PopulateList(timeSheet.TimeSpanEntries, timeSpanBlockButton);
	}



	#region PreviewTriggered
	private void SetDate() => SetDate(date.Text);
	private void SetDate(string dateText)
	{
		bool success = DateOnly.TryParse(dateText, out DateOnly parsedDate);

		if (success)
		{
			date.Text = dateText;
			timeSheet.Date = parsedDate;
		}

	}
	#endregion



	#region Signals
	private void TriggerDatePreview() => Manager.Instance.OpenTextPreview(Callable.From<string>(SetDate));



    private void AddTimeSpan()
	{
		var newEntry = timeSpanBlockButton.Instantiate<TimeSpanBlockButton>();
		timeSpanList.AddChild(newEntry);
	}



	private void SwitchToMainMenu()
	{
		SetDate();

		if (timeSheet.TimeSpanEntries.Count > 0)
			FileManager.SaveTimeSheet(timeSheet);

		Manager.Instance.selectedSheet = null;
		Manager.Instance.CallDeferred("SwitchScene", "MainMenu");
	}
#endregion
}
