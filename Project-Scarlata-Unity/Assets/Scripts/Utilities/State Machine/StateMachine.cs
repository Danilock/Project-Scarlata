using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateMachine<T>
{
    private List<State<T>> _states = new List<State<T>>();

    public List<State<T>> States => _states;

    public State<T> CurrentState;

    public T Owner { get; set; }

    public StateMachine(T owner)
    {
        Owner = owner;
    }

    public void AddState<StateToAdd>() where StateToAdd : State<T>, new ()
    {
        _states.Add(new StateToAdd());
    }

    public void SetState<StateToSet>() where StateToSet : State<T>, new()
    {
        CurrentState?.OnExit(Owner);

        CurrentState = _states.Find(x => x.GetType() == typeof(StateToSet));

        if(CurrentState == null)
        {
            Application.Quit();
            throw new System.NotImplementedException("No state found!");
        }

        CurrentState?.OnEnter(Owner);
    }
}
