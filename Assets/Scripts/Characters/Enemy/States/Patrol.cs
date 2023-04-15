using System.Collections;
using System.Collections.Generic;
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

  protected override void OnActivate() => FollowPath();

  void FollowPath()
  {
    movement.MoveTo(waypoints.GetChild(currentWaypoint).position, () =>
    {
      if (IsActive) AdvanceTarget();
    });

  }

  void AdvanceTarget()
  {
    currentWaypoint = (currentWaypoint + 1) % waypoints.childCount;
    FollowPath();
  }
}
