// <copyright file="DroneFireController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class DroneFireController : MonoBehaviour
{
  private AudioSource droneFireSound;

  internal void Awake()
  {
    droneFireSound = GetComponent<AudioSource>();
  }

  internal void Update()
  {
    if (!droneFireSound.isPlaying)
    {
      Destroy(this.gameObject);
    }
  }
}
