using Godot;

public partial class Mob : RigidBody2D, IDamageable
{
	[Export] public int MaxHealth { get; set; } = 2;

	[Export] public float MinSpeed { get; set; } = 150f;
	[Export] public float MaxSpeed { get; set; } = 260f;

	public Node2D Target { get; set; }

	private int _health;
	private float _speed;
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		GravityScale = 0;
		LockRotation = true;

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		string[] mobTypes = _animatedSprite.SpriteFrames.GetAnimationNames();
		var mob = mobTypes[GD.Randi() % mobTypes.Length];
		_animatedSprite.Play(mob);
		
		switch (mob){
			case "aranha":
				_health = 1;
				_speed = MaxSpeed;
				break;
			case "joaninha":
				_health = 2;
				_speed = MinSpeed;
				break;
		}
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
