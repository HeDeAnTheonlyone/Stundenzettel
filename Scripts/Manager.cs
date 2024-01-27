using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
//using Octokit;

/*
	Active Working Sessions:
	+ 6h
	+ 17h
	+ 11h
	+ 6.5h
	+ 4.5h
	+ 3.5h
	+ 4.5h
	+ 8h
	+ 2.5h
	+ 3h
	+ 3.5h
	+ 3h
	+ 1h
	+ 4h
	+ 4h
	+ 3h
	+ 0.5h
	+ 3h
	+ 2,5h
	+ 5h
	+ 1h
	+ 6h
	+ 2h
============
	105h
*/

public partial class Manager : CanvasLayer
{
	public static Manager Instance { get; private set; }
	public Dictionary settingsData;
	public const string settingsFilePath = "user://Settings.json";
	public const string customerNamesFilePath = "user://CustomerNames.json";
	public static readonly string documentsFilePath = OS.GetSystemDir(OS.SystemDir.Documents);
	public static readonly string excelTimeSheetTemplatePath = OS.GetExecutablePath().GetBaseDir().PathJoin("ExcelTemplates/StundenzettelTemplate.xlsx");
	private DirAccess documentsDir;
	public TimeOnly lastTimeStamp;
	public TimeSheet selectedSheet;
	public TimeSpanEntry selectedEntry;
	public List<string> customerNames = new List<string>(); 



	public override void _Ready()
    {
		GD.PushWarning("Make Date Uneditable!!!");

		if (!OS.RequestPermissions())
			GetTree().Quit();

#region Singleton init logic

		if (Instance == null)
		{
			Instance = this;
			ProcessMode = ProcessModeEnum.Always;
			SetProcess(false);
		}
		else
			QueueFree();
		#endregion

		documentsDir = DirAccess.Open($"{documentsFilePath}");

		LoadSettings();

		lastTimeStamp = TimeOnly.Parse((string)settingsData["startTime"]);

		// FIXME GetActivationState
		// GetActivationState();

		LoadCustomerNames();
	}



	private void LoadSettings()
	{
		var file = FileAccess.Open(settingsFilePath, FileAccess.ModeFlags.Read);

		if (FileAccess.GetOpenError() == Error.FileNotFound)
		{
			file = FileAccess.Open(settingsFilePath, FileAccess.ModeFlags.Write);

			settingsData = new Dictionary()
			{
				{ "active", true },
				{ "workerName", "" },
				{ "startTime", "7:30" }
			};

			SaveSettings(file);
		}
		else
			settingsData = (Dictionary)Json.ParseString(file.GetAsText(true));
	}


	// FIXME
	// private async Task GetActivationState()
	// {
	// 	const string owner = "HeDeAnTheonlyone";
	// 	const string repo = "External-Config";
	// 	const string branch = "main";
	// 	const string filePath = "Stundenzettel.txt";

	// 	bool active;

	// 	GitHubClient github = new GitHubClient(new ProductHeaderValue("HeDeAn"));

	// 	try
	// 	{
	// 		var contents = await github.Repository.Content.GetAllContentsByRef(owner, repo, filePath, branch);
	// 		if (contents.Count() > 0)
	// 		{
	// 			string dataString = contents[0].Content;
	// 			settingsData["active"] = bool.Parse(dataString);
	// 			active = bool.Parse(dataString);
	// 		}
	// 		else
	// 			active = (bool)settingsData["active"];
	// 	}
	// 	catch
	// 	{
	// 		active = (bool)settingsData["active"];
	// 	}

	// 	if (!active)
	// 		GetTree().Quit();
	// }



	public void FixDocumentDirectory()
	{
		if (!documentsDir.DirExists("Stundenzettel/TimeSheets"))
			documentsDir.MakeDirRecursive("Stundenzettel/TimeSheets");
	}



	public void SaveSettings(FileAccess file)
	{
		string dataString = Json.Stringify(settingsData, "\t");
		file.StoreString(dataString);
		file.Close();
	}



	private void LoadCustomerNames()
	{
		using(var file = FileAccess.Open(customerNamesFilePath, FileAccess.ModeFlags.Read))
		{
			if (FileAccess.GetOpenError() == Error.FileNotFound)
				SaveCustomerNames();
			else
			{
				string[] dataString = (string[])Json.ParseString(file.GetAsText(true));
				customerNames = dataString.ToList();
			}
		}
	}



	public void SaveCustomerNames()
	{
		using(var file = FileAccess.Open(customerNamesFilePath, FileAccess.ModeFlags.Write))
		{
			string jsonString = Json.Stringify(customerNames.ToArray(), "\t");

			file.StoreString(jsonString);
		}
	}



    public void SwitchScene(string nextScene) => GetTree().ChangeSceneToFile($"res://Scenes/{nextScene}.tscn");
}
