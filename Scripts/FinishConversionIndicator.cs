using Godot;

public partial class FinishConversionIndicator : Label
{
	public override void _Process(double delta)
	{
		if (Visible)
		{
			Modulate = Modulate with { A = Modulate.A - 0.01f };
			
			if (Modulate.A <= 0)
				Visible = false;
		}
	}



    private void SetVanishTrue() => Visible = true;

}
