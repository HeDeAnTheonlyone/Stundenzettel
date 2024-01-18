using Godot;

public partial class ConfirmationPanel : Panel
{
	private Callable deleteMethod;



    public void OpenConfirmDeletion(Callable method)
	{
		deleteMethod = method;
		Visible = true;
	}



	private void ConfirmDeleteItem()
	{
		deleteMethod.Call();
		Visible = false;
	}



    private void NotConfirmDeleteItem() => Visible = false;

}
