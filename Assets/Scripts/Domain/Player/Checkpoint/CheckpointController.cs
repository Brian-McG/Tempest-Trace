// <copyright file="Checkpoint.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

namespace Domain.Player.Checkpoint
{

/// <summary>
/// Controls checkpoint input
/// </summary>
  public class CheckpointController : MonoBehaviour
  {
    private Vector3 position;
    private Vector3 orientation;
    private Hashtable checkpoints;
    private uint count;

    /// <summary>
    /// Gets number of checkpoints passed.
    /// </summary>
    public uint NumberPassed
    {
      get
      {
        return count;
      }
    }

    /// <summary>
    /// Gets position of the checkpoint
    /// </summary>
    public Vector3 Position
    {
      get
      {
        return position;
      }
    }

    /// <summary>
    /// Gets orientation of the checkpoint
    /// </summary>
    public Vector3 Orientation
    {
      get
      {
        return orientation;
      }
    }

    internal void Awake()
    {
      count = 0;
      position = transform.position;
      orientation = transform.localEulerAngles;
      checkpoints = new Hashtable();
    }

    /// <summary>
    /// Player collides with checkpoint and has not collided with this checkpoint in the past.
    /// </summary>
    /// <param name="other">Object collided with</param>
    internal void OnTriggerEnter(Collider other)
    {
      if (other.tag == "Checkpoint" &&
        !checkpoints.ContainsKey(other.transform.position))
      {
        CheckpointReached(other);
      }
    }

    /// <summary>
    /// Increment checkpoint counter and set next checkpoint.
    /// </summary>
    /// <param name="checkpoint">Checkpoint collided with</param>
    private void CheckpointReached(Collider checkpoint)
    {
      if (!checkpoints.ContainsKey(checkpoint.transform.position))
      {
        position = checkpoint.transform.position;
        orientation = checkpoint.transform.localEulerAngles;
        checkpoints.Add(Position, null);
        ++count;
      }
    }
  }
}