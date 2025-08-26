using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class DashState : State
{
    private float _dashDistance = 250.0f;
    private float _dashDuration;
    private Vector2? _direction;

    public override void Enter()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Velocity = Vector2.Zero;

        _dashDuration = _dashDistance / subject.Stats.Dash;
        var target = subject.InputController.GetTargetPosition(subject);
        _direction = (target - subject.GlobalPosition).Normalized();
        subject.LookAtPoint(target);

        subject.Modulate = Color.Color8(255, 255, 0);
    }

    public override void Exit()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Modulate = Color.Color8(255, 255, 255, 255);
    }

    public override string? ProcessPhysics(double delta)
    {
        var subject = GetSubject<Player>();
        if (subject is null) return null;

        if (_direction is null) return "idle";

        var direction = (Vector2)_direction;

        subject.Velocity = direction * subject.Stats.Dash;
        subject.MoveAndSlide();

        _dashDuration -= (float)delta;

        if (_dashDuration <= 0)
        {
            return "idle";
        }

        return null;
    }
}
