using CookieDungeon.Scripts.Movement;
using Godot;

namespace CookieDungeon.Scripts.States;

[GlobalClass]
public partial class State : Node
{
    public StateMachine? StateMachine { get; set; }

    public virtual string? ProcessInput(InputEvent @event) => null;
    public virtual string? ProcessPhysics(double delta) => null;
    public virtual string? ProcessFrame(double delta) => null;
    public virtual void Enter() { }
    public virtual void Exit() { }
}
