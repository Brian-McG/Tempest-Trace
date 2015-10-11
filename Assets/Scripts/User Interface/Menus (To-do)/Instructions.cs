// <copyright file="Instructions.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Domain.UserInterface.Menu
{
  public class Instructions : MonoBehaviour
  {
    public Canvas MenuCanvas;
    public Canvas InstructionsCanvas;
    private bool showInstructions;

    internal void Update()
    {
      if (showInstructions)
      {
        MenuCanvas.enabled = false;
        InstructionsCanvas.enabled = true;
      }
      else
      {
        MenuCanvas.enabled = true;
        InstructionsCanvas.enabled = false;
      }
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        if (showInstructions)
        {
          CloseInstructions();
        }
      }
    }

    public void OpenInstructions()
    {
      showInstructions = true;
    }
    public void CloseInstructions()
    {
      showInstructions = false;
    }
  }
}