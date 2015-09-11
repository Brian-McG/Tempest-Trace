// <copyright file="Drone.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Drone : MoveableObject
{
  private const float ActionBufferAngle = 2.0f;
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
  private int inverseLayer;
  private SphereCollider collider;
  private float trackTime;
  private GameObject playerOne;
  private GameObject playerTwo;
  private FirstPersonMovement playerOneMovement;
  private FirstPersonMovement playerTwoMovement;

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
    Random.seed = System.Environment.TickCount;
    trackTime = 0.0f;
    playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
    playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
    playerOneMovement = playerOne.GetComponent<FirstPersonMovement>();
    playerTwoMovement = playerTwo.GetComponent<FirstPersonMovement>();
  }

  public void Patrol()
  {
    Vector3 currentWaypoint = patrolRoute[index].transform.position;
    Vector3 difference = currentWaypoint - drone.transform.position;
    if (difference.magnitude < 5f)
    {
      index = (index + 1) % patrolRoute.Length;
      currentWaypoint = patrolRoute[index].transform.position;
      difference = currentWaypoint - drone.transform.position;
    }

    FaceDirection(difference);
    Move(difference);
  }

  public void Chase()
  {
    Vector3 sightingDeltaPos = enemySighting.PersonalLastSighting - drone.transform.position;
    currentTarget = enemySighting.PersonalLastSighting;
    Vector3 moveTarget = new Vector3(currentTarget.x, 0, currentTarget.z);
    Vector3 droneLocation = new Vector3(drone.transform.position.x, 0, drone.transform.position.z);
    float remainingDistance = (moveTarget - droneLocation).magnitude;

    if (remainingDistance < stoppingDistance)
    {
      chaseTimer += Time.deltaTime;
      if (chaseTimer >= chaseWaitTime)
      {
        lastPlayerSighting.Position = lastPlayerSighting.ResetPosition;
        enemySighting.PersonalLastSighting = lastPlayerSighting.ResetPosition;
        chaseTimer = 0.0f;
        trackTime = 0.0f;
      }
    }
    else
    {
      trackTime += Time.deltaTime;
      chaseTimer = 0.0f;
    }

    // Debug.Log(chaseTimer);
    Vector3 difference = currentTarget - drone.transform.position;
    Vector3 differenceLevel = new Vector3(difference.x, 0, difference.z).normalized;
    if (enemySighting.TargetedPlayer != 0)
    {
      FaceDirection(difference);
    }
    else
    {
      FaceDirection(differenceLevel);
    }

    Move(differenceLevel);
  }

  public void Shoot()
  {
    // TODO: Add shoot
    trackTime += Time.deltaTime;
    currentTarget = enemySighting.PersonalLastSighting;
  }

  public void Rise()
  {
    drone.transform.position = drone.transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
  }

  public void UpdateState()
  {
    Vector3 difference = currentTarget - drone.transform.position;

    // Debug.Log((difference - drone.transform.forward.normalized).magnitude);
    Vector3 horizontalForward = drone.transform.forward;
    horizontalForward.y = 0;
    horizontalForward = horizontalForward.normalized;
    if (Physics.Raycast(drone.transform.position, horizontalForward, 10.0f, inverseLayer)
      || Physics.Raycast(drone.transform.position, Vector3.down, 2.0f, inverseLayer))
    {
      Debug.Log("Rise");
      Rise();
    }
    else if (enemySighting.TargetedPlayer != 0 && (difference.normalized - drone.transform.forward.normalized).magnitude < 0.1 /* && (currentTarget - drone.transform.position).magnitude < ((collider.radius * drone.transform.localScale.x) / 2.0f)*/)
    {
      float offsetMagnitude = CalculateShootOffset(difference.magnitude);

      // Debug.Log(offsetMagnitude);
      float offsetHalf = offsetMagnitude / 2.0f;
      float offsetX = (Random.value * offsetMagnitude) - offsetHalf;
      float offsetY = (Random.value * offsetMagnitude) - offsetHalf;
      float offsetZ = (Random.value * offsetMagnitude) - offsetHalf;
      Vector3 debugTarget = new Vector3(currentTarget.x + offsetX, currentTarget.y + offsetY, currentTarget.z + offsetZ);
      Debug.DrawLine(drone.transform.position, debugTarget, Color.red, 2.0f, false);
      Debug.Log("Shoot: Pew Pew!");
      Shoot();
    }
    else if (enemySighting.PreviousSighting != lastPlayerSighting.ResetPosition)
    {
      Debug.Log("Chase Mode");
      Chase();
    }
    else
    {
      Debug.Log("Patrol");
      Patrol();
    }
  }

  private float CalculateShootOffset(float distance)
  {
    float timeTrackOffset = TimeTrackedShootOffset(trackTime);
    float playerMoveSpeedShootOffset;
    if (enemySighting.TargetedPlayer == 1)
    {
      playerMoveSpeedShootOffset = PlayerMoveSpeedShootOffset(playerOneMovement.Velocity.magnitude);
    }
    else
    {
      playerMoveSpeedShootOffset = PlayerMoveSpeedShootOffset(playerTwoMovement.Velocity.magnitude);
    }

    float distanceToPlayerOffset = DistanceToPlayerOffset(distance);
    return timeTrackOffset + playerMoveSpeedShootOffset + distanceToPlayerOffset;
  }

  private float TimeTrackedShootOffset(float timeTracked)
  {
    return 5.0f * Mathf.Pow(0.5f, timeTracked);
  }

  private float PlayerMoveSpeedShootOffset(float moveSpeed)
  {
    return 0.1f * moveSpeed;
  }

  private float DistanceToPlayerOffset(float distance)
  {
    return 0.1f * distance;
  }
}
