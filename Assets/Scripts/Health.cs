using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
  // === EVENTS

  public UnityEvent OnDeath;

  // === STATE

  // Whether is alive
  public bool Alive { get; private set; } = true;

  public void TakeDamage()
  {
    Die();
  }

  void Die()
  {
    Alive = false;

    var sprite = GetComponent<SpriteRenderer>();
    if (sprite) sprite.color = Color.gray;

    OnDeath.Invoke();
  }

}
