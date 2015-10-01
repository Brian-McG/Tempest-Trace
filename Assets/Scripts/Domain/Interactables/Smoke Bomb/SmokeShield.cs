// <copyright file="SmokeShield.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class SmokeShield : MonoBehaviour
{
  [Tooltip("The time at which the shield is removed")]
  public float
    Duration;
  private float currentDuration;
  internal void Awake()
  {  
    currentDuration = 0.0f;
  }

  internal void Update()
  {
    currentDuration += Time.deltaTime;
    if (currentDuration > Duration)
    {
      Destroy(this.gameObject);
    }
  }
}
