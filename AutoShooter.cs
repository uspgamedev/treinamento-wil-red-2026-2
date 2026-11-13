using Godot;

public partial class AutoShooter : Node2D
{
#pragma warning disable 649
	[Export]
	public PackedScene BulletScene { get; set; }
#pragma warning restore 649

	[Export] public float Range { get; set; } = 300f;
	[Export] public float FireCooldown { get; set; } = 0.5f;
	[Export] public string TargetGroup { get; set; } = "mobs";

	private double _cooldownLeft;

	public override void _Ready()
	{
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		_cooldownLeft -= delta;
		if (_cooldownLeft > 0) return;

		Node2D target = FindNearestTarget();
		if (target is null) return;

		Shoot(target);
		_cooldownLeft = FireCooldown;
	}

	public void Start()
	{
		_cooldownLeft = 0;
		SetProcess(true);
	}

	public void Stop()
	{
		SetProcess(false);
	}

	private Node2D FindNearestTarget()
	{
		Node2D nearest = null;
		float nearestDistanceSq = Range * Range;

		foreach (Node node in GetTree().GetNodesInGroup(TargetGroup))
		{
			if (node is not Node2D candidate || candidate.IsQueuedForDeletion()) continue;

			float distanceSq = GlobalPosition.DistanceSquaredTo(candidate.GlobalPosition);
			if (distanceSq <= nearestDistanceSq)
			{
				nearestDistanceSq = distanceSq;
				nearest = candidate;
			}
		}

		return nearest;
	}

	private void Shoot(Node2D target)
	{
		if (BulletScene is null)
		{
			GD.PrintErr("Bullet scene is not set. You need to set it in the inspector.");
			return;
		}

		Bullet bullet = BulletScene.Instantiate<Bullet>();
		bullet.AddToGroup("bullets");
		bullet.Direction = GlobalPosition.DirectionTo(target.GlobalPosition);

		// Fica fora do Player para não seguir o movimento dele
		GetTree().CurrentScene.AddChild(bullet);
		bullet.GlobalPosition = GlobalPosition;
	}
}
