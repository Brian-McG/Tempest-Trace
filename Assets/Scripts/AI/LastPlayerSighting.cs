﻿// <copyright file="LastPlayerSighting.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Maintains the position that the player was last spotted.
/// </summary>
public class LastPlayerSighting
{
  public Vector3 Position = new Vector3(1000f, 1000f, 1000f);
  private Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);

  public Vector3 ResetPosition
  {
    get
    {
      return resetPosition;
    }
  }
}
