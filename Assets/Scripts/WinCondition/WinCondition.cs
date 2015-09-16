// <copyright file="WinCondition.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
  private byte index;
  private byte winner;
  private GameTime gameTime;
  private byte[] completionOrder;

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

  internal void OnTriggerEnter(Collider other)
  {
    if (other.tag == "PlayerOne" || other.tag == "PlayerTwo")
    {
      PlayerLifeHandler lifeHandler = other.gameObject.GetComponent<PlayerLifeHandler>();
      if (other.tag == "PlayerOne")
      {
        gameTime.PlayerOneActive = false;
        lifeHandler.CompletedCourse();
        completionOrder[index] = 1;
      }
      else if (other.tag == "PlayerTwo")
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
  }

  private void AllPlayersFinished()
  {
    Debug.Log("Transition to end-game screen");

    // TODO: End game screen
  }
}
