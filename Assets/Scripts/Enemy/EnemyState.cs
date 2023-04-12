using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
  // === EVENTS

  public Event.DoubleEnemyState OnChangeState;

  // === STATE

  public enum State
  {
    Idle,
    Patrol,
    Chase,
    RangedAttack
  }

  State _activeState = State.Idle;
  public State ActiveState
  {
    get
    {
      return _activeState;
    }
    set
    {
      if (value == _activeState) return;

      var oldState = _activeState;
      _activeState = value;

      OnChangeState.Invoke(value, oldState);
    }
  }

  public bool OneOf(params State[] states)
  {
    foreach (State state in states)
      if (ActiveState == state) return true;

    return false;
  }

  public bool Not(params State[] states)
  {
    return OneOf(states) == false;
  }
}
