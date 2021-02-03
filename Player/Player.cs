using System.Threading.Tasks;
using Godot;

public class Player : Character
{
  [Signal] public delegate void Dead();
  [Signal] public delegate void Win();
  [Signal] public delegate void Moved();

  public override void _Ready()
  {
    base._Ready();

    health = Global.Life;
  }

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
      else if (raycasts[direction].IsColliding())
      {
        var collider = raycasts[direction].GetCollider();
        if (!(collider is TileMap tileMap)) continue;
        
        var tilePosition = tileMap.WorldToMap(Position);
        tilePosition -= raycasts[direction].GetCollisionNormal();

        var tileId = tileMap.GetCellv(tilePosition);
        var tileName = tileMap.TileSet.TileGetName(tileId);
        
        if (tileName != "gate") continue;
        if (!HasKey) continue;

        Global.Keys--;
        tileMap.SetCellv(tilePosition, -1);
      }
    }
  }

  // ReSharper disable once UnusedMember.Local
  private async void _on_Player_area_entered(Area2D area)
  {
    if (area.IsInGroup("enemies"))
    {
      var enemy = area as Character;
      enemy.TakeDamage(Damage);
      TakeDamage(enemy.Damage);
      Global.Life = health;
    }

    if (area.Name == "Exit")
    {
      CollisionShape2D.Disabled = true;
      AnimationPlayer.Play("exit");
      await ToSignal(AnimationPlayer, "animation_finished");
      EmitSignal(nameof(Win));
    }

    if (!(area is Pickup pickup)) return;

    pickup.DoPickup();
    GD.Print($"Pickup up {pickup.Type}!"); // TODO: revisit
    if (pickup.Type == "key")
    {
      Global.Keys++;
    }
    // TODO: implementation for other pickups
  }

  private bool HasKey => Global.Keys > 0;

  protected override void Die()
  {
    base.Die();
    EmitSignal(nameof(Dead));
  }
}
