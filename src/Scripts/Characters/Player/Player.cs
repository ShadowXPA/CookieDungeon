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
	public Stats Stats { get; set; } = ResourceManager.Load<Stats>(ResourceManager.Identifier.PlayerStats);
	public Skills Skills { get; set; } = new();
    public IInputController InputController { get; private set; } = PlayerInputController.Instance;
	public AnimatedSprite2D? Character { get; private set; }
	public AnimationPlayer? Animations { get; private set; }
	public Marker2D? ProjectileSpawner { get; private set; }
	public Node? ProjectileContainer { get; private set; }

	public override void _Ready()
	{
		StateMachine?.Initialize(this);

		Character = GetNode<AnimatedSprite2D>("%Character");
		Animations = GetNode<AnimationPlayer>("%Animations");
		ProjectileSpawner = GetNode<Marker2D>("%ProjectileSpawner");
		ProjectileContainer = GetNode<Node>("%ProjectileContainer");
		var stateLabel = GetNode<Label>("%StateLabel");

		SignalBus.StateChanged += (_, newState) =>
		{
			stateLabel.Text = newState;
		};
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
	}
}
