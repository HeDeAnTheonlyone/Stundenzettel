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

      Manager.Instance.FixDocumentDirectory();

      string filePath = $"{Manager.documentsFilePath}/Stundenzettel/TimeSheets/{sheet.Date.ToString("yyyy/MM/dd")}.json";

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

      return Manager.Instance.selectedSheet;
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



// TODO Split this class into different smaller ones

// FIXME Add logo manually per code, to controll the size
#region Conversion to .xlsx
   public static bool ConvertToExcelFiles(string[] filesNames)
   {
      if (filesNames.Length == 0)
         return false;

      byte[] templateBytes = FileAccess.GetFileAsBytes("res://ExcelTemplates/StundenzettelTemplate.xlsx");
      
      if (FileAccess.GetOpenError() != Error.Ok)
         throw new Exception("Failed to load .xlsx timesheet template file");

      DateOnly referenceDate = DateOnly.Parse(filesNames[0].ReplaceN(".json", ""));
      DateOnly[] currentWeek = GetWeekDates(referenceDate);

      using(var ms = new System.IO.MemoryStream(templateBytes))
      {
         XLWorkbook workbook = new XLWorkbook(ms);
         string savePath;
         byte[] logoImageBytes = FileAccess.GetFileAsBytes("res://Assets/Logo.jpg");

         using(var logoMs = new System.IO.MemoryStream(logoImageBytes))
         {

            foreach (string timeSheetName in filesNames)
            {
               TimeSheet currentFile = GetTimeSheetFromFile(timeSheetName);

               if (!currentWeek.Contains(currentFile.Date))
               {
                  savePath = $"{Manager.documentsFilePath}/Stundenzettel/Rapportzettel - {Manager.Instance.settingsData["workerName"]} - [ {currentWeek[0]} - {currentWeek[currentWeek.Length - 1]} ].xlsx";
                  workbook.SaveAs(savePath);
                  workbook.Dispose();

                  workbook = new XLWorkbook(ms);

                  currentWeek = GetWeekDates(currentFile.Date);
               }

               IXLWorksheet sheet = workbook.Worksheet((int)currentFile.Date.DayOfWeek);

               sheet.FillSheet(currentFile, logoMs);
            }
            savePath = $"{Manager.documentsFilePath}/Stundenzettel/Rapportzettel - {Manager.Instance.settingsData["workerName"]} - [ {currentWeek[0]} - {currentWeek[currentWeek.Length - 1]} ].xlsx";
            workbook.SaveAs(savePath);
            workbook.Dispose();
         }
      }

      return true;
   }



   private static IXLWorksheet FillSheet(this IXLWorksheet sheet, TimeSheet currentFile, System.IO.MemoryStream logoData)
   {
      TimeSpanData[] timeSpanEntryData = Enum.GetValues<TimeSpanData>();

      int row;
      int col;
      string cellValue;

      TimeSpanEntry entry;
      Dictionary timeSpanDataDict;

      TimeOnly allWorkTime = new TimeOnly();
      TimeOnly allBreakTime = new TimeOnly();
      int allKmDriven = 0;

      for (int i = 0; i < currentFile.TimeSpanEntries.Count; i++)
      {
         entry = currentFile.TimeSpanEntries.ElementAt(i);
         timeSpanDataDict = entry.ToDictionary();
         row = i + 8;

         for (int j = 0; j < timeSpanEntryData.Length; j++)
         {
            switch(timeSpanEntryData[j])
            {
               case TimeSpanData.FromTime:
                  col = 2;
                  break;

               case TimeSpanData.ToTime:
                  col = 3;
                  break;

               case TimeSpanData.Customer:
                  col = 4;
                  break;

               case TimeSpanData.Purpose:
                  col = 5;
                  break;

               case TimeSpanData.Description:
                  // FIXME Change how description is handeled
                  col = 14;
                  break;
               
               case TimeSpanData.KmStart:
                  col = 10;
                  break;

               case TimeSpanData.KmEnd:
                  col = 11;
                  break;

               default:
                  throw new Exception($"Recieved unexpected timespanentry valuename {nameof(timeSpanEntryData)}");
            }

            if (timeSpanEntryData[j] == TimeSpanData.Purpose)
               cellValue = PurposeNames.GetName(entry.Purpose);
            else
               cellValue = (string)timeSpanDataDict[timeSpanEntryData[j].ToString().ToCamelCase()];

            sheet.Cell(row, col).Value = cellValue;
         }

         if (entry.Purpose == Purposes.Break)
         {
            TimeSpan breakTime = entry.ToTime  - entry.FromTime;
            sheet.Cell(row, 9).Value = breakTime.ToString("hh\\:mm");
            allBreakTime = allBreakTime.Add(breakTime);
         }
         else
         {
            TimeSpan workTime = entry.ToTime - entry.FromTime;
            sheet.Cell(row, 8).Value = workTime.ToString("hh\\:mm");
            allWorkTime = allWorkTime.Add(workTime);
         }
         
         int kmDriven = entry.KmEnd - entry.KmStart;
         //sheet.Cell(row, 22).Value = kmDriven;
         allKmDriven += kmDriven;
      }
      
      sheet.Cell(27, 11).Value = allKmDriven;
      sheet.Cell(28, 8).Value = allWorkTime.ToString();
      sheet.Cell(28, 9).Value = allBreakTime.ToString(); 

      sheet.Cell(36, 2).Value = currentFile.Date.ToString();
      sheet.Cell(36, 5).Value = (string)Manager.Instance.settingsData["workerName"];

      sheet.AddPicture(logoData).MoveTo(sheet.Cell(1, 1)).Scale(0.5); 

      return sheet;
   }
#endregion



   private static DateOnly[] GetWeekDates(DateOnly referenceDate)
   {
      DateOnly date = referenceDate.AddDays(-((int)referenceDate.DayOfWeek - 1));
      DateOnly[] weekDates = new DateOnly[5];

      for (int i = 0; i < weekDates.Length; i++)
      {
         weekDates[i] = date;
         date = date.AddDays(1);
      }

      return weekDates;
   }
}