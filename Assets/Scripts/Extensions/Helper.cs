using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
  public static void AssertNotNull(params Object[] objects)
  {
    foreach (Object thing in objects) if (thing == null)
      {
        // Get unity's helpful error message into ours
        try
        {
          thing.GetType();
        }
        catch (System.Exception error)
        {
          throw new System.Exception(
            "Got a null reference to some component. Unity's error message:\n" + error.Message
          );
        }
      }
  }


  public static Vector2 RadianToVector2(float radian)
  {
    return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
  }

  public static Vector2 DegreeToVector2(float degree)
  {
    return RadianToVector2(degree * Mathf.Deg2Rad);
  }
}
