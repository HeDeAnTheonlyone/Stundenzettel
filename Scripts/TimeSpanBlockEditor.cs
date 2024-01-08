using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class TimeSpanBlockEditor : CanvasLayer
{
   private VBoxContainer timeList;
   private VBoxContainer driveList;
   private LineEdit fromTime; 
   private LineEdit toTime; 
   private LineEdit customer;
   private ColorRect customerPresetSettings;
   public List<string> customerNames;
   private VBoxContainer customerPresetList;
   private PackedScene customerNameButton;
   private OptionButton purpose;
   private TextEdit description;
   private OptionButton car;
   private LineEdit kmStart;
   private LineEdit kmEnd;
   private TimeSpanEntry entry = Manager.Instance.selectedEntry;



   public override void _Ready()
   {
      timeList = GetNode<VBoxContainer>("Padding/TimeList");
      driveList = GetNode<VBoxContainer>("Padding/DriveList");
      fromTime = timeList.GetNode<LineEdit>("FromTime/Input");
      toTime = timeList.GetNode<LineEdit>("ToTime/Input");
      customer = timeList.GetNode<LineEdit>("Customer/Input");
      customerPresetSettings = GetNode<ColorRect>("CustomerPresetBg");
      customerPresetList = customerPresetSettings.GetNode<VBoxContainer>("Padding/ItemList/ScrollContainer/NameList");
      customerNameButton = GD.Load("res://Objects/CustomerNameButton.tscn") as PackedScene;
      purpose = timeList.GetNode<OptionButton>("Purpose/Input");
      description = timeList.GetNode<TextEdit>("DescriptionInput");
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
		customer.Text = entry.Customer;
		purpose.Selected = (int)entry.Purpose;
		description.Text = entry.Description;
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
		
      if (success)
			entry.FromTime = parsedTime;
		else
			fromTime.Text = entry.FromTime.ToString();
	}



   private void SetToTime() => SetToTime(toTime.Text);
   private void SetToTime(string timeText)
   {
      bool success = TimeOnly.TryParse(timeText, out TimeOnly parsedTime);

      if (success)
         entry.ToTime = parsedTime;
      else
         toTime.Text = entry.ToTime.ToString();
   }



   public void SetCustomer(string name)
   {
      entry.Customer = name;

      if (customer.Text != entry.Customer)
         customer.Text = entry.Customer;
   }



   #region CustomerPresetSettings
   private void CustomerPresetSettings(InputEvent e)
   {
      if (e.IsActionPressed("RightClick"))
      {
         customerPresetSettings.Visible = true;
         UpdateCustomerNameList();
      }
   }



   private void UpdateCustomerNameList() => customerPresetList.PopulateList(customerNames, customerNameButton);



   private void SaveName()
   {
      if (!string.IsNullOrEmpty(customer.Text))
      {
         if (!customerNames.Contains(customer.Text))
         {
            customerNames.Add(customer.Text);
            UpdateCustomerNameList();
         }
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



   private void SetCar(int index) => entry.Car = (Car)index;



   private void SetKmStart() => SetKmStart(kmStart.Text);
   private void SetKmStart(string kmText)
   {
      bool success = int.TryParse(kmText, out int km);

      if (success)
      {
         if (km < 0)
            kmStart.Text = entry.KmStart.ToString();
         else
            entry.KmStart = km;
      }
      else
         kmStart.Text = entry.KmStart.ToString();
   }



   private void SetKmEnd() => SetKmEnd(kmEnd.Text);
   private void SetKmEnd(string kmText)
   {
      bool success  = int.TryParse(kmText, out int km);

      if (success)
      {
         if (km < 0)
            kmEnd.Text = entry.KmEnd.ToString();
         else
            entry.KmEnd = km;
      }     
      else
         kmEnd.Text = entry.KmEnd.ToString();
   }



   private void SwitchTabs(int tabIndex)
   {
      switch(tabIndex)
      {
         case 0:
            timeList.Visible = true;
            driveList.Visible = false;
            break;

         case 1:
            timeList.Visible = false;
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
      
      Manager.Instance.SwitchScene("TimeSheetEditor");
   }
#endregion
}
