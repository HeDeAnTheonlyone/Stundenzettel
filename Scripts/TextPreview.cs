using Godot;



public partial class TextPreview : ColorRect
{
    private Callable inputProccessingMethod;
    private LineEdit textField;



    public override void _Ready()
    {
        textField = GetNode<LineEdit>("Padding/Input");
    }



    public void Setup(string content, Callable _inputProccessingMethod)
    {
        inputProccessingMethod = _inputProccessingMethod;
        textField.Text = content;
        textField.GrabFocus();
        textField.CaretColumn = content.Length;
    }



    #region Signals
    private void ProcessInput(string input)
    {
        GD.Print(Manager.Instance.textPreviewExists);
        QueueFree();
    }



    private void ClosePreview()
    {
        Manager.Instance.textPreviewExists = false;
        QueueFree();
    }

    #endregion
}