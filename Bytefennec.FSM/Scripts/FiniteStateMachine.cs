#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytefennec.FSM
{

/// <summary>
/// Represents a finite state machine.
/// </summary>
public class FiniteStateMachine<TStateEnum, TStateEvents> : BaseState<TStateEnum, TStateEvents>, IFSM
	where TStateEnum : Enum
	where TStateEvents : Enum
{
	private readonly Dictionary<TStateEnum, BaseState<TStateEnum, TStateEvents>> _states = new();
	private readonly List<BaseTransition<TStateEnum>> _globalTransitions = new();
	private readonly Dictionary<TStateEvents, List<BaseTransition<TStateEnum>>> _globalEventTransitions = new();
	#pragma warning disable CS8619, CS8601
	private (TStateEnum ID, bool Set) _startState = (default, false);
	public TStateEnum ActiveStateID {get; private set;} = default;
	#pragma warning restore CS8619, CS8601
	public BaseState<TStateEnum, TStateEvents>? ActiveState {get; private set;} = null;
	public dynamic UnsafeActiveState => ActiveState!;
	public bool HasActiveState => ActiveState is not null;
	private BaseTransition<TStateEnum>? _pendingTransition = null;
	private bool _rememberLastState = false;

	public FiniteStateMachine(bool rememberLastState = false, StateFlags flags = StateFlags.Interruptable)
		: base(flags)
	{
		_rememberLastState = rememberLastState;
	}

	public void AddState(TStateEnum id, BaseState<TStateEnum, TStateEvents> state)
	{
		if(_states.ContainsKey(id))
		{
			return;
		}

		state.FSM = this;
		_states.Add(id, state);
	}

	public void AddTransition(TStateEnum fromID, BaseTransition<TStateEnum> transition)
	{
		if(!_states.ContainsKey(fromID))
		{
			return;
		}

		_states[fromID].AddTransition(transition);
	}

	public void AddTransition(TStateEvents @event, TStateEnum fromID, BaseTransition<TStateEnum> transition)
	{
		if(!_states.ContainsKey(fromID))
		{
			return;
		}

		_states[fromID].AddTransition(@event, transition);		
	}

	public void AddGlobalTransition(BaseTransition<TStateEnum> transition)
	{
		_globalTransitions.Add(transition);
	}

	public void AddGlobalTransition(TStateEvents @event, BaseTransition<TStateEnum> transition)
	{
		if(!_globalEventTransitions.TryGetValue(@event, out List<BaseTransition<TStateEnum>> transitions))
		{
			transitions = new();
			_globalEventTransitions.Add(@event, transitions);
		}

			transitions.Add(transition);
	}

	public void Initialize(TStateEnum id)
	{
		if(FSM is not null)
		{
			return;
		}

		SetStartState(id);
		OnEnter();
	}

	public void SetStartState(TStateEnum id)
	{
		if(!_states.ContainsKey(id))
		{
			return;
		}

		_startState = (id, true);
	}

	private void ChangeActiveState(TStateEnum ID)
	{
		ActiveState?.OnExit();
		ActiveState = _states[ID];
		ActiveStateID = ID;
		ActiveState.OnEnter();

		//inform transitions too
		for(int i = 0; i < _globalTransitions.Count; i++)
		{
			var transition = _globalTransitions[i];
			transition.OnEnter();
		}

		foreach(var transitions in _globalEventTransitions.Values)
		{
			for(int i = 0; i < transitions.Count; i++)
			{
				var transition = transitions[i];
				transition.OnEnter();
			}	
		}
		
		if(ActiveState.Transitions is not null)
		{
			for(int i = 0; i < ActiveState.Transitions.Count; i++)
			{
				ActiveState.Transitions[i].OnEnter();
			}	
		}

		if(ActiveState.EventTransitions is not null)
		{
			foreach(var transitions in ActiveState.EventTransitions.Values)
			{
				for(int i = 0; i < transitions.Count; i++)
				{
					var transition = transitions[i];
					transition.OnEnter();
				}	
			}
		}

		if(ActiveState.Instant)
		{
			ProcessStateTransitions();
		}
	}

	public override void OnEnter()
	{
		if(!_startState.Set)
		{
			return;
		}

		_pendingTransition = null;
		ChangeActiveState(_startState.ID);
	}

	public override void OnExit()
	{
		if(ActiveState is null)
		{
			return;
		}

		if(_rememberLastState)
		{
			_startState.ID = ActiveStateID;
		}

		ActiveState.OnExit();
		ActiveState = null;
	}

	public override void RequestExit()
	{
		if(ActiveState is not null && !ActiveState.Interruptable)
		{
			ActiveState.RequestExit();
		}
	}

	public void Update()
	{
		OnUpdate();		
	}

	public override void OnUpdate()
	{
		if(ProcessGlobalTransitions() || ProcessStateTransitions())
		{
			goto STATE_UPDATE;
		}

		STATE_UPDATE:
		ActiveState?.OnUpdate();
	}

	private bool ProcessGlobalTransitions()
	{
		if(ActiveState is null)
		{
			return false;
		}

		for(int i = 0; i < _globalTransitions.Count; i++)
		{
			var transition = _globalTransitions[i];
			if(!transition.NoEqualityComparison && EqualityComparer<TStateEnum>.Default.Equals(transition.ToID, ActiveStateID))
			{
				continue;
			}

			if(ProcessTransition(transition))
			{
				return true;		
			}
		}

		return false;
	}

	private bool ProcessStateTransitions()
	{
		if(ActiveState is null || ActiveState.Transitions is null)
		{
			return false;
		}

		for(int i = 0; i < ActiveState.Transitions.Count; i++)
		{
			var transition = ActiveState.Transitions[i];

			if(ProcessTransition(transition))
			{
				return true;		
			}
		}
	
		return false;
	}

	private bool ProcessTransition(BaseTransition<TStateEnum> transition)
	{
		if(!transition.CanTransition())
		{
			return false;
		}

		if(transition.IsFSMExit)
		{
			if(FSM is null)
			{
				return false;
			}

			ProcessFSMExit(transition);
			return true;	
		}

		ProcessStateChange(transition);
		return true;
	}

	private void ProcessStateChange(BaseTransition<TStateEnum> transition)
	{
		if(ActiveState is null)
		{
			return;
		}

		if(ActiveState.Interruptable || transition.ForceInterrupt)
		{
			_pendingTransition = null;
			transition.PreTransition();
			ChangeActiveState(transition.ToID);
		}
		else
		{
			_pendingTransition = transition;
			ActiveState.RequestExit();
		}
	}

	private void ProcessFSMExit(BaseTransition<TStateEnum> transition)
	{
		if(ActiveState is null)
		{
			return;
		}

		if(ActiveState.Interruptable || transition.ForceInterrupt)
		{
			_pendingTransition = null;
			FSM?.ExitState();
		}
		else
		{
			_pendingTransition = transition;
			ActiveState.RequestExit();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="event"></param>
	/// <returns><c>true</c> if transitioned, <c>false</c> otherwise.</returns>
	public bool TriggerEvent(TStateEvents @event)
	{
		if(ActiveState is null)
		{
			return false;
		}

		if(!_globalEventTransitions.TryGetValue(@event, out List<BaseTransition<TStateEnum>> transitions))
		{
			return false;
		}

		for(int i = 0; i < transitions.Count; i++)
		{
			var transition = transitions[i];
			if(!transition.NoEqualityComparison && EqualityComparer<TStateEnum>.Default.Equals(transition.ToID, ActiveStateID))
			{
				continue;	
			}	

			if(ProcessTransition(transition))
			{
				return true;
			}
		}

		if(ActiveState.EventTransitions is null || !ActiveState.EventTransitions.TryGetValue(@event, out transitions))
		{
			return false;
		}

		for(int i = 0; i < transitions.Count; i++)
		{
			var transition = transitions[i];
			if(!transition.NoEqualityComparison && EqualityComparer<TStateEnum>.Default.Equals(transition.ToID, ActiveStateID))
			{
				continue;	
			}	

			if(ProcessTransition(transition))
			{
				return true;
			}
		}

		return false;
	}

	public void ExitState()
	{
		if(_pendingTransition is null)
		{
			return;
		}

		if(_pendingTransition.IsFSMExit)
		{
			_pendingTransition = null;
			FSM?.ExitState();
		}
		else
		{
			_pendingTransition.PreTransition();
			TStateEnum to = _pendingTransition.ToID;
			_pendingTransition = null;
			ChangeActiveState(to);
		}
	}

	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append("/");
		if(ActiveState is not null)
		{
			sb.Append(ActiveStateID.ToString());
			if(_pendingTransition is not null)
			{
				sb.Append("->");
				sb.Append(_pendingTransition.ToString());
			}

			sb.Append(ActiveState.ToString()); //display nested FSM if possible
		}
		
		return sb.ToString();
	}
}

}