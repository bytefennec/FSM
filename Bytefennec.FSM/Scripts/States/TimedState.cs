#nullable enable
using System;
using UnityEngine;

namespace Bytefennec.FSM
{

public class TimedState<TStateEnum, TStateEvents> : BaseState<TStateEnum, TStateEvents>
	where TStateEnum : Enum
	where TStateEvents : Enum
{
	private float _enterTime;
	private readonly float _delay;
	private readonly Action<TimedState<TStateEnum, TStateEvents>>? _onEnter;
	private readonly Action<TimedState<TStateEnum, TStateEvents>>? _onExit;
	private readonly Action<TimedState<TStateEnum, TStateEvents>>? _requestExit;
	private readonly Action<TimedState<TStateEnum, TStateEvents>>? _onUpdate;

	public TimedState(float delay,
		Action<TimedState<TStateEnum, TStateEvents>>? onEnter = null,
		Action<TimedState<TStateEnum, TStateEvents>>? onExit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? requestExit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? onUpdate = null,
		StateFlags flags = StateFlags.None)
		: base(flags)
	{
		_delay = delay;
		_onEnter = onEnter;
		_onExit = onExit;
		_requestExit = requestExit;
		_onUpdate = onUpdate;
	}

	public override void OnEnter()
	{
		_enterTime = Time.time;
		_onEnter?.Invoke(this);	
	}

	public override void OnExit()
	{
		_onExit?.Invoke(this);	
	}

	public override void RequestExit()
	{
		if(Time.time - _enterTime < _delay)
		{
			return;
		}

		if(_requestExit is null)
		{
			FSM!.ExitState();
			return;	
		}

		_requestExit.Invoke(this);
	}

	public override void OnUpdate()
	{
		_onUpdate?.Invoke(this);

		RequestExit();
	}
}

}