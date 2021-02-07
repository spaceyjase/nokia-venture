using System.Linq;
using Godot;

public class HunterCharacter : Character
{
  public override async void _Ready()
  {
    base._Ready();

    facing = moves.Keys.ToArray()[GD.Randi() % moves.Keys.Count];
    await ToSignal(GetTree().CreateTimer(2f), "timeout");
    canMove = true;
  }
  
  public override void _Process(float delta)
  {
    base._Process(delta);
    
    if (!canMove) return;

    if (Mathf.Abs(Map.PlayerPosition.x - Position.x) < float.Epsilon)
    {
      facing = Map.PlayerPosition.y > Position.y ? Facing.Down : Facing.Up;
    }
    else
    {
      facing = Map.PlayerPosition.x > Position.x ? Facing.Right : Facing.Left;
    }

    if (Move(facing)) return;
    
    // Couldn't move - try another direction?
    facing = moves.Keys.ToArray()[GD.Randi() % moves.Keys.Count];
    Move(facing);
  }
}