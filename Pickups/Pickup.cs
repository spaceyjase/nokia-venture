using Godot;
using System;
using Godot.Collections;

public class Pickup : Area2D
{
  private readonly Dictionary<string, int> textureIndex = new Dictionary<string, int>
  {
    { "key", 0 },
    { "chest", 1 },
  };

  private Tween Tween => GetNode<Tween>("Tween");
  private Sprite Sprite => GetNode<Sprite>("Sprite");
  private CollisionShape2D CollisionShape2D => GetNode<CollisionShape2D>("CollisionShape2D");

  public string Type { get; set; }

  public override void _Ready()
  {
    base._Ready();

    Tween.InterpolateProperty(Sprite, "scale", new Vector2(1, 1),
      new Vector2(3, 3), 0.5f, Tween.TransitionType.Quad, Tween.EaseType.InOut);
    Tween.InterpolateProperty(Sprite, "modulate",
      new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 0.5f,
      Tween.TransitionType.Quad, Tween.EaseType.InOut);
  }

  public void Init(string type, Vector2 position)
  {
    Sprite.Frame = textureIndex[type];
    Position = position;
    Type = type;
  }

  public void DoPickup()
  {
    CollisionShape2D.Disabled = true;
    Tween.Start();
  }

  public void _on_Tween_tween_completed(object o, NodePath key)
  {
    QueueFree();
  }
}
