using CookieDungeon.Scripts.Characters;
using CookieDungeon.Scripts.Characters.Enemy;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Objects;

public partial class Egg : Area2D
{
    [Export]
    public Stats Stats { get; private set; } = new();

    private AnimatedSprite2D? _animations;
    private CollisionShape2D? _hitbox;
    private Vector2? _direction;
    private float _lifetime = .75f;

    public override void _Ready()
    {
        Stats = new (ResourceManager.Load<Stats>(ResourceManager.Identifier.EggStats));
        _animations = GetNode<AnimatedSprite2D>("%Animations");
        _hitbox = GetNode<CollisionShape2D>("%HitBox");
        _animations.AnimationFinished += QueueFree;
        BodyEntered += OnHit;
    }

    public override void _ExitTree()
    {
        if (_animations is not null)
            _animations.AnimationFinished -= QueueFree;
        BodyEntered -= OnHit;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_direction is null) return;

        var direction = (Vector2)_direction;
        GlobalPosition += direction * Stats.Speed * (float)delta;

        _lifetime -= (float)delta;

        if (_lifetime <= 0)
        {
            Break();
        }
    }

    public void MoveTowardTarget(Vector2 target, Vector2 direction)
    {
        _direction = direction;
        this.LookAtPoint(target);
        _animations?.Play("thrown");
    }

    public void OnHit(Node2D body)
    {
        Break();

        if (body is Enemy enemy)
        {
			var crit = GD.Randf();
			var dmg = crit <= Stats.CriticalRate ? Mathf.FloorToInt(Stats.Attack * Stats.CriticalDamage) : Stats.Attack;
			enemy.ApplyDamage(dmg);
        }
    }

    private void Break()
    {
        _direction = null;
        _animations?.Play("broken");
        _hitbox?.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
}
