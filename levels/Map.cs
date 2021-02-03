using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

public class Map : Node2D
{
  private TileMap Ground => GetNode<TileMap>("Ground");
  private TileMap Walls => GetNode<TileMap>("Walls");
  private TileMap Items => GetNode<TileMap>("Items");
  
  private Player Player => GetNode<Player>("Player");
  private HUD HUD => GetNode<HUD>("HUD");
  private Camera2D Camera => GetNode<Camera2D>("Player/Camera2D");

  private List<object> gates = new List<object>();
  
  public override void _Ready()
  {
    base._Ready();

    GD.Randomize();
    Items.Hide();
    
    var gateId = Walls.TileSet.FindTileByName("gate");
    foreach (var cell in Walls.GetUsedCellsById(gateId))
    {
      gates.Add(cell);
    }

    SpawnItems();
    Player.Connect(nameof(Player.Dead), this, nameof(OnGameOver));
    Player.Connect(nameof(Player.Win), this, nameof(OnPlayerWin));
    Global.Instance.Connect(nameof(Global.KeysChanged), HUD, nameof(HUD.UpdateKeys));
    HUD.UpdateKeys();
  }

  private void SpawnItems()
  {
    // TODO: GameManager
    var lookup = new Dictionary<string, string>
    {
      { nameof(Player), nameof(Player) },
      { nameof(Slime), "Enemy" },
      { "stairs_two", nameof(Pickup) },
      { "key", nameof(Pickup) },
      { "chest", nameof(Pickup) },
    };
    
    foreach (Vector2 cell in Items.GetUsedCells())
    {
      var id = Items.GetCellv(cell);
      var cellType = Items.TileSet.TileGetName(id);
      var position = Items.MapToWorld(cell) + Items.CellSize / 2;
      if (!lookup.TryGetValue(cellType, out var type)) continue;
      switch (type)
      {
        case nameof(Player):
          Player.Position = position;
          Player.TileSize = (int)Items.CellSize.x;
          break;
        case nameof(Pickup):
          var pickup = ResourceLoader.Load<PackedScene>($"res://Pickups/Pickup.tscn");
          if (pickup != null)
          {
            var p = pickup.Instance() as Pickup;
            p.Init(cellType, position);
            AddChild(p);
          }
          break;
        case "Enemy": // TODO: game constants
          // TODO: global/singleton factory method
          var enemy = ResourceLoader.Load<PackedScene>($"res://Enemies/{cellType}.tscn");
          if (enemy != null)
          {
            var c = enemy.Instance() as Character;
            c.Position = position;
            c.TileSize = (int)Items.CellSize.x;
            AddChild(c);
          }
          break;
      }
    }
  }

  private void SetCameraLimits()
  {
    var mapSize = Ground.GetUsedRect();
    var cellSize = Ground.CellSize;
    Camera.LimitLeft = (int)mapSize.Position.x * (int)cellSize.x;
    Camera.LimitTop = (int)mapSize.Position.y * (int)cellSize.y;
    Camera.LimitRight = (int)mapSize.End.x * (int)cellSize.x;
    Camera.LimitBottom = (int)mapSize.End.y * (int)cellSize.y;
  }

  private void OnGameOver()
  {
    Global.GameOver();
  }
  
  private void OnPlayerWin()
  {
    Global.NextLevel();
  }
}

