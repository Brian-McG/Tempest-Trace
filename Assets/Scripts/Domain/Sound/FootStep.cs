// <copyright file="FootStep.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Player.Movement;
namespace Domain.Sound
{
  public class FootStep : MonoBehaviour
  {
    public AudioSource FootStepSound;
    private FirstPersonMovement playerOneMovement;
    private float defaultVolume;
    private float defaultPitch;
    private float updateTimer;
    private float updateTimerLimit = 0.5f;
    internal void Start()
    {  
      updateTimer = 0;
      playerOneMovement = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<FirstPersonMovement>();
      defaultVolume = FootStepSound.volume;
      defaultPitch = FootStepSound.pitch;
    }

    internal void Update()
    {
      if (FootStepSound.isPlaying)
      {
        updateTimer += Time.deltaTime;
        if (updateTimer > updateTimerLimit)
        {
          if (Random.value < 0.5f)
          {
            FootStepSound.volume = defaultVolume + Random.Range(-0.08f, 0.08f);
          }
          else
          {
            FootStepSound.pitch = defaultPitch + Random.Range(-0.2f, 0.2f);
          }
        }
      }
    }
  }
}