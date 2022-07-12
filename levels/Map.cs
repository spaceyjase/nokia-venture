using Godot;
using System.Collections.Generic;

public class Map : Node2D
{
  private TileMap Walls => GetNode<TileMap>("Walls");
  private TileMap Items => GetNode<TileMap>("Items");
  
  private Player Player => GetNode<Player>("Player");
  private HUD HUD => GetNode<HUD>("HUD");

  private readonly List<object> gates = new List<object>();

  public Vector2 PlayerPosition { get; private set; } = Vector2.Zero;

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
    Player.Connect(nameof(Player.Moved), this, nameof(OnPlayerMoved));
    Global.Instance.Connect(nameof(Global.LifeChanged), HUD, nameof(HUD.UpdateHealth));
    Global.Instance.Connect(nameof(Global.KeysChanged), HUD, nameof(HUD.UpdateKeys));
    
    HUD.UpdateKeys();
    HUD.UpdateHealth();
  }

  private void SpawnItems()
  {
    // TODO: GameManager
    var lookup = new Dictionary<string, string>
    {
      { nameof(Player), nameof(Player) },
      { nameof(Slime), "Enemy" },
      { nameof(Bat), "Enemy" },
      { nameof(Zombie), "Enemy" },
      { nameof(Wolf), "Enemy" },
      { nameof(Knight), "Enemy" },
      { nameof(Dragon), "Enemy" },
      { "stairs_two", nameof(Pickup) },
      { "key", nameof(Pickup) },
      { "chest", nameof(Pickup) },
      { "flask", nameof(Pickup) },
      { "potion", nameof(Pickup) },
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
          if (pickup?.Instance() is Pickup p)
          {
            p.Init(cellType, position);
            AddChild(p);
          }
          break;
        case "Enemy": // TODO: game constants
          // TODO: global/singleton factory method
          var enemy = ResourceLoader.Load<PackedScene>($"res://Enemies/{cellType}.tscn");
          if (enemy?.Instance() is Character c)
          {
            c.Position = position;
            c.TileSize = (int)Items.CellSize.x;
            Player.Connect(nameof(Player.Moved), c, nameof(OnPlayerMoved));
            AddChild(c);
          }
          break;
      }
    }
  }

  private void OnGameOver()
  {
    Global.GameOver();
  }
  
  private void OnPlayerWin()
  {
    Global.NextLevel();
  }

  private void OnPlayerMoved()
  {
    PlayerPosition = Player.Position;
  }
}

