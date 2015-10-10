// <copyright file="SniperTrigger.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Player.Health;

namespace Domain.ArtificialIntelligence.Sniper
{
/// <summary>
/// Triggers sniper to target player.
/// </summary>
  public class SniperTrigger : MonoBehaviour
  {
    [Tooltip("The number of seconds it takes for the player to become tracked once inside the collider")]
    public float
      ReactionDelay;

    [Tooltip("The controller that manages a specific sniper")]
    public SniperController
      SniperControl;

    private Sniper sniper;
    private float currentDelay;
    private Vector3 heightOffset;
    private GameObject playerOne;
    private GameObject playerTwo;

    internal void Start()
    {  
      sniper = SniperControl.SniperObj;
      currentDelay = 0.0f;
      playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
      heightOffset = new Vector3(0, playerOne.GetComponent<CharacterController>().center.y, 0);
      playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
    }

    /// <summary>
    /// Set targeted player
    /// </summary>
    /// <param name="other">Object collided with.</param>
    internal void SightCollider(Collider other)
    {
      if (sniper != null)
      {
        if (other.tag == "PlayerOne" || other.tag == "PlayerTwo")
        {
          sniper.TargetedPlayer = 0;
          currentDelay += Time.deltaTime;
          if (sniper.TargetedPlayer == 0 && other.tag == "PlayerOne" && currentDelay > ReactionDelay && !other.GetComponent<PlayerLifeHandler>().Dead)
          {
            RaycastHit hit;
            Vector3 direction = (other.transform.position + HeightOffset(playerOne)) - sniper.SniperGameObj.transform.position;
            if (Physics.Raycast(sniper.SniperGameObj.transform.position, direction, out hit, 1000f, sniper.InverseIgnoredColliders) && hit.collider.tag == "PlayerOne")
            {
              sniper.TargetedPlayer = 1;
            }
          }
          else if (sniper.TargetedPlayer == 0 && other.tag == "PlayerTwo" && currentDelay > ReactionDelay && !other.GetComponent<PlayerLifeHandler>().Dead)
          {
            RaycastHit hit;
            Vector3 direction = (other.transform.position + HeightOffset(playerTwo)) - sniper.SniperGameObj.transform.position;
            if (Physics.Raycast(sniper.SniperGameObj.transform.position, direction, out hit, 1000f, sniper.InverseIgnoredColliders) && hit.collider.tag == "PlayerTwo")
            {
              sniper.TargetedPlayer = 2;
            }
          }
        }
      }
    }

    internal void OnTriggerStay(Collider other)
    {
      SightCollider(other);
    }

    /// <summary>
    /// Reset targeted player when player exists collider.
    /// </summary>
    /// <param name="other">Object collided with.</param>
    internal void OnTriggerExit(Collider other)
    {
      if ((other.tag == "PlayerOne" && sniper.TargetedPlayer == 1) || (other.tag == "PlayerTwo" && sniper.TargetedPlayer == 2))
      {
        sniper.TargetedPlayer = 0;
        currentDelay = 0.0f;
      }
    }

    private Vector3 HeightOffset(GameObject player)
    {
      return new Vector3(0, heightOffset.y * player.transform.localScale.y, 0);
    }
  }
}
