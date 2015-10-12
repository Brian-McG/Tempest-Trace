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
	public Canvas ControlCanvas;
    private bool showInstructions;
	private bool showControls;

    internal void Update()
    {
      if (showInstructions)
      {
        MenuCanvas.enabled = false;
        InstructionsCanvas.enabled = true;
		ControlCanvas.enabled = false;
      }
      else if (showControls)
	  {
		MenuCanvas.enabled = false;
		InstructionsCanvas.enabled = false;
		ControlCanvas.enabled = true;
	  }
	  else
      {
        MenuCanvas.enabled = true;
        InstructionsCanvas.enabled = false;
		ControlCanvas.enabled = false;
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
	public void OpenControls()
	{
	  showControls = true;
	  showInstructions = false;
	}
	public void CloseControls()
	{
	  showControls = false;
	  showInstructions = true;
	}
  }
}