using CookieDungeon.Scripts.Objects;
using CookieDungeon.Scripts.Skills;
using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player.States;

public partial class AttackState : State
{
    private bool _animating;
    private Vector2? _target;
    private Skill? _skill;
    private PackedScene _eggScene = ResourceManager.Load<PackedScene>(ResourceManager.Identifier.Egg);

    public override void Enter()
    {
        var subject = GetSubject<Player>();
        if (subject is null) return;

        subject.Velocity = Vector2.Zero;
        subject.Stats.Mana -= subject.Skills.NormalAttack.Cost;
        subject.Skills.NormalAttack.CurrentCooldown = subject.Skills.NormalAttack.Cooldown;
        SignalBus.BroadcastManaUpdated(subject.Stats.Mana, subject.Stats.MaxMana);

        _target = subject.InputController.GetTargetPosition(subject);

        if (_target is null) return;
        var target = (Vector2)_target;
        subject.LookAtTarget(target);

        subject.Animations?.ResetAndPlay(subject.Skills.NormalAttack.Animation);
    }

    public override string? ProcessInput(InputEvent @event)
    {
        var subject = GetSubject<Player>();
        if (subject is null) return null;

        if (subject.InputController.IsDashing() && subject.Skills.Dash.CanCast(subject.Stats.Mana))
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
        egg.MoveTowardTarget((Vector2)_target, ((Vector2)_target - subject.ProjectileSpawner.GlobalPosition).Normalized());
    }
}
