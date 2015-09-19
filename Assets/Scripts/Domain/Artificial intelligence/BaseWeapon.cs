// <copyright file="BaseWeapon.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.CameraEffects;
using Domain.Player.Health;

namespace Domain.ArtificialIntelligence
{
  public abstract class BaseWeapon
  {
    private float damage;
    private float shootRate;
    private float currentShootInverval;
    private HitFlash hitFlash;
    private PlayerHealth playerOneHealth;
    private PlayerHealth playerTwoHealth;

    public BaseWeapon(float damage, float shootRate, HitFlash hitFlash, PlayerHealth playerOneHealth, PlayerHealth playerTwoHealth)
    {  
      this.damage = damage;
      this.shootRate = shootRate;
      this.currentShootInverval = 0.0f;
      this.hitFlash = hitFlash;
      this.playerOneHealth = playerOneHealth;
      this.playerTwoHealth = playerTwoHealth;
    }

    public float Damage
    {
      get
      {
        return this.damage;
      }
    }

    public float ShootRate
    {
      get
      {
        return this.shootRate;
      }
    }

    public HitFlash HitFlash
    {
      get
      {
        return hitFlash;
      }
    }

    public PlayerHealth PlayerOneHealth
    {
      get
      {
        return playerOneHealth;
      }
    }

    public PlayerHealth PlayerTwoHealth
    {
      get
      {
        return playerTwoHealth;
      }
    }

    public float CurrentShootInverval
    {
      get
      {
        return this.currentShootInverval;
      }

      set
      {
        this.currentShootInverval = value;
      }
    }

/// <summary>
    /// Fires a bullet from a position in a given direction, if the bullet hits a player, the player is damaged
/// </summary>
/// <returns>True if target is killed</returns>
/// <param name="position">Position.</param>
/// <param name="direction">Direction.</param>
/// <param name="range">Range.</param>
/// <param name="ignoreLayer">Ignore layer.</param>
    public abstract bool Fire(Vector3 position, Vector3 direction, float range, int ignoreLayer);
  }
}