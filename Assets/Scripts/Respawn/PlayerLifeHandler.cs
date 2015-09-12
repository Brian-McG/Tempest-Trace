// <copyright file="PlayerLifeHandler.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeHandler : MonoBehaviour
{
  public float FadeOutSpeed;
  public float FadeInSpeed;
  public float FadeTriggerVelocity;
  public RawImage Fadeout;
  private PlayerHealth playerHealth;
  private FirstPersonMovement firstPersonMovement;
  private Checkpoint checkpoint;
  private bool isEnabled;
  private float total;
  private bool killInitiated;
  
  public void KillPlayer()
  {
    killInitiated = true;
  }

  internal void Awake()
  {  
    firstPersonMovement = this.gameObject.GetComponent<FirstPersonMovement>();
    playerHealth = GetComponent<PlayerHealth>();
    checkpoint = GetComponent<Checkpoint>();
    Fadeout.enabled = false;
    Fadeout.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2.0f, Screen.height);
    Fadeout.color = Color.clear;
    total = 0.0f;
  }

  internal void Update()
  {
    if (!isEnabled && firstPersonMovement.Velocity.y < FadeTriggerVelocity)
    {
      FadeOverlay();
    }
    else if (killInitiated)
    {
      FadeOverlay();
    }
    else
    {
      UnFadeOverlay();
    }
  }

  private void Kill()
  {
    killInitiated = false;
    isEnabled = true;
    Fadeout.color = Color.black;
    firstPersonMovement.ResetState();
    transform.position = checkpoint.Position;
    transform.localEulerAngles = checkpoint.Orientation;
    playerHealth.ResetHP();
  }

  private void FadeOverlay()
  {
    Fadeout.enabled = true;
    total += FadeOutSpeed * Time.deltaTime;
    Fadeout.color = Color.Lerp(Fadeout.color, Color.black, total);
    if (Fadeout.color.a > 0.99f)
    {
      total = 0.0f;
      Kill();
    }
  }

  private void UnFadeOverlay()
  {
    total += FadeInSpeed * Time.deltaTime;
    Fadeout.color = Color.Lerp(Fadeout.color, Color.clear, total);
    if (Fadeout.color.a < 0.01f)
    {
      total = 0.0f;
      ClearScreenOverlay();
    }
  }

  private void ClearScreenOverlay()
  {
    isEnabled = false;
    Fadeout.enabled = false;
    Fadeout.color = Color.clear;
  }
}
