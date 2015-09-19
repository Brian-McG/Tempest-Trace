// <copyright file="SniperTrigger.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

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

    internal void Start()
    {  
      sniper = SniperControl.SniperObj;
      currentDelay = 0.0f;
      heightOffset = new Vector3(0, GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<CharacterController>().center.y, 0);
    }

    /// <summary>
    /// Set targeted player
    /// </summary>
    /// <param name="other">Object collided with.</param>
    internal void OnTriggerStay(Collider other)
    {
      if (sniper != null)
      {
        if (other.tag == "PlayerOne" || other.tag == "PlayerTwo")
        {
          sniper.TargetedPlayer = 0;
          currentDelay += Time.deltaTime;
          if (sniper.TargetedPlayer == 0 && other.tag == "PlayerOne" && currentDelay > ReactionDelay)
          {
            RaycastHit hit;
            Vector3 direction = (other.transform.position + heightOffset) - sniper.SniperGameObj.transform.position;
            if (Physics.Raycast(sniper.SniperGameObj.transform.position, direction, out hit, 1000f, sniper.InverseIgnoredColliders) && hit.collider.tag == "PlayerOne")
            {
              sniper.TargetedPlayer = 1;
            }
          }
          else if (sniper.TargetedPlayer == 0 && other.tag == "PlayerTwo" && currentDelay > ReactionDelay)
          {
            RaycastHit hit;
            Vector3 direction = (other.transform.position + heightOffset) - sniper.SniperGameObj.transform.position;
            if (Physics.Raycast(sniper.SniperGameObj.transform.position, direction, out hit, 1000f, sniper.InverseIgnoredColliders) && hit.collider.tag == "PlayerTwo")
            {
              sniper.TargetedPlayer = 2;
            }
          }
        }
      }
    }

    /// <summary>
    /// Reset targeted player when player exists collider.
    /// </summary>
    /// <param name="other">Object collided with.</param>
    internal void OnTriggerExit(Collider other)
    {
      if (other.tag == "PlayerOne" || other.tag == "PlayerTwo")
      {
        sniper.TargetedPlayer = 0;
        currentDelay = 0.0f;
      }
    }
  }
}