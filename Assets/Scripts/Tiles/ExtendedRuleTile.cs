using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// Based on video https://youtu.be/FwOxLkJTXag
[CreateAssetMenu]
public class ExtendedRuleTile : RuleTile<ExtendedRuleTile.Neighbor>
{
  public bool checkThisInAny = true;
  public bool checkSpecifiedInNotThis = true;
  public bool alwaysConnect = true;

  public TileBase[] tilesToConnect;

  public class Neighbor : RuleTile.TilingRule.Neighbor
  {
    // public const int This = 1;
    // public const int NotThis = 2;
    public const int Any = 3;
    public const int Specified = 4;
    public const int Empty = 5;
  }

  public override bool RuleMatch(int neighbor, TileBase tile)
  {
    switch (neighbor)
    {
      case Neighbor.This: return CheckThis(tile);
      case Neighbor.NotThis: return CheckNotThis(tile);
      case Neighbor.Any: return CheckAny(tile);
      case Neighbor.Specified: return CheckSpecified(tile);
      case Neighbor.Empty: return CheckEmpty(tile);
    }
    return base.RuleMatch(neighbor, tile);
  }

  private bool CheckThis(TileBase tile) => tile == this || alwaysConnect && tilesToConnect.Contains(tile);

  private bool CheckNotThis(TileBase tile) => tile != this && (checkSpecifiedInNotThis == false || tilesToConnect.Contains(tile) == false);

  private bool CheckAny(TileBase tile) => checkThisInAny ? tile != null : tile != null && tile != this;

  private bool CheckSpecified(TileBase tile) => tilesToConnect.Contains(tile);

  private bool CheckEmpty(TileBase tile) => tile == null;
}