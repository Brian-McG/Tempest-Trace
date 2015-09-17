// <copyright file="HitFlash.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Flash screen when player is shot
/// </summary>
public class HitFlash : MonoBehaviour
{
  [Tooltip("Player one flash image.")]
  public RawImage
    FlashPlayerOne;
  [Tooltip("Player two flash image.")]
  public RawImage
    FlashPlayerTwo;
  [Tooltip("Rate at which screen should clear flash.")]
  public float
    ClearRate;

  private Color defaultColor;
  private float currentDurationPlayerOne;
  private float currentDurationPlayerTwo;

  internal void Awake()
  {
    FlashPlayerOne.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2.0f, Screen.height);
    FlashPlayerTwo.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2.0f, Screen.height);
    FlashPlayerOne.enabled = false;
    FlashPlayerTwo.enabled = false;
    currentDurationPlayerOne = 0.0f;
    currentDurationPlayerTwo = 0.0f;
    defaultColor = FlashPlayerOne.color;
  }
  
  /// <summary>
  /// Flashes camera of a given player.
  /// </summary>
  /// <param name="player">Player to flash the camera for.</param>
  public void FlashCamera(byte player)
  {
    if (player == 0 || player == 1)
    {
      currentDurationPlayerOne = 0.0f;
      FlashPlayerOne.enabled = true;
      FlashPlayerOne.color = defaultColor;
    }
    else
    {
      currentDurationPlayerTwo = 0.0f;
      FlashPlayerTwo.enabled = true;
      FlashPlayerTwo.color = defaultColor;
    }
  }

  /// <summary>
  /// Update flash overlay
  /// </summary>
  internal void Update()
  {
    if (FlashPlayerOne.enabled)
    {
      currentDurationPlayerOne += Time.deltaTime;
      UnFadeOverlay(1);
    }
    
    if (FlashPlayerTwo.enabled)
    {
      currentDurationPlayerTwo += Time.deltaTime;
      UnFadeOverlay(2);
    }
  }

  /// <summary>
  /// Incrementally un-fades the flash overlay.
  /// </summary>
  /// <param name="player">Player to clear the overlay for.</param>
  private void UnFadeOverlay(byte player)
  {
    if (player == 0 || player == 1)
    {
      currentDurationPlayerOne += ClearRate * Time.deltaTime;
      FlashPlayerOne.color = Color.Lerp(FlashPlayerOne.color, Color.clear, currentDurationPlayerOne);
      if (FlashPlayerOne.color.a < 0.01f)
      {
        currentDurationPlayerOne = 0.0f;
        FlashPlayerOne.enabled = false;
      }
    }
    else
    {
      currentDurationPlayerTwo += ClearRate * Time.deltaTime;
      FlashPlayerTwo.color = Color.Lerp(FlashPlayerTwo.color, Color.clear, currentDurationPlayerTwo);
      if (FlashPlayerOne.color.a < 0.01f)
      {
        currentDurationPlayerTwo = 0.0f;
        FlashPlayerTwo.enabled = false;
      }
    }
  }
}