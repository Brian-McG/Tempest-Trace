// <copyright file="SmokeBomb.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Domain.Interactables.Smokebomb;
namespace UserInterface.HeadsUpDisplay
{
  public class SmokeBomb : MonoBehaviour
  {
    public Text PlayerOneSmokeText;
    public Text PlayerTwoSmokeText;
    private SmokebombController playerOneSmokeController;
    private SmokebombController playerTwoSmokeController;
    internal void Awake()
    {  
      playerOneSmokeController = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<SmokebombController>();
      playerTwoSmokeController = null;
      GameObject playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
      if (playerTwo != null)
      {
        playerTwoSmokeController = playerTwo.GetComponent<SmokebombController>();
      }
    }

    internal void Update()
    {
      PlayerOneSmokeText.text = "Smoke Bomb\n" + (playerOneSmokeController.NumberOfBombs - playerOneSmokeController.CurrentBombCount) + " of " + playerOneSmokeController.NumberOfBombs;
      if (playerTwoSmokeController != null)
      {
        PlayerTwoSmokeText.text = "Smoke Bomb\n" + (playerTwoSmokeController.NumberOfBombs - playerTwoSmokeController.CurrentBombCount) + " of " + playerTwoSmokeController.NumberOfBombs;
      }
    }
  }
}