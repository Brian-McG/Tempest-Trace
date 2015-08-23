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
  public GameObject[] Waypoints;
  private Drone drone;

  internal void Start()
  {  
    drone = new Drone(MoveSpeed, RotationSpeed, this.gameObject, Waypoints);
  }

  internal void Update()
  {
    drone.Move();
  }
}
