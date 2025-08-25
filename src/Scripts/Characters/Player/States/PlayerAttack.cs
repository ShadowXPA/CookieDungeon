using CookieDungeon.Scripts.States;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class PlayerAttack : State
{
    // TODO:
    //  - When animation stops, go back to Idle, while animation hasn't stopped, go to next attack
    //  - Queue attacks

    private float _cooldown;
    private int combo;

    public override void Enter()
    {
        if (StateMachine is null || StateMachine.Subject is null) return;
        StateMachine.Subject.Velocity = Vector2.Zero;
        _cooldown = .5f;
        combo = 0;
        StateMachine.Subject.Modulate = Color.Color8(255, 0, 0);
    }

    public override void Exit()
    {
        StateMachine.Subject.Modulate = Color.Color8(255, 255, 255);
    }

    public override string? ProcessInput(InputEvent @event)
    {
        if (StateMachine is null) return null;

        if (StateMachine.InputController.IsDashing())
        {
            return "dash";
        }

        if (StateMachine.InputController.IsNormalAttacking() && combo < 3)
        {
            combo++;
        }

        return null;
    }

    public override string? ProcessFrame(double delta)
    {
        _cooldown -= (float)delta;

        if (_cooldown <= 0.0f)
        {
            // If there are attacks in the queue, increase combo, return null
            // Else return to Idle
            return "idle";
        }

        return null;
    }
}
