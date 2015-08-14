// <copyright file="PlayerSlow.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class PlayerSlow : MonoBehaviour
{ 
  public GameObject Aircon;
  private Aircon aircon;
  private FirstPersonMovement firstPersonMovement;
  private float defaultRunSpeed;
  private float slowSpeed;

  internal void Start()
  {  
    firstPersonMovement = GetComponent<FirstPersonMovement>();
    defaultRunSpeed = firstPersonMovement.RunSpeed;
    slowSpeed = defaultRunSpeed / 3.0f;
    aircon = Aircon.GetComponent<Aircon>();
  }

  internal void Update()
  {
  }

  internal void OnTriggerEnter(Collider other)
  {
    if (aircon.Status && other.tag == "SteamArea")
    {
      firstPersonMovement.RunSpeed = slowSpeed;
    }
  }
  
  internal void OnTriggerExit(Collider other)
  {
    if (other.tag == "SteamArea")
    {
      firstPersonMovement.RunSpeed = defaultRunSpeed;
    }
  }
}
