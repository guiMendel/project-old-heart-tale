using System.Collections;
using ExtensionMethods;
using Unity.VisualScripting;
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

    var angle = target.GetComponent<FacingDirection>().Direction.AsDegrees();

    Vector2 angle1 = Helper.DegreeToVector2(angle + enemyVision.angle / 2);
    Vector2 angle2 = Helper.DegreeToVector2(angle - enemyVision.angle / 2);

    Handles.color = Color.yellow;

    Handles.DrawLine(enemyVision.transform.position, enemyVision.transform.position + (Vector3)angle1 * enemyVision.range);
    Handles.DrawLine(enemyVision.transform.position, enemyVision.transform.position + (Vector3)angle2 * enemyVision.range);

    Handles.color = Color.red;

    foreach (var target in enemyVision.ActiveTargets)
      Handles.DrawLine(enemyVision.transform.position, target.position);
  }
}
