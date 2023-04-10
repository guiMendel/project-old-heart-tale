using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Range
{
  [Serializable]
  public class Float
  {
    public float min;
    public float max;
    public Float(float min, float max) { this.min = min; this.max = max; }
    public Float((float, float) valueTuple) { this.min = valueTuple.Item1; this.max = valueTuple.Item2; }
    public static implicit operator Float((float, float) valueTuple) { return new Float(valueTuple); }

  }
}