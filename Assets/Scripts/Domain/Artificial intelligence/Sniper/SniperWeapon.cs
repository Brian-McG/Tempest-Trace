﻿// <copyright file="SniperWeapon.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class SniperWeapon : BaseWeapon
{
  private PlayerHealth playerOneHealth;
  private PlayerHealth playerTwoHealth;
  private PlayerSlow playerOneSlow;
  private PlayerSlow playerTwoSlow;
  private AudioSource sniperFireSound;
  private HitFlash hitFlash;
  private float speedPenalty;

  public SniperWeapon(float damage,
                      float shootRate,
                      PlayerHealth playerOneHealth,
                      PlayerHealth playerTwoHealth,
                      PlayerSlow playerOneSlow,
                      PlayerSlow playerTwoSlow,
                      AudioSource sniperFireSound,
                      HitFlash hitFlash,
                      float speedPenalty) : base(damage, shootRate)
  {  
    this.playerOneHealth = playerOneHealth;
    this.playerTwoHealth = playerTwoHealth;
    this.playerOneSlow = playerOneSlow;
    this.playerTwoSlow = playerTwoSlow;
    this.sniperFireSound = sniperFireSound;
    this.hitFlash = hitFlash;
    this.speedPenalty = speedPenalty;
  }

  public override bool Fire(Vector3 position, Vector3 direction, float range, int ignoreLayer)
  {
    bool returnValue = false;
    CurrentShootInverval += Time.deltaTime;
    RaycastHit hit;
    // Fire towards player
    if (CurrentShootInverval > ShootRate && Physics.Raycast(position, direction, out hit, range, ignoreLayer))
    {
      sniperFireSound.Play();
      if (hit.collider.tag == "PlayerOne")
      {
        Debug.Log("Player one shot");
        if (playerOneHealth.DeductHP(Damage))
        {
          returnValue = true;
        }
        playerOneSlow.ApplyGeneralSlow(0.0f, 8.0f, 0.1f, speedPenalty);
        hitFlash.FlashCamera(1);

      }
      else if (hit.collider.tag == "PlayerTwo")
      {
        Debug.Log("Player two shot");
        if (playerTwoHealth.DeductHP(Damage))
        {
          returnValue = true;
        }
        playerTwoSlow.ApplyGeneralSlow(0.0f, 8.0f, 0.1f, speedPenalty);
        hitFlash.FlashCamera(2);
      }
      
      CurrentShootInverval -= ShootRate;
    }
    return returnValue;
  }
}
