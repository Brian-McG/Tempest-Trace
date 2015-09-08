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
  private EnemySighting enemySighting;
  private LastPlayerSighting lastPlayerSighting;
  private float chaseTimer;
  private Vector3 currentTarget;
  private float stoppingDistance;
  private float chaseWaitTime;

  public Drone(float movementSpeed,
               float rotationSpeed,
               float riseSpeed,
               GameObject drone,
               GameObject[] patrolRoute,
               EnemySighting enemySighting,
               float stoppingDistance,
               float chaseWaitTime,
               LastPlayerSighting lastPlayerSighting)
        : base(movementSpeed,
               rotationSpeed,
               drone)
  {
    index = 0;
    chaseTimer = 0.0f;
    this.drone = drone;
    this.patrolRoute = patrolRoute;
    this.riseSpeed = riseSpeed;
    this.movementSpeed = movementSpeed;
    this.enemySighting = enemySighting;
    this.stoppingDistance = stoppingDistance;
    this.chaseWaitTime = chaseWaitTime;
    this.lastPlayerSighting = lastPlayerSighting;
    Debug.Log(this.lastPlayerSighting);
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

  public void Chase()
  {
    Vector3 sightingDeltaPos = enemySighting.PersonalLastSighting - drone.transform.position;

    if (sightingDeltaPos.sqrMagnitude > 4f)
    {
      currentTarget = enemySighting.PersonalLastSighting;
    }
    float remainingDistance = (currentTarget - drone.transform.position).magnitude;
    if (stoppingDistance < remainingDistance)
    {
      chaseTimer += Time.deltaTime;
      if (chaseTimer >= chaseWaitTime)
      {
        lastPlayerSighting.Position = lastPlayerSighting.ResetPosition;
        enemySighting.PersonalLastSighting = lastPlayerSighting.ResetPosition;
        chaseTimer = 0.0f;
      }
    }
  }

  public void Shoot()
  {
    // TODO: Add shoot
    Debug.Log("Shoot: Pew Pew!");
  }

  public void Rise()
  {
    drone.transform.position = drone.transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
  }
}
