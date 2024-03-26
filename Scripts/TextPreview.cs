using Godot;



public partial class TextPreview : ColorRect
{
    private Callable inputProccessingMethod;
    private LineEdit textField;



    public override void _Ready()
    {
        textField = GetNode<LineEdit>("Padding/Input");
    }



    public void Setup(Callable _inputProccessingMethod)
    {
        inputProccessingMethod = _inputProccessingMethod;
        textField.GrabFocus();
    }



    #region Signals
    private void ProcessInput(string input)
    {
        inputProccessingMethod.Call(input);
        QueueFree();
    }



    private void ClosePreview() => QueueFree();
    #endregion
}