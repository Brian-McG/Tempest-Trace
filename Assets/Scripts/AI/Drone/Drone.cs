// <copyright file="Drone.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages state and behaviour of the drone.
/// </summary>
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
  private HitFlash hitFlash;
  private GameObject droneFireSound;

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
               float droneDamage,
               GameObject droneFireSound)
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
    this.droneFireSound = droneFireSound;
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
    hitFlash = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HitFlash>();
  }

  /// <summary>
  /// Patrol behavior for the drone.
  /// </summary>
  public void Patrol()
  {
    Vector3 currentWaypoint = patrolRoute[index].transform.position;
    Vector3 difference = currentWaypoint - drone.transform.position;

    // Move towards next waypoint
    if (difference.magnitude < 5f)
    {
      index = (index + 1) % patrolRoute.Length;
      currentWaypoint = patrolRoute[index].transform.position;
      difference = currentWaypoint - drone.transform.position;
    }

    FaceDirection(difference);
    Move(difference);
  }

  /// <summary>
  /// Behaviour to chase after player
  /// </summary>
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
        // Reset drone state
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
        // Update time player is tracked
        trackTime += Time.deltaTime;
      }
      else
      {
        // Reset track time as player is no longer spotted.
        trackTime = 0.0f;
      }
      chaseTimer = 0.0f;
    }

    Vector3 difference = currentTarget - drone.transform.position;
    Vector3 differenceLevel = new Vector3(difference.x, 0, difference.z).normalized;

    // If player is targeted then directly face the player
    // Else, stay upright and move to last seen position
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

  /// <summary>
  /// Behaviour for the drone to shoot the tracked player.
  /// </summary>
  /// <param name="playerDirection">Vector towards the tracked player.</param>
  public void Shoot(Vector3 playerDirection)
  {
    trackTime += Time.deltaTime;
    currentTarget = enemySighting.PersonalLastSighting;
    currentShootInverval += Time.deltaTime;
    flashInterval += Time.deltaTime;

    // Turn off muzzle flash light
    if (flashInterval > 0.05f)
    {
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = false;
      }
    }

    // Turn off shoot animation state if shoot animation is complete
    if (!droneAnimator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
    {
      droneAnimator.SetBool("Shooting", false);
    }

    if (currentShootInverval > shootRate)
    {
      currentShootInverval -= shootRate;
      droneAnimator.SetBool("Shooting", true);

      // Shoot sound
      GameObject.Instantiate(droneFireSound, drone.transform.position, Quaternion.identity);

      // Start muzzle flash effect
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = true;
        muzzle.particleSystem.Play();
        flashInterval = 0.0f;
      }

      // Calculate bullet direction
      float offsetMagnitude = CalculateShootOffset(playerDirection.magnitude);
      float offsetHalf = offsetMagnitude / 2.0f;
      float offsetX = (Random.value * offsetMagnitude) - offsetHalf;
      float offsetY = (Random.value * offsetMagnitude) - offsetHalf;
      float offsetZ = (Random.value * offsetMagnitude) - offsetHalf;
      Vector3 bulletTarget = new Vector3(currentTarget.x + offsetX, currentTarget.y + offsetY, currentTarget.z + offsetZ);
      Vector3 direction = bulletTarget - drone.transform.position;
      Debug.DrawLine(drone.transform.position, drone.transform.position + (direction * 100.0f), Color.red, 2.0f, false);

      // Raycast in bullet direction
      RaycastHit hit;
      if (Physics.Raycast(drone.transform.position, direction, out hit, 100.0f, inverseShootLayer))
      {
        if (hit.collider.gameObject.tag == "PlayerOne")
        {
          hitFlash.FlashCamera(1);
          if (playerOneHealth.DeductHP(droneDamage))
          {
            trackTime = 0.0f;
          }
        }
        else if (hit.collider.gameObject.tag == "PlayerTwo")
        {
          hitFlash.FlashCamera(2);
          if (playerTwoHealth.DeductHP(droneDamage))
          {
            trackTime = 0.0f;
          }
        }
      }
    }
  }

  /// <summary>
  /// Drone moves up vertically
  /// </summary>
  public void Rise()
  {
    drone.transform.position = drone.transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
  }

  /// <summary>
  /// Sets the state of the drone and runs that behaviour
  /// </summary>
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
      Rise();
    }
    else if (enemySighting.TargetedPlayer != 0 && (difference.normalized - drone.transform.forward.normalized).magnitude < 0.1 /* && (currentTarget - drone.transform.position).magnitude < ((collider.radius * drone.transform.localScale.x) / 2.0f)*/)
    {
      isShooting = true;
      Shoot(difference);
    }
    else if (enemySighting.PersonalLastSighting != lastPlayerSighting.ResetPosition)
    {
      Chase();
    }
    else
    {
      Patrol();
    }

    // Turn off muzzle flash if not in a shooting state
    if (!isShooting)
    {
      droneAnimator.SetBool("Shooting", false);
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = false;
      }
    }
  }

  /// <summary>
  /// Calculates the amount the direction of the bullet is offset.
  /// The offset is made up of three parts:
  ///   The time player has been tracked for
  ///   The movement speed of the tracked player
  ///   The distance from the player
  /// </summary>
  /// <returns>The shoot offset.</returns>
  /// <param name="distance">Distance to target.</param>
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
    // Debug.Log(timeTrackOffset + playerMoveSpeedShootOffset + distanceToPlayerOffset);
    return timeTrackOffset + playerMoveSpeedShootOffset + distanceToPlayerOffset;
  }

  /// <summary>
  /// Offset is calculated using an exponential decay function based on timed tracked.
  /// </summary>
  /// <returns>An offset.</returns>
  /// <param name="timeTracked">Time tracked.</param>
  private float TimeTrackedShootOffset(float timeTracked)
  {
    return 1.0f * Mathf.Pow(0.5f, timeTracked);
  }

  /// <summary>
  /// Offset is calculated using a linear function based on movement speed of player.
  /// </summary>
  /// <returns>An offset.</returns>
  /// <param name="moveSpeed">Move speed.</param>
  private float PlayerMoveSpeedShootOffset(float moveSpeed)
  {
    return 0.05f * moveSpeed;
  }

  /// <summary>
  /// Offset is calcualted using a linear function based on distance from drone to player.
  /// </summary>
  /// <returns>An offset</returns>
  /// <param name="distance">Distance.</param>
  private float DistanceToPlayerOffset(float distance)
  {
    return 0.05f * distance;
  }
}
