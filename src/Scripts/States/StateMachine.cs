using System.Collections.Immutable;
using CookieDungeon.Scripts.Movement;
using CookieDungeon.Scripts.Utils;
using Godot;

namespace CookieDungeon.Scripts.States;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export]
    public State? InitialState { get; set; }
    public State? CurrentState { get; private set; }

    public CharacterBody2D? Subject { get; private set; }
    public AnimatedSprite2D? Animations { get; private set; }
    public IInputController InputController { get; private set; } = EmptyInputController.Instance;

    public ImmutableDictionary<string, State> States { get => _states.ToImmutableDictionary(); }
    private readonly Dictionary<string, State> _states = [];

    public void Initialize(CharacterBody2D? subject = null, AnimatedSprite2D? animations = null, IInputController? inputController = null)
    {
        Subject = subject;
        Animations = animations;
        InputController = inputController ?? EmptyInputController.Instance;

        var states = GetChildren();
        foreach (var state in states)
        {
            if (state is State s)
            {
                _states.Add(s.Name.ToString().ToLower(), s);
                s.StateMachine = this;
            }
        }

        if (InitialState is not null)
            ChangeState(InitialState.Name);
    }

    public void ProcessInput(InputEvent @event)
    {
        var newState = CurrentState?.ProcessInput(@event);

        if (newState is not null)
            ChangeState(newState);
    }

    public void ProcessPhysics(double delta)
    {
        var newState = CurrentState?.ProcessPhysics(delta);

        if (newState is not null)
            ChangeState(newState);
    }

    public void ProcessFrame(double delta)
    {
        var newState = CurrentState?.ProcessFrame(delta);

        if (newState is not null)
            ChangeState(newState);
    }

    public void ChangeState(string state)
    {
        state = state.ToLower();
        var currentState = CurrentState?.Name.ToString().ToLower();

        if (currentState == state)
        {
            return;
        }

        var oldState = CurrentState;
        var hasState = _states.TryGetValue(state, out var newState);

        if (!hasState)
        {
            return;
        }

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();

        SignalBus.BroadcastStateChanged(oldState?.Name ?? string.Empty, newState?.Name ?? string.Empty);
    }
}
