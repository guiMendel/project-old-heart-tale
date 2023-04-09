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
}
