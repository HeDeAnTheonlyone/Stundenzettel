using Godot;

public partial class TimeSheetButton : HSplitContainer
{
	public string FileName { get; set; }
	private string dateText;
	private Button editButton;

    public override void _Ready()
    {
		editButton = GetNode<Button>("Edit");

		editButton.Text = FileManager.FileNameToDateText(FileName);
    }



#region Signals
	private void SwitchToTimeSheetEditor()
	{
		Manager.Singleton.selectedSheet = FileManager.GetTimeSheetFromFile(FileName);
		
		Manager.Singleton.SwitchScene("TimeSheetEditor");
	}



    private void DeleteEntry()
	{
		DirAccess.RemoveAbsolute($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{FileName}");
		QueueFree();
	}
#endregion
}
