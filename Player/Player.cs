using Godot;

public class Player : Character
{
  [Signal] public delegate void Dead();
  [Signal] public delegate void Win();
  [Signal] public delegate void Moved();

  private AudioStream walkSfx;
  private AudioStream pickupKeySfx;
  private AudioStream pickupHealthSfx;
  private AudioStream hitSfx;
  private AudioStream deadSfx;
  private AudioStream unlockSfx;
  private AudioStream exitSfx;
  private Node audioManager;

  public override void _Ready()
  {
    base._Ready();

    health = Global.Life;
    canMove = true;
    walkSfx = ResourceLoader.Load<AudioStream>("res://data/sfx/blip14.wav");
    pickupKeySfx = ResourceLoader.Load<AudioStream>("res://data/sfx/good1.wav");
    pickupHealthSfx = ResourceLoader.Load<AudioStream>("res://data/sfx/good3.wav");
    hitSfx = ResourceLoader.Load<AudioStream>("res://data/sfx/hit2.wav");
    deadSfx = ResourceLoader.Load<AudioStream>("res://data/sfx/negative1.wav");
    unlockSfx = ResourceLoader.Load<AudioStream>("res://data/sfx/blip11.wav");
    exitSfx = ResourceLoader.Load<AudioStream>("res://data/sfx/blip6.wav");
    audioManager = GetNode("/root/AudioManager");
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
        audioManager.Call("play_sfx", walkSfx);
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
        if (!Global.HasKey) continue;

        Global.Keys--;
        tileMap.SetCellv(tilePosition, -1);
        audioManager.Call("play_sfx", unlockSfx);
      }
    }
  }

  // ReSharper disable once UnusedMember.Local
  private async void _on_Player_area_entered(Area2D area)
  {
    if (area.IsInGroup("enemies"))
    {
      audioManager.Call("play_sfx", hitSfx);
      var enemy = area as Character;
      enemy.TakeDamage(Damage);
      TakeDamage(enemy.Damage);
      Global.Life = health;
      if (Global.Life <= 0)
      {
        CollisionShape2D.Disabled = true;
        audioManager.Call("play_sfx", deadSfx);
        AnimationPlayer.Play("death");
        await ToSignal(AnimationPlayer, "animation_finished");
        EmitSignal(nameof(Dead));
      }
    }

    if (area.Name.Contains("Exit"))
    {
      DoWin(true);
    }

    if (!(area is Pickup pickup)) return;

    pickup.DoPickup();
    switch (pickup.Type)
    {
      case "key":
        audioManager.Call("play_sfx", pickupKeySfx);
        Global.Keys++;
        break;
      case "potion":
        audioManager.Call("play_sfx", pickupHealthSfx);
        Global.Life += 5;
        break;
      case "flask":
        audioManager.Call("play_sfx", pickupHealthSfx);
        Global.Life++;
        break;
      case "chest":
        DoWin(false);
        break;
    }

    // TODO: implementation for other pickups
  }

  private async void DoWin(bool exit)
  {
      audioManager.Call("play_sfx", exitSfx);
      CollisionShape2D.Disabled = true;
      if (exit)
      {
        AnimationPlayer.Play("exit");
        await ToSignal(AnimationPlayer, "animation_finished");
      }
      EmitSignal(nameof(Win));
  }

  protected override void Die()
  {
    base.Die();
    EmitSignal(nameof(Dead));
  }
}
