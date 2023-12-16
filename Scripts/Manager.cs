using System;
using Godot;
using Godot.Collections;

public partial class Manager : Node
{
	public static Manager Singleton;
	public Dictionary settingsData;
	public const string settingsFilePath = "user://settings.json";
	public static readonly string documentsFilePath = OS.GetSystemDir(OS.SystemDir.Documents);
	public static readonly string excelTimeSheetTemplatePath = OS.GetExecutablePath().GetBaseDir().PathJoin("ExcelTemplates/StundenzettelTemplate.xlsx");
	private DirAccess documentsDir;
	public TimeOnly lastTimeStamp;
	public TimeSheet selectedSheet;
	public TimeSpanEntry selectedEntry;
	


	public override void _Ready()
    {

#region Singleton logic

			if (Singleton == null)
		{
			Singleton = this;
			ProcessMode = ProcessModeEnum.Always;
			SetProcess(false);
		}
		else
			QueueFree();
		#endregion

		documentsDir = DirAccess.Open($"{documentsFilePath}");

#region Set/Load Default Settings
		var file = FileAccess.Open(settingsFilePath, FileAccess.ModeFlags.Read);

		var err = FileAccess.GetOpenError();
		if (err == Error.FileNotFound)
		{
			file = FileAccess.Open(settingsFilePath, FileAccess.ModeFlags.Write);

			settingsData = new Dictionary()
			{
				{ "workerName", "" },
				{ "startTime", "7:30" }
			};

			SaveSettings(file);
		}
		else
		{
			string dataString = (string)Json.ParseString(file.GetAsText(true));
			settingsData = (Dictionary)Json.ParseString(dataString);
		}
#endregion

		lastTimeStamp = TimeOnly.Parse((string)settingsData["startTime"]);
	}



	public void FixDocumentDirectory()
	{
		if (!documentsDir.DirExists("Stundenzettel / TimeSheets"))
			documentsDir.MakeDirRecursive("Stundenzettel/TimeSheets");
	}




	public void SaveSettings(FileAccess file)
	{
		string dataString = Json.Stringify(settingsData, "\t");
		file.StoreString(dataString);
		file.Close();
	}



    public void SwitchScene(string nextScene) => GetTree().ChangeSceneToFile($"res://Scenes/{nextScene}.tscn");
}
