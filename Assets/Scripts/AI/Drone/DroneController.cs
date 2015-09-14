﻿// <copyright file="DroneMovement.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class DroneController : MonoBehaviour
{
  public float MoveSpeed;
  public float RotationSpeed;
  public float RiseSpeed;
  public GameObject[] Waypoints;
  public float StoppingDistance;
  public float ChaseWaitTime;
  public float ShotsPerSecond;
  public GameObject[] MuzzleFlash;
  public float Damage;
  private float defaultFloatHeight;
  private Drone drone;
  private EnemySighting enemySighting;
  private LastPlayerSighting lastPlayerSighting;

  // Ensure that this remains Start to maintain correct ordering of script startup.
  internal void Start()
  {  
    enemySighting = GetComponent<EnemySighting>();
    defaultFloatHeight = this.transform.position.y;
    lastPlayerSighting = enemySighting.LastSighting;
    drone = new Drone(MoveSpeed, RotationSpeed, RiseSpeed, this.gameObject, Waypoints, enemySighting, StoppingDistance, ChaseWaitTime, lastPlayerSighting, defaultFloatHeight, ShotsPerSecond, MuzzleFlash, Damage);
  }

  internal void Update()
  { 
    drone.UpdateState();
  }

  internal void OnCollisionStay(Collision collision)
  {
    if (collision.gameObject.tag != "Drone")
    {
      drone.Rise();
    }
  }
}