// <copyright file="DroneMovement.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class DroneMovement : MonoBehaviour
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

  internal void Awake()
  {  
    enemySighting = GetComponent<EnemySighting>();
    defaultFloatHeight = this.transform.position.y;
    lastPlayerSighting = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LastPlayerSighting>();
    drone = new Drone(MoveSpeed, RotationSpeed, RiseSpeed, this.gameObject, Waypoints, enemySighting, StoppingDistance, ChaseWaitTime, lastPlayerSighting, defaultFloatHeight, ShotsPerSecond, MuzzleFlash, Damage);
  }

  internal void Update()
  { 
    drone.UpdateState();
  }

  internal void OnCollisionStay(Collision collision)
  {
    drone.Rise();
  }
}
