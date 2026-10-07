using System;

namespace Bytefennec.FSM
{

[Flags]
public enum TransitionFlags : byte
{
	None = 0 << 0,
	/// <summary>
	/// If set, this transition can always interrupt a state.
	/// </summary>
	ForceInterrupt = 1 << 0,
	/// <summary>
	/// If set, this transition exits the child FSM to the parent FSM.
	/// </summary>
	IsFSMExit = 1 << 1,
	/// <summary>
	/// If set, this transition does not compare the current and new states, and will cause a state change even if the current and new states are the same.
	/// </summary>
	NoEqualityComparison = 1 << 2,
} 

}