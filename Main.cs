using Godot;

public partial class Main : Node
{
#pragma warning disable 649
    [Export]
    public PackedScene MobScene { get; set; }

    [Export]
    public PackedScene CoinScene { get; set; }
#pragma warning restore 649

    [Export]
    public float WallThickness { get; set; } = 52f; // Borda verde, em pixels da imagem original

    [Export]
    public float MinSpawnDistance { get; set; } = 300f;

    private int _score;
    private Rect2 _playArea;

    public override void _Ready()
    {
        var map = GetNode<Sprite2D>("Map");
        Rect2 mapRect = map.GetGlobalTransform() * map.GetRect();
        _playArea = mapRect.Grow(-WallThickness * map.Scale.X);

        GetNode<Player>("Player").SetMapBounds(mapRect, _playArea);
    }

    public void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("CoinTimer").Stop();

        GetNode<HUD>("HUD").ShowGameOver();

        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    public void NewGame()
    {
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
        GetTree().CallGroup("coins", Node.MethodName.QueueFree);
        GetTree().CallGroup("bullets", Node.MethodName.QueueFree);

        _score = 0;

        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<HUD>("HUD");
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");

        GetNode<AudioStreamPlayer>("Music").Play();
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("CoinTimer").Start();
    }

    private void OnCoinSpawnTimerTimeout()
    {
        if (CoinScene is null) return;

        Coin coin = CoinScene.Instantiate<Coin>();
        coin.AddToGroup("coins");

        coin.Collected += OnCoinCollected;

        coin.Position = GetRandomPointInPlayArea(40f);

        AddChild(coin);
    }

    private void OnCoinCollected()
    {
        _score++;
        GetNode<HUD>("HUD").UpdateScore(_score);
    }

    private void OnMobTimerTimeout()
    {
        if (MobScene is null)
        {
            GD.PrintErr("Mob scene is not set. You need to set it in the inspector.");
            return;
        }

        var player = GetNode<Player>("Player");

        Vector2 spawnPosition = GetRandomPointInPlayArea(40f);
        for (int attempt = 0; attempt < 10 && spawnPosition.DistanceTo(player.Position) < MinSpawnDistance; attempt++)
        {
            spawnPosition = GetRandomPointInPlayArea(40f);
        }

        if (spawnPosition.DistanceTo(player.Position) < MinSpawnDistance) return;

        Mob mob = MobScene.Instantiate<Mob>();
        mob.Position = spawnPosition;
        mob.Target = player;

        AddChild(mob);
    }

    private Vector2 GetRandomPointInPlayArea(float margin)
    {
        Rect2 area = _playArea.Grow(-margin);
        return new Vector2(
            (float)GD.RandRange(area.Position.X, area.End.X),
            (float)GD.RandRange(area.Position.Y, area.End.Y)
        );
    }
}
