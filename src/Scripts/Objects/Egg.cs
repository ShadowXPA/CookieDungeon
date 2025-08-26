using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Objects;

public partial class Egg : Area2D
{
    private AnimatedSprite2D? _animations;
    private Vector2? _direction;
    private float _speed = 500.0f;
    private float _lifetime = 1.0f;

    public override void _Ready()
    {
        _animations = GetNode<AnimatedSprite2D>("%Animations");
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
        GlobalPosition += direction * _speed * (float)delta;

        _lifetime -= (float)delta;

        if (_lifetime <= 0)
        {
            Break();
        }
    }

    public void MoveTowardTarget(Vector2 target, Vector2 direction)
    {
        _direction = direction;
        GD.PrintS("Egg Direction:", _direction);
        this.LookAtPoint(target);
        _animations?.Play("thrown");
    }

    public void OnHit(Node2D body)
    {
        Break();

        if (body is CharacterBody2D characterBody2D)
        {
            // TODO: do dmg
        }
    }

    private void Break()
    {
        _direction = null;
        _animations?.Play("broken");
    }
}
