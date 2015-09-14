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

  struct PatrolArea
  {

  }

  public Sniper(float rotationSpeed, GameObject[] activationColliders, GameObject sniper)
    :base(rotationSpeed, sniper)
  {
    this.rotationSpeed = rotationSpeed;
    this.sniper = sniper;
    colliders = new BoxCollider[activationColliders.Length];
    for (int i = 0; i < activationColliders.Length; ++i)
    {
      colliders[i] = activationColliders[i].GetComponent<BoxCollider>();
    }
    lineRenderer = sniper.GetComponent<LineRenderer>();
    currentTarget = Vector3.zero;
    previousDirection = new Vector3(1000f, 1000f, 1000f);
    ignoredColliders = ~(1 << LayerMask.NameToLayer("SniperCollider"));
    GenerateRandomTarget();
  }


  public void UpdateState()
  {
    Patrol();
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

  private void GenerateRandomTarget()
  {
    float randomX = Random.Range(colliders[0].bounds.min.x, colliders[0].bounds.max.x);
    float minY = colliders[0].bounds.min.y;
    float randomZ = Random.Range(colliders[0].bounds.min.z, colliders[0].bounds.max.z);
    currentTarget = new Vector3(randomX, minY, randomZ);
  }
}
