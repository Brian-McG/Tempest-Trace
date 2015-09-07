// <copyright file="Checkpoint.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
  private Vector3 position;
  private Vector3 orientation;
  private Hashtable checkpoints;

  public Vector3 Position
  {
    get
    {
      return position;
    }
  }

  public Vector3 Orientation
  {
    get
    {
      return orientation;
    }
  }

  internal void Awake()
  {
    position = transform.position;
    orientation = transform.localEulerAngles;
    checkpoints = new Hashtable();
  }

  internal void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Checkpoint" &&
      !checkpoints.ContainsKey(other.transform.position))
    {
      CheckpointReached(other);
    }
  }

  private void CheckpointReached(Collider checkpoint)
  {
    if (!checkpoints.ContainsKey(checkpoint.transform.position))
    {
      position = checkpoint.transform.position;
      orientation = checkpoint.transform.localEulerAngles;
      checkpoints.Add(Position, null);
    }
  }
}
