using System;
using Godot;

public partial class TimeSheetButton : HSplitContainer
{
	private string displayName;
	public string DisplayName
	{
		get => displayName;
		set
		{
			displayName = value;
			fileName = $"{DateOnly.Parse(value):yyyy/MM/dd}.json";
		}
	}
	private string fileName;
	private Button editButton;



    public override void _Ready()
    {
		editButton = GetNode<Button>("Edit");
		editButton.Text = DisplayName;
    }



    #region Signals
    private void SwitchToTimeSheetEditor()
	{
		Manager.Instance.selectedSheet = FileManager.GetTimeSheetFromFile(fileName);
		
		Manager.Instance.SwitchScene("TimeSheetEditor");
	}



    private void DeleteEntry() => GetNode<ConfirmationPanel>("../../../../ConfirmationPanel").OpenConfirmDeletion(Callable.From(DeleteEntryConfirmed));



    private void DeleteEntryConfirmed()
	{
		DirAccess.RemoveAbsolute($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{fileName}");
		QueueFree();
	}
#endregion
}
