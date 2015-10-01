// <copyright file="SniperController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

namespace Domain.ArtificialIntelligence.Sniper
{
/// <summary>
/// Controls input and updates the status of the sniper.
/// </summary>
  public class SniperController : MonoBehaviour
  {
    [Tooltip("Speed at which the laser moves")]
    public float
      RotationSpeed;
    [Tooltip("Areas for sniper laser to patrol")]
    public GameObject[]
      ActivationColliders;
    [Tooltip("Time it takes once tracked for sniper to fire.")]
    public float
      ShootDelay;
    [Tooltip("Sound of sniper fire")]
    public AudioSource
      SniperFireSound;
    [Tooltip("Damage of the sniper")]
    public float
      Damage;
    [Tooltip("The slow factor applied to the player when shot")]
    public float
      SpeedPenalty;
    [Tooltip("Set of three colors that define the threat level of the sniper")]
    public Vector3[]
      ColorSet;
  
    private Sniper sniper;

    /// <summary>
    /// Gets reference to sniper object.
    /// </summary>
    /// <value>Sniper object.</value>
    public Sniper SniperObj
    {
      get
      {
        return sniper;
      }
    }
  
    internal void Awake()
    {  
      sniper = new Sniper(RotationSpeed, ActivationColliders, this.gameObject, ShootDelay, SpeedPenalty, Damage, SniperFireSound, ColorSet);
    }

    /// <summary>
    /// Update sniper state.
    /// </summary>
    internal void Update()
    {
      sniper.UpdateState();
    }
  }
}