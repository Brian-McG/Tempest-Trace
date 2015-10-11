// <copyright file="WinCondition.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using UserInterface.HeadsUpDisplay;
using Domain.Player.Health;

namespace Domain.GameEnd
{
///  summary>
/// Controls what occurs when a player reaches the finish line.
/// </summary>
  public class WinCondition : MonoBehaviour
  {
    public GameObject leaderboardCanvas;
    public GameObject playerOneUI;
    public GameObject playerTwoUI;

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

    public void ReturnToMainMenu()
    {
      Application.LoadLevel(0);
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

      playerOneUI.active = false;
      playerTwoUI.active = false;
      leaderboardCanvas.active = true;

      Transform scorePanelTrans = leaderboardCanvas.transform.Find("ScorePanel");
      Text winnerLabel = scorePanelTrans.Find("WinnerLabel").GetComponent<Text>();
      Text winnerTime = scorePanelTrans.Find("WinnerTime").GetComponent<Text>();
      Text loserLabel = scorePanelTrans.Find("LoserLabel").GetComponent<Text>();
      Text loserTime = scorePanelTrans.Find("LoserTime").GetComponent<Text>();

      winnerLabel.text = "Player "+completionOrder[0];
      if(completionOrder[0] == 1)
      {
        winnerTime.text = GameTime.Instance.PlayerOneTime.GetComponent<Text>().text;
      }
      else
      {
        winnerTime.text = GameTime.Instance.PlayerTwoTime.GetComponent<Text>().text;
      }

      loserLabel.text = "Player "+completionOrder[1];
      if(completionOrder[1] == 1)
      {
        loserTime.text = GameTime.Instance.PlayerOneTime.GetComponent<Text>().text;
      }
      else
      {
        loserTime.text = GameTime.Instance.PlayerTwoTime.GetComponent<Text>().text;
      }
    }
  }
}
