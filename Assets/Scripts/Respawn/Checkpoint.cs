// <copyright file="Checkpoint.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
  public Vector3 Position;
  public Vector3 Orientation;
  private Hashtable checkpoints;

  internal void Awake()
  {
    Position = transform.position;
    Orientation = transform.localEulerAngles;
    checkpoints = new Hashtable();
  }

  internal void OnTriggerEnter(Collider other)
  {
    if ((tag == "PlayerOne" && other.tag == "PlayerOneCheckpoint") || (tag == "PlayerTwo" && other.tag == "PlayerTwoCheckpoint"))
    {
      if (!checkpoints.ContainsKey(other.transform.position))
      {
        // TODO: Reset player rotation to straight forward
        Position = other.transform.position;
        checkpoints.Add(Position, null);
      }
    }
  }
}
