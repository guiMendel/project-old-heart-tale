using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.Assertions;

public class FacingDirection : MonoBehaviour
{
  // === PARAMS

  public Sprite[] directions;

  // === EVENTS

  // Called when facing direction changes
  public Event.DoubleVector2 OnChangeDirection;

  // === STATE

  Coroutine followCoroutine;

  public Transform FollowTarget { get; private set; }

  Vector2 _direction = Vector2.down;
  public Vector2 Direction
  {
    get { return _direction; }

    private set
    {
      OnChangeDirection.Invoke(value, _direction);
      _direction = value;
    }
  }

  // === REFS

  SpriteRenderer spriteRenderer;


  public void FaceDirection(Vector2 direction) => FaceDirection(direction, true);

  void FaceDirection(Vector2 direction, bool interruptFollow)
  {
    if (interruptFollow && followCoroutine != null) StopCoroutine(followCoroutine);

    float angle = direction.AsDegrees();

    // Snap to steps of 45 degrees
    float newAngle = Mathf.Round(angle / 45) * 45;
    Direction = Helper.DegreeToVector2(newAngle);
    UpdateSprite(newAngle);
  }

  public void Follow(Transform target)
  {
    FollowTarget = target;
    followCoroutine = StartCoroutine(FaceTargetCoroutine());
  }

  public void StopFollowing() => FollowTarget = null;

  private void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();

    Helper.AssertNotNull(spriteRenderer);

    Debug.Assert(directions.Length == 5, "There should be exactly 5 directions");

    // Reorder directions to start facing right and repeat frames to the left
    directions = new Sprite[] {
      directions[2],
      directions[3],
      directions[4],
      directions[3],
      directions[2],
      directions[1],
      directions[0],
      directions[1]
    };
  }

  IEnumerator FaceTargetCoroutine()
  {
    var delay = new WaitForSeconds(0.1f);

    while (FollowTarget != null)
    {
      FaceDirection(FollowTarget.position - transform.position, false);

      yield return delay;
    }
  }

  private void UpdateSprite(float angle)
  {
    if (angle < 0) angle += 360;

    print((gameObject.name, angle));

    // Pick corresponding sprite
    spriteRenderer.sprite = directions[Mathf.RoundToInt(angle / 45)];

    bool mirror = angle > 90f && angle < 270f;

    transform.localScale = new Vector2(mirror ? -1f : 1f, 1f);
  }
}
