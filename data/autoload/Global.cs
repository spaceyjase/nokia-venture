using Godot;
using System;

public class Global : Node
{
  [Signal]
  public delegate void KeysChanged();
  
  public static Global Instance { get; private set; }

  public override void _Ready()
  {
    base._Ready();
    Instance = this;
  }

  private static int keys;
  public static int Keys
  {
    get => keys;
    set
    {
      keys = value;
      Instance.EmitSignal(nameof(KeysChanged));
    }
  }
}
