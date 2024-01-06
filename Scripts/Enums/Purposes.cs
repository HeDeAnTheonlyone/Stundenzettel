using System;

public enum Purposes
{
   WorkStart,
   WorkEnd,
   Work,
   WorkshopWork,
   Meeting,
   LoadCar,
   DriveToCustomer,
   DriveToCompany,
   DriveToGasStation,
   DriveToOther,
   Refuel,
   ErrandTrip,
   Break,
   Sick,
   NoPurpose
}

public static class PurposeNames
{
   public static string GetName(Purposes purpose)
   {
      string name;

      switch(purpose)
      {
         case Purposes.WorkStart:
            name = "Arbeitsbeginn";
            break;

         case Purposes.WorkEnd:
            name = "Arbeitsende";
            break;

         case Purposes.Work:
            name = "Arbeit";
            break;

         case Purposes.WorkshopWork:
            name = "Werkstattarbeiten";
            break;

         case Purposes.Meeting:
            name = "Besprechung";
            break;

         case Purposes.LoadCar:
            name = "Auto laden";
            break;

         case Purposes.DriveToCustomer:
            name = "Fahrt zum Kunden";
            break;

         case Purposes.DriveToCompany:
            name = "Fahrt zur Firma";
            break;

         case Purposes.DriveToGasStation:
            name = "Fahrt zur Tankstelle";
            break;

         case Purposes.DriveToOther:
            name = "Sonstige Fahrt";
            break;

         case Purposes.Refuel:
            name = "Tanken";
            break;

         case Purposes.ErrandTrip:
            name = "Materialbesorgung";
            break;

         case Purposes.Break:
            name = "Pause";
            break;

         case Purposes.Sick:
            name = "Krank";
            break;
         
         case Purposes.NoPurpose:
            name = "Nicht angegeben";
            break;
         
         default:
            throw new Exception("Invalid Purpose. Unable to return Purposename.");
      }

      return name;
   }
}