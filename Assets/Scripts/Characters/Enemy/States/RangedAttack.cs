using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEditor.SearchService;
using UnityEngine;

public class RangedAttack : CharacterState
{
  // === PARAMS

  public GameObject projectile;

  public Range.Float range = (1f, 4f);

  public float cooldown = 3f;

  public float maxDetectionDelay = 0.1f;

  public float aimDelay = 0.6f;

  public float recoveryDelay = 0.3f;

  // === STATE

  Transform currentTarget;

  Movement movement => GetComponent<Movement>();

  // === REFS

  EnemyVision vision;

  override protected void Awake()
  {
    base.Awake();

    vision = GetComponent<EnemyVision>();

    Helper.AssertNotNull(vision);
  }

  private void Start()
  {
    StartCoroutine(DetectTargetInRange());
  }

  IEnumerator DetectTargetInRange()
  {
    var delay = new WaitForSeconds(maxDetectionDelay);
    var cooldownDelay = new WaitForSeconds(cooldown);

    while (true)
    {
      if (manager.ActiveState is Patrol || manager.ActiveState is Chase)
        foreach (var target in vision.ActiveTargets)
        {
          float sqrDistance = transform.position.SqrDistance(target.position);

          if (range.min * range.min > sqrDistance || sqrDistance > range.max * range.max)
            continue;

          Activate(target);

          yield return cooldownDelay;

          break;
        }

      yield return delay;
    }
  }

  private void Activate(Transform target)
  {
    currentTarget = target;

    manager.ActiveState = this;
  }

  protected override IEnumerator OnDeactivate()
  {
    movement.FaceMovement();
    yield break;
  }

  protected override IEnumerator OnActivate()
  {
    movement.Halt();
    movement.Face(currentTarget);

    yield return new WaitForSeconds(aimDelay);

    if (IsActive == false) yield break;

    ShootAt(currentTarget);

    yield return new WaitForSeconds(recoveryDelay);

    if (IsActive == false) yield break;

    if (vision.InVision(currentTarget))
      GetComponent<Chase>().Activate(currentTarget);

    else GetComponent<Patrol>().Activate();
  }

  private void ShootAt(Transform target)
  {
    Vector2 targetDirection = AimFor(target);

    Projectile spawnedProjectile =
      Instantiate(
        projectile, transform.position, Quaternion.Euler(0, 0, targetDirection.AsDegrees()))
      .GetComponent<Projectile>();

    spawnedProjectile.target = target;
    spawnedProjectile.CurrentDirection = targetDirection;
  }

  private Vector2 AimFor(Transform target)
  {
    return MovementPrediction.PredictTargetMovement(
      transform,
      target,
      projectile.GetComponent<Projectile>().speed);
  }
}
