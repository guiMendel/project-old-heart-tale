using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : CharacterState
{
  // === PARAMS

  public float chaseSpeedModifier = 1.5f;

  public float noSightChaseTolerance = 1f;

  // === STATE

  Transform chaseTarget;

  Coroutine loseTargetTimer;

  float initialSpeed;

  // === REFS

  Movement movement;
  EnemyVision enemyVision;

  protected override void Awake()
  {
    base.Awake();

    movement = GetComponent<Movement>();
    enemyVision = GetComponent<EnemyVision>();

    Helper.AssertNotNull(movement, enemyVision);

    initialSpeed = movement.speed;

    enemyVision.OnDetectTarget.AddListener(target =>
    {
      if (IsActive || manager.ActiveState is Patrol) Activate(target);
    });

    enemyVision.OnLoseTarget.AddListener((target) =>
    {
      if (IsActive && chaseTarget == target) loseTargetTimer = StartCoroutine(MaybeLoseTarget());
    });
  }

  protected override IEnumerator OnDeactivate()
  {
    chaseTarget = null;
    movement.speed = initialSpeed;

    yield break;
  }

  void Spot(Transform target)
  {
    if (manager.ActiveState is Patrol)
    {
      Activate(target);
      return;
    }

    if (IsActive)
    {
      if (loseTargetTimer != null)
      {
        StopCoroutine(loseTargetTimer);
        loseTargetTimer = null;
      }

      chaseTarget = target;

      movement.Follow(chaseTarget);
    }
  }

  public void Activate(Transform target)
  {
    if (target == chaseTarget && loseTargetTimer != null)
    {
      StopCoroutine(loseTargetTimer);
      loseTargetTimer = null;
    }

    chaseTarget = target;

    manager.ActiveState = this;
  }


  protected override IEnumerator OnActivate()
  {
    movement.Follow(chaseTarget);
    movement.speed = initialSpeed * chaseSpeedModifier;

    yield break;
  }

  IEnumerator MaybeLoseTarget()
  {
    yield return new WaitForSeconds(noSightChaseTolerance);

    if (IsActive == false) yield break;

    movement.MoveTo(chaseTarget.position, () =>
    {
      if (IsActive) GetComponent<Patrol>().Activate();
    });

    loseTargetTimer = null;
  }
}
