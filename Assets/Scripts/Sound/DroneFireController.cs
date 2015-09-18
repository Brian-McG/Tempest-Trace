// <copyright file="DroneFireController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the fire sound of the drone.
/// </summary>
public class DroneFireController : MonoBehaviour
{
  private AudioSource droneFireSound;

  internal void Awake()
  {
    droneFireSound = GetComponent<AudioSource>();
  }

  internal void Update()
  {
    DestroyWhenNotPlaying();
  }

  /// <summary>
  /// Destroys the drone fire sound object when the sound is completed playing.
  /// </summary>
  private void DestroyWhenNotPlaying()
  {
    if (!droneFireSound.isPlaying)
    {
      Destroy(this.gameObject);
    }
  }
}
