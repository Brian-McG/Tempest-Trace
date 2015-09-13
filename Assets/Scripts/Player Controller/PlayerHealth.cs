// <copyright file="PlayerHealth.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
  public float DefaultHP;
  public float TimeToStartRegeneratingAfterDamaged;
  public float RegenerationRate;
  public RawImage HitOverlay;
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

  public void ResetHP()
  {
    hp = DefaultHP;
    regenTimer = TimeToStartRegeneratingAfterDamaged;
  }

  /// <summary>
  /// Returns true if player is killed from the action
  /// </summary>
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
    // Debug.Log("HP: " + hp);
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
