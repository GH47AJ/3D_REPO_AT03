using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    public IState CurrentState { get; private set; } //auto property

    protected virtual void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateUpdate();
        }
    }

    public void SetState(IState state)
    {
        if (CurrentState != null)
        {
            CurrentState.OnStateExit();
        }
        CurrentState = state;
        CurrentState.OnStateEnter();
    }

}

public interface IState //Interfaces force classes to implement everything below.
{
    public void OnStateEnter();

    public void OnStateUpdate();

    public void OnStateExit();

}



