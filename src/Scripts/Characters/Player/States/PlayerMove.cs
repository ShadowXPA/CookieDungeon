using CookieDungeon.Scripts.States;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class PlayerMove : State
{
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
        if (StateMachine is null || StateMachine.Subject is null) return null;

        var direction = StateMachine.InputController.GetDirection();

        StateMachine.Subject.Velocity = StateMachine.Subject.Velocity.MoveToward(direction * 500.0f,
            (direction == Vector2.Zero ? 7500.0f : 5000.0f) * (float)delta);
        StateMachine.Subject.MoveAndSlide();

        if (StateMachine.Subject.Velocity == Vector2.Zero)
        {
            return "idle";
        }

        return null;
    }
}
