using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyVision))]
public class FieldOfView : Editor
{
  private void OnSceneGUI()
  {
    EnemyVision enemyVision = (EnemyVision)target;

    Handles.color = Color.white;

    Handles.DrawWireArc(enemyVision.transform.position, Vector3.forward, Vector2.right, 360, enemyVision.range);

    // Handles.DrawLine(enemyVision.transform.position, Helper.DegreeToVector2())
  }
}
