#nullable enable
using System;
using System.Text;
using UnityEngine;

namespace Bytefennec.FSM
{

public class TransitionRandom<TStateEnum> : BaseTransition<TStateEnum>
	where TStateEnum : Enum
{
	private float _enterTime;
	private readonly float _delay;
	private readonly Func<BaseTransition<TStateEnum>, bool>? _canTransition;
	private readonly TStateEnum[] _ids;

	public TransitionRandom(TStateEnum[] ids, float delay, Func<BaseTransition<TStateEnum>, bool>? canTransition = null, TransitionFlags flags = TransitionFlags.None)
		: base(ids[0], flags | TransitionFlags.NoEqualityComparison)
	{
		_canTransition = canTransition;
		_ids = ids;
		_delay = delay;
	}

	public override void OnEnter()
	{
		_enterTime = Time.time;
	}

	public override void PreTransition()
	{
		ToID = _ids[UnityEngine.Random.Range(0, _ids.Length)];
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

	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append('[');
		for(int i = 0; i < _ids.Length; i++)
		{
			sb.Append(_ids[i].ToString());
			if(i < _ids.Length - 1)
			{
				sb.Append(", ");
			}
		}

		sb.Append(']');
		return sb.ToString();
	}
}

}