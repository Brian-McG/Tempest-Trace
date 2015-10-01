// <copyright file="EnemySighting.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

namespace Domain.ArtificialIntelligence.Drone
{
/// <summary>
/// Controls sighting of drone.
/// </summary>
  public class EnemySighting : MonoBehaviour
  {
    private Vector3 personalLastSighting;
    private SphereCollider sphereCollider;
    private LastPlayerSighting lastPlayerSighting;
    private GameObject playerOne;
    private GameObject playerTwo;
    private Vector3 previousSighting;
    private byte targetedPlayer;
    private int inverseIgnoreLayer;
    private Vector3 heightOffset;

    /// <summary>
    /// Gets player that drone is targeting
    /// </summary>
    /// <value>
    /// 0 if no player is targeted
    /// 1 if player one is targeted
    /// 2 if player two is targeted
    /// </value>
    public byte TargetedPlayer
    {
      get
      {
        return targetedPlayer;
      }
    }

    /// <summary>
    /// Gets or sets last sighting of player by the drone.
    /// </summary>
    /// <value>Position of player</value>
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

    /// <summary>
    /// Gets Last player sighting
    /// </summary>
    /// <value>Reference to lastPlayerSighting object</value>
    public LastPlayerSighting LastSighting
    {
      get
      {
        return lastPlayerSighting;
      }
    }

    internal void Awake()
    {  
      sphereCollider = GetComponent<SphereCollider>();
      lastPlayerSighting = new LastPlayerSighting();
      playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
      playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
      personalLastSighting = lastPlayerSighting.ResetPosition;
      previousSighting = lastPlayerSighting.ResetPosition;
      targetedPlayer = 0;
      inverseIgnoreLayer = ~(1 << LayerMask.NameToLayer("Drone") | (1 << 17));
      heightOffset = new Vector3(0, playerOne.GetComponent<CharacterController>().center.y, 0);
    }

    /// <summary>
    /// Update sighting variables.
    /// </summary>
    internal void Update()
    {

      if (lastPlayerSighting.Position != previousSighting)
      {
        personalLastSighting = lastPlayerSighting.Position;
      }

      previousSighting = lastPlayerSighting.Position;
    }

    /// <summary>
    /// Set sighting position and player targeted.
    /// </summary>
    /// <param name="other">Object collided with</param>
    internal void SightCollider(Collider other)
    {
      if (other.gameObject.tag == "PlayerOne" && targetedPlayer != 2)
      {
        targetedPlayer = 0;
        Vector3 direction = (other.transform.position + heightOffset) - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, sphereCollider.radius * transform.localScale.x, inverseIgnoreLayer))
        {
          if (hit.collider.gameObject.tag == "PlayerOne")
          {
            targetedPlayer = 1;
            lastPlayerSighting.Position = playerOne.transform.position + heightOffset;

            // Debug.Log("Update Player Position");
          }
        }
      }
      else if (other.gameObject.tag == "PlayerTwo" && targetedPlayer != 1)
      {
        targetedPlayer = 0;
        Vector3 direction = (other.transform.position + heightOffset) - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, sphereCollider.radius * transform.localScale.x, inverseIgnoreLayer))
        {
          if (hit.collider.gameObject.tag == "PlayerTwo")
          {
            targetedPlayer = 2;
            lastPlayerSighting.Position = playerTwo.transform.position + heightOffset;
          }
        }
      }
    }

    internal void OnTriggerEnter(Collider other)
    {
        SightCollider(other);
    }

    internal void OnTriggerStay(Collider other)
    {
        SightCollider(other);
    }

    /// <summary>
    /// Set targeted player to not target any player.
    /// </summary>
    /// <param name="other">Object collided with.</param>
    internal void OnTriggerExit(Collider other)
    {
      if ((other.gameObject.tag == "PlayerOne" && targetedPlayer == 1) || (other.gameObject.tag == "PlayerTwo" && targetedPlayer == 2))
      {
        targetedPlayer = 0;
      }
    }
  }
}
