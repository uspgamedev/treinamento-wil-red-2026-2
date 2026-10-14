using Godot;

public partial class Main : Node
{
#pragma warning disable 649
    [Export]
    public PackedScene MobScene { get; set; }

    [Export]
    public PackedScene CoinScene { get; set; } 
#pragma warning restore 649

    private int _score;

    public void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("CoinTimer").Stop(); //  spawn dos coletáveis

        GetNode<HUD>("HUD").ShowGameOver(); 

        GetNode<AudioStreamPlayer>("Music").Stop(); 
        GetNode<AudioStreamPlayer>("DeathSound").Play(); 
    }

    public void NewGame()
    {
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree); 
        GetTree().CallGroup("coins", Node.MethodName.QueueFree); 

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
        GetNode<Timer>("CoinTimer").Start(); // Inicia o spawn dos coletáveis
        
    }

    private void OnCoinSpawnTimerTimeout()
    {
        if (CoinScene is null) return;

        Coin coin = CoinScene.Instantiate<Coin>();
        coin.AddToGroup("coins");

        // Conecta o evento de coleta para somar os pontos
        coin.Collected += OnCoinCollected;

        // Gera uma posição aleatória respeitando o tamanho da tela e margens
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        float margin = 40f;
        coin.Position = new Vector2(
            (float)GD.RandRange(margin, viewportSize.X - margin),
            (float)GD.RandRange(margin, viewportSize.Y - margin)
        );

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

        Mob mob = MobScene.Instantiate<Mob>(); 

        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation"); 
        mobSpawnLocation.ProgressRatio = GD.Randf(); 

        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2; 
        mob.Position = mobSpawnLocation.Position; 

        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4); 
        mob.Rotation = direction; 

        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0); 
        mob.LinearVelocity = velocity.Rotated(direction); 

        AddChild(mob); 
    }
}
