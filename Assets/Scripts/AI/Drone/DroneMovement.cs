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
  private float defaultFloatHeight;
  private Drone drone;
  private int inverseLayer;
  private EnemySighting enemySighting;
  private LastPlayerSighting lastPlayerSighting;

  internal void Awake()
  {  
    enemySighting = GetComponent<EnemySighting>();
    defaultFloatHeight = this.transform.position.y;
    inverseLayer = ~(1 << LayerMask.NameToLayer("Drone"));
    lastPlayerSighting = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LastPlayerSighting>();
    drone = new Drone(MoveSpeed, RotationSpeed, RiseSpeed, this.gameObject, Waypoints, enemySighting, StoppingDistance, ChaseWaitTime, lastPlayerSighting);
  }

  internal void Update()
  { 

    if (Physics.Raycast(this.transform.position, this.transform.forward, 10.0f, inverseLayer)
      || Physics.Raycast(this.transform.position, -this.transform.up, 2.0f, inverseLayer))
    {
      drone.Rise();
    }
    else if (enemySighting.TargetedPlayer != 0)
    {
      drone.Shoot();
    }
    else if (enemySighting.PreviousSighting != lastPlayerSighting.ResetPosition)
    {
      drone.Chase();
    }
    else
    {
      drone.Move();
    }
  }

  internal void OnCollisionStay(Collision collision)
  {
    drone.Rise();
  }
}
