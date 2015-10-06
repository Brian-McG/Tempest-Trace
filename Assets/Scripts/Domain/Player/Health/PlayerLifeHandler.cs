// <copyright file="PlayerLifeHandler.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Domain.Player.Checkpoint;
using Domain.Player.Movement;
using Domain.Player.Look;


namespace Domain.Player.Health
{
/// <summary>
/// Manages behaviour and circumstatances to player death.
/// </summary>
  public class PlayerLifeHandler : MonoBehaviour
  {
    [Tooltip("Speed that screen fades to black after death.")]
    public float
      FadeOutSpeed;
    [Tooltip("Speed that screen fades clear after respawn.")]
    public float
      FadeInSpeed;
    [Tooltip("The velocity that player needs to be traveling for the death fade to kick in.")]
    public float
      FadeTriggerVelocity;
    [Tooltip("Fadeout image.")]
    public RawImage
      Fadeout;

    private PlayerHealth playerHealth;
    private FirstPersonMovement firstPersonMovement;
    private Domain.Player.Checkpoint.CheckpointController checkpoint;
    private bool isEnabled;
    private float totalFadeout;
    private bool killInitiated;
    private bool completedCourse;

    /// <summary>
    /// Sets screen instantly black then clears at set rate.
    /// </summary>
    public void BlackToClear()
    {
      totalFadeout = 1;
      killInitiated = true;
    }

    /// <summary>
    /// Kills this player.
    /// </summary>
    public void KillPlayer()
    {
      killInitiated = true;
    }

    /// <summary>
    /// Fades screen to black
    /// Player loses input control.
    /// </summary>
    public void CompletedCourse()
    {
      completedCourse = true;
      firstPersonMovement.enabled = false;
    }

    internal void Awake()
    {  
      if (this.gameObject.tag == "PlayerOne" || this.gameObject.tag == "PlayerTwo")
      {
        firstPersonMovement = this.gameObject.GetComponent<FirstPersonMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        checkpoint = GetComponent<CheckpointController>();
        Fadeout.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2.0f, Screen.height);
      }
      else
      {
        Fadeout.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
      }
      Fadeout.enabled = false;
      Fadeout.color = Color.clear;
      totalFadeout = 0.0f;
      completedCourse = false;
    }

    /// <summary>
    /// Update fade overlay state
    /// </summary>
    internal void Update()
    {
      if (!isEnabled && (this.gameObject.tag == "PlayerOne" || this.gameObject.tag == "PlayerTwo") && firstPersonMovement.Velocity.y < FadeTriggerVelocity)
      {
        FadeOverlay();
      }
      else if (killInitiated || completedCourse)
      {
        FadeOverlay();
      }
      else
      {
        UnFadeOverlay();
      }
    }

    /// <summary>
    /// Carries out procedure to kill player.
    /// </summary>
    private void Kill()
    {
      killInitiated = false;
      isEnabled = true;
      Fadeout.color = Color.black;
      if (this.gameObject.tag == "PlayerOne" || this.gameObject.tag == "PlayerTwo")
      {
        firstPersonMovement.ResetState();
        transform.position = checkpoint.Position;
        transform.localEulerAngles = checkpoint.Orientation;
        playerHealth.ResetHP();
      }
    }

    /// <summary>
    /// Fades overlay to back by some increment.
    /// Once overlay is sufficiently black the player is then killed.
    /// </summary>
    private void FadeOverlay()
    {
      Fadeout.enabled = true;
      totalFadeout += FadeOutSpeed * Time.deltaTime;
      Fadeout.color = Color.Lerp(Fadeout.color, Color.black, totalFadeout);
      if (Fadeout.color.a > 0.99f && !completedCourse)
      {
        totalFadeout = 0.0f;
        Kill();
      }
      else if (Fadeout.color.a > 0.99f && completedCourse)
      {
        firstPersonMovement.enabled = true;
        rigidbody.useGravity = false;
        firstPersonMovement.ResetState();
        transform.position = GameObject.FindGameObjectWithTag("FinishLine").transform.position;
        firstPersonMovement.enabled = false;
        GetComponent<FirstPersonCameraHorizontal>().enabled = false;
        this.transform.FindChild("Camera").GetComponent<FirstPersonCameraVertical>().enabled = false;
      }
    }

    /// <summary>
    /// Fades overlay back to clear.
    /// </summary>
    private void UnFadeOverlay()
    {
      totalFadeout += FadeInSpeed * Time.deltaTime;
      Fadeout.color = Color.Lerp(Fadeout.color, Color.clear, totalFadeout);
      if (Fadeout.color.a < 0.01f)
      {
        totalFadeout = 0.0f;
        ClearScreenOverlay();
      }
    }

    /// <summary>
    /// Reset overlay to fully clear.
    /// </summary>
    private void ClearScreenOverlay()
    {
      isEnabled = false;
      Fadeout.enabled = false;
      Fadeout.color = Color.clear;
    }
  }
}