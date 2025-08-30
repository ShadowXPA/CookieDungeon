using CookieDungeon.Scripts.Components;
using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Enemy;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public StateMachine? StateMachine { get; private set; }
	[Export]
	public required Stats Stats { get; set; }
	public Area2D? AttackRange { get; private set; }
	public Area2D? AggroRange { get; private set; }
	public Player.Player? Target { get; private set; }
	public AnimatedSprite2D? Character { get; private set; }
	public AnimationPlayer? Animations { get; private set; }
	public Area2D? WeaponHitBox { get; private set; }
	public Node2D? CharacterBoxes { get; private set; }
	public bool IsDead { get; private set; }
	private ProgressBar? _healthBar;
	private Label? _level;
	private float _lastX;
	private bool _connected;

	public override void _Ready()
	{
		AttackRange = GetNode<Area2D>("%AttackRange");
		AggroRange = GetNode<Area2D>("%AggroRange");
		Character = GetNode<AnimatedSprite2D>("%Character");
		Animations = GetNode<AnimationPlayer>("%Animations");
		WeaponHitBox = GetNode<Area2D>("%HitBox");
		CharacterBoxes = GetNode<Node2D>("%CharacterBoxes");
		_healthBar = GetNode<ProgressBar>("%HPBar");
		_level = GetNode<Label>("%Level");

		if (Stats is not null)
			SetStats(Stats);
		// SetStats(new(ResourceManager.Load<Stats>(ResourceManager.Identifier.EnemyStats)));
		ConnectEvents();

		StateMachine?.Initialize(this);
	}

	public override void _ExitTree()
	{
		DisconnectEvents();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		StateMachine?.ProcessInput(@event);
	}

	public override void _Process(double delta)
	{
		StateMachine?.ProcessFrame(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		StateMachine?.ProcessPhysics(delta);

		var diff = GlobalPosition.X - _lastX;
		var left = diff < 0;

		if (CharacterBoxes is not null && diff != 0)
		{
			CharacterBoxes.Scale = CharacterBoxes.Scale with { X = left ? -1 : 1 };
		}

		_lastX = Position.X;
	}

	public void SetStats(Stats stats)
	{
		Stats = stats;

		if (_healthBar is not null)
		{
			_healthBar.MaxValue = Stats.MaxHealth;
			_healthBar.Value = Stats.Health;
		}

		if (_level is not null)
		{
			_level.Text = $"Lv. {Stats.Level}";
		}
	}

	public void ApplyDamage(int dmg)
	{
		var finalDmg = dmg - Stats.Defense;
		Stats.Health -= Mathf.Max(finalDmg, 1);

		StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Hurt");

		if (_healthBar is not null)
		{
			_healthBar.Value = Stats.Health;
		}

		if (Stats.Health <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		DisconnectEvents();
		StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Death");
		IsDead = true;
		SignalBus.BroadcastMonsterKilled(Stats.Experience);
	}

	private void AttackPlayer(Node2D body)
	{
		StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Attack");
	}

	private void StopAttacking(Node2D body)
	{
		var isInRange = Target is not null && (AttackRange?.OverlapsBody(Target) ?? false);

		if (isInRange)
		{
			StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Attack");
		}
		else if (Target is not null)
		{
			StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Chase");
		}
		else
		{
			StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Idle");
		}
	}

	private void ChasePlayer(Node2D body)
	{
		if (body is Player.Player player)
		{
			Target = player;
			StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Chase");
		}
	}

	private void LostAggro(Node2D body)
	{
		Target = null;
		StateMachine?.CallDeferred(StateMachine.MethodName.ChangeState, "Idle");
	}

	private void DamagePlayer(Node2D body)
	{
		if (body is HurtBox hurtBox)
		{
			var enemy = hurtBox.Subject as Player.Player;
			var crit = GD.Randf();
			var dmg = crit <= Stats.CriticalRate ? Mathf.FloorToInt(Stats.Attack * Stats.CriticalDamage) : Stats.Attack;
			enemy?.ApplyDamage(dmg);
		}
	}

	private void ConnectEvents()
	{
		if (_connected) return;
		_connected = true;

		if (WeaponHitBox is not null)
		{
			WeaponHitBox.AreaEntered += DamagePlayer;
		}

		if (AggroRange is not null)
		{
			AggroRange.BodyEntered += ChasePlayer;
			AggroRange.BodyExited += LostAggro;
		}

		if (AttackRange is not null)
		{
			AttackRange.BodyEntered += AttackPlayer;
			AttackRange.BodyExited += StopAttacking;
		}
	}

	private void DisconnectEvents()
	{
		if (!_connected) return;
		_connected = false;

		if (WeaponHitBox is not null)
		{
			WeaponHitBox.AreaEntered -= DamagePlayer;
		}

		if (AggroRange is not null)
		{
			AggroRange.BodyEntered -= ChasePlayer;
			AggroRange.BodyExited -= LostAggro;
		}

		if (AttackRange is not null)
		{
			AttackRange.BodyEntered -= AttackPlayer;
			AttackRange.BodyExited -= StopAttacking;
		}
	}
}
