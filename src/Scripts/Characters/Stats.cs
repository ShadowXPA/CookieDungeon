using Godot;

namespace CookieDungeon.Scripts.Characters;

[GlobalClass]
public partial class Stats : Resource
{
    [Export]
    public int Level { get; set; } = 1;
    [Export]
    public int Health { get; set; }
    [Export]
    public int Mana { get; set; }
    [Export]
    public int Attack { get; set; }
    [Export]
    public int Defense { get; set; }
    [Export]
    public float Speed { get; set; }
    [Export]
    public float CriticalRate { get; set; } = .5f;
    [Export]
    public float CriticalDamage { get; set; } = 1.5f;
    public float Acceleration => Speed * 10.0f;
    public float Deceleration => Speed * 15.0f;
    public float Dash => Speed * 5.0f;
}
