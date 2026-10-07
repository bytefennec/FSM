#nullable enable
using System;

namespace Bytefennec.FSM
{
	public class TransitionInverter<TStateEnum> : BaseTransition<TStateEnum>
		where TStateEnum : Enum
	{
		private readonly BaseTransition<TStateEnum> _transition;

		public TransitionInverter(BaseTransition<TStateEnum> transition)
			: base(transition.ToID, transition.Flags)
		{
			_transition = transition;
		}

		public override void OnEnter()
		{
			_transition.OnEnter();
		}

		public override void PreTransition()
		{
			_transition.PreTransition();
		}

		public override bool CanTransition()
		{
			return !_transition.CanTransition();
		}
	}
}