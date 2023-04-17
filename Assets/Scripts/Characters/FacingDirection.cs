using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class FacingDirection : MonoBehaviour
{
  // === PARAMS

  public Sprite down;
  public Sprite downRight;
  public Sprite right;
  public Sprite upRight;
  public Sprite up;

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

  public void FaceDirection(Vector2 direction) => FaceDirection(direction, true);

  void FaceDirection(Vector2 direction, bool interruptFollow)
  {
    if (interruptFollow && followCoroutine != null) StopCoroutine(followCoroutine);

    float angle = direction.AsDegrees();

    // Snap to steps of 45 degrees
    Direction = Helper.DegreeToVector2(Mathf.Round(angle / 45) * 45);
  }

  public void Follow(Transform target)
  {
    FollowTarget = target;
    followCoroutine = StartCoroutine(FaceTargetCoroutine());
  }

  public void StopFollowing() => FollowTarget = null;

  private void Awake()
  {
    Helper.AssertNotNull(down, downRight, right, upRight, up);
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
}
