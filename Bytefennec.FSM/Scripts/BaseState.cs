#nullable enable
using System;
using System.Collections.Generic;

namespace Bytefennec.FSM
{

/// <summary>
/// Represents a state inside the finite state machine.
/// </summary>
public class BaseState<TStateEnum, TStateEvents>
	where TStateEnum : Enum
	where TStateEvents : Enum
{
	public readonly StateFlags Flags;
	/// <summary>
	/// If <c>true</c> this state can exit at any point, if <c>false</c> this state must tell when it can be exited.
	/// <c>FSM.StateCanExit()</c> must be called in <c>OnUpdate</c> or <c>OnExitRequested</c>.
	/// </summary>
	public bool Interruptable => (Flags & StateFlags.Interruptable) > 0;
	/// <summary>
	/// If <c>true<c> this state is instantly exited, processing the next state in same frame.
	/// </summary>
	public bool Instant => (Flags & StateFlags.Instant) > 0;
	/// <summary>
	/// Reference to the FSM, without tangling generics if FSM is nested.
	/// </summary>
	public IFSM? FSM = null;

	public List<BaseTransition<TStateEnum>>? Transitions {get; private set;} = null;
	public Dictionary<TStateEvents, List<BaseTransition<TStateEnum>>>? EventTransitions {get; private set;} = null;

	public BaseState(StateFlags flags)
	{
		Flags = flags;
	}

	internal void AddTransition(BaseTransition<TStateEnum> transition)
	{
		Transitions ??= new();
		Transitions.Add(transition);
	}

	internal void AddTransition(TStateEvents @event, BaseTransition<TStateEnum> transition)
	{
		EventTransitions ??= new();
		List<BaseTransition<TStateEnum>> transitions;
		if(!EventTransitions.TryGetValue(@event, out transitions))
		{
			transitions = new();
			EventTransitions.Add(@event, transitions);
		}

		transitions.Add(transition);
	}

	/// <summary>
	/// Called when the state machine transitions to this state.
	/// </summary>
	public virtual void OnEnter()
	{
	}

	/// <summary>
	/// Called when the state machine transitions from this state.
	/// </summary>
	public virtual void OnExit()
	{
	}

	/// <summary>
	/// Called when the state machine asks if this state can exit.
	/// Must call <c>FSM.ExitState();</c> in <c>RequestExit()</c> or <c>OnUpdate()</c>
	/// </summary>
	public virtual void RequestExit()
	{
	}

	/// <summary>
	/// Called when this state is active.
	/// </summary>
	public virtual void OnUpdate()
	{	
	}

	public override string ToString()
	{
		return string.Empty;
	}
}

}