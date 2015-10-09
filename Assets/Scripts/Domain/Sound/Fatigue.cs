// <copyright file="Fatigue.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Player.Movement;

namespace Domain.Sound
{
  public class Fatigue : MonoBehaviour
  {
    public AudioSource FatigueSound;
    private FirstPersonMovement playerOneMovement;
    private float deafaultVolume;
    private float updateTimer;
    private float updateTimerLimit;

    internal void Awake()
    {
      playerOneMovement = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<FirstPersonMovement>();
      deafaultVolume = FatigueSound.volume;
      updateTimer = 0;
      updateTimerLimit = 1.1f;
    }

    internal void Update()
    {
      if (FatigueSound.isPlaying)
      {
        if (updateTimer > updateTimerLimit)
        {
          FatigueSound.volume = deafaultVolume + Random.Range(-0.1f, 0.1f);
          updateTimer = 0;
        }
        else
        {
          updateTimer += Time.deltaTime;
        }
      }
      if (!FatigueSound.isPlaying && playerOneMovement.HorizontalVelocity.magnitude > 0.4f)
      {
        FatigueSound.Play();
      }
      else if (FatigueSound.isPlaying && playerOneMovement.HorizontalVelocity.magnitude < 0.4f)
      {
        FatigueSound.Pause();
      }
    }
  }
}
