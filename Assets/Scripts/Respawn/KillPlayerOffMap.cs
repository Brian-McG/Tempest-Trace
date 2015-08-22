// <copyright file="KillPlayerOffMap.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

// TODO: Faciliate for two players
public class KillPlayerOffMap : MonoBehaviour
{
  public float FadeOutSpeed;
  public float FadeInSpeed;
  public float FadeTriggerVelocity;
  public GUITexture Fadeout;
  private FirstPersonMovement firstPersonMovement;
  private Checkpoint checkpoint;
  private bool isEnabled;

  internal void Awake()
  {  
    firstPersonMovement = GetComponent<FirstPersonMovement>();
    checkpoint = GetComponent<Checkpoint>();
    Fadeout.guiTexture.enabled = false;
    Fadeout.guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height); 
    Fadeout.guiTexture.color = Color.clear;
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
    Fadeout.guiTexture.enabled = true;
    Fadeout.guiTexture.color = Color.Lerp(Fadeout.guiTexture.color, Color.black, FadeOutSpeed * Time.deltaTime);
    if (Fadeout.guiTexture.color.a >= 0.7f)
    {
      KillPlayer();
    }
  }

  private void UnFadeOverlay()
  {
    Fadeout.guiTexture.color = Color.Lerp(Fadeout.guiTexture.color, Color.clear, FadeInSpeed * Time.deltaTime);
    if (Fadeout.guiTexture.color.a < 0.05f)
    {
      ClearScreenOverlay();
    }
  }

  private void KillPlayer()
  {
    isEnabled = true;
    Fadeout.guiTexture.color = Color.black;
    firstPersonMovement.ResetState();
    transform.position = checkpoint.Position;
    transform.localEulerAngles = checkpoint.Orientation;
  }

  private void ClearScreenOverlay()
  {
    isEnabled = false;
    Fadeout.guiTexture.enabled = false;
    Fadeout.guiTexture.color = Color.clear;
  }
}
