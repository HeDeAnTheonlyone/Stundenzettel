using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public static class FileManager
{
   public static string lastSaveString;



   public static void SaveTimeSheet(TimeSheet sheet)
   {
      string[] data = new string[sheet.TimeSpanEntries.Count];

      for (int i = 0; i < sheet.TimeSpanEntries.Count; i++)
         data[i] = sheet.TimeSpanEntries.ElementAt(i).ToJsonString();

      string dataString = Json.Stringify(data, "\t");

      Manager.Instance.FixDocumentDirectory();

      string filePath = $"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{sheet.Date:yyyy/MM/dd}.json";

      using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write))
      {
         file.StoreString(dataString);
      }
   }



   public static TimeSheet LoadSelectedTimeSheet()
   {
      if (Manager.Instance.selectedSheet == null)
         Manager.Instance.selectedSheet = new TimeSheet
         (
            DateOnly.FromDateTime(DateTime.Today),
            new List<TimeSpanEntry>()
         );

      string fileName = $"{Manager.Instance.selectedSheet.Date:yyyy/MM/dd}.json";
      string[] timeSheetFiles = DirAccess.GetFilesAt($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets");
      
      if (timeSheetFiles.Contains(fileName))
         Manager.Instance.selectedSheet = GetTimeSheetFromFile(fileName);

      return Manager.Instance.selectedSheet;
   }



   private static string GetSaveString(string fileName)
   {
      try
      {
         using (var file = FileAccess.Open($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{fileName}", FileAccess.ModeFlags.Read))
         {
            lastSaveString = file.GetAsText(true);
         }
      }
      catch
      {
         throw new System.IO.FileNotFoundException($"Couldn't find file {fileName}");
      }

      return lastSaveString;
   }



   public static TimeSheet GetTimeSheetFromFile(string fileName)
   {
      List<TimeSpanEntry> entries = new List<TimeSpanEntry>();

      string dataString = GetSaveString(fileName);

      string[] data = (string[])Json.ParseString(dataString);

      foreach (string jsonString in data)
      {
         Dictionary dict = (Dictionary)Json.ParseString(jsonString);
         entries.Add(new TimeSpanEntry(dict));
      }

      return new TimeSheet(DateOnly.Parse(fileName.ReplaceN(".json", "")), entries);
   }
}