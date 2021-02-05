using Godot;
using Godot.Collections;

public class Character : Area2D
{
  [field: Export] protected internal int Damage { get; } = 1;
  [Export] protected int health = 1;
  [Export] private int speed = 1;

  public int TileSize { get; set; } = 8;
  protected bool canMove;
  protected Facing facing = Facing.Right;

  protected Map Map => GetParent<Map>();
  protected AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
  private Tween MoveTween => GetNode<Tween>("MoveTween");
  private Tween DeathTween => GetNode<Tween>("DeathTween");
  private Sprite Sprite => GetNode<Sprite>("Sprite");
  protected CollisionShape2D CollisionShape2D => GetNode<CollisionShape2D>("CollisionShape2D");

  protected readonly Dictionary<Facing, Vector2> moves = new Dictionary<Facing,Vector2>
  {
    { Facing.Right, new Vector2(1, 0) },
    { Facing.Left, new Vector2(-1, 0) },
    { Facing.Up, new Vector2(0, -1) },
    { Facing.Down, new Vector2(0, 1) }
  };

  protected Dictionary<Facing, RayCast2D> raycasts;

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
    
    DeathTween.InterpolateProperty(Sprite, "scale", new Vector2(1, 1),
      new Vector2(3, 3), 0.5f, Tween.TransitionType.Quad);
    DeathTween.InterpolateProperty(Sprite, "modulate",
      new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 0.5f,
      Tween.TransitionType.Quad);
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
  
  protected internal void TakeDamage(int damage)
  {
    health -= damage;
    if (health >= 0) return;
    CollisionShape2D.Disabled = true;
    DeathTween.Start();
  }

  private void _on_MoveTween_tween_completed(object o, NodePath key)
  {
    canMove = true;
  }
  
  private void _on_DeathTween_tween_completed(object o, NodePath key)
  {
    Die();
  }

  protected virtual void Die()
  {
    QueueFree();
  }
}