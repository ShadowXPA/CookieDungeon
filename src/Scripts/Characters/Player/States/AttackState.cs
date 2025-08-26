using CookieDungeon.Scripts.Objects;
using CookieDungeon.Scripts.Skills;
using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class AttackState : State
{
    // TODO:
    //  - When animation stops, go back to Idle, while animation hasn't stopped, go to next attack
    //  - Queue attacks

    private bool _animating;
    private Vector2? _target;
    private Skill? _skill;
    private PackedScene _eggScene = ResourceManager.Load<PackedScene>(ResourceManager.Identifier.Egg);

    public override void Enter()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Velocity = Vector2.Zero;

        _target = subject.InputController.GetTargetPosition(subject);

        if (_target is null) return;
        var target = (Vector2)_target;
        subject.LookAtPoint(target);

        subject.Animations?.Play(subject.Skills.NormalAttack.Animation);
        subject.Modulate = Color.Color8(255, 0, 0);
    }

    public override void Exit()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Animations?.Stop();
        subject.Modulate = Color.Color8(255, 255, 255);
    }

    public override string? ProcessInput(InputEvent @event)
    {
        var subject = GetSubject<Player>();
        if (subject is null) return null;

        if (subject.InputController.IsDashing())
        {
            return "dash";
        }

        return null;
    }

    public override string? ProcessFrame(double delta)
    {
        var subject = GetSubject<Player>();
        if (subject is null) return null;

        if (subject.Animations is null || !subject.Animations.IsPlaying())
        {
            return "idle";
        }

        return null;
    }

    public void SpawnEgg()
    {
        var subject = GetSubject<Player>();
        if (subject is null || subject.ProjectileContainer is null || subject.ProjectileSpawner is null || _target is null) return;

        var egg = _eggScene.Instantiate<Egg>();
        subject.ProjectileContainer.AddChild(egg);
        egg.GlobalPosition = subject.ProjectileSpawner.GlobalPosition;
        egg.MoveTowardTarget((Vector2)_target, ((Vector2)_target - subject.GlobalPosition).Normalized());
    }
}
