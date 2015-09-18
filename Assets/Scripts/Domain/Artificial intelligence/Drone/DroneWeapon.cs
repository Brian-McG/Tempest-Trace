// <copyright file="DroneWeapon.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class DroneWeapon : BaseWeapon
{
  private GameObject droneFireSound;
  private float muzzleFlashInterval;
  private GameObject[] muzzleFlash;
  private Animator droneAnimator;

  public DroneWeapon(float damage,
                     float shootRate,
                     PlayerHealth playerOneHealth,
                     PlayerHealth playerTwoHealth,
                     GameObject droneFireSound,
                     HitFlash hitFlash,
                     GameObject[] muzzleFlash,
                     Animator droneAnimator) : base(damage, shootRate, hitFlash, playerOneHealth, playerTwoHealth)
  {
    this.droneFireSound = droneFireSound;
    this.muzzleFlashInterval = 0.0f;
    this.muzzleFlash = muzzleFlash;
    this.droneAnimator = droneAnimator;
  }

  public override bool Fire(Vector3 position, Vector3 direction, float range, int ignoreLayer)
  {
    bool returnValue = false;
    CurrentShootInverval += Time.deltaTime;
    muzzleFlashInterval += Time.deltaTime;

    // Turn off muzzle flash light
    if (muzzleFlashInterval > 0.05f)
    {
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = false;
      }
    }

    // Turn off shoot animation state if shoot animation is complete
    if (!droneAnimator.GetCurrentAnimatorStateInfo(1).IsName("Shoot"))
    {
      droneAnimator.SetBool("Shooting", false);
    }

    if (CurrentShootInverval > ShootRate)
    {
      Debug.DrawLine(position, position + (direction * range), Color.red, 2.0f, false);
      CurrentShootInverval -= ShootRate;
      droneAnimator.SetBool("Shooting", true);
      
      // Shoot sound
      GameObject.Instantiate(droneFireSound, position, Quaternion.identity);
      
      // Start muzzle flash effect
      foreach (GameObject muzzle in muzzleFlash)
      {
        muzzle.light.enabled = true;
        muzzle.particleSystem.Play();
        muzzleFlashInterval = 0.0f;
      }

      // Raycast in bullet direction
      RaycastHit hit;
      if (Physics.Raycast(position, direction, out hit, 100.0f, ignoreLayer))
      {
        if (hit.collider.gameObject.tag == "PlayerOne")
        {
          HitFlash.FlashCamera(1);
          if (PlayerOneHealth.DeductHP(Damage))
          {
            returnValue = true;
          }
        }
        else if (hit.collider.gameObject.tag == "PlayerTwo")
        {
          HitFlash.FlashCamera(2);
          if (PlayerTwoHealth.DeductHP(Damage))
          {
            returnValue = true;
          }
        }
      }
    }
    return returnValue;
  }
}
