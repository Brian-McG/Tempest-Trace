// <copyright file="Door.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Opens door when activated.
/// </summary>
public class Door : MonoBehaviour
{
  [Tooltip("Rate at which door opens.")]
  public float
    OpenRate;
  [Tooltip("Total degrees that door must open.")]
  public float
    DegreesToOpen;
  private bool status;
  private bool played;
  private float totalOpened;

  /// <summary>
  /// Start opening door.
  /// </summary>
  public void Activate()
  {
    if (!status)
    {
      status = true;
    }
  }

  internal void Awake()
  {  
    status = false;
    played = false;
    totalOpened = 0.0f;
  }

  /// <summary>
  /// Update door
  /// </summary>
  internal void Update()
  {
    if (status && !played && totalOpened < DegreesToOpen)
    {
      OpenDoor();
    }
  }

  /// <summary>
  /// Opens the door some increment
  /// </summary>
  private void OpenDoor()
  {
    float deltaAngle = OpenRate * Time.deltaTime;
    totalOpened += deltaAngle;
    transform.Rotate(new Vector3(0, deltaAngle, 0));
  }
}
