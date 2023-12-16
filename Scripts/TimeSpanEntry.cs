using System;
using Godot;
using Godot.Collections;

public class TimeSpanEntry
{
   public TimeOnly FromTime { get; set; }
   public TimeOnly ToTime { get; set; }
   public string Customer { get; set; }
   public Purposes? Purpose { get; set; }
   public string Description { get; set; }



#region Constructors
   public TimeSpanEntry(TimeOnly fromTime = new TimeOnly(), TimeOnly toTime = new TimeOnly(), string customer = "", Purposes? purpose = null, string description = "")
   {
      FromTime = fromTime;
      ToTime = toTime;
      Customer = customer;
      Purpose = purpose;
      Description = description;
   }



   public TimeSpanEntry(Dictionary dict)
   {
      FromTime = TimeOnly.Parse((string)dict["fromTime"]);
      ToTime = TimeOnly.Parse((string)dict["toTime"]);
      Customer = (string)dict["customer"] == "" ? null : (string)dict["customer"];
      Purpose = (string)dict["purpose"] == "" ? null : (Purposes)(int)dict["purpose"];
      Description = (string)dict["description"] == "" ? null : (string)dict["description"];
   }
#endregion



   public Dictionary ToDictionary() => new Dictionary
   {
      { "fromTime", FromTime.ToString() },
      { "toTime", ToTime.ToString() },
      { "customer", Customer == null ? "" : Customer },
      { "purpose", Purpose == null ? "" : (int)Purpose },
      { "description", Description == null ? "" :Description }
   }; 



   public string ToJsonString()
   {
      return Json.Stringify(this.ToDictionary(), "\t");
   }
}
