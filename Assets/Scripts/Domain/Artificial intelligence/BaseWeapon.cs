// <copyright file="BaseWeapon.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public abstract class BaseWeapon
{
  private float damage;
  private float shootRate;
  private float currentShootInverval;

  public BaseWeapon(float damage, float shootRate)
  {  
    this.damage = damage;
    this.shootRate = shootRate;
    this.currentShootInverval = 0.0f;
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
  /*
    if (currentShootInverval > shootRate)
    {
      RaycastHit hit;
      if (Physics.Raycast(position, direction, out hit, range, ignoreLayer))
      {
        if (hit.collider.tag == "PlayerOne")
        {
          return 1;
        }
        else if (hit.collider.tag == "PlayerTwo")
        {
          return 2;
        }
      }
    }
    return 0;
*/
}
