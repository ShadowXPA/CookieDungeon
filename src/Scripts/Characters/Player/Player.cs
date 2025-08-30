using CookieDungeon.Scripts.Movement;
using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player;

public partial class Player : CharacterBody2D
{
	[Export]
	public StateMachine? StateMachine { get; set; }
	[Export]
	public Stats Stats { get; private set; } = new();
	public Skills Skills { get; set; } = new();
	public IInputController InputController { get; private set; } = PlayerInputController.Instance;
	public AnimatedSprite2D? Character { get; private set; }
	public AnimationPlayer? Animations { get; private set; }
	public Marker2D? ProjectileSpawner { get; private set; }
	public Node? ProjectileContainer { get; private set; }
	public Area2D? WeaponHitBox { get; private set; }
	public Node2D? CharacterBoxes { get; private set; }

	private Marker2D? _teleportTarget;

	public override void _Ready()
	{
		Stats = new(ResourceManager.Load<Stats>(ResourceManager.Identifier.PlayerStats));

		Character = GetNode<AnimatedSprite2D>("%Character");
		Animations = GetNode<AnimationPlayer>("%Animations");
		ProjectileSpawner = GetNode<Marker2D>("%ProjectileSpawner");
		ProjectileContainer = GetNode<Node>("%ProjectileContainer");
		WeaponHitBox = GetNode<Area2D>("%HitBox");
		CharacterBoxes = GetNode<Node2D>("%CharacterBoxes");
		var stateLabel = GetNode<Label>("%StateLabel");

		SignalBus.BroadcastLevelUpdated(Stats.Level);
		SignalBus.BroadcastHealthUpdated(Stats.Health, Stats.MaxHealth);
		SignalBus.BroadcastManaUpdated(Stats.Mana, Stats.MaxMana);
		SignalBus.BroadcastDashCooldownUpdated(Skills.Dash.CurrentCooldown, Skills.Dash.Cooldown);

		SignalBus.MonsterKilled += MonsterKilled;

		WeaponHitBox.BodyEntered += DamageEnemy;

		if (StateMachine is not null)
		{
			StateMachine.StateChanged += (_, newState) =>
			{
				stateLabel.Text = newState;
			};

			StateMachine.Initialize(this);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		StateMachine?.ProcessInput(@event);

		if (@event.IsAction("action") && _teleportTarget is not null)
		{
			InputController = EmptyInputController.Instance;
			Animations?.ResetAndPlay("teleport");
		}
	}

	public override void _Process(double delta)
	{
		StateMachine?.ProcessFrame(delta);

		if (Skills.Dash.CurrentCooldown != 0)
		{
			Skills.Dash.CurrentCooldown = Mathf.Max(Skills.Dash.CurrentCooldown - (float)delta, 0);
			SignalBus.BroadcastDashCooldownUpdated(Skills.Dash.CurrentCooldown, Skills.Dash.Cooldown);
		}

		if (Skills.NormalAttack.CurrentCooldown != 0)
		{
			Skills.NormalAttack.CurrentCooldown = Mathf.Max(Skills.NormalAttack.CurrentCooldown - (float)delta, 0);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		StateMachine?.ProcessPhysics(delta);
	}

	public void SetTeleportTarget(Marker2D? target)
	{
		_teleportTarget = target;
	}

	public void TeleportToTarget()
	{
		if (_teleportTarget is null) return;

		GlobalPosition = _teleportTarget.GlobalPosition;
		InputController = PlayerInputController.Instance;
	}

	public void LookAtTarget(Vector2 target)
	{
		var left = (target.X - GlobalPosition.X) < 0;

		if (CharacterBoxes is not null)
		{
			CharacterBoxes.Scale = CharacterBoxes.Scale with { X = left ? -1 : 1 };
		}
	}

	public void ApplyDamage(int dmg)
	{
		var finalDmg = dmg - Stats.Defense;
		Stats.Health -= Mathf.Max(finalDmg, 1);
		SignalBus.BroadcastHealthUpdated(Stats.Health, Stats.MaxHealth);

		StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Hurt");

		if (Stats.Health <= 0)
		{
			StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Death");
		}
	}

	private void MonsterKilled(int experience)
	{
		Stats.AddExperience(experience);
        SignalBus.BroadcastLevelUpdated(Stats.Level);
        SignalBus.BroadcastHealthUpdated(Stats.Health, Stats.MaxHealth);
        SignalBus.BroadcastManaUpdated(Stats.Mana, Stats.MaxMana);
	}

	private void DamageEnemy(Node2D body)
	{
		if (body is Enemy.Enemy enemy)
		{
			var crit = GD.Randf();
			var dmg = crit <= Stats.CriticalRate ? Mathf.FloorToInt(Stats.Attack * Stats.CriticalDamage) : Stats.Attack;
			enemy.ApplyDamage(dmg);
		}
	}
}
