using Godot;
using System;
using System.Linq;
using Array = Godot.Collections.Array;

public class Slime : Character
{
  public override async void _Ready()
  {
    base._Ready();

    facing = moves.Keys.ToArray()[GD.Randi() % moves.Keys.Count];
    await ToSignal(GetTree().CreateTimer(1f), "timeout");
    canMove = true;
  }

  public override void _Process(float delta)
  {
    base._Process(delta);
    
    if (!canMove) return;
    
    if (Move(facing) || GD.Randi() % 10 > 5)
    {
      facing = moves.Keys.ToArray()[GD.Randi() % moves.Keys.Count];
    }
  }
}
