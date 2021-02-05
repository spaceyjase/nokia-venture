using Godot;
using System;
using System.Linq;

public class Wolf : Character
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

        if (Mathf.Abs(Map.PlayerPosition.x - Position.x) < float.Epsilon)
        {
            facing = Map.PlayerPosition.y > Position.y ? Facing.Down : Facing.Up;
        }
        else
        {
            facing = Map.PlayerPosition.x > Position.x ? Facing.Right : Facing.Left;
        }
        Move(facing);
    }
}
