using Godot;

public partial class Bullet : Area2D
{
	[Export] public float Speed { get; set; } = 500f;
	[Export] public int Damage { get; set; } = 1;
	[Export] public float MaxDistance { get; set; } = 600f;

	public Vector2 Direction { get; set; } = Vector2.Right;

	private float _traveledDistance;
	private bool _hasHit = false;

	public override void _Ready()
	{
		Rotation = Direction.Angle() + Mathf.DegToRad(90);

		BodyEntered += OnBodyEntered;
		GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D").ScreenExited += OnScreenExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		float step = Speed * (float)delta;
		Position += Direction * step;
		_traveledDistance += step;

		if (_traveledDistance >= MaxDistance)
		{
			QueueFree();
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		// Evita acertar dois corpos no mesmo frame
		if (_hasHit || body is not IDamageable damageable) return;

		_hasHit = true;
		damageable.TakeDamage(Damage);
		QueueFree();
	}

	private void OnScreenExited()
	{
		QueueFree();
	}
}
