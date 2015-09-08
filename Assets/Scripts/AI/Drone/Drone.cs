// <copyright file="Drone.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Drone : MoveableObject
{
  private int index;
  private GameObject drone;
  private GameObject[] patrolRoute;
  private float riseSpeed;
  private float movementSpeed;

  public Drone(float movementSpeed,
               float rotationSpeed,
               float riseSpeed,
               GameObject drone,
               GameObject[] patrolRoute)
        : base(movementSpeed,
               rotationSpeed,
               drone)
  {
    index = 0;
    this.drone = drone;
    this.patrolRoute = patrolRoute;
    this.riseSpeed = riseSpeed;
    this.movementSpeed = movementSpeed;
  }

  public void Move()
  {
    Vector3 currentTarget = patrolRoute[index].transform.position;
    Vector3 difference = currentTarget - drone.transform.position;
    if (difference.magnitude < 5f)
    {
      index = (index + 1) % patrolRoute.Length;
      currentTarget = patrolRoute[index].transform.position;
      difference = currentTarget - drone.transform.position;
    }

    FaceDirection(difference);
    Move(difference.normalized);
  }

  public void Rise()
  {
    drone.rigidbody.AddForce(Vector3.up * movementSpeed);
  }
}
