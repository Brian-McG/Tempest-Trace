// <copyright file="SniperTrigger.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class SniperTrigger : MonoBehaviour
{
  [Tooltip("The number of seconds it takes for the player to become tracked once inside the collider")]
  public float
    ReactionDelay;
  public SniperController SniperControl;
  private Sniper sniper;
  private float currentDelay;

  internal void Start()
  {  
    sniper = SniperControl.SniperObj;
    currentDelay = 0.0f;
  }

  internal void OnTriggerStay(Collider other)
  {
    if (sniper != null)
    {
      if (other.tag == "PlayerOne" || other.tag == "PlayerTwo")
      {
        currentDelay += Time.deltaTime;
        if (sniper.TargetedPlayer == 0 && other.tag == "PlayerOne" && currentDelay > ReactionDelay)
        {
          sniper.TargetedPlayer = 1;
        }
        else if (sniper.TargetedPlayer == 0 && other.tag == "PlayerTwo" && currentDelay > ReactionDelay)
        {
          sniper.TargetedPlayer = 2;
        }
      }
    }
  }

  internal void OnTriggerExit(Collider other)
  {
    sniper.TargetedPlayer = 0;
    currentDelay = 0.0f;
  }
}
