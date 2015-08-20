// <copyright file="PlayerSlow.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class PlayerSlow : MonoBehaviour
{ 
  public GameObject Aircon;
  public float LerpDownTime;
  public float LerpUpTime;
  public float SlowFactor = 3.0f;
  private Aircon aircon;
  private FirstPersonMovement firstPersonMovement;
  private float defaultRunSpeed;
  private float slowSpeed;
  private float currentLerpTime;
  private bool isEnabled;

  internal void Awake()
  {  
    firstPersonMovement = GetComponent<FirstPersonMovement>();
    defaultRunSpeed = firstPersonMovement.RunSpeed;
    slowSpeed = defaultRunSpeed / SlowFactor;
    aircon = Aircon.GetComponent<Aircon>();
  }

  internal void Update()
  {
    if (isEnabled && LerpDownTime - currentLerpTime > 0.01f)
    {
      currentLerpTime += Time.deltaTime;
      float percentage = currentLerpTime / LerpDownTime;
      firstPersonMovement.RunSpeed = Mathf.Lerp(defaultRunSpeed, slowSpeed, percentage);
    }
    else if (!isEnabled && LerpUpTime - currentLerpTime < (LerpUpTime - 0.01f))
    {
      currentLerpTime -= Time.deltaTime;
      float percentage = currentLerpTime / LerpUpTime;
      firstPersonMovement.RunSpeed = Mathf.Lerp(defaultRunSpeed, slowSpeed, percentage);
    }
  }

  internal void OnTriggerEnter(Collider other)
  {
    if (aircon.Status && other.tag == "SteamArea")
    {
      currentLerpTime = Mathf.Clamp(LerpDownTime * (defaultRunSpeed - firstPersonMovement.RunSpeed) / (defaultRunSpeed - slowSpeed), 0, LerpDownTime);
      isEnabled = true;
    }
  }
  
  internal void OnTriggerExit(Collider other)
  {
    if (other.tag == "SteamArea")
    {
      currentLerpTime = Mathf.Clamp(LerpUpTime * (defaultRunSpeed - firstPersonMovement.RunSpeed) / (defaultRunSpeed - slowSpeed), 0, LerpUpTime);
      isEnabled = false;
    }
  }
}
