using Godot;

public class Player : Character
{
  [Signal] public delegate void Dead();
  [Signal] public delegate void Win();
  [Signal] public delegate void Moved();

  public override void _Process(float delta)
  {
    base._Process(delta);

    if (!canMove) return;
    
    foreach (var direction in moves.Keys)
    {
      if (!Input.IsActionPressed(direction.ToString())) continue;
      
      if (Move(direction))
      {
        EmitSignal(nameof(Moved));
      }
    }
  }

  private void _on_Player_area_entered(Area2D area)
  {
    if (area.IsInGroup("enemies"))
    {
      GD.Print("Enemy");
    }
    if (area is Pickup pickup)
    {
      pickup.DoPickup();
      if (pickup.Type == "key") // TODO: revisit
      {
        GD.Print("Pickup up key!");
      }
      // TODO: implementation for other pickups
    }
  }
}
