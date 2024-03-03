using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class TimeSpanBlockEditor : CanvasLayer
{
   private VBoxContainer timeList;
   private VBoxContainer customerList;
   private VBoxContainer driveList;
   //
   private LineEdit fromTime; 
   private LineEdit toTime; 
   private ColorRect customerPresetSettings;
   private VBoxContainer customerPresetList;
   private OptionButton purpose;
   private TextEdit description;
   //
   private LineEdit customerName;
   private LineEdit customerTown;
   private LineEdit customerStreet;
   //
   private OptionButton car;
   private LineEdit kmStart;
   private LineEdit kmEnd;
   //
   public List<Customer> customerNames;
   private PackedScene customerNameButton;
   private TimeSpanEntry entry = Manager.Instance.selectedEntry;
   private int previousTab = 0;



   public override void _Ready()
   {
      timeList = GetNode<VBoxContainer>("Padding/TimeList");
      customerList = GetNode<VBoxContainer>("Padding/CustomerList");
      driveList = GetNode<VBoxContainer>("Padding/DriveList");

      fromTime = timeList.GetNode<LineEdit>("FromTime/Input");
      toTime = timeList.GetNode<LineEdit>("ToTime/Input");
      customerPresetSettings = GetNode<ColorRect>("CustomerPresetBg");
      customerPresetList = customerPresetSettings.GetNode<VBoxContainer>("Padding/ItemList/ScrollContainer/NameList");
      customerNameButton = GD.Load("res://Objects/CustomerNameButton.tscn") as PackedScene;
      purpose = timeList.GetNode<OptionButton>("Purpose/Input");
      description = timeList.GetNode<TextEdit>("DescriptionInput");

      customerName = customerList.GetNode<LineEdit>("Name/Input");
      customerTown = customerList.GetNode<LineEdit>("Town/Input");
      customerStreet = customerList.GetNode<LineEdit>("Street/Input");
      
      car = driveList.GetNode<OptionButton>("Car/Input");
      kmStart = driveList.GetNode<LineEdit>("KmBegin/Input");
      kmEnd = driveList.GetNode<LineEdit>("KmEnd/Input");

      customerNames = Manager.Instance.customerNames;

      PopulateOptionButtons();
      
      SetInput(entry);
   }



   private void SetInput(TimeSpanEntry entry)
   {
		fromTime.Text = entry.FromTime.ToString();
		toTime.Text = entry.ToTime.ToString();
		purpose.Selected = (int)entry.Purpose;
		description.Text = entry.Description;

		customerName.Text = entry.CustomerName;
		customerTown.Text = entry.CustomerTown;
      customerStreet.Text = entry.CustomerStreet;
      
      car.Selected = (int)entry.Car;
      kmStart.Text = entry.KmStart.ToString();
      kmEnd.Text = entry.KmEnd.ToString();
   }



   private void PopulateOptionButtons()
   {
      foreach (Purposes p in Enum.GetValues(typeof(Purposes)))
         purpose.AddItem(PurposeNames.GetName(p));

      foreach (Car c in Enum.GetValues(typeof(Car)))
         car.AddItem(CarNames.GetName(c));
   }



#region Signals
   private void SetFromTime() => SetFromTime(fromTime.Text);
   private void SetFromTime(string timeText)
   {
      bool success = TimeOnly.TryParse(timeText, out TimeOnly parsedTime);
		
      if (success && parsedTime < entry.ToTime)
			entry.FromTime = parsedTime;
		else
			fromTime.Text = entry.FromTime.ToString();
	}



   private void SetToTime() => SetToTime(toTime.Text);
   private void SetToTime(string timeText)
   {
      bool success = TimeOnly.TryParse(timeText, out TimeOnly parsedTime);

      if (success && parsedTime > entry.FromTime)
         entry.ToTime = parsedTime;
      else
         toTime.Text = entry.ToTime.ToString();
   }



   #region CustomerPresetSettings
   private void CustomerPresetSettings()
   {
      customerPresetSettings.Visible = true;
      UpdateCustomerNameList();
   }



   private void UpdateCustomerNameList() => customerPresetList.PopulateList(customerNames, customerNameButton);



   private void SaveName()
   {
      Customer newCustomer = new Customer(entry.CustomerName, entry.CustomerTown, entry.CustomerStreet); 
      if
      (
         newCustomer.Name != "" ||
         newCustomer.Town != "" ||
         newCustomer.Street != "" &&
         !customerNames.Contains(newCustomer)
      )
      {
         customerNames.Add(newCustomer);
         UpdateCustomerNameList();
      }
   }



   private void CloseCustomerPresetSettings()
   {
      if (!customerNames.SequenceEqual(Manager.Instance.customerNames))
      {
         Manager.Instance.customerNames = customerNames;
         Manager.Instance.SaveCustomerNames();
      }

      customerPresetSettings.Visible = false;
   }
   #endregion



   private void SetPurtpose(int index) => entry.Purpose = (Purposes)index;
   



   private void SetDescription() => entry.Description = description.Text;



   public void SetCustomerName(string name)
   {
      entry.CustomerName = name;

      if (customerName.Text != entry.CustomerName)
         customerName.Text = entry.CustomerName;
   }
   
   
   
   public void SetCustomerTown(string town)
   {
      entry.CustomerTown = town;

      if (customerTown.Text != entry.CustomerTown)
         customerTown.Text = entry.CustomerTown;
   }



   public void SetCustomerStreet(string street)
   {
      entry.CustomerStreet = street;

      if (customerStreet.Text != entry.CustomerStreet)
         customerStreet.Text = entry.CustomerStreet;
   }



   private void SetCar(int index) => entry.Car = (Car)index;



   private void SetKmStart() => SetKmStart(kmStart.Text);
   private void SetKmStart(string kmText)
   {
      bool success = int.TryParse(kmText, out int km);

      if (success && km > 0)
      {
         entry.KmStart = km;
         
         if (km > entry.KmEnd)
         {
            entry.KmEnd = km;
            kmEnd.Text = km.ToString();
         }
      }
      else
         kmStart.Text = entry.KmStart.ToString();
   }



   private void SetKmEnd() => SetKmEnd(kmEnd.Text);
   private void SetKmEnd(string kmText)
   {
      bool success  = int.TryParse(kmText, out int km);

      if (success && km > 0 && km > entry.KmStart)
      {
         entry.KmEnd = km;
      }     
      else
         kmEnd.Text = entry.KmEnd.ToString();
   }



   private void SwitchTabs(int tabIndex)
   {
      if (previousTab == 1)
         Manager.Instance.SaveCustomerNames();
      
      previousTab = tabIndex;

      switch (tabIndex)
      {
         case 0:
            timeList.Visible = true;
            customerList.Visible = false;
            driveList.Visible = false;
            break;

         case 1:
            timeList.Visible = false;
            customerList.Visible = true;
            driveList.Visible = false;
            break;

         case 2:
            timeList.Visible = false;
            customerList.Visible = false;
            driveList.Visible = true;
            break;
      }
   }



   private void SwitchToTimeSheetEditor()
   {
      SetFromTime();
      SetToTime();
      SetKmStart();
      SetKmEnd();

      FileManager.SaveTimeSheet(Manager.Instance.selectedSheet);
      Manager.Instance.SaveCustomerNames();
      
      Manager.Instance.SwitchScene("TimeSheetEditor");
   }
#endregion
}
