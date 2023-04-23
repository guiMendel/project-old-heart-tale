using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Patrol : CharacterState
{
  // === PARAMS

  public Transform waypoints;

  // === STATE

  int currentWaypoint = 0;

  // === REFS

  Movement movement;

  override protected void Awake()
  {
    base.Awake();

    movement = GetComponent<Movement>();

    Helper.AssertNotNull(movement);
  }


  private void Start()
  {
    Debug.Assert(waypoints.childCount > 0, "Must assign waypoints to the enemy movement script");
  }

  public void Activate() => manager.ActiveState = this;

  protected override IEnumerator OnActivate()
  {
    FollowPath();
    yield break;
  }

  void FollowPath()
  {
    Null Advance()
    {
      if (IsActive) AdvanceTarget();

      return null;
    }

    movement.MoveTo(waypoints.GetChild(currentWaypoint).position)
      .Then(_ => Advance())
      .OnFail(_ => Advance());
  }

  void AdvanceTarget()
  {
    currentWaypoint = (currentWaypoint + 1) % waypoints.childCount;
    FollowPath();
  }
}
