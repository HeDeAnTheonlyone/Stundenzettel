using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using ClosedXML.Excel;

public class XlsxConverter
{
   TimeSpan[] workTimeSummary = new TimeSpan[5];
   TimeSpan[] breakTimeSummary = new TimeSpan[5];
   int[] kmSummary = new int[5];
   string[] carSummary = new string[5];



   public bool ToXlsx(string[] filesNames)
   {
      if (filesNames.Length == 0)
         return false;

      byte[] templateBytes = FileAccess.GetFileAsBytes("res://ExcelTemplates/StundenzettelTemplate.xlsx");

      if (FileAccess.GetOpenError() != Error.Ok)
         throw new System.IO.FileNotFoundException("Failed to load .xlsx timesheet template file");

      DateOnly referenceDate = DateOnly.Parse(filesNames[0].ReplaceN(".json", ""));
      DateOnly[] currentWeek = GetWeekDates(referenceDate);

      using (var ms = new System.IO.MemoryStream(templateBytes))
      {
         XLWorkbook workbook = new XLWorkbook(ms);
         IXLWorksheet sheet;
         string savePath;
         byte[] logoImageBytes = FileAccess.GetFileAsBytes("res://Assets/Logo.jpg");

         using (var logoMs = new System.IO.MemoryStream(logoImageBytes))
         {
            foreach (string timeSheetName in filesNames)
            {
               TimeSheet currentFile = FileManager.GetTimeSheetFromFile(timeSheetName);

               if (!currentWeek.Contains(currentFile.Date))
               {
                  if (currentFile.Date.DayOfWeek == DayOfWeek.Saturday || currentFile.Date.DayOfWeek == DayOfWeek.Sunday)
                     continue;

                  sheet = workbook.Worksheet("Wertezusammenfassung");
                  sheet.Cell(1, 1).CreateComment().AddText(FileManager.lastSaveString);
                  FillSheet(sheet, currentWeek, workTimeSummary, breakTimeSummary, kmSummary, carSummary);

                  savePath = $"{Manager.documentsFilePath}/Stundenzettel/Rapportzettel - {Manager.Instance.settingsData["workerName"]} - [ {currentWeek[0]} - {currentWeek[currentWeek.Length - 1]} ].xlsx";
                  workbook.SaveAs(savePath);
                  workbook.Dispose();

                  workbook = new XLWorkbook(ms);

                  currentWeek = GetWeekDates(currentFile.Date);
               }

               sheet = workbook.Worksheet((int)currentFile.Date.DayOfWeek);
               if (!FillSheet(sheet, currentFile, logoMs))
                  return false;
            }

            sheet = workbook.Worksheet("Wertezusammenfassung");
            sheet.Cell(1, 1).CreateComment().AddText(FileManager.lastSaveString);
            FillSheet(sheet, currentWeek, workTimeSummary, breakTimeSummary, kmSummary, carSummary);

            savePath = $"{Manager.documentsFilePath}/Stundenzettel/Rapportzettel - {Manager.Instance.settingsData["workerName"]} - [ {currentWeek[0]} - {currentWeek[currentWeek.Length - 1]} ].xlsx";
            workbook.SaveAs(savePath);
            workbook.Dispose();
         }
      }

      return true;
   }



   private bool FillSheet
   (
      IXLWorksheet sheet,
      TimeSheet currentFile,
      System.IO.MemoryStream logoData
   )
   {
      TimeSpanData[] timeSpanEntryData = Enum.GetValues<TimeSpanData>();

      int row;
      int col;
      string cellValue;


      TimeSpanEntry entry;
      Dictionary timeSpanDataDict;

      List<string> usedCars = new List<string>();
      TimeSpan allWorkTime = new TimeSpan();
      TimeSpan allBreakTime = new TimeSpan();
      int allKmDriven = 0;

      for (int i = 0; i < currentFile.TimeSpanEntries.Count; i++)
      {
         entry = currentFile.TimeSpanEntries.ElementAt(i);
         
         if (!entry.IsEverythingSet)
            return false;

         timeSpanDataDict = entry.ToDictionary();
         row = i + 8;

         for (int j = 0; j < timeSpanEntryData.Length; j++)
         {
            switch (timeSpanEntryData[j])
            {
               case TimeSpanData.FromTime:
                  col = 1;
                  break;

               case TimeSpanData.ToTime:
                  col = 2;
                  break;

               case TimeSpanData.Customer:
                  col = 3;
                  break;

               case TimeSpanData.Purpose:
                  col = 8;
                  break;

               case TimeSpanData.Description:
                  col = 1;
                  row += 7;
                  break;

               case TimeSpanData.KmStart:
                  col = 12;
                  break;

               case TimeSpanData.KmEnd:
                  col = 13;
                  break;

               default:
                  throw new ArgumentException($"Recieved unexpected timespanentry proprtyname: {nameof(timeSpanEntryData)}");
            }

            if (timeSpanEntryData[j] == TimeSpanData.Purpose)
               cellValue = PurposeNames.GetName(entry.Purpose);
            else
               cellValue = (string)timeSpanDataDict[timeSpanEntryData[j].ToString().ToCamelCase()];

            sheet.Cell(row, col).Value = cellValue;
         }

         if (entry.Purpose == Purposes.Break)
         {
            TimeSpan breakTime = entry.ToTime - entry.FromTime;
            sheet.Cell(row, 11).Value = breakTime.ToString("hh\\:mm");
            allBreakTime = allBreakTime.Add(breakTime);
         }
         else
         {
            TimeSpan workTime = entry.ToTime - entry.FromTime;
            sheet.Cell(row, 10).Value = workTime.ToString("hh\\:mm");
            allWorkTime = allWorkTime.Add(workTime);
         }

         if (!usedCars.Contains(CarNames.GetName(entry.Car)))
            usedCars.Add(CarNames.GetName(entry.Car));

         int kmDriven = entry.KmEnd - entry.KmStart;
         //sheet.Cell(row, 22).Value = kmDriven;
         allKmDriven += kmDriven;
      }

      string cars = "";
      if (usedCars.Count > 1)
      {
         usedCars.Remove("Nicht Fahrer");

         foreach (string car in usedCars)
            cars = $"{car}, {cars}";
      }
      else
         cars = usedCars[0];

      sheet.Cell(5, 9).Value = cars;
      carSummary[(int)currentFile.Date.DayOfWeek - 1] = cars;

      sheet.Cell(22, 10).Value = allWorkTime.ToString("hh\\:mm");
      workTimeSummary[(int)currentFile.Date.DayOfWeek - 1] = allWorkTime;

      if (allBreakTime.TotalMinutes < 30)
         allBreakTime = new TimeSpan(0, 30, 0);

      sheet.Cell(30, 11).Value = allBreakTime.ToString("hh\\:mm");
      breakTimeSummary[(int)currentFile.Date.DayOfWeek - 1] = allBreakTime;
      
      sheet.Cell(22, 12).Value = allKmDriven;
      kmSummary[(int)currentFile.Date.DayOfWeek - 1] = allKmDriven;

      sheet.Cell(38, 2).Value = currentFile.Date.ToString();
      sheet.Cell(38, 5).Value = (string)Manager.Instance.settingsData["workerName"];

      sheet.AddPicture(logoData).MoveTo(sheet.Cell(1, 1)).Scale(0.5);

      return true;
   }



   private void FillSheet
   (
      IXLWorksheet sheet,
      DateOnly[] weekDates,
      TimeSpan[] workTimes,
      TimeSpan[] breakTimes,
      int[] kms,
      string[] cars)
   {

      TimeSpan weekWork = new TimeSpan();
      TimeSpan weekBreak = new TimeSpan();
      int weekKm = 0;

      for (int i = 0; i < 5; i++)
      {
         sheet.Cell(i + 3, 1).Value = weekDates[i].ToString();

         sheet.Cell(i + 3, 2).Value = workTimes[i].ToString("hh\\:mm");
         weekWork = weekWork.Add(workTimes[i]);

         sheet.Cell(i + 3, 3).Value = breakTimes[i].ToString("hh\\:mm");
         weekBreak = weekBreak.Add(breakTimes[i]);

         sheet.Cell(i + 3, 4).Value = kms[i];
         weekKm += kms[i];
         
         sheet.Cell(i + 3, 5).Value = cars[i];
      }

      sheet.Cell(8, 2).Value = weekWork.ToString("hh\\:mm");
      sheet.Cell(8, 3).Value = weekBreak.ToString("hh\\:mm");
      sheet.Cell(8, 4).Value = weekKm;
   }



   private DateOnly[] GetWeekDates(DateOnly referenceDate)
   {
      DateOnly date = referenceDate.AddDays(-((int)referenceDate.DayOfWeek - 1));
      DateOnly[] weekDates = new DateOnly[5];

      for (int i = 0; i < weekDates.Length; i++)
         weekDates[i] = date.AddDays(i);

      return weekDates;
   }
}