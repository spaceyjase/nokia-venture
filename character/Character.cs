using Godot;
using Godot.Collections;

public class Character : Area2D
{
  [Export] private int speed = 1;

  public int TileSize { get; set; } = 8;
  protected bool canMove = true;
  protected Facing facing = Facing.Right;

  private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
  private Tween MoveTween => GetNode<Tween>("MoveTween");

  protected readonly Dictionary<Facing, Vector2> moves = new Dictionary<Facing,Vector2>
  {
    { Facing.Right, new Vector2(1, 0) },
    { Facing.Left, new Vector2(-1, 0) },
    { Facing.Up, new Vector2(0, -1) },
    { Facing.Down, new Vector2(0, 1) }
  };

  private Dictionary<Facing, RayCast2D> raycasts;

  public override void _Ready()
  {
    base._Ready();
    
    raycasts = new Dictionary<Facing, RayCast2D>
    {
      {Facing.Right, GetNode<RayCast2D>("RayCastRight")},
      {Facing.Left, GetNode<RayCast2D>("RayCastLeft")},
      {Facing.Up, GetNode<RayCast2D>("RayCastUp")},
      {Facing.Down, GetNode<RayCast2D>("RayCastDown")},
    };
  }

  protected bool Move(Facing direction)
  {
    AnimationPlayer.PlaybackSpeed = speed;
    facing = direction;
    
    if (raycasts[facing].IsColliding()) return false;

    canMove = false;
    MoveTween.InterpolateProperty(this, "position", Position,
      Position + moves[facing] * TileSize, 1f / speed,
      Tween.TransitionType.Sine);
    MoveTween.Start();

    return true;
  }

  private void _on_MoveTween_tween_completed(object o, NodePath key)
  {
    canMove = true;
  }
}