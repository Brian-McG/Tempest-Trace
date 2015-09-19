// <copyright file="LastPlayerSighting.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

namespace Domain.ArtificialIntelligence.Drone
{
/// <summary>
/// Maintains the position that the player was last spotted.
/// </summary>
  public class LastPlayerSighting
  {
    private Vector3 position = new Vector3(1000f, 1000f, 1000f);
    private Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);

    public Vector3 Position
    {
      get
      {
        return position;
      }

      set
      {
        position = value;
      }
    }

    public Vector3 ResetPosition
    {
      get
      {
        return resetPosition;
      }
    }
  }
}