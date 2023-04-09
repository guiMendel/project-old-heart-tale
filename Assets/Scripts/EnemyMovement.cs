using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
  // === PARAMS
  
  public Transform waypoints;

  // === STATE

  int currentTarget = 0;

  // === REFS

  Movement movement;

  private void Awake()
  {
    movement = GetComponent<Movement>();

    Helper.AssertNotNull(movement);
  }

  private void Start()
  {
    Debug.Assert(waypoints.childCount > 0, "Must assign waypoints to the enemy movement script");

    transform.position = waypoints.GetChild(0).position;

    void MoveToNextTarget()
    {
      AdvanceTarget();
      movement.MoveTo(waypoints.GetChild(currentTarget).position);
    }

    movement.OnReachPosition.AddListener(MoveToNextTarget);

    MoveToNextTarget();
  }

  void AdvanceTarget()
  {
    currentTarget = (currentTarget + 1) % waypoints.childCount;
  }
}
