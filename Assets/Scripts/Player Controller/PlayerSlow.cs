// <copyright file="PlayerSlow.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class PlayerSlow : MonoBehaviour
{ 
  [Header("Aircon Parameters")]
  public GameObject
    Aircon;

  public float AirconLerpDownTime;
  public float AirconLerpUpTime;
  public float AirconSlowFactor = 3.0f;
  private Aircon aircon;
  private FirstPersonMovement firstPersonMovement;
  private float defaultRunSpeed;
  private float slowSpeed;
  private float currentLerpTime;
  private bool isEnabled;

  // Variables that are used to apply a general slow to a player (e.g. hit by sniper)
  private bool calledSlow;
  private bool calledSlowEnabled;
  private float currentDuration;
  private float generalDuration;
  private float generalLerpDownTime;
  private float generalLerpUpTime;
  private float generalSlowSpeed;

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
    ;
  }

  internal void Awake()
  {  
    firstPersonMovement = GetComponent<FirstPersonMovement>();
    defaultRunSpeed = firstPersonMovement.DefaultRunSpeed;
    slowSpeed = defaultRunSpeed / AirconSlowFactor;
    isEnabled = false;
    calledSlow = false;
    calledSlowEnabled = false;
    aircon = Aircon.GetComponent<Aircon>();
  }

  internal void Update()
  {
    VentSlow();
    CalledSlow();
  }

  internal void OnTriggerEnter(Collider other)
  {
    if (aircon.Status && other.tag == "SteamArea")
    {
      currentLerpTime = Mathf.Clamp(AirconLerpDownTime * (defaultRunSpeed - firstPersonMovement.RunSpeed) / (defaultRunSpeed - slowSpeed), 0, AirconLerpDownTime);
      isEnabled = true;
    }
  }
  
  internal void OnTriggerExit(Collider other)
  {
    if (other.tag == "SteamArea")
    {
      currentLerpTime = Mathf.Clamp(AirconLerpUpTime * (defaultRunSpeed - firstPersonMovement.RunSpeed) / (defaultRunSpeed - slowSpeed), 0, AirconLerpUpTime);
      isEnabled = false;
    }
  }

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

  private void CalledSlow()
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
