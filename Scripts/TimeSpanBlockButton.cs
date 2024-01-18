using Godot;
using System;

public partial class TimeSpanBlockButton : HSplitContainer
{
	public TimeSpanEntry entry { get; set; }
    private Button editButton;



    public override void _Ready()
    {
		if (entry == null)
		{
			entry = new TimeSpanEntry
			(
			Manager.Instance.lastTimeStamp,
			TimeOnly.FromDateTime(DateTime.Now)
			);

			Manager.Instance.lastTimeStamp = entry.ToTime;

			Manager.Instance.selectedSheet.TimeSpanEntries.Add(entry);
		}			

		editButton = GetNode<Button>("Edit");

		editButton.Text = $"{entry.FromTime} - {entry.ToTime}";
    }



#region Signals
	private void SwitchToTimeSpanBlockEditor()
	{
		Manager.Instance.selectedEntry = entry;
		Manager.Instance.SwitchScene("TimeSpanBlockEditor");
	}



	private void DeleteEntry() => GetNode<ConfirmationPanel>("../../../../../ConfirmationPanel").OpenConfirmDeletion(Callable.From(DeleteEntryConfirmed));



	private void DeleteEntryConfirmed()
    {
		Manager.Instance.selectedSheet.TimeSpanEntries.Remove(entry);
        QueueFree();
    }
#endregion
}
