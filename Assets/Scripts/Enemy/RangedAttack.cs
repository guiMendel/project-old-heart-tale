using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEditor.SearchService;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
  // === PARAMS

  public GameObject projectile;

  public Range.Float range = (1f, 4f);

  public float cooldown = 3f;

  public float maxDetectionDelay = 0.1f;

  // === REFS

  EnemyVision vision;

  private void Awake()
  {
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
      foreach (var target in vision.ActiveTargets)
      {
        float sqrDistance = transform.position.SqrDistance(target.position);

        if (range.min * range.min > sqrDistance || sqrDistance > range.max * range.max)
          continue;

        ShootAt(target);

        yield return cooldownDelay;

        break;
      }

      yield return delay;
    }
  }

  private void ShootAt(Transform target)
  {
    Vector2 targetDirection = AimFor(target);

    Projectile spawnedProjectile =
      Instantiate(
        projectile, transform.position, Quaternion.Euler(0, 0, targetDirection.AsDegrees()))
      .GetComponent<Projectile>();

    spawnedProjectile.target = target;
    spawnedProjectile.currentDirection = targetDirection;
  }

  private Vector2 AimFor(Transform target)
  {
    return (target.position - transform.position).normalized;
  }
}
