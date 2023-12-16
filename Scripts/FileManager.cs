using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using OfficeOpenXml;

public static class FileManager
{
   public static void SaveTimeSheet(TimeSheet sheet)
   {
      string[] data = new string[sheet.TimeSpanEntries.Count];

      for (int i = 0; i < sheet.TimeSpanEntries.Count; i++)
         data[i] = sheet.TimeSpanEntries.ElementAt(i).ToJsonString();

      string dataString = Json.Stringify(data, "\t");

      Manager.Singleton.FixDocumentDirectory();

      string filePath = $"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{sheet.Date.ToString("yyyy/MM/dd")}.json";

      using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write))
      {
         file.StoreString(dataString);
      }
   }



   public static TimeSheet LoadSelectedTimeSheet()
   {
      if (Manager.Singleton.selectedSheet == null)
         Manager.Singleton.selectedSheet = new TimeSheet
         (
            DateOnly.FromDateTime(DateTime.Today),
            new List<TimeSpanEntry>()
         );

      return Manager.Singleton.selectedSheet;
   }



   public static TimeSheet GetTimeSheetFromFile(string fileName)
   {
      List<TimeSpanEntry> entries = new List<TimeSpanEntry>();
      string dataString;

      using (var file = FileAccess.Open($"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{fileName}", FileAccess.ModeFlags.Read))
      {
         dataString = file.GetAsText(true);
      }

      string[] data = (string[])Json.ParseString(dataString);

      foreach (string jsonString in data)
      {
         Dictionary dict = (Dictionary)Json.ParseString(jsonString);
         entries.Add(new TimeSpanEntry(dict));
      }

      return new TimeSheet(DateOnly.Parse(CleanFileName(fileName)), entries);
   }



   public static string CleanFileName(string fileName) => fileName.Remove(fileName.Length - 5);



   public static string FileNameToDateText(string fileName) => DateOnly.Parse(CleanFileName(fileName)).ToString();



#region Conversion to .xlsx
   public static void ConvertToExcelFiles(string[] filesNames)
   {
      string[] timeSpanEntryNames = new string[5] { "fromTime", "toTime", "customer", "purpose", "description"};
      byte[] templateBytes = FileAccess.GetFileAsBytes("res://ExcelTemplates/StundenzettelTemplate.xlsx");

      System.IO.MemoryStream ms = new System.IO.MemoryStream(templateBytes);

      foreach (string timeSheetName in filesNames)
      {
         
         TimeSheet currentFile = GetTimeSheetFromFile(timeSheetName);
         ExcelPackage package = new ExcelPackage(ms);
         ExcelWorksheet sheet = package.Workbook.Worksheets[0];

         sheet.FillExcelFile(currentFile, CleanFileName(timeSheetName), timeSpanEntryNames);

         string  savePath = $"{Manager.documentsFilePath}/Stundenzettel/Rapportzettel - {Manager.Singleton.settingsData["workerName"]} - {currentFile.Date}.xlsx";
         package.SaveAs(savePath);
      }

      ms.Close();
   }



   private static ExcelWorksheet FillExcelFile(this ExcelWorksheet sheet, TimeSheet currentFile, string timeSheetName, string[] timeSpanEntryNames)
   {
      string workerName = (string)Manager.Singleton.settingsData["workerName"];
      int row;
      int col = 0;

      TimeSpanEntry entry;
      Dictionary timeSpanData;

      TimeOnly allWorkTime = new TimeOnly();
      TimeOnly allBreakTime = new TimeOnly();

      sheet.Cells[8, 2].Value = timeSheetName;
      sheet.Cells[43, 3].Value = workerName;

      for (int i = 0; i < currentFile.TimeSpanEntries.Count; i++)
      {
         entry = currentFile.TimeSpanEntries.ElementAt(i);
         timeSpanData = entry.ToDictionary();
         row = i + 14;

         for (int j = 0; j < timeSpanEntryNames.Length; j++)
         {
            switch(timeSpanEntryNames[j])
            {
               case "fromTime":
                  col = 1;
                  break;

               case "toTime":
                  col = 2;
                  break;

               case "customer":
                  col = 3;
                  break;

               case "purpose":
                  col = 5;
                  break;

               case "description":
                  col = 12;
                  break;
            }

            sheet.Cells[row, col].Value = timeSpanData[$"{timeSpanEntryNames[j]}"];
            
            if (entry.Purpose == Purposes.Break)
            {
               TimeSpan breakTime = entry.ToTime  - entry.FromTime;
               sheet.Cells[row, 10].Value = breakTime.ToString("hh\\:mm");
               allBreakTime.Add(breakTime);
            }
            else
            {
               TimeSpan workTime = entry.ToTime - entry.FromTime;
               sheet.Cells[row, 9].Value = workTime.ToString("hh\\:mm");
               allWorkTime.Add(workTime);
            }
         }
      }
      
      sheet.Cells[39, 9].Value = allWorkTime;      
      sheet.Cells[39, 10].Value = allBreakTime;      

      return sheet;
   }
#endregion
}