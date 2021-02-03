using Godot;

public class HUD : CanvasLayer
{
  private Label KeyLabel => GetNode<Label>("MarginContainer/KeyLabel");
  private Label HealthLabel => GetNode<Label>("MarginContainer/Health");
  
  public void UpdateKeys()
  {
    KeyLabel.Text = $"Key: {Global.Keys}";
  }
  
  public void UpdateHealth()
  {
    HealthLabel.Text = $"Life: {Global.Life}";
  }
}
