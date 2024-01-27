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
            set =
            (
               set &&
               Description.Length > 0 &&
               CustomerName.Length > 0 &&
               CustomerTown.Length > 0 &&
               CustomerStreet.Length > 0
            );
         
         if
         (
            Purpose == Purposes.DriveToCustomer ||
            Purpose == Purposes.DriveToCompany ||
            Purpose == Purposes.DriveToOther ||
            Purpose == Purposes.DrivePrivate ||
            Purpose == Purposes.DriveToGasStation
         )
            set = set && Car != Car.None;

         return set;
      }
   }
   public TimeOnly FromTime { get; set; }
   public TimeOnly ToTime { get; set; }
   public Purposes Purpose { get; set; }
   public string Description { get; set; }

   public string CustomerName { get; set; }
   public string CustomerTown { get; set; }
   public string CustomerStreet { get; set; }

   public Car Car { get; set; } 
   public int KmStart { get; set; }
   public int KmEnd { get; set; }



#region Constructors
   public TimeSpanEntry
   (
      TimeOnly fromTime = new TimeOnly(),
      TimeOnly toTime = new TimeOnly(),
      Purposes purpose = Purposes.NoPurpose,
      string description = "",

      string customerName = "",
      string customerTown = "",
      string customerStreet = "",
      
      Car car = Car.None,
      int kmStart = 0,
      int kmEnd = 0
   )
   {
      FromTime = fromTime;
      ToTime = toTime;
      Purpose = purpose;
      Description = description;

      CustomerName = customerName;
      CustomerTown = customerTown;
      CustomerStreet = customerStreet;
      
      Car = car;
      KmStart = kmStart;
      KmEnd = kmEnd;
   }



   public TimeSpanEntry(Dictionary dict)
   {
      FromTime = TimeOnly.Parse((string)dict["fromTime"]);
      ToTime = TimeOnly.Parse((string)dict["toTime"]);
      Purpose = (Purposes)(int)dict["purpose"];
      Description = (string)dict["description"];

      CustomerName = (string)dict["customerName"];
      CustomerTown = (string)dict["customerTown"];
      CustomerStreet = (string)dict["customerStreet"];
      
      Car = (Car)(int)dict["car"];
      KmStart = (int)dict["kmStart"];
      KmEnd = (int)dict["kmEnd"];
   }
#endregion



   public Dictionary ToDictionary() => new Dictionary
   {
      { "fromTime", FromTime.ToString() },
      { "toTime", ToTime.ToString() },
      { "purpose", (int)Purpose },
      { "description", Description},

      { "customerName", CustomerName},
      { "customerTown", CustomerTown},
      { "customerStreet", CustomerStreet},

      { "car", (int)Car },
      { "kmStart", KmStart },
      { "kmEnd", KmEnd }
   };



   public string ToJsonString() => Json.Stringify(ToDictionary(), "\t");
}
