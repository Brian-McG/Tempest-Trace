﻿// <copyright file="WinCondition.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UserInterface.HeadsUpDisplay;
using Domain.Player.Health;

namespace Domain.GameEnd
{
/// <summary>
/// Controls what occurs when a player reaches the finish line.
/// </summary>
  public class WinCondition : MonoBehaviour
  {
    private byte index;
    private byte winner;
    private GameTime gameTime;
    private byte[] completionOrder;

    /// <summary>
    /// Gets the order in which the players completed the race
    /// </summary>
    /// <value>Byte array of length two where index 0 contains the player number that won 
    /// and index 1 contains the player number of the player that came second.</value>
    public byte[] CompletionOrder
    {
      get
      {
        return completionOrder;
      }
    }

    internal void Awake()
    {  
      index = 0;
      gameTime = GameObject.FindGameObjectWithTag("UIController").GetComponent<GameTime>();
      completionOrder = new byte[2];
    }

    /// <summary>
    /// Triggers when object triggers finish line.
    /// </summary>
    /// <param name="other">Object collided with</param>
    internal void OnTriggerEnter(Collider other)
    {
      if (other.tag == "PlayerOne" || other.tag == "PlayerTwo")
      {
        CapturePlayerCompletion(other);
      }
    }

    /// <summary>
    /// Captures the player completion of the course and the order that the player completed.
    /// </summary>
    /// <param name="player">Player that just completed race.</param>
    private void CapturePlayerCompletion(Collider player)
    {
      PlayerLifeHandler lifeHandler = player.gameObject.GetComponent<PlayerLifeHandler>();
      if (player.tag == "PlayerOne")
      {
        gameTime.PlayerOneActive = false;
        lifeHandler.CompletedCourse();
        completionOrder[index] = 1;
      }
      else if (player.tag == "PlayerTwo")
      {
        gameTime.PlayerTwoActive = false;
        lifeHandler.CompletedCourse();
        completionOrder[index] = 2;
      }
      ++index;
      if (index == 2)
      {
        AllPlayersFinished();
      }
    }

    /// <summary>
    /// Triggers end game screen
    /// </summary>
    private void AllPlayersFinished()
    {
      Debug.Log("Transition to end-game screen");

      // TODO: End game screen
    }
  }
}