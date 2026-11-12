using Godot;

public partial class Mob : RigidBody2D, IDamageable
{
	[Export] public int MaxHealth { get; set; } = 1;

	[Export] public float MinSpeed { get; set; } = 180f;
	[Export] public float MaxSpeed { get; set; } = 260f;

	public Node2D Target { get; set; }

	private int _health;
	private float _speed;
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		_health = MaxHealth;
		_speed = (float)GD.RandRange(MinSpeed, MaxSpeed);

		GravityScale = 0;
		LockRotation = true;

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		string[] mobTypes = _animatedSprite.SpriteFrames.GetAnimationNames();
		_animatedSprite.Play(mobTypes[GD.Randi() % mobTypes.Length]);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(Target) || !Target.Visible)
		{
			LinearVelocity = Vector2.Zero;
			return;
		}

		Vector2 direction = GlobalPosition.DirectionTo(Target.GlobalPosition);
		LinearVelocity = direction * _speed;
		_animatedSprite.Rotation = direction.Angle();
	}

	public void TakeDamage(int amount)
	{
		if (_health <= 0) return; // Já morreu neste frame

		_health -= amount;

		if (_health <= 0)
		{
			QueueFree();
		}
	}
}
