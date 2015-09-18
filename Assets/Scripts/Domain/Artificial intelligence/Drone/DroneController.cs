// <copyright file="DroneController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls input and updates the status of the drone.
/// </summary>
public class DroneController : MonoBehaviour
{
  [Tooltip("Speed drone moves within the world.")]
  public float
    MoveSpeed;
  [Tooltip("Speed drone rotates within the world.")]
  public float
    RotationSpeed;
  [Tooltip("Points the drone moves between when in patrol state.")]
  public GameObject[]
    Waypoints;
  [Tooltip("Distance that drone stops from destination.")]
  public float
    StoppingDistance;
  [Tooltip("Time drone stays in chase state after losing sight of player.")]
  public float
    ChaseWaitTime;
  [Tooltip("Number times per second that drone fires.")]
  public float
    ShotsPerSecond;
  [Tooltip("Objects containing muzzle flash effects.")]
  public GameObject[]
    MuzzleFlash;
  [Tooltip("Damage of each shot that hits player.")]
  public float
    Damage;
  [Tooltip("Sound that drone makes when it fires at player.")]
  public GameObject
    DroneFireSound;

  private Drone drone;
  private EnemySighting enemySighting;
  private LastPlayerSighting lastPlayerSighting;

  // Ensure that this remains Start to maintain correct ordering of script startup.
  internal void Start()
  {  
    enemySighting = GetComponent<EnemySighting>();
    lastPlayerSighting = enemySighting.LastSighting;
    drone = new Drone(MoveSpeed, RotationSpeed, this.gameObject, Waypoints, enemySighting, StoppingDistance, ChaseWaitTime, lastPlayerSighting, ShotsPerSecond, MuzzleFlash, Damage, DroneFireSound);
  }

  /// <summary>
  /// Update drone state
  /// </summary>
  internal void Update()
  { 
    drone.UpdateState();
  }

  /// <summary>
  /// Raise drone on collision
  /// </summary>
  /// <param name="collision">Object collided into</param>
  internal void OnCollisionStay(Collision collision)
  {
    if (collision.gameObject.tag != "Drone")
    {
      drone.Rise();
    }
  }
}
