Class based hierarchical finite state machine for Unity.


A long long time ago I rewrote [UnityHFSM](https://github.com/Inspiaaa/UnityHFSM) from scratch, and just kept using my solution in private.
If you need a more fleshed out finite state machine, I recommend you go use HFSM instead (they even have a fancy visual debugger these days).

A lot of how this version works is similar, so if you've used HFSM before this is pretty straight forward to use.
The two biggest differences are:
- Enum based, no strings allowed for states/transitions.
- Flags are used in place of booleans.

XML comments are available where its not clearly evident from the name. There is no other documentation currently.
I can't guarantee support however if you find any major bugs feel free to pop an issue ticket.

A rough example of how this version can be used:
```
using UnityEngine;
using Bytefennec.FSM;
using UnityEngine.InputSystem;

public class TestFSM : MonoBehaviour
{
    public enum States
    {
        A,
        B,
        C,
        D,
        E,
        F,
    }

    public enum Events
    {
        Test,
    }

    FiniteStateMachine<States, Events> _fsm;

    void Start()
    {
        _fsm = new();

        _fsm.AddState(States.A, exit: s => Debug.Log("A Exit"));
        _fsm.AddState(States.B, exit: s => Debug.Log("B Exit"));
        _fsm.AddState(States.C, flags: StateFlags.Interruptable | StateFlags.Instant);
        _fsm.AddTimedState(States.D, 2.0f,
            enter: s => Debug.Log("D enter"),
            exit: s => Debug.Log("D exit"));
        
        _fsm.AddTransitionAfterTwoWay(States.A, States.B, 1.0f);
        _fsm.AddTransition(States.C, States.D);
        _fsm.AddGlobalTransition(Events.Test, States.C, flags: TransitionFlags.ForceInterrupt);
        _fsm.AddTransitionRandom(States.D, new[]{States.A, States.B});

        _fsm.Initialize(States.A);
    }

    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _fsm.TriggerEvent(Events.Test);    
        }

        _fsm.OnUpdate();
    }
}
```

Rough example on how to extend and add a state type for a character controller:

```
//CharacterState.cs
#nullable enable
using System;
using UnityEngine;
using Bytefennec.FSM;

public interface ICharacterState
{
	public void ProcessMovement(ref Vector3 currentVelocity, float deltaTime);
}

public delegate void CharacterStateFunction(ref Vector3 currentVelocity, float deltaTime);

public class CharacterState<TStateEnum, TStateEvents> : TimedState<TStateEnum, TStateEvents>, ICharacterState
	where TStateEnum : Enum
    where TStateEvents : Enum
{
	private readonly CharacterStateFunction _callback;

	public CharacterState(CharacterStateFunction callback,
		float delay = 0.0f,
		Action<TimedState<TStateEnum, TStateEvents>>? onEnter = null,
		Action<TimedState<TStateEnum, TStateEvents>>? onExit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? requestExit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? onUpdate = null,
		StateFlags flags = StateFlags.Interruptable)
		: base(delay, onEnter, onExit, requestExit, onUpdate, flags)
	{
		_callback = callback;
	}

	public void ProcessMovement(ref Vector3 currentVelocity, float deltaTime)
	{
		_callback.Invoke(ref currentVelocity, deltaTime);
	}
}
```

recommended extension method for said custom state:
```
	public static void AddCharacterState<TStateEnum, TStateEvents>(this FiniteStateMachine<TStateEnum, TStateEvents> fsm,
		TStateEnum id, CharacterStateFunction callback, float delay = 0.0f,
		Action<TimedState<TStateEnum, TStateEvents>>? enter = null,
		Action<TimedState<TStateEnum, TStateEvents>>? exit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? requestExit = null,
		Action<TimedState<TStateEnum, TStateEvents>>? update = null,
		StateFlags flags = StateFlags.Interruptable)
		where TStateEnum : Enum
        where TStateEvents : Enum
	{
		fsm.AddState(id, new CharacterState<TStateEnum, TStateEvents>(
            callback,
            delay:delay,
            onEnter:enter,
            onExit:exit,
            requestExit:requestExit,
            onUpdate:update,
            flags:flags));
	}
```

and somewhere in your character code, you can safely call it as such:
```
//in this situation, we hold IFSM reference to the FSM so we can use this between say.. FSM<PlayerStates, PlayerTriggers> and FSM<EnemyStates, EnemyTriggers>
public IFSM? IFSM = null;

<snip>

public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
{
    if(IFSM is null || !IFSM.HasActiveState || IFSM.UnsafeActiveState is not ICharacterState state)
    {
        return;
    }

    state.ProcessMovement(ref currentVelocity, deltaTime);
}
```