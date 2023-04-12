using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
  // === PARAMS

  public Transform waypoints;

  public float chaseSpeedModifier = 1.5f;

  public float noSightChaseTolerance = 1f;

  // === STATE

  int currentWaypoint = 0;

  Transform chaseTarget;

  Coroutine loseTargetTimer;

  float initialSpeed;

  // === REFS

  Movement movement;
  EnemyVision enemyVision;
  EnemyState enemyState;

  private void Awake()
  {
    movement = GetComponent<Movement>();
    enemyVision = GetComponent<EnemyVision>();
    enemyState = GetComponent<EnemyState>();

    Helper.AssertNotNull(movement, enemyVision, enemyState);

    initialSpeed = movement.speed;

    enemyVision.OnDetectTarget.AddListener(target =>
    {
      if (enemyState.OneOf(EnemyState.State.Patrol, EnemyState.State.Idle, EnemyState.State.Chase)) Chase(target);
    });

    enemyVision.OnLoseTarget.AddListener(
      (target) => loseTargetTimer = StartCoroutine(MaybeLoseTarget(target)));

    enemyState.OnChangeState.AddListener((_, oldState) =>
    {
      if (oldState == EnemyState.State.Chase)
        chaseTarget = null;
    });
  }

  private void Start()
  {
    Debug.Assert(waypoints.childCount > 0, "Must assign waypoints to the enemy movement script");

    AdvanceTarget();
  }

  void Patrol()
  {
    enemyState.ActiveState = EnemyState.State.Patrol;
    movement.speed = initialSpeed;

    movement.MoveTo(waypoints.GetChild(currentWaypoint).position, () =>
    {
      if (enemyState.OneOf(EnemyState.State.Patrol)) AdvanceTarget();
    });
  }

  void Chase(Transform target)
  {
    enemyState.ActiveState = EnemyState.State.Chase;

    if (chaseTarget != null)
    {
      if (target == chaseTarget && loseTargetTimer != null)
      {
        StopCoroutine(loseTargetTimer);
        loseTargetTimer = null;
      }
    }

    chaseTarget = target;

    movement.Follow(target);
    movement.speed = initialSpeed * chaseSpeedModifier;
  }

  IEnumerator MaybeLoseTarget(Transform target)
  {
    if (chaseTarget != target) yield break;

    yield return new WaitForSeconds(noSightChaseTolerance);

    movement.MoveTo(target.position, () =>
    {
      if (enemyState.OneOf(EnemyState.State.Chase)) Patrol();
    });

    loseTargetTimer = null;
  }

  void AdvanceTarget()
  {
    currentWaypoint = (currentWaypoint + 1) % waypoints.childCount;
    Patrol();
  }
}
