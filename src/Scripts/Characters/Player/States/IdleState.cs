using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class IdleState : State
{
    public override void Enter()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Velocity = Vector2.Zero;
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

        if (direction != Vector2.Zero)
        {
            return "move";
        }

        return null;
    }
}
