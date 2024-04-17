using Godot;



public partial class TextPreview : ColorRect
{
    private Callable inputProcessingMethod;
    private LineEdit textField;



    public override void _Ready()
    {
        textField = GetNode<LineEdit>("Padding/Input");
    }



    public void Setup(string content, Callable _inputProcessingMethod)
    {
        inputProcessingMethod = _inputProcessingMethod;
        textField.Text = content;
        textField.GrabFocus();
        textField.CaretColumn = content.Length;
    }



    #region Signals
    private void ProcessInput(string input)
    {
        inputProcessingMethod.Call(textField.Text);
        ClosePreview();
    }



    private void ClosePreview()
    {
        Manager.Instance.textPreviewExists = false;
        QueueFree();
    }

    #endregion
}