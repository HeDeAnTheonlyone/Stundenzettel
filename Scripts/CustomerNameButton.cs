using Godot;

public partial class CustomerNameButton : HSplitContainer
{
	public Customer Customer { get; set; }
	private TimeSpanBlockEditor timeSpanEditor;
	private Button selectButton;



	public override void _Ready()
	{
		timeSpanEditor = GetNode<TimeSpanBlockEditor>("../../../../../..");
		selectButton = GetNode<Button>("Select");

		selectButton.Text = $"{Customer.Name},\n{Customer.Town},\n{Customer.Street}";
	}



    #region Signals
    private void SelectName()
    {
        timeSpanEditor.SetCustomerName(Customer.Name);
        timeSpanEditor.SetCustomerTown(Customer.Town);
        timeSpanEditor.SetCustomerStreet(Customer.Street);
    }


    private void DeleteName()
    {
		timeSpanEditor.customerNames.Remove(Customer);
        QueueFree();
    }
    #endregion
}
