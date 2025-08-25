using CookieDungeon.Scripts.States;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class PlayerIdle : State
{
    public override void Enter()
    {
        if (StateMachine is null || StateMachine.Subject is null) return;
        StateMachine.Subject.Velocity = Vector2.Zero;
    }

    public override string? ProcessInput(InputEvent @event)
    {
        if (StateMachine is null) return null;

        if (StateMachine.InputController.IsDashing())
        {
            return "dash";
        }

        if (StateMachine.InputController.IsNormalAttacking())
        {
            return "attack";
        }

        return null;
    }

    public override string? ProcessPhysics(double delta)
    {
        if (StateMachine is null) return null;

        var direction = StateMachine.InputController.GetDirection();

        if (direction != Vector2.Zero)
        {
            return "move";
        }

        return null;
    }
}
