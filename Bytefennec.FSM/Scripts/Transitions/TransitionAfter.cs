#nullable enable
using System;
using UnityEngine;

namespace Bytefennec.FSM
{

public class TransitionAfter<TStateEnum> : BaseTransition<TStateEnum>
	where TStateEnum : Enum
{
	private float _enterTime;
	private readonly float _delay;
	private readonly Func<BaseTransition<TStateEnum>, bool>? _canTransition;

	public TransitionAfter(TStateEnum toID, float delay, Func<BaseTransition<TStateEnum>, bool>? canTransition = null, TransitionFlags flags = TransitionFlags.None)
		: base(toID, flags)
	{
		_canTransition = canTransition;
		_delay = delay;
	}

	public override void OnEnter()
	{
		_enterTime = Time.time;
	}

	public override bool CanTransition()
	{
		if(Time.time - _enterTime < _delay)
		{
			return false;
		}

		if(_canTransition is null)
		{
			return true;
		}

		return _canTransition(this);
	}
}

}