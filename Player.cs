using Godot;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed { get; set; } = 400;

	public Rect2 PlayArea { get; private set; }

	public override void _Ready()
	{
		Hide();
	}

	public void SetMapBounds(Rect2 mapRect, Rect2 playArea)
	{
		PlayArea = playArea;

		var camera = GetNode<Camera2D>("Camera2D");
		camera.LimitLeft = (int)mapRect.Position.X;
		camera.LimitTop = (int)mapRect.Position.Y;
		camera.LimitRight = (int)mapRect.End.X;
		camera.LimitBottom = (int)mapRect.End.Y;
	}

	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite.Play();
		}
		else
		{
			animatedSprite.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, PlayArea.Position.X, PlayArea.End.X),
			y: Mathf.Clamp(Position.Y, PlayArea.Position.Y, PlayArea.End.Y)
		);
		
		/*
		if (velocity.X != 0)
		{
			animatedSprite.Animation = "left";
			animatedSprite.FlipV = false;
			animatedSprite.FlipH = velocity.X > 0;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite.Animation = "up";
			animatedSprite.FlipV = velocity.Y > 0;
		}
		*/
		
		if (velocity.Y != 0 || velocity.X != 0) {
			animatedSprite.Rotation = Mathf.Atan2(velocity.Y, velocity.X);
			animatedSprite.FlipH = true;
		}
	}

	public void Start(Vector2 position)
	{
		Position = position;
		GetNode<Camera2D>("Camera2D").ResetSmoothing();
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		SetProcess(true);
		GetNode<AutoShooter>("AutoShooter").Start();
	}

	public void OnBodyEntered(PhysicsBody2D body)
	{
		Hide();
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		SetProcess(false);
		GetNode<AutoShooter>("AutoShooter").Stop();
	}
}
