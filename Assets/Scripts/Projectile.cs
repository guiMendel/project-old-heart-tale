using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  // === PARAMS

  public float speed = 9f;

  public Transform target;

  public Vector2 currentDirection;

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
    body.velocity = currentDirection * speed;

    transform.rotation = Quaternion.Euler(0, 0, currentDirection.AsDegrees() - 90);

    Destroy(gameObject, timeToLive);
  }
}
