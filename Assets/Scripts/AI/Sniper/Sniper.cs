// <copyright file="Sniper.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Sniper : DirectableObject
{
  private float rotationSpeed;
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
  private float shootDelay;
  private float speedPenalty;
  private float currentShootDelay;
  private float damage;
  private PlayerHealth playerOneHealth;
  private PlayerHealth playerTwoHealth;
  
  public Sniper(float rotationSpeed, GameObject[] activationColliders, GameObject sniper, float shootDelay, float speedPenalty, float damage)
    : base(rotationSpeed, sniper)
  {
    this.rotationSpeed = rotationSpeed;
    this.sniper = sniper;
    this.playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
    this.playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
    this.playerOneHealth = playerOne.GetComponent<PlayerHealth>();
    this.playerTwoHealth = playerTwo.GetComponent<PlayerHealth>();
    this.shootDelay = shootDelay;
    this.speedPenalty = speedPenalty;
    this.damage = damage;
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
    heightOffset = new Vector3(0, playerOne.GetComponent<CapsuleCollider>().center.y, 0);
    currentShootDelay = 0.0f;

    GenerateRandomTarget();
  }

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
  
  public void UpdateState()
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

  public void Patrol()
  {
    if (sniper.transform.localEulerAngles == previousDirection)
    {
      GenerateRandomTarget();
    }

    Vector3 direction = currentTarget - sniper.transform.position;
    RaycastHit hit;
    if (Physics.Raycast(sniper.transform.position, sniper.transform.forward, out hit, 1000f, ignoredColliders))
    {
      // Debug.Log("HIT: " + hit.collider.name);
      Vector3 currentDirection = hit.transform.position - sniper.transform.position;
      lineRenderer.SetPosition(1, new Vector3(0, 0, currentDirection.magnitude));
    }
    else
    {
      lineRenderer.SetPosition(1, new Vector3(0, 0, direction.magnitude));
    }

    previousDirection = sniper.transform.localEulerAngles;
    FaceDirection(direction);
  }

  public void Chase()
  {
    UpdatePlayerTarget();
    Vector3 direction = currentTarget - sniper.transform.position;
    RaycastHit hit;
    if (Physics.Raycast(sniper.transform.position, sniper.transform.forward, out hit, 1000f, ignoredColliders))
    {
      Vector3 currentDirection = hit.transform.position - sniper.transform.position;
      lineRenderer.SetPosition(1, new Vector3(0, 0, currentDirection.magnitude));
    }
    else
    {
      lineRenderer.SetPosition(1, new Vector3(0, 0, direction.magnitude));
    }
    previousDirection = currentTarget - sniper.transform.position;
    FaceDirection(direction);
  }

  public void Shoot()
  {
    UpdatePlayerTarget();
    sniper.transform.LookAt(currentTarget);
    RaycastHit hit;
    if (Physics.Raycast(sniper.transform.position, sniper.transform.forward, out hit, 1000f, ignoredColliders))
    {
      Vector3 currentDirection = hit.transform.position - sniper.transform.position;
      lineRenderer.SetPosition(1, new Vector3(0, 0, currentDirection.magnitude));
    }
    else
    {
      Vector3 direction = currentTarget - sniper.transform.position;
      lineRenderer.SetPosition(1, new Vector3(0, 0, direction.magnitude));
    }
    currentShootDelay += Time.deltaTime;
    if (currentShootDelay > shootDelay && Physics.Raycast(sniper.transform.position, sniper.transform.forward, out hit, 1000f, ignoredColliders))
    {
      if (hit.collider.tag == "PlayerOne")
      {
        Debug.Log("Player one shot");
        playerOneHealth.DeductHP(damage);
      }
      else if (hit.collider.tag == "PlayerTwo")
      {
        Debug.Log("Player two shot");
        playerTwoHealth.DeductHP(damage);
      }
      currentShootDelay = 0.0f;
    }
  }

  private void GenerateRandomTarget()
  {
    int randomIndex = Random.Range(0, colliders.Length);
    float randomX = Random.Range(colliders[randomIndex].bounds.min.x, colliders[randomIndex].bounds.max.x);
    float minY = colliders[0].bounds.min.y;
    float randomZ = Random.Range(colliders[randomIndex].bounds.min.z, colliders[randomIndex].bounds.max.z);
    currentTarget = new Vector3(randomX, minY, randomZ);
  }

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
