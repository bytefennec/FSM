#nullable enable
using System;

namespace Bytefennec.FSM
{

public class Transition<TStateEnum> : BaseTransition<TStateEnum>
	where TStateEnum : Enum
{
	private readonly Func<BaseTransition<TStateEnum>, bool>? _canTransition;

	public Transition(TStateEnum toID, Func<BaseTransition<TStateEnum>, bool>? canTransition = null, TransitionFlags flags = TransitionFlags.None)
		: base(toID, flags)
	{
		_canTransition = canTransition;
	}

	public override bool CanTransition()
	{
		if(_canTransition is null)
		{
			return true;
		}

		return _canTransition(this);
	}
}

}