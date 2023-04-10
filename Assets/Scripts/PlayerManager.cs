using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
  // === PARAMS

  public float deathRestartTimer = 1f;

  // === REFS

  Health health;

  private void Awake()
  {
    health = GetComponent<Health>();

    Helper.AssertNotNull(health);
  }

  private void Start()
  {
    health.OnDeath.AddListener(() => StartCoroutine(RestartSceneIn(deathRestartTimer)));
  }

  IEnumerator RestartSceneIn(float seconds)
  {
    yield return new WaitForSeconds(seconds);

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}
