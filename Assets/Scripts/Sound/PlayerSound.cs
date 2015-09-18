// <copyright file="PlayerSound.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls when player footstep sound is to be played.
/// </summary>
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
    PlayFootStepSound();
  }

  /// <summary>
  /// Plays footstep sound if: 
  ///   The sound is not playing
  ///   The player is on the ground
  ///   The player is not in a contextual action
  ///   The movement of the player is above a set threshold
  /// </summary>
  private void PlayFootStepSound()
  {
    if (!FootStepSound.isPlaying && firstPersonMovement.IsGrounded &&
        firstPersonMovement.CurrentMotion == DefinedMotion.NONE &&
        firstPersonMovement.HorizontalVelocity.sqrMagnitude > 0.5f)
    {
      FootStepSound.Play();
    }
  }
}
