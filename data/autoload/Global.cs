using Godot;
using System.Collections.Generic;

public class Global : Node
{
  [Signal] public delegate void KeysChanged();
  [Signal] public delegate void LifeChanged();
  
  public static Global Instance { get; private set; }

  private const int maxLife = 10;

  public override void _Ready()
  {
    base._Ready();
    Instance = this;
  }

  private static readonly List<string> levels = new List<string>
  {
    "res://levels/Level1.tscn",
    "res://levels/Level2.tscn",
    "res://levels/Level3.tscn",
    "res://levels/Level4.tscn",
    "res://levels/Level5.tscn",
    "res://levels/Level6.tscn",
    "res://levels/Level7.tscn"
  };
  
  private static int keys;
  private static int currentLevel;
  private static int life;

  public static int Keys
  {
    get => keys;
    set
    {
      keys = value;
      Instance.EmitSignal(nameof(KeysChanged));
    }
  }

  public static bool HasKey => Keys > 0;
  
  public static int Life
  {
    get => life;
    set
    {
      life = value;
      if (life > maxLife) life = maxLife;
      if (life < 0) life = 0;
      Instance.EmitSignal(nameof(LifeChanged));
    }
  }

  public static void NewGame()
  {
    keys = 0;
    currentLevel = -1;
    life = maxLife;  // TODO: game stats/config
    
    NextLevel();
  }

  public static void NextLevel()
  {
    currentLevel++;
    Instance.GetTree().ChangeScene(currentLevel >= levels.Count ? "res://data/scenes/Win.tscn" : levels[currentLevel]);
  }

  public static void GameOver()
  {
    Instance.GetTree().ChangeScene("res://data/scenes/GameOver.tscn");
  }
}
