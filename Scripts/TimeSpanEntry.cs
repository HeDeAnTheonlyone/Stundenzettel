using System;
using Godot;
using Godot.Collections;

public class TimeSpanEntry
{
   public bool IsEverythingSet
   {
      get
      {
         bool set;

         set = Purpose != Purposes.NoPurpose; 

         if (Purpose == Purposes.Work)
            set = set && Description.Length > 0 && Customer.Length > 0;

         return set;
      }
   }
   public TimeOnly FromTime { get; set; }
   public TimeOnly ToTime { get; set; }
   public string Customer { get; set; }
   public Purposes Purpose { get; set; }
   public string Description { get; set; }
   public Car Car { get; set; } 
   public int KmStart { get; set; }
   public int KmEnd { get; set; }



#region Constructors
   public TimeSpanEntry
   (
      TimeOnly fromTime = new TimeOnly(),
      TimeOnly toTime = new TimeOnly(),
      string customer = "",
      Purposes purpose = Purposes.NoPurpose,
      string description = "",
      Car car = Car.Nicht_Fahrer,
      int kmStart = 0,
      int kmEnd = 0
   )
   {
      FromTime = fromTime;
      ToTime = toTime;
      Customer = customer;
      Purpose = purpose;
      Description = description;
      Car = car;
      KmStart = kmStart;
      KmEnd = kmEnd;
   }



   public TimeSpanEntry(Dictionary dict)
   {
      FromTime = TimeOnly.Parse((string)dict["fromTime"]);
      ToTime = TimeOnly.Parse((string)dict["toTime"]);
      Customer = (string)dict["customer"];
      Purpose = (Purposes)(int)dict["purpose"];
      Description = (string)dict["description"];
      Car = (Car)(int)dict["car"];
      KmStart = (int)dict["kmStart"];
      KmEnd = (int)dict["kmEnd"];
   }
#endregion



   public Dictionary ToDictionary() => new Dictionary
   {
      { "fromTime", FromTime.ToString() },
      { "toTime", ToTime.ToString() },
      { "customer", Customer},
      { "purpose", (int)Purpose },
      { "description", Description},
      { "car", (int)Car },
      { "kmStart", KmStart },
      { "kmEnd", KmEnd }
   };



   public string ToJsonString() => Json.Stringify(ToDictionary(), "\t");
}
