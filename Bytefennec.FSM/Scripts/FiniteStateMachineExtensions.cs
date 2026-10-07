#nullable enable
using System;

namespace Bytefennec.FSM
{
	
public static class FiniteStateMachineExtensions
{
	public static void AddState<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum id, FiniteStateMachine<TStateEnum, TStateEvents> other)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddState(id, other);
	}

	public static void AddState<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum id,
		Action<State<TStateEnum, TStateEvents>>? enter = null,
		Action<State<TStateEnum, TStateEvents>>? exit = null,
		Action<State<TStateEnum, TStateEvents>>? requestExit = null,
		Action<State<TStateEnum, TStateEvents>>? update = null,
		StateFlags flags = StateFlags.Interruptable)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddState(id, new State<TStateEnum, TStateEvents>(onEnter:enter, onExit:exit, requestExit:requestExit, onUpdate:update, flags:flags));
	}

	public static void AddTimedState<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum id, float delay,
		Action<TimedState<TStateEnum, TStateEvents>>? enter = null,
		Action<TimedState<TStateEnum, TStateEvents>>? exit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? requestExit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? update = null,
		StateFlags flags = StateFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddState(id, new TimedState<TStateEnum, TStateEvents>(delay:delay, onEnter:enter, onExit:exit, requestExit:requestExit, onUpdate:update, flags:flags));
	}

	public static void AddTransition<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum fromID, TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(fromID, new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
	}

	public static void AddTransition<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum fromID, TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(@event, fromID, new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
	}

	public static void AddTransitionTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum fromID, TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(fromID, new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
		fsm.AddTransition(toID, new TransitionInverter<TStateEnum>(new Transition<TStateEnum>(toID:fromID, canTransition:canTransition, flags:flags)));
	}

	public static void AddTransitionTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum fromID, TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(@event, fromID, new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
		fsm.AddTransition(@event, toID, new TransitionInverter<TStateEnum>(new Transition<TStateEnum>(toID:fromID, canTransition:canTransition, flags:flags)));
	}

	public static void AddGlobalTransition<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
	}

	public static void AddGlobalTransition<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(@event, new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
	}

	public static void AddGlobalTransitionTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum fromID, TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
		fsm.AddGlobalTransition(new TransitionInverter<TStateEnum>(new Transition<TStateEnum>(toID:fromID, canTransition:canTransition, flags:flags)));
	}

	public static void AddGlobalTransitionTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum fromID, TStateEnum toID,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(@event, new Transition<TStateEnum>(toID:toID, canTransition:canTransition, flags:flags));
		fsm.AddGlobalTransition(@event, new TransitionInverter<TStateEnum>(new Transition<TStateEnum>(toID:fromID, canTransition:canTransition, flags:flags)));
	}

	public static void AddTransitionAfter<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum fromID, TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(fromID, new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
	}

	public static void AddTransitionAfter<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum fromID, TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(@event, fromID, new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
	}

	public static void AddTransitionAfterTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum fromID, TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(fromID, new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
		fsm.AddTransition(toID, new TransitionInverter<TStateEnum>(new TransitionAfter<TStateEnum>(toID:fromID, delay:delay, canTransition:canTransition, flags:flags)));
	}

	public static void AddTransitionAfterTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum fromID, TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(@event, fromID, new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
		fsm.AddTransition(@event, toID, new TransitionInverter<TStateEnum>(new TransitionAfter<TStateEnum>(toID:fromID, delay:delay, canTransition:canTransition, flags:flags)));
	}

	public static void AddGlobalTransitionAfter<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
	}

	public static void AddGlobalTransitionAfter<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(@event, new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
	}

	public static void AddGlobalTransitionAfterTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum fromID, TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
		fsm.AddGlobalTransition(new TransitionInverter<TStateEnum>(new TransitionAfter<TStateEnum>(toID:fromID, delay:delay, canTransition:canTransition, flags:flags)));
	}

	public static void AddGlobalTransitionAfterTwoWay<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum fromID, TStateEnum toID, float delay,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(@event, new TransitionAfter<TStateEnum>(toID:toID, delay:delay, canTransition:canTransition, flags:flags));
		fsm.AddGlobalTransition(@event, new TransitionInverter<TStateEnum>(new TransitionAfter<TStateEnum>(toID:fromID, delay:delay, canTransition:canTransition, flags:flags)));
	}

	public static void AddTransitionRandom<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum fromID, TStateEnum[] ids, float delay = 0.0f,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(fromID, new TransitionRandom<TStateEnum>(ids:ids, delay:delay, canTransition:canTransition, flags:flags));
	}

	public static void AddTransitionRandom<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum fromID, TStateEnum[] ids, float delay = 0.0f,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddTransition(@event, fromID, new TransitionRandom<TStateEnum>(ids:ids, delay:delay, canTransition:canTransition, flags:flags));
	}

	public static void AddGlobalTransitionRandom<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum[] ids, float delay = 0.0f,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(new TransitionRandom<TStateEnum>(ids:ids, delay:delay, canTransition:canTransition, flags:flags));
	}

	public static void AddGlobalTransitionRandom<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEvents @event, TStateEnum[] ids, float delay = 0.0f,
		Func<BaseTransition<TStateEnum>, bool>? canTransition = null,
		TransitionFlags flags = TransitionFlags.None)
		where TStateEnum : Enum
		where TStateEvents : Enum
	{
		fsm.AddGlobalTransition(@event, new TransitionRandom<TStateEnum>(ids:ids, delay:delay, canTransition:canTransition, flags:flags));
	}
}

}