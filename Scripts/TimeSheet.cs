using System;
using System.Collections.Generic;

public class TimeSheet 
{
   public DateOnly Date { get; set; }
   public List<TimeSpanEntry> TimeSpanEntries { get; set; }

   public TimeSheet(DateOnly date, List<TimeSpanEntry> timeSpanEntries)
   {
      Date = date;
      TimeSpanEntries = timeSpanEntries;
   }
}