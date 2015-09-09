// <copyright file="KillPlayerOffMap.cs" company="University of Cape Town">
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
  private FirstPersonMovement firstPersonMovement;
  private Checkpoint checkpoint;
  private bool isEnabled;
  private float total;

  public void KillPlayer()
  {
    isEnabled = true;
    Fadeout.color = Color.black;
    firstPersonMovement.ResetState();
    transform.position = checkpoint.Position;
    transform.localEulerAngles = checkpoint.Orientation;
  }

  internal void Awake()
  {  
    firstPersonMovement = this.gameObject.GetComponent<FirstPersonMovement>();
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
    else
    {
      UnFadeOverlay();
    }
  }

  private void FadeOverlay()
  {
    Fadeout.enabled = true;
    total += FadeOutSpeed * Time.deltaTime;
    Fadeout.color = Color.Lerp(Fadeout.color, Color.black, total);
    if (Fadeout.color.a > 0.99f)
    {
      total = 0.0f;
      KillPlayer();
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
