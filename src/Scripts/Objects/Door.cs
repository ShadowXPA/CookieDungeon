using Godot;

namespace CookieDungeon.Scripts.Objects;

public partial class Door : StaticBody2D
{
	private AnimatedSprite2D? _sprite;
	private Marker2D? _marker;
	private Area2D? _interaction;
	private Label? _doorLabel;
	private State _state;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("%Sprite");
		_marker = GetNode<Marker2D>("%Marker");
		_interaction = GetNode<Area2D>("%Interaction");
		_doorLabel = GetNode<Label>("%DoorLabel");

		_interaction.BodyEntered += OnPlayerEnter;
		_interaction.BodyExited += OnPlayerExit;
	}

	public override void _ExitTree()
	{
		if (_interaction is not null)
		{
			_interaction.BodyExited -= OnPlayerEnter;
		}
	}

	public void SetState(State state)
	{
		_state = state;

		if (state == State.Locked)
		{
			_sprite?.Play("locked");
		}
		else
		{
			_sprite?.Play("unlocked");
		}
	}

	private void OnPlayerEnter(Node2D body)
	{
		if (_doorLabel is null) return;

		if (_state == State.Locked)
		{
			_doorLabel.Text = "Locked";
		}
		else
		{
			_doorLabel.Text = "Press F to enter";
			_sprite?.Play("open");
		}
	}

	private void OnPlayerExit(Node2D body)
	{
		if (_doorLabel is null) return;

		_doorLabel.Text = "";

		if (_state != State.Unlocked) return;
		_sprite?.Play("unlocked");
	}

	public enum State
	{
		Locked,
		Unlocked
	}
}
