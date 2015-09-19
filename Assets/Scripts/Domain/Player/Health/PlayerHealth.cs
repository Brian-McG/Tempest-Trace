// <copyright file="PlayerHealth.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Domain.Player.Health
{
/// <summary>
/// Manages health of the player
/// </summary>
  public class PlayerHealth : MonoBehaviour
  {
    [Tooltip("The amount of health the player starts with")]
    public float
      DefaultHP;
    [Tooltip("The amount of time it takes the player to start regenerating health after taking damage")]
    public float
      TimeToStartRegeneratingAfterDamaged;
    [Tooltip("The rate at which health is restored to the player")]
    public float
      RegenerationRate;

    private float hp;
    private PlayerLifeHandler playerLifeHandler;
    private float regenTimer;

    public float CurrentHealth
    {
      get
      {
        return hp;
      }
    }

    /// <summary>
    /// Reset health and regeneration timer to default
    /// </summary>
    public void ResetHP()
    {
      hp = DefaultHP;
      regenTimer = TimeToStartRegeneratingAfterDamaged;
    }

    /// <summary>
    /// Deducts a given amount of health from the player.
    /// If the health equals or is below zero then the player is killed
    /// </summary>
    /// <returns><c>true</c>If player is killed from the action<c>false</c>Otherwise</returns>
    /// <param name="deduction">Amount of health to deduct</param>
    public bool DeductHP(float deduction)
    {
      hp -= deduction;
      regenTimer = 0.0f;
      if (hp <= 0.0f)
      {
        hp = DefaultHP;
        playerLifeHandler.KillPlayer();
        return true;
      }

      return false;
    }
  
    internal void Awake()
    {  
      hp = DefaultHP;
      playerLifeHandler = GetComponent<PlayerLifeHandler>();
      regenTimer = TimeToStartRegeneratingAfterDamaged;
    }
  
    internal void Update()
    {
      UpdateHealth();
    }

    /// <summary>
    /// Updates player health if the regen timer has been exceeded
    /// Increments regen timer if it does not exceed the time to start regenerating
    /// </summary>
    private void UpdateHealth()
    {
      if (regenTimer > TimeToStartRegeneratingAfterDamaged)
      {
        if (hp < DefaultHP)
        {
          hp += RegenerationRate * Time.deltaTime;
          if (hp > DefaultHP)
          {
            hp = DefaultHP;
          }
        }
      }
      else
      {
        regenTimer += Time.deltaTime;
      }
    }
  }
}