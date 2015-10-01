// <copyright file="DynamicHUDMovement.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Domain.Player.Movement;

namespace UserInterface.HeadsUpDisplay
{
/// <summary>
/// Controls the movement of the heads up display
/// </summary>
  public class DynamicHUDMovement : MonoBehaviour
  {
    [Tooltip("Reference to player one object")]
    public GameObject
      PlayerOne;
    [Tooltip("Reference to player two object")]
    public GameObject
      PlayerTwo;
    [Tooltip("Reference to player one heads up display objects")]
    public GameObject[]
      PlayerOneHud;
    [Tooltip("Reference to player two heads up display objects")]
    public GameObject[]
      PlayerTwoHud;
    [Tooltip("The time it takes for heads up display to inverse horizontal movement")]
    public float
      XSwapTime;
    [Tooltip("The time it takes for heads up display to inverse vertical movement")]
    public float
      YSwapTime;
    [Tooltip("Rate at which the heads up display moves horizontally")]
    public float
      XMovement;
    [Tooltip("Rate at which the heads up display moves vertically")]
    public float
      YMovement;

    private FirstPersonMovement playerOneMovement;
    private FirstPersonMovement playerTwoMovement;
    private RectTransform[] playerOneHud;
    private RectTransform[] playerTwoHud;
    private Vector2[] playerOneHudStartPositions; 
    private Vector2[] playerTwoHudStartPositions;
    private float playerOneXTime;
    private float playerOneYTime;
    private float playerTwoXTime;
    private float playerTwoYTime;
    private float playerOneXMovement;
    private float playerOneYMovement;
    private float playerTwoXMovement;
    private float playerTwoYMovement;

    internal void Awake()
    {  
      playerOneMovement = PlayerOne.GetComponent<FirstPersonMovement>();
      playerOneHud = new RectTransform[PlayerOneHud.Length];
      playerOneHudStartPositions = new Vector2[PlayerOneHud.Length];

      if (PlayerTwo != null)
      {
        playerTwoMovement = PlayerTwo.GetComponent<FirstPersonMovement>();
        playerTwoHud = new RectTransform[PlayerTwoHud.Length];
        playerTwoHudStartPositions = new Vector2[PlayerTwoHud.Length];
      }

      for (uint i = 0; i < PlayerOneHud.Length; ++i)
      {
        playerOneHud[i] = PlayerOneHud[i].GetComponent<RectTransform>();
        playerOneHudStartPositions[i] = playerOneHud[i].anchoredPosition;
        if (PlayerTwo != null)
        {
          playerTwoHud[i] = PlayerTwoHud[i].GetComponent<RectTransform>();
          playerTwoHudStartPositions[i] = playerTwoHud[i].anchoredPosition;
        }
      }

      playerOneXTime = 0.0f;
      playerOneYTime = 0.0f;
      playerTwoXTime = 0.0f;
      playerTwoYTime = 0.0f;
      playerOneXMovement = XMovement;
      playerOneYMovement = YMovement;
      playerTwoXMovement = XMovement;
      playerTwoYMovement = YMovement;
    }

    internal void Update()
    {
      UpdateHUDPosition();
    }

    /// <summary>
    /// Updates the position of the heads up display and increments heads up display timers for each player if:
    ///   Player is moving
    ///   Player is on the ground
    ///   Player is not in a contextual action
    /// </summary>
    private void UpdateHUDPosition()
    {
      // Update Player one HUD
      if (playerOneMovement.Velocity.magnitude > 0.0f && playerOneMovement.IsGrounded && playerOneMovement.CurrentMotion == DefinedMotion.NONE)
      {
        for (uint i = 0; i < playerOneHud.Length; ++i)
        {
          RectTransform element = playerOneHud[i];
          float x = element.anchoredPosition.x;
          float y = element.anchoredPosition.y;    
          playerOneHud[i].anchoredPosition = new Vector2(x + (playerOneXMovement * Time.deltaTime), y + (playerOneYMovement * Time.deltaTime));
        }
      
        playerOneXTime += Time.deltaTime;
        playerOneYTime += Time.deltaTime;

        // Swap horizontal movement
        if (playerOneXTime > XSwapTime)
        {
          playerOneXMovement *= -1.0f;
          playerOneXTime -= XSwapTime;
        }

        // Swap vertical movement
        if (playerOneYTime > YSwapTime)
        {
          playerOneYMovement *= -1.0f;
          playerOneYTime -= YSwapTime;
        }
      }

      // Update Player two HUD
      if (PlayerTwo != null && playerTwoMovement.Velocity.magnitude > 0.0f && playerTwoMovement.IsGrounded && playerTwoMovement.CurrentMotion == DefinedMotion.NONE)
      {
        for (uint i = 0; i < playerTwoHud.Length; ++i)
        {
          RectTransform element = playerTwoHud[i];
          float x = element.anchoredPosition.x;
          float y = element.anchoredPosition.y;    
          playerTwoHud[i].anchoredPosition = new Vector2(x + (playerTwoXMovement * Time.deltaTime), y + (playerTwoYMovement * Time.deltaTime));
        }
      
        playerTwoXTime += Time.deltaTime;
        playerTwoYTime += Time.deltaTime;
        if (playerTwoXTime > XSwapTime)
        {
          playerTwoXMovement *= -1.0f;
          playerTwoXTime -= XSwapTime;
        }
      
        if (playerTwoYTime > YSwapTime)
        {
          playerTwoYMovement *= -1.0f;
          playerTwoYTime -= YSwapTime;
        }
      }
    }
  }
}
