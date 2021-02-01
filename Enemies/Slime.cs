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

    //var players = GetTree().GetNodesInGroup("player"); // TODO: magic string
    //(players[0] as Player).Connect(nameof(Player.Moved), this, nameof(_on_Player_Moved));
    
    await ToSignal(GetTree().CreateTimer(0.5f), nameof(Timeout));
  }

  private void Timeout()
  {
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

  // public void _on_Player_Moved()
  // {
  //   if (Move(facing) || GD.Randi() % 10 > 5)
  //   {
  //     facing = moves.Keys.ToArray()[GD.Randi() % moves.Keys.Count];
  //   }
  // }
}
