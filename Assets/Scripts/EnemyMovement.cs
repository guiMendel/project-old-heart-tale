using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
  // === PARAMS

  public float speed = 5.5f;
  public float sprintSpeed = 7f;

  public Transform waypoints;

  // === STATE

  int currentTarget = 0;

  private void Start()
  {
    Debug.Assert(waypoints.childCount > 0, "Must assign waypoints to the enemy movement script");

    transform.position = waypoints.GetChild(0).position;
    AdvanceTarget();
  }

  private void Update()
  {
    var targetPosition = waypoints.GetChild(currentTarget).position;

    transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);

    if (transform.position.SqrDistance(targetPosition) <= 0.01f) AdvanceTarget();
  }

  void AdvanceTarget()
  {
    currentTarget = (currentTarget + 1) % waypoints.childCount;
  }


}
