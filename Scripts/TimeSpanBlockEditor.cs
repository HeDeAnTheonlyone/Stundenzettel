using System;
using Godot;

public partial class TimeSpanBlockEditor : CanvasLayer
{
   private const string constPathPart = "Padding/ItemList/";
   private LineEdit fromTime; 
   private LineEdit toTime; 
   private LineEdit customer; 
   private OptionButton purpose; 
   private TextEdit description;
   private TimeSpanEntry entry = Manager.Singleton.selectedEntry;



   public override void _Ready()
   {
      fromTime = GetNode<LineEdit>($"{constPathPart}FromTime/Input");
      toTime = GetNode<LineEdit>($"{constPathPart}ToTime/Input");
      customer = GetNode<LineEdit>($"{constPathPart}Customer/Input");
      purpose = GetNode<OptionButton>($"{constPathPart}Purpose/Input");
      description = GetNode<TextEdit>($"{constPathPart}DescriptionInput");

      SetInput(entry);
   }



   private void SetInput(TimeSpanEntry entry)
   {
		fromTime.Text = entry.FromTime.ToString();
		toTime.Text = entry.ToTime.ToString();
		customer.Text = entry.Customer;
		purpose.Selected = entry.Purpose != null ? (int)entry.Purpose : -1;
		description.Text = entry.Description;
   }



#region Signals
   private void SetFromTime() => SetFromTime(fromTime.Text);
   private void SetFromTime(string timeText)
   {
		try
		{
			entry.FromTime = TimeOnly.Parse(timeText);
		}
		catch
		{
			fromTime.Text = entry.FromTime.ToString();
		}
	}



   private void SetToTime() => SetToTime(toTime.Text);
   private void SetToTime(string timeText)
   {
		try
		{
			entry.ToTime = TimeOnly.Parse(timeText);
		}
		catch
		{
			toTime.Text = entry.ToTime.ToString();
		}
	}



   private void SetCustomer(string name) => entry.Customer = name;



   private void SetPurtpose(int index) => entry.Purpose = (Purposes)index;
   



   private void SetDescription() => entry.Description = description.Text;



   private void SwitchToTimeSheetEditor() => Manager.Singleton.SwitchScene("TimeSheetEditor");
   #endregion
}
