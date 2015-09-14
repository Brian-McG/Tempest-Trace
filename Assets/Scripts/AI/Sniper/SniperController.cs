// <copyright file="SniperMovement.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class SniperController : MonoBehaviour
{
  public float RotationSpeed;
  public GameObject[] ActivationColliders;
  private Sniper sniper;
  
  internal void Awake()
  {  
    sniper = new Sniper(RotationSpeed, ActivationColliders, this.gameObject);
  }
  
  internal void Update()
  {
    sniper.UpdateState();
  }
}
