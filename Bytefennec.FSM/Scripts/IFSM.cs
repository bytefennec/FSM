namespace Bytefennec.FSM
{

public interface IFSM
{
	public bool HasActiveState {get;}
	public dynamic UnsafeActiveState {get;}
	public void Update();
	public void ExitState();
}

}