using Godot;

namespace CookieDungeon.Scripts.Skills;

[GlobalClass]
public partial class Skill : Resource
{
    [Export]
    public string Animation { get; set; } = string.Empty;
    [Export]
    public float Cooldown { get; set; }
    public float CurrentCooldown { get; set; }
    [Export]
    public int Cost { get; set; }
}
