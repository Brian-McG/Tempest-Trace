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
  private float movementSpeed;
  private EnemySighting enemySighting;
  private LastPlayerSighting lastPlayerSighting;
  private float chaseTimer;
  private Vector3 currentTarget;
  private float stoppingDistance;
  private float chaseWaitTime;
  private int inverseLayer;
  private int inverseShootLayer;
  private float trackTime;
  private GameObject playerOne;
  private GameObject playerTwo;
  private FirstPersonMovement playerOneMovement;
  private FirstPersonMovement playerTwoMovement;
  private Animator droneAnimator;
  private float shootRate;
  private float currentShootInverval;
  private GameObject[] muzzleFlash;
  private float flashInterval;
  private float droneDamage;
  private PlayerHealth playerOneHealth;
  private PlayerHealth playerTwoHealth;

  public Drone(float movementSpeed,
               float rotationSpeed,
               GameObject drone,
               GameObject[] patrolRoute,
               EnemySighting enemySighting,
               float stoppingDistance,
               float chaseWaitTime,
               LastPlayerSighting lastPlayerSighting,
               float shootRate,
               GameObject[] muzzleFlash,
               float droneDamage)
        : base(movementSpeed,
               rotationSpeed,
               drone)
  {
    index = 0;
    chaseTimer = 0.0f;
    this.drone = drone;
    this.patrolRoute = patrolRoute;
    this.movementSpeed = movementSpeed;
    this.enemySighting = enemySighting;
    this.stoppingDistance = stoppingDistance;
    this.chaseWaitTime = chaseWaitTime;
    this.lastPlayerSighting = lastPlayerSighting;
    inverseLayer = ~(1 << LayerMask.NameToLayer("Drone"));
    inverseShootLayer = ~(1 << LayerMask.NameToLayer("Drone") | 1 << LayerMask.NameToLayer("SmokeBomb"));
    Random.seed = System.Environment.TickCount;
    trackTime = 0.0f;
    playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
    playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
    playerOneMovement = playerOne.GetComponent<FirstPersonMovement>();
    playerTwoMovement = playerTwo.GetComponent<FirstPersonMovement>();
    droneAnimator = drone.GetComponent<Animator>();
    this.shootRate = 1.0f / shootRate;
    currentShootInverval = 0.0f;
    this.muzzleFlash = muzzleFlash;
    flashInterval = 0.0f;
    this.droneDamage = droneDamage;
    playerOneHealth = playerOne.GetComponent<PlayerHealth>();
    playerTwoHealth = playerTwo.GetComponent<PlayerHealth>();
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
      if (enemySighting.TargetedPlayer != 0)
      {
        trackTime += Time.deltaTime;
      }
      else
      {
        trackTime = 0.0f;
      }
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

  public void Shoot(Vector3 deltaTargetDrone)
  {
    trackTime += Time.deltaTime;
    currentTarget = enemySighting.PersonalLastSighting;
    currentShootInverval += Time.deltaTime;
    flashInterval += Time.deltaTime;
    if (flashInterval > 0.05f)
    {
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = false;
      }
    }

    if (!droneAnimator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
    {
      droneAnimator.SetBool("Shooting", false);
    }

    if (currentShootInverval > shootRate)
    {
      currentShootInverval -= shootRate;
      droneAnimator.SetBool("Shooting", true);
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = true;
        muzzle.particleSystem.Play();
        flashInterval = 0.0f;
      }

      float offsetMagnitude = CalculateShootOffset(deltaTargetDrone.magnitude);
      float offsetHalf = offsetMagnitude / 2.0f;
      float offsetX = (Random.value * offsetMagnitude) - offsetHalf;
      float offsetY = (Random.value * offsetMagnitude) - offsetHalf;
      float offsetZ = (Random.value * offsetMagnitude) - offsetHalf;
      Vector3 bulletTarget = new Vector3(currentTarget.x + offsetX, currentTarget.y + offsetY, currentTarget.z + offsetZ);
      Vector3 direction = bulletTarget - drone.transform.position;
      Debug.DrawLine(drone.transform.position, drone.transform.position + (direction * 100.0f), Color.red, 2.0f, false);
      RaycastHit hit;
      if (Physics.Raycast(drone.transform.position, direction, out hit, 100.0f, inverseShootLayer))
      {
        if (hit.collider.gameObject.tag == "PlayerOne")
        {
          if (playerOneHealth.DeductHP(droneDamage))
          {
            trackTime = 0.0f;
          }
        }
        else if (hit.collider.gameObject.tag == "PlayerTwo")
        {
          if (playerTwoHealth.DeductHP(droneDamage))
          {
            trackTime = 0.0f;
          }
        }
      }
    }
  }

  public void Rise()
  {
    drone.transform.position = drone.transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
  }

  public void UpdateState()
  {
    Vector3 difference = currentTarget - drone.transform.position;
    bool isShooting = false;
    Vector3 horizontalForward = drone.transform.forward;
    horizontalForward.y = 0;
    horizontalForward = horizontalForward.normalized;
    if (Physics.Raycast(drone.transform.position, horizontalForward, 10.0f, inverseLayer)
      || Physics.Raycast(drone.transform.position, Vector3.down, 2.0f, inverseLayer))
    {
      // Debug.Log("Rise");
      Rise();
    }
    else if (enemySighting.TargetedPlayer != 0 && (difference.normalized - drone.transform.forward.normalized).magnitude < 0.1 /* && (currentTarget - drone.transform.position).magnitude < ((collider.radius * drone.transform.localScale.x) / 2.0f)*/)
    {
      isShooting = true;
      Shoot(difference);
    }
    else if (enemySighting.PreviousSighting != lastPlayerSighting.ResetPosition)
    {
      // Debug.Log("Chase Mode");
      Chase();
    }
    else
    {
      // Debug.Log("Patrol");
      Patrol();
    }

    if (!isShooting)
    {
      droneAnimator.SetBool("Shooting", false);
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = false;
      }
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
    Debug.Log(5.0f * Mathf.Pow(0.5f, timeTracked));
    return 5.0f * Mathf.Pow(0.5f, timeTracked);
  }

  private float PlayerMoveSpeedShootOffset(float moveSpeed)
  {
    return 0.05f * moveSpeed;
  }

  private float DistanceToPlayerOffset(float distance)
  {
    return 0.05f * distance;
  }
}
