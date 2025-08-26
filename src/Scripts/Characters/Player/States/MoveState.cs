using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class MoveState : State
{
    public override void Enter()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Character?.Play("move");
    }

    public override void Exit()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Character?.Play("default");
    }

    public override string? ProcessInput(InputEvent @event)
    {
        var subject = GetSubject<Player>();
        if (subject is null) return null;

        if (@event is InputEventMouseMotion mouseMotion)
        {
            var target = subject.InputController.GetTargetPosition(subject);
            subject.LookAtPoint(target);
        }

        if (subject.InputController.IsDashing())
        {
            return "dash";
        }

        if (subject.InputController.IsNormalAttacking())
        {
            return "attack";
        }

        return null;
    }

    public override string? ProcessPhysics(double delta)
    {
        var subject = GetSubject<Player>();
        if (subject is null) return null;

        var direction = subject.InputController.GetDirection();

        subject.Velocity = subject.Velocity.MoveToward(direction * subject.Stats.Speed,
            (direction == Vector2.Zero ? subject.Stats.Deceleration : subject.Stats.Acceleration) * (float)delta);
        subject.MoveAndSlide();

        if (subject.Velocity == Vector2.Zero)
        {
            return "idle";
        }

        return null;
    }
}
