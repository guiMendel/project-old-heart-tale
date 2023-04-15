using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

abstract public class CharacterState : MonoBehaviour
{
  // === REFS

  [NonSerialized] public CharacterStateManager manager;

  public bool IsActive => manager.ActiveState == this;


  virtual protected void OnActivate() { }
  virtual protected void OnDeactivate() { }

  virtual protected void Awake()
  {
    manager = GetComponent<CharacterStateManager>();

    Helper.AssertNotNull(manager);

    manager.OnChangeState.AddListener((newState, oldState) =>
    {
      if (newState == this) OnActivate();
      else if (oldState == this) OnDeactivate();
    });
  }

}
