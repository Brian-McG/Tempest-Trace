// <copyright file="Sniper.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages state and behaviour of the sniper.
/// </summary>
public class Sniper : DirectableObject
{
  private GameObject sniper;
  private BoxCollider[] colliders;
  private LineRenderer lineRenderer;
  private Vector3 currentTarget;
  private Vector3 previousDirection;
  private int ignoredColliders;
  private byte targetedPlayer;
  private GameObject playerOne;
  private GameObject playerTwo;
  private Vector3 heightOffset;
  private Vector3 transformScale;
  private SniperWeapon sniperWeapon;
  
  public Sniper(float rotationSpeed, GameObject[] activationColliders, GameObject sniper, float shootDelay, float speedPenalty, float damage, AudioSource sniperFire)
    : base(rotationSpeed, sniper)
  {
    this.sniper = sniper;
    this.playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
    this.playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
    PlayerHealth playerOneHealth = playerOne.GetComponent<PlayerHealth>();
    PlayerHealth playerTwoHealth = playerTwo.GetComponent<PlayerHealth>();
    PlayerSlow playerOneSlow = playerOne.GetComponent<PlayerSlow>();
    PlayerSlow playerTwoSlow = playerTwo.GetComponent<PlayerSlow>();
    HitFlash hitFlash = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HitFlash>();
    colliders = new BoxCollider[activationColliders.Length];
    for (int i = 0; i < activationColliders.Length; ++i)
    {
      colliders[i] = activationColliders[i].GetComponent<BoxCollider>();
    }

    Random.seed = System.Environment.TickCount;
    lineRenderer = sniper.GetComponent<LineRenderer>();
    currentTarget = Vector3.zero;
    previousDirection = new Vector3(1000f, 1000f, 1000f);
    ignoredColliders = ~(1 << LayerMask.NameToLayer("SniperCollider") | 1 << LayerMask.NameToLayer("Checkpoint"));
    targetedPlayer = 0;
    heightOffset = new Vector3(0, playerOne.GetComponent<CharacterController>().center.y, 0);
    transformScale = new Vector3(1.0f / sniper.transform.lossyScale.x,
                                         1.0f / sniper.transform.lossyScale.y,
                                         1.0f / sniper.transform.lossyScale.z);

    sniperWeapon = new SniperWeapon(damage, shootDelay, playerOneHealth, playerTwoHealth, playerOneSlow, playerTwoSlow, sniperFire, hitFlash, speedPenalty);
    if (colliders.Length > 0)
    {
      GenerateRandomTarget();
    }
  }

  /// <summary>
  /// Returns a reference to the sniper game object within unity.
  /// </summary>
  /// <value>The sniper game object.</value>
  public GameObject SniperGameObj
  {
    get
    {
      return sniper;
    }
  }

  /// <summary>
  /// Gets the colliders which are ignored by the sniper
  /// </summary>
  /// <value>The ignored colliders.</value>
  public int IgnoredColliders
  {
    get
    {
      return ignoredColliders;
    }
  }

  /// <summary>
  /// Gets or sets the player targeted.
  /// </summary>
  /// <value>The targeted player.</value>
  public byte TargetedPlayer
  {
    get
    {
      return targetedPlayer;
    }
    
    set
    {
      targetedPlayer = value;
    }
  }

  /// <summary>
  /// Sets the state of the sniper and executes its behaviour
  /// </summary>
  public void UpdateState()
  {
    if (colliders.Length > 0)
    {
      UpdatePlayerTarget();
      Vector3 direction = currentTarget - sniper.transform.position;
      if (targetedPlayer != 0 && (direction.normalized - sniper.transform.forward.normalized).magnitude < 0.005f)
      {
        Shoot();
      }
      else if (targetedPlayer != 0)
      {
        Chase();
      }
      else
      {
        Patrol();
      }
    }
  }

  /// <summary>
  /// Patrol behaviour for the sniper.
  /// </summary>
  public void Patrol()
  {
    // Calculate a new position for laser to move towards if laser has reached its target.
    if (sniper.transform.localEulerAngles == previousDirection)
    {
      GenerateRandomTarget();
    }

    Vector3 direction = currentTarget - sniper.transform.position;
    UpdateLaser(direction);
    previousDirection = sniper.transform.localEulerAngles;
    FaceDirection(direction, 0.01f);
  }

  /// <summary>
  /// Chase bahaviour of the sniper.
  /// </summary>
  public void Chase()
  {
    UpdatePlayerTarget();
    Vector3 direction = currentTarget - sniper.transform.position;
    UpdateLaser(direction);
    previousDirection = currentTarget - sniper.transform.position;
    FaceDirection(direction);
  }

  /// <summary>
  /// Shoot bahaviour of the sniper.
  /// </summary>
  public void Shoot()
  {
    UpdatePlayerTarget();
    sniper.transform.LookAt(currentTarget);
    UpdateLaser(currentTarget - sniper.transform.position);
    sniperWeapon.Fire(sniper.transform.position, sniper.transform.forward, 1000.0f, ignoredColliders);
  }

  /// <summary>
  /// // Set length of laser as long as distance between sniper and the point it collides with.
  /// </summary>
  /// <param name="direction">Direction towards target.</param>
  private void UpdateLaser(Vector3 direction)
  {
    RaycastHit hit;
    if (Physics.Raycast(sniper.transform.position, sniper.transform.forward, out hit, 1000f, ignoredColliders))
    {
      Vector3 currentDirection = hit.transform.position - sniper.transform.position;
      lineRenderer.SetPosition(1, new Vector3(0, 0, currentDirection.magnitude * transformScale.z));
    }
    else
    {
      lineRenderer.SetPosition(1, new Vector3(0, 0, direction.magnitude * transformScale.z));
    }
  }

  /// <summary>
  /// Sets a random position within the sniper colliders for laser to track towards.
  /// The y value of the random target is always the minimum y value of the collider.
  /// </summary>
  private void GenerateRandomTarget()
  {
    int randomIndex = Random.Range(0, colliders.Length);
    float randomX = Random.Range(colliders[randomIndex].bounds.min.x, colliders[randomIndex].bounds.max.x);
    float minY = colliders[0].bounds.min.y;
    float randomZ = Random.Range(colliders[randomIndex].bounds.min.z, colliders[randomIndex].bounds.max.z);
    currentTarget = new Vector3(randomX, minY, randomZ);
  }

  /// <summary>
  /// Update current target based on targeted player.
  /// </summary>
  private void UpdatePlayerTarget()
  {
    if (targetedPlayer == 1)
    {
      currentTarget = playerOne.transform.position + heightOffset;
    }
    else if (targetedPlayer == 2)
    {
      currentTarget = playerTwo.transform.position + heightOffset;
    }
  }
}
