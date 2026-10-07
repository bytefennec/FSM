using System;

namespace Bytefennec.FSM
{

/// <summary>
/// Represents a transition between two states inside the finite state machine. 
/// </summary>
/// <typeparam name="TStateEnum"></typeparam>
public class BaseTransition<TStateEnum>
	where TStateEnum : Enum
{
	public TStateEnum ToID;
	public readonly TransitionFlags Flags;
	/// <summary>
	/// If <c>true</c> this transition can always interrupt a state. 
	/// </summary>
	public bool ForceInterrupt => (Flags & TransitionFlags.ForceInterrupt) > 0;
	/// <summary>
	/// If <c>true</c> this transition exits the child FSM to the parent FSM.
	/// </summary>
	public bool IsFSMExit => (Flags & TransitionFlags.IsFSMExit) > 0;
	/// <summary>
	/// If <c>true</c> this transition does not compare the current and new states, and will cause a state change even if the current and new states are the same.
	/// </summary>
	public bool NoEqualityComparison => (Flags & TransitionFlags.NoEqualityComparison) > 0;

	public BaseTransition(TStateEnum toID, TransitionFlags flags)
	{
		ToID = toID;
		Flags = flags;
	}

	/// <summary>
	/// Called when the state machine transitions to a state this transition belongs to.
	/// </summary>
	public virtual void OnEnter()
	{
	}

	/// <summary>
	/// Called before the state machine transitions using this transition. 
	/// </summary>
	public virtual void PreTransition()
	{
	}

	/// <summary>
	/// Called when the state machine asks if this transition can happen.
	/// </summary>
	/// <returns><c>true</c> if transition can happen, otherwise <c>false</c></returns>
	public virtual bool CanTransition()
	{
		return true;		
	}

	public override string ToString()
	{
		return ToID.ToString();
	}
}

}