using Godot;

public partial class Coin : Area2D
{
    [Signal]
    public delegate void CollectedEventHandler();

    [Export] public float FlySpeed { get; set; } = 500f;
    [Export] public float MagnetRadius { get; set; } = 140f;

    private Player _targetPlayer;
    private bool _isFlying = false;

    public override void _Ready()
    {
        _targetPlayer = GetTree().Root.FindChild("Player", true, false) as Player;
        AreaEntered += OnCoinAreaEntered;
    }

    public override void _Process(double delta)
    {
        // Se o player for nulo ou se ele morreu não faz nada
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
            EmitSignal(SignalName.Collected);
            QueueFree();
        }
    }
}