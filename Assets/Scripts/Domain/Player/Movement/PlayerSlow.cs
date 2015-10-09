// <copyright file="PlayerSlow.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Interactables.Aircon;

namespace Domain.Player.Movement
{
/// <summary>
/// Controls the degree to which the player is slowed from interactions with the world.
/// </summary>
  public class PlayerSlow : MonoBehaviour
  { 
    [Header("Aircon Parameters")]
    [Tooltip("Reference to aircon object.")]
    public GameObject
      Aircon;
    [Tooltip("Time it takes from player to slow down when affected by the slow of the aircon.")]
    public float
      AirconLerpDownTime;
    [Tooltip("Time it takes from player to speed up when no longer affected by the slow of the aircon.")]
    public float
      AirconLerpUpTime;
    [Tooltip("Degree to which the player is slowed by the aircon.")]
    public float
      AirconSlowFactor = 3.0f;
    [Tooltip("Sound that plays when player is slowed by vent for the first time.")]
    public AudioSource
      AirconPainSound;


    private Aircon aircon;
    private FirstPersonMovement firstPersonMovement;
    private float defaultRunSpeed;
    private float slowSpeed;
    private float currentLerpTime;
    private bool isEnabled;
    private bool painPlayed;

    // Variables that are used to apply a general slow to a player (e.g. hit by sniper)
    private bool calledSlow;
    private bool calledSlowEnabled;
    private float currentDuration;
    private float generalDuration;
    private float generalLerpDownTime;
    private float generalLerpUpTime;
    private float generalSlowSpeed;

    /// <summary>
    /// Slows player down for some duration and to some degree.
    /// </summary>
    /// <param name="duration">How long until the player starts speeding up</param>
    /// <param name="lerpUpTime">The time it takes to speed up</param>
    /// <param name="lerpDownTime">The time it takes to slow down</param>
    /// <param name="slowFactor">Degree to which the player is slowed</param>
    public void ApplyGeneralSlow(float duration, float lerpUpTime, float lerpDownTime, float slowFactor = 3.0f)
    {
      calledSlow = true;
      calledSlowEnabled = true;
      generalDuration = duration;
      generalLerpUpTime = lerpUpTime;
      generalLerpDownTime = lerpDownTime;
      currentDuration = 0.0f;
      generalSlowSpeed = defaultRunSpeed / slowFactor;
      currentLerpTime = Mathf.Clamp(generalLerpDownTime * (defaultRunSpeed - firstPersonMovement.RunSpeed) / (defaultRunSpeed - generalSlowSpeed), 0, generalLerpDownTime);
    }

    internal void Awake()
    {  
      firstPersonMovement = GetComponent<FirstPersonMovement>();
      defaultRunSpeed = firstPersonMovement.DefaultRunSpeed;
      slowSpeed = defaultRunSpeed / AirconSlowFactor;
      isEnabled = false;
      calledSlow = false;
      calledSlowEnabled = false;
      painPlayed = false;
      aircon = Aircon.GetComponent<Aircon>();
    }

    internal void Update()
    {
      VentSlow();
      CalledSlow();
    }

    /// <summary>
    /// Start slowing player upon entering the area filled with steam.
    /// </summary>
    /// <param name="other">Object collided with</param>
    internal void OnTriggerEnter(Collider other)
    {
      if (aircon.Status && other.tag == "SteamArea")
      {
        currentLerpTime = Mathf.Clamp(AirconLerpDownTime * (defaultRunSpeed - firstPersonMovement.RunSpeed) / (defaultRunSpeed - slowSpeed), 0, AirconLerpDownTime);
        isEnabled = true;
        if (!painPlayed)
        {
          AirconPainSound.Play();
          painPlayed = true;
        }
      }
    }

    /// <summary>
    /// Start speeding player up upon leaving the area filled with steam.
    /// </summary>
    /// <param name="other">Object collided with</param>
    internal void OnTriggerExit(Collider other)
    {
      if (other.tag == "SteamArea")
      {
        currentLerpTime = Mathf.Clamp(AirconLerpUpTime * (defaultRunSpeed - firstPersonMovement.RunSpeed) / (defaultRunSpeed - slowSpeed), 0, AirconLerpUpTime);
        isEnabled = false;
      }
    }

    /// <summary>
    /// Calculate and set run speed for player based on aircon paramaters.
    /// </summary>
    private void VentSlow()
    {

      if (isEnabled && !calledSlow && AirconLerpDownTime - currentLerpTime > 0.01f)
      {
        currentLerpTime += Time.deltaTime;
        float percentage = currentLerpTime / AirconLerpDownTime;
        firstPersonMovement.RunSpeed = Mathf.Lerp(defaultRunSpeed, slowSpeed, percentage);
      }
      else if (!isEnabled && !calledSlow && AirconLerpUpTime - currentLerpTime < (AirconLerpUpTime - 0.01f))
      {
        currentLerpTime -= Time.deltaTime;
        float percentage = currentLerpTime / AirconLerpUpTime;
        firstPersonMovement.RunSpeed = Mathf.Lerp(defaultRunSpeed, slowSpeed, percentage);
      }
    }

    /// <summary>
    /// Calculate and set run speed for player based on the paramters provided to ApplyGeneralSlow
    /// </summary>
    private void CalledSlow()
    {
      if (firstPersonMovement.CurrentMotion != DefinedMotion.SLIDE)
      {
        if (calledSlow && calledSlowEnabled)
        {
          currentLerpTime += Time.deltaTime;
          float percentage = currentLerpTime / generalLerpDownTime;
          firstPersonMovement.RunSpeed = Mathf.Lerp(defaultRunSpeed, generalSlowSpeed, percentage);
          if (generalLerpDownTime - currentLerpTime < 0.01f)
          {
            currentDuration += Time.deltaTime;
            if (currentDuration > generalDuration)
            {
              currentLerpTime = generalLerpUpTime;
              calledSlowEnabled = false;
            }
          }
        }
        else if (calledSlow && !calledSlowEnabled && generalLerpUpTime - currentLerpTime < (generalLerpUpTime - 0.01f))
        {
          currentLerpTime -= Time.deltaTime;
          float percentage = currentLerpTime / generalLerpUpTime;
          firstPersonMovement.RunSpeed = Mathf.Lerp(defaultRunSpeed, generalSlowSpeed, percentage);
          if (generalLerpUpTime - currentLerpTime > (generalLerpUpTime - 0.01f))
          {
            calledSlow = false;
            firstPersonMovement.RunSpeed = defaultRunSpeed;
          }
        }
      }
    }
  }
}