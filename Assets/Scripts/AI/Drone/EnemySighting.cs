﻿// <copyright file="EnemySighting.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class EnemySighting : MonoBehaviour
{
  public float FieldOfViewAngle = 110f;
  private Vector3 personalLastSighting;
  private SphereCollider collider;
  private LastPlayerSighting lastPlayerSighting;
  private GameObject playerOne;
  private GameObject playerTwo;
  private Vector3 previousSighting;
  private byte targetedPlayer;
  private int inverseLayer;
  private Vector3 heightOffset;

  public byte TargetedPlayer
  {
    get
    {
      return targetedPlayer;
    }
  }

  public Vector3 PreviousSighting
  {
    get
    {
      return previousSighting;
    }
  }

  public Vector3 PersonalLastSighting
  {
    get
    {
      return personalLastSighting;
    }

    set
    {
      personalLastSighting = value;
    }
  }

  internal void Awake()
  {  
    collider = GetComponent<SphereCollider>();
    lastPlayerSighting = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LastPlayerSighting>();
    playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
    playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
    personalLastSighting = lastPlayerSighting.ResetPosition;
    previousSighting = lastPlayerSighting.ResetPosition;
    targetedPlayer = 0;
    inverseLayer = ~(1 << LayerMask.NameToLayer("Drone"));
    heightOffset = new Vector3(0, playerOne.GetComponent<CapsuleCollider>().center.y, 0);
  }

  internal void Update()
  {
    if (lastPlayerSighting.Position != previousSighting)
    {
      personalLastSighting = lastPlayerSighting.Position;
    }

    previousSighting = lastPlayerSighting.Position;
  }

  internal void OnTriggerStay(Collider other)
  {
    if (other.gameObject.tag == "PlayerOne" && targetedPlayer != 2)
    {
      targetedPlayer = 0;
      Vector3 direction = (other.transform.position + heightOffset) - transform.position;
      RaycastHit hit;
      if (Physics.Raycast(transform.position, direction.normalized, out hit, collider.radius * transform.localScale.x, inverseLayer))
      {
        // Debug.Log(hit.collider.name);
        if (hit.collider.gameObject.tag == "PlayerOne")
        {
          targetedPlayer = 1;
          lastPlayerSighting.Position = playerOne.transform.position + heightOffset;

          // Debug.Log("Update Player Position");
        }
      }
    }

    // Disabled until player one is completed
    /*
    else if (other.gameObject.tag == "PlayerTwo" && targetedPlayer != 1)
    {
      targetedPlayer = 0;
      Vector3 direction = other.transform.position - transform.position;
      float angle = Vector3.Angle(direction, transform.forward);
      Debug.Log(angle + " : " + FieldOfViewAngle * 0.5f);
      if (angle < FieldOfViewAngle * 0.5f)
      {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, collider.radius, inverseLayer))
        {
          if (hit.collider.gameObject.tag == "PlayerTwo")
          {
            targetedPlayer = 2;
            lastPlayerSighting.Position = playerTwo.transform.position;
          }
        }
      }
    }
    */
  }

  internal void OnTriggerExit(Collider other)
  {
    targetedPlayer = 0;
  }
}
