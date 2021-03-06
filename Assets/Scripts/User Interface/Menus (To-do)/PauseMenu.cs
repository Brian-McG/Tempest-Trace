﻿using UnityEngine;
using System.Collections;
using Domain.Player.Look;
namespace Domain.UserInterface.Menu
{
  public class PauseMenu : MonoBehaviour
  {
    public bool isPaused;
	public bool gameOver;
    public GameObject PauseMenuCanvas;
    private GameObject player1;
    private GameObject player2;
	
    // Update is called once per frame
    void Awake()
    {
      player1 = GameObject.FindGameObjectWithTag("PlayerOne");
      player2 = GameObject.FindGameObjectWithTag("PlayerTwo");
	  gameOver = false;
    }
    void Update()
    {
      if (isPaused)
      {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0;
        player1.GetComponent<FirstPersonCameraHorizontal>().enabled = false;
        player1.GetComponentInChildren<FirstPersonCameraVertical>().enabled = false;
        if (player2 != null)
        {
          player2.GetComponent<FirstPersonCameraHorizontal>().enabled = false;
          player2.GetComponentInChildren<FirstPersonCameraVertical>().enabled = false;
        }
      }
      else
      {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1;
        player1.GetComponent<FirstPersonCameraHorizontal>().enabled = true;
        player1.GetComponentInChildren<FirstPersonCameraVertical>().enabled = true;
        if (player2 != null)
        {
          player2.GetComponent<FirstPersonCameraHorizontal>().enabled = true;
          player2.GetComponentInChildren<FirstPersonCameraVertical>().enabled = true;
        }
      }
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        if (isPaused)
        {
          isPaused = false;
		  Screen.showCursor = false;
        }
        else
        {
          isPaused = true;
		  Screen.showCursor = true;
        }
      }
	
    }
    public void ResumeButton()
    {
      isPaused = false;
	  Screen.showCursor = false;
    }
    public void ChangeToScene(int address)
    {
      Application.LoadLevel(address);
    }
  }
}