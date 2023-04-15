using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  // === PARAMS

  public float steerSpeed = 90;

  public float speed = 9f;

  public Transform target;

  Vector2 _currentDirection;
  public Vector2 CurrentDirection
  {
    get { return _currentDirection; }

    set
    {
      _currentDirection = value;
      body.velocity = value * speed;
      transform.rotation = Quaternion.Euler(0, 0, value.AsDegrees() - 90);
    }
  }

  Vector2 steerDirection;

  public float timeToLive = 10f;

  // === REFS

  Rigidbody2D body;

  private void Awake()
  {
    body = GetComponent<Rigidbody2D>();

    Helper.AssertNotNull(body);
  }

  private void Start()
  {
    Destroy(gameObject, timeToLive);
  }

  private void Update()
  {
    if (target == null) return;

    steerDirection = (target.position - transform.position).normalized;

    float angle = CurrentDirection.AsDegrees();

    float deltaAngle = Mathf.DeltaAngle(angle, steerDirection.AsDegrees());

    float angleChange = Mathf.Sign(deltaAngle) * Mathf.Min(Mathf.Abs(deltaAngle), steerSpeed * Time.deltaTime);

    CurrentDirection = Quaternion.Euler(0, 0, angleChange) * CurrentDirection;
  }
}
