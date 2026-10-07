#nullable enable
using System;
using UnityEngine;

namespace Bytefennec.FSM
{

public class State<TStateEnum, TStateEvents> : BaseState<TStateEnum, TStateEvents>
	where TStateEnum : Enum
	where TStateEvents : Enum
{
	private readonly Action<State<TStateEnum, TStateEvents>>? _onEnter;
	private readonly Action<State<TStateEnum, TStateEvents>>? _onExit;
	private readonly Action<State<TStateEnum, TStateEvents>>? _requestExit;
	private readonly Action<State<TStateEnum, TStateEvents>>? _onUpdate;

	public State(Action<State<TStateEnum, TStateEvents>>? onEnter = null,
		Action<State<TStateEnum, TStateEvents>>? onExit = null,
		Action<State<TStateEnum, TStateEvents>>? requestExit = null,
		Action<State<TStateEnum, TStateEvents>>? onUpdate = null,
		StateFlags flags = StateFlags.Interruptable)
		: base(flags)
	{
		_onEnter = onEnter;
		_onExit = onExit;
		_requestExit = requestExit;
		_onUpdate = onUpdate;
	}

	public override void OnEnter()
	{
		_onEnter?.Invoke(this);	
	}

	public override void OnExit()
	{
		_onExit?.Invoke(this);	
	}

	public override void RequestExit()
	{
		_requestExit?.Invoke(this);	
	}

	public override void OnUpdate()
	{
		_onUpdate?.Invoke(this);	
	}
}

}