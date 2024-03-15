using Godot;
using System;

public partial class TimeSpanBlockButton : HSplitContainer
{
	public TimeSpanEntry entry { get; set; }
    private Button editButton;
	private bool moveMode = false;
	private Vector2 lastPos;



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

        if (!entry.IsEverythingSet)
			Modulate = Modulate with { R = 1f, G = 0.196f, B = 0.196f };

        editButton = GetNode<Button>("Edit");
		

		editButton.Text = $"{entry.FromTime} - {entry.ToTime}";
	}



    public override void _Process(double delta)
    {
        if (moveMode)
			GlobalPosition = GetGlobalMousePosition() - Size / 2;
    }



	public void ResetPos() => Position = lastPos;



    #region Signals
    private void ButtonInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("RightClick"))
		{
			lastPos = Position;
			moveMode = true;
		}

		if (Input.IsActionJustReleased("RightClick"))
		{
			moveMode = false;
			GetNode<TimeSheetEditor>("../../../../..").UpdateOrder(this);
		}
	}



	private void SwitchToTimeSpanBlockEditorBuffer() => CallDeferred("SwitchToTimeSpanBlockEditor");
	private void SwitchToTimeSpanBlockEditor()
	{
		if (!moveMode)
		{
			Manager.Instance.selectedEntry = entry;
			Manager.Instance.SwitchScene("TimeSpanBlockEditor");
		}
	}



	private void DeleteEntry() => GetNode<ConfirmationPanel>("../../../../../ConfirmationPanel").OpenConfirmDeletion(Callable.From(DeleteEntryConfirmed));



	private void DeleteEntryConfirmed()
    {
		Manager.Instance.lastTimeStamp = entry.FromTime;
		Manager.Instance.selectedSheet.TimeSpanEntries.Remove(entry);
        QueueFree();
    }
#endregion
}
