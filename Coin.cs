using Godot;

public partial class Coin : Area2D
{
	[Signal]
	public delegate void CollectedEventHandler(int points);

	[Export] public float FlySpeed { get; set; } = 400f;
	[Export] public float MagnetRadius { get; set; } = 80f;

	private Player _targetPlayer;
	private bool _isFlying = false;
	
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite2D");
		string[] coinTypes = _animatedSprite.SpriteFrames.GetAnimationNames();
		var coin = coinTypes[GD.Randi() % coinTypes.Length];
		_animatedSprite.Play(coin);
		_targetPlayer = GetTree().Root.FindChild("Player", true, false) as Player;
		AreaEntered += OnCoinAreaEntered;
	}
	
	public override void _Process(double delta)
	{
		if (!IsInstanceValid(_targetPlayer) || !_targetPlayer.Visible) return;

		float distance = Position.DistanceTo(_targetPlayer.Position);

		if (!_isFlying && distance <= MagnetRadius)
		{
			_isFlying = true;
		}

		if (_isFlying)
		{
			Position = Position.MoveToward(_targetPlayer.Position, FlySpeed * (float)delta);
		}
	}
	
	private void OnCoinAreaEntered(Area2D area)
	{
		if (area is Player)
		{
			if (_animatedSprite.Animation == "mascavo"){
				EmitSignal(SignalName.Collected, 5);
			} else {
				EmitSignal(SignalName.Collected, 1);
			}
			QueueFree();
		}
	}
}
