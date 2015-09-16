// <copyright file="SniperController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class SniperController : MonoBehaviour
{
  public float RotationSpeed;
  public GameObject[] ActivationColliders;
  public float ShootDelay;
  [Tooltip("The slow factor applied to the player when shot")]
  public float
    SpeedPenalty;

  public float Damage;
  private Sniper sniper;

  public Sniper SniperObj
  {
    get
    {
      return sniper;
    }
  }
  
  internal void Awake()
  {  
    sniper = new Sniper(RotationSpeed, ActivationColliders, this.gameObject, ShootDelay, SpeedPenalty, Damage);
  }
  
  internal void Update()
  {
    sniper.UpdateState();
  }
}
