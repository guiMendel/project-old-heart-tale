using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{
  // === PARAMS

  public CharacterState initialState;

  // === EVENTS

  public Event.DoubleCharacterState OnChangeState;

  // === STATE
  CharacterState _activeState = null;

  public CharacterState ActiveState
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

  private void Start()
  {
    ActiveState = initialState;
  }

  public bool OneOf(params CharacterState[] states)
  {
    foreach (CharacterState state in states)
      if (ActiveState == state) return true;

    return false;
  }

  public bool Not(params CharacterState[] states)
  {
    return OneOf(states) == false;
  }
}
