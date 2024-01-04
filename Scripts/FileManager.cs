using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using ClosedXML.Excel;

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

      return new TimeSheet(DateOnly.Parse(fileName.ReplaceN(".json", "")), entries);
   }



   public static string FileNameToDateText(string fileName) => DateOnly.Parse(fileName.ReplaceN(".json", "")).ToString();



#region Conversion to .xlsx
   public static bool ConvertToExcelFiles(string[] filesNames)
   {      
      byte[] templateBytes = FileAccess.GetFileAsBytes("res://ExcelTemplates/StundenzettelTemplate.xlsx");
      
      if (FileAccess.GetOpenError() != Error.Ok)
         throw new Exception("Failed to load .xlsx timesheet template file");

      DateOnly[] currentWeek = new DateOnly[5];
      currentWeek[0] = DateOnly.Parse(filesNames[0].ReplaceN(".json", ""));
      for (int i = 1; i < 5; i++)
      {
         
      }

      using(var ms = new System.IO.MemoryStream(templateBytes))
      {
         foreach (string timeSheetName in filesNames)
         {
            TimeSheet currentFile = GetTimeSheetFromFile(timeSheetName);
            using(var workbook = new XLWorkbook(ms))
            {
// TODO Put all timesheets of one week in one workbook
               IXLWorksheet sheet = workbook.Worksheet(1);

               sheet.FillExcelFile(currentFile, timeSheetName.ReplaceN(".json", ""));

               string  savePath = $"{Manager.documentsFilePath}/Stundenzettel/Rapportzettel - {Manager.Singleton.settingsData["workerName"]} - {currentFile.Date}.xlsx";
               workbook.SaveAs(savePath);
            }
         }
      }

      return true;
   }



   private static IXLWorksheet FillExcelFile(this IXLWorksheet sheet, TimeSheet currentFile, string date)
   {
      string[] timeSpanEntryNames = new string[7] { "fromTime", "toTime", "customer", "purpose", "description", "kmStart", "kmEnd" };

      string workerName = (string)Manager.Singleton.settingsData["workerName"];
      int row;
      int col;
      string cellValue;

      TimeSpanEntry entry;
      Dictionary timeSpanData;

      TimeOnly allWorkTime = new TimeOnly();
      TimeOnly allBreakTime = new TimeOnly();
      int allKmDriven = 0;

      sheet.Cell(9, 2).Value = date;

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
               
               case "kmStart":
                  col = 20;
                  break;

               case "kmEnd":
                  col = 21;
                  break;

               default:
                  throw new Exception($"Recieved unexpected timespanentry valuename {nameof(timeSpanEntryNames)}");
            }

            if (timeSpanEntryNames[j] == "purpose")
            {
               cellValue = PurposeNames.GetName(entry.Purpose);
            }
            else
               cellValue = (string)timeSpanData[$"{timeSpanEntryNames[j]}"];

            sheet.Cell(row, col).Value = cellValue;
         }

         if (entry.Purpose == Purposes.Break)
         {
            TimeSpan breakTime = entry.ToTime  - entry.FromTime;
            sheet.Cell(row, 10).Value = breakTime.ToString("hh\\:mm");
            allBreakTime = allBreakTime.Add(breakTime);
         }
         else
         {
            TimeSpan workTime = entry.ToTime - entry.FromTime;
            sheet.Cell(row, 9).Value = workTime.ToString("hh\\:mm");
            allWorkTime = allWorkTime.Add(workTime);
         }
         
         int kmDriven = entry.KmEnd - entry.KmStart;
         sheet.Cell(row, 22).Value = $"{kmDriven} km";
         allKmDriven += kmDriven;
      }
      
      sheet.Cell(39, 9).Value = allWorkTime.ToString();
      sheet.Cell(39, 10).Value = allBreakTime.ToString(); 
      sheet.Cell(39, 22).Value = $"{allKmDriven} km";

      sheet.Cell(43, 3).Value = workerName;

      return sheet;
   }
#endregion
}