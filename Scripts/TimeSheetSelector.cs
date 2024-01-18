using System;
using System.Collections;
using Godot;

public partial class TimeSheetSelector : CanvasLayer
{
	private Label finishConversionIndicator;
	private Timer timer;
	private ColorRect bg;
	private readonly string timeSheetsPath = $"{Manager.documentsFilePath}/Stundenzettel/TimeSheets";
	private PackedScene timeSheetButton;
	private VBoxContainer timeSheetList;
	private string[] timeSheetFiles;



	public override void _Ready()
	{
		timeSheetButton = GD.Load("res://Objects/TimeSheetButton.tscn") as PackedScene;
		timeSheetList = GetNode<VBoxContainer>("Padding/ScrollList/TimeSheetList");
		finishConversionIndicator = GetNode<Label>("FinishConversionIndicator");
		timer = finishConversionIndicator.GetNode<Timer>("Timer");
		bg = finishConversionIndicator.GetNode<ColorRect>("TextPadding/Background");

		Manager.Instance.FixDocumentDirectory();
		timeSheetFiles = DirAccess.GetFilesAt(timeSheetsPath);
		Array.Sort(timeSheetFiles, (a, b) => Comparer.Default.Compare(b, a));

		timeSheetList.PopulateList(timeSheetFiles, timeSheetButton);
	}

	

#region Signals
	private void GenerateXlsxFiles()
	{
		bool isDone;
		XlsxConverter xlsx = new XlsxConverter();

		Array.Sort(timeSheetFiles);
		isDone = xlsx.ToXlsx(timeSheetFiles);

		if (isDone)
		{
			finishConversionIndicator.Text = "Umwandeln abgeschlossen";
			bg.Color = new Color("#7dfd7d");
		}
		else
		{
			finishConversionIndicator.Text = "Umwandeln fehlgeschlagen";
			bg.Color = new Color("#ff3d2d");
		}
		
		finishConversionIndicator.Modulate = finishConversionIndicator.Modulate with { A = 1 };
		timer.Start();
	}



	private void SwitchToMainMenu() => Manager.Instance.SwitchScene("MainMenu");
#endregion
}
