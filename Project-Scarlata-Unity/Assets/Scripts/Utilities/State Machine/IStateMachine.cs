using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateMachine<T>
{
    public StateMachine<T> StateMachine { get; set; }
}
