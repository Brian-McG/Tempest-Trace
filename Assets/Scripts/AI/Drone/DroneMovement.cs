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
  private float defaultFloatHeight;
  private Drone drone;
  private int inverseLayer;

  internal void Start()
  {  
    drone = new Drone(MoveSpeed, RotationSpeed, RiseSpeed, this.gameObject, Waypoints);
    defaultFloatHeight = this.transform.position.y;
    inverseLayer = ~(1 << LayerMask.NameToLayer("Drone"));
  }

  internal void Update()
  { 

    if (Physics.Raycast(this.transform.position, this.transform.forward, 10.0f, inverseLayer)
      || Physics.Raycast(this.transform.position, -this.transform.up, 2.0f, inverseLayer))
    {
      drone.Rise();
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
