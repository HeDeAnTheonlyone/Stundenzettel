using Godot;
using System;

public partial class TimeSpanBlockButton : HSplitContainer
{
	public TimeSpanEntry Entry { get; set; }
    private Button editButton;



    public override void _Ready()
    {
		if (Entry == null)
		{
			Entry = new TimeSpanEntry
			(
			Manager.Singleton.lastTimeStamp,
			TimeOnly.FromDateTime(DateTime.Now)
			);

			Manager.Singleton.lastTimeStamp = Entry.ToTime;

			Manager.Singleton.selectedSheet.TimeSpanEntries.Add(Entry);
		}			

		editButton = GetNode<Button>("Edit");

		editButton.Text = $"{Entry.FromTime} - {Entry.ToTime}";
    }



#region Signals
	private void SwitchToTimeSpanBlockEditor()
	{
		Manager.Singleton.selectedEntry = Entry;
		Manager.Singleton.SwitchScene("TimeSpanBlockEditor");
	}



    private void DeleteEntry()
    {
		Manager.Singleton.selectedSheet.TimeSpanEntries.Remove(Entry);
        QueueFree();
    }
#endregion
}
