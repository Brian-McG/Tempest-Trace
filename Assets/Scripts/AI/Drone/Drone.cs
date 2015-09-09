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
  private float defaultFloatHeight;
  private const float actionBufferAngle = 2.0f;
  private int inverseLayer;
  private SphereCollider collider;

  public Drone(float movementSpeed,
               float rotationSpeed,
               float riseSpeed,
               GameObject drone,
               GameObject[] patrolRoute,
               EnemySighting enemySighting,
               float stoppingDistance,
               float chaseWaitTime,
               LastPlayerSighting lastPlayerSighting,
               float defaultFloatHeight)
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
    this.defaultFloatHeight = defaultFloatHeight;
    inverseLayer = ~(1 << LayerMask.NameToLayer("Drone"));
    this.collider = drone.GetComponent<SphereCollider>();
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
    Move(difference);
  }

  public void Chase()
  {
    Debug.Log("Chase Mode");
    Vector3 sightingDeltaPos = enemySighting.PersonalLastSighting - drone.transform.position;
    Debug.Log("SqrMag = " + sightingDeltaPos.sqrMagnitude);
    currentTarget = enemySighting.PersonalLastSighting;
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
    Vector3 difference = currentTarget - drone.transform.position;
    Vector3 differenceLevel = new Vector3(difference.x, 0.0f, difference.z);
    FaceDirection(differenceLevel);
    Move(differenceLevel);
  }

  public void Shoot()
  {
    // TODO: Add shoot
    currentTarget = enemySighting.PersonalLastSighting;
    Debug.Log("Shoot: Pew Pew!");
  }

  public void Rise()
  {
    drone.transform.position = drone.transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
  }

  public void UpdateState()
  {
    Vector3 difference = currentTarget - drone.transform.position;
    Vector3 differenceLevel = new Vector3(difference.x, 0.0f, difference.z);
    float angle = AngleBetween(new Vector3(drone.transform.forward.x, 0.0f, drone.transform.forward.z),
                               new Vector3(differenceLevel.x, 0.0f, differenceLevel.z),
                               Vector3.up);
    if (Physics.Raycast(drone.transform.position, drone.transform.forward, 10.0f, inverseLayer)
      || Physics.Raycast(drone.transform.position, -drone.transform.up, 2.0f, inverseLayer))
    {
      Rise();
    }
    else if (enemySighting.TargetedPlayer != 0 && angle < actionBufferAngle /* && (currentTarget - drone.transform.position).magnitude < ((collider.radius * drone.transform.localScale.x) / 2.0f)*/)
    {
      Shoot();
    }
    else if (enemySighting.PreviousSighting != lastPlayerSighting.ResetPosition)
    {
      Chase();
    }
    else
    {
      Move();
    }
  }
}
