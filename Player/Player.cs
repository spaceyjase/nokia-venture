using Godot;

public class Player : Character
{
  [Signal] delegate void Dead();
  [Signal] delegate void Switch();
  [Signal] delegate void Win();
  [Signal] delegate void Moved();

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
}
