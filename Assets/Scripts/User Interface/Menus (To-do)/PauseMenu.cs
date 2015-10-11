using UnityEngine;
using System.Collections;
using Domain.Player.Look;

public class PauseMenu : MonoBehaviour
{
  public bool isPaused;
  public GameObject PauseMenuCanvas;
  private GameObject player1;
  private GameObject player2;
  
  void Awake()
  {
    player1 = GameObject.FindGameObjectWithTag ("PlayerOne");
    player2 = GameObject.FindGameObjectWithTag ("PlayerTwo");
  }

  void Update ()
  {
    if (Input.GetKeyDown (KeyCode.Escape))
    {
      if(isPaused)
      {
        isPaused = false;

        PauseMenuCanvas.SetActive (false);
        Time.timeScale = 1.0f;
        player1.GetComponent<FirstPersonCameraHorizontal>().enabled = true;
        player1.GetComponentInChildren<FirstPersonCameraVertical>().enabled = true;
        if(player2 != null)
        {
          player2.GetComponent<FirstPersonCameraHorizontal>().enabled = true;
          player2.GetComponentInChildren<FirstPersonCameraVertical>().enabled = true;
        }
      }
      else
      {
        isPaused = true;

        PauseMenuCanvas.SetActive (true);
        Time.timeScale = 0.0f;
        player1.GetComponent<FirstPersonCameraHorizontal>().enabled = false;
        player1.GetComponentInChildren<FirstPersonCameraVertical>().enabled = false;
        if(player2 != null)
        {
          player2.GetComponent<FirstPersonCameraHorizontal>().enabled = false;
          player2.GetComponentInChildren<FirstPersonCameraVertical>().enabled = false;
        }
      }
    }
  }
  public void ResumeButton()
  {
    isPaused = false;
  }
  public void ChangeToScene (int address)
  {
    Application.LoadLevel (address);
    if (address == 1)
    {
      Screen.showCursor = false;
    }
  }
}
