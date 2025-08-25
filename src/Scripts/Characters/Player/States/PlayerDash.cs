using CookieDungeon.Scripts.States;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class PlayerDash : State
{
    private float _dashTime;
    private Vector2? _target;

    public override void Enter()
    {
        if (StateMachine is null) return;
        _dashTime = .3f;
        _target = StateMachine.InputController.GetTargetPosition(StateMachine.Subject);
        StateMachine.Subject.Velocity = Vector2.Zero;
        StateMachine.Subject.Modulate = Color.Color8(255, 255, 0);
    }

    public override void Exit()
    {
        StateMachine.Subject.Modulate = Color.Color8(255, 255, 255, 255);
    }

    public override string? ProcessPhysics(double delta)
    {
        if (StateMachine is null || StateMachine.Subject is null) return null;

        if (_target is null) return "idle";

        _dashTime -= (float)delta;

        var target = ((Vector2)_target).Normalized();

        StateMachine.Subject.Velocity = target * 500.0f * 1.5f;
        StateMachine.Subject.MoveAndSlide();

        if (_dashTime <= 0)
        {
            return "idle";
        }

        return null;
    }
}
