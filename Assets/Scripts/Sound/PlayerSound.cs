// <copyright file="PlayerSound.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
  public AudioSource FootStepSound;
  private FirstPersonMovement firstPersonMovement;

  internal void Start()
  {  
    firstPersonMovement = GetComponent<FirstPersonMovement>();
  }

  internal void Update()
  {
    if (!FootStepSound.isPlaying && firstPersonMovement.IsGrounded &&
        firstPersonMovement.CurrentMotion == DefinedMotion.NONE &&
        firstPersonMovement.HorizontalVelocity.sqrMagnitude > 0.5f)
    {
      FootStepSound.Play();
    }
  }
}
