using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

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
	+ 2.5h
	+ 5h
	+ 1h
	+ 6h
	+ 2h
	+ 4h
	+ 7h
	+ 5h
	+ 3h
	+ 4h
	+ 1h
	+ 1.5h
	+ 3h
	+ 1.5h
	+ 0.5h
============
	134h
*/

public partial class Manager : CanvasLayer
{
	private const string version = "1.2.2";
	public static Manager Instance { get; private set; }
	public PackedScene textPreviewResource = GD.Load<PackedScene>("res://Objects/TextPreview.tscn");
	public bool textPreviewExists = false;
	public Dictionary settingsData;
	private string currentScene = "MainMenu";
	public static readonly string documentsFilePath = OS.GetSystemDir(OS.SystemDir.Documents);
	public readonly string settingsFilePath = $"{documentsFilePath}/Stundenzettel/Internal/Settings.json";
	public readonly string customerNamesFilePath = $"{documentsFilePath}/Stundenzettel/Internal/CustomerNames.json";
	public static readonly string excelTimeSheetTemplatePath = OS.GetExecutablePath().GetBaseDir().PathJoin("ExcelTemplates/StundenzettelTemplate.xlsx");
	private DirAccess documentsDir;
	public TimeOnly lastTimeStamp;
	public TimeSheet selectedSheet;
	public TimeSpanEntry selectedEntry;
	public List<Customer> customerNames = new List<Customer>(); 



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

		float refreshRate = DisplayServer.ScreenGetRefreshRate();
		Engine.MaxFps = refreshRate < 0 ? 60 : (int)refreshRate;

		documentsDir = DirAccess.Open($"{documentsFilePath}");

		LoadSettings();

		lastTimeStamp = TimeOnly.Parse((string)settingsData["startTime"]);

		LoadCustomerNames();

		//TODO Do this V
		// SearchForUpdates();
		// GetActivationState();
	}



	private void LoadSettings()
	{
		FixDocumentDirectory();

		var file = FileAccess.Open(settingsFilePath, FileAccess.ModeFlags.Read);
		Error err = FileAccess.GetOpenError();

		settingsData = new Dictionary()
		{
			{ "version", version},
			{ "active", true },
			{ "workerName", "" },
			{ "startTime", "7:30" }
		};

		Dictionary oldSettingsData = null;

		if (err == Error.Ok)
			oldSettingsData = (Dictionary)Json.ParseString(file.GetAsText(true));

		if (err == Error.Ok && oldSettingsData != null)
		{
			var keys = oldSettingsData.Keys;
			
			foreach (var key in keys)
				if (settingsData.ContainsKey(key))
					settingsData[key] = oldSettingsData[key];
		}

		settingsData["version"] = version;

		file = FileAccess.Open(settingsFilePath, FileAccess.ModeFlags.Write);
		SaveSettings(file);
	}



	public void SaveSettings(FileAccess file)
	{
		FixDocumentDirectory();

		string dataString = Json.Stringify(settingsData, "\t");
		file.StoreString(dataString);
		file.Close();
	}



	// TODO Add remote activation variable
	// private void GetActivationState()
	// {
	// bool active;
	// 	
	// 	if (!active)
	// 		GetTree().Quit();
	// }



	//TODO Add auto updates
	// private void SearchForUpdates()
	// {
		
	// } 



	public void FixDocumentDirectory()
	{
		if (!documentsDir.DirExists("Stundenzettel/TimeSheets"))
			documentsDir.MakeDirRecursive("Stundenzettel/TimeSheets");
		
		if (!documentsDir.DirExists("Stundenzettel/Internal"))
			documentsDir.MakeDirRecursive("Stundenzettel/Internal");
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

				foreach (string s in dataString)
				{
					customerNames.Add(new Customer((Dictionary)Json.ParseString(s)));
				}			
			}
		}
	}



	public void SaveCustomerNames()
	{
		using(var file = FileAccess.Open(customerNamesFilePath, FileAccess.ModeFlags.Write))
		{
			List<string> customerStrings = new List<string>(); 
			foreach (Customer c in customerNames)
			{
				Dictionary dict = c.ToDict();
				customerStrings.Add(Json.Stringify(dict, "\t"));
			}

			string jsonString = Json.Stringify(customerStrings.ToArray(), "\t");

			file.StoreString(jsonString);
		}
	}



	public void OpenTextPreview(string content, Callable inputProcessingMethod)
	{
		if (textPreviewExists)
			return;

		TextPreview textPreview = textPreviewResource.Instantiate<TextPreview>();
		GetTree().Root.GetNode(currentScene).AddChild(textPreview);
		textPreviewExists = true;
		textPreview.Setup(content, inputProcessingMethod);
	}



    public void SwitchScene(string nextScene)
    {
		currentScene = nextScene;
        GetTree().ChangeSceneToFile($"res://Scenes/{nextScene}.tscn");
    }
}
