using System;

namespace Bytefennec.FSM
{

[Flags]
public enum StateFlags : byte
{
	None = 0 << 0,
	/// <summary>
	/// If set, this state can exit at any point
	/// </summary>
	Interruptable = 1 << 0,
	/// <summary>
	/// If set, this state is instantly exited
	/// </summary>
	Instant = 1 << 1,
} 

}