using CookieDungeon.Scripts.Movement;
using CookieDungeon.Scripts.States;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.Characters.Player;

public partial class Player : CharacterBody2D
{
	// [Export]
	// public float Speed { get; set; } = 500.0f;
	// [Export]
	// public float Acceleration { get; set; } = 5000.0f;
	// [Export]
	// public float Deceleration { get; set; } = 7500.0f;

	[Export]
	public StateMachine? StateMachine { get; set; }
	[Export]
	public Stats Stats { get; set; } = new();

	public override void _Ready()
	{
		StateMachine?.Initialize(this, inputController: PlayerInputController.Instance);

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

		// var direction = _inputController.GetDirection();

		// Velocity = Velocity.MoveToward(direction * Speed,
		// 	(direction == Vector2.Zero ? Deceleration : Acceleration) * (float)delta);
		// MoveAndSlide();
	}
}
