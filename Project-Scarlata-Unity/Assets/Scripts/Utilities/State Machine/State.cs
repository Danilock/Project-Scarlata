using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T>
{
    public virtual void OnEnter(T entity) { }
    public virtual void OnUpdate(T entity) { }
    
    public virtual void OnExit(T entity) { }
}
