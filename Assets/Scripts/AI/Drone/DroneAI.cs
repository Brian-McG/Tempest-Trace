// <copyright file="DroneAI.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class DroneAI : MonoBehaviour
{
  public float PatrolSpeed = 2.0f;
  public float ChaseSpeed = 5.0f;
  public float ChaseWaitTime = 5.0f;
  public float PatrolWaitTime = 1.0f;
  public Transform[] PatrolWayPoints;

  private NavMeshAgent nav;
  private float patrolTimer;
  private uint wayPointIndex;

  internal void Awake()
  {  
    nav = GetComponent<NavMeshAgent>();
    patrolTimer = 0.0f;
  }

  internal void Update()
  {
    Patrol();
  }

  private void Patrol()
  {
    nav.speed = PatrolSpeed;
    if (nav.remainingDistance < nav.stoppingDistance)
    {
      patrolTimer += Time.deltaTime;
      if (patrolTimer >= PatrolWaitTime)
      {
        if (wayPointIndex == PatrolWayPoints.Length - 1)
        {
          wayPointIndex = 0;
        }
        else
        {
          ++wayPointIndex;
        }
        patrolTimer = 0;
      }
    }
    nav.destination = PatrolWayPoints[wayPointIndex].position;
  }
}
