using Godot;

public partial class TimeSheetSelector : CanvasLayer
{
	private readonly string timeSheetsPath = $"{Manager.documentsFilePath}/Stundenzettel/TimeSheets";
	private PackedScene timeSheetButton;
	private VBoxContainer timeSheetList;
	private string[] timeSheetFiles;



	public override void _Ready()
	{
		timeSheetButton = GD.Load("res://Objects/TimeSheetButton.tscn") as PackedScene;
		timeSheetList = GetNode<VBoxContainer>("Padding/ScrollList/TimeSheetList");
		
		Manager.Singleton.FixDocumentDirectory();
		timeSheetFiles = DirAccess.GetFilesAt(timeSheetsPath);
		
		timeSheetList.PopulateList(timeSheetFiles, timeSheetButton);
	}

	

#region Signals
	private void GenerateExcelFiles()
	{
		FileManager.ConvertToExcelFiles(timeSheetFiles);
	}



	private void SwitchToMainMenu() => Manager.Singleton.SwitchScene("MainMenu");
#endregion
}
