﻿// <copyright file="GameTime.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.HeadsUpDisplay
{
  public class GameTime : MonoBehaviour
  { 
    private static GameTime _instance;
    public static GameTime Instance
    {
      get { return _instance; }
    }

    public bool PlayerOneActive = true;
    public bool PlayerTwoActive = true;
    public GameObject PlayerOneTime;
    public GameObject PlayerTwoTime;
    private Text playerOneTime;
    private Text playerTwoTime;
    private float seconds = 0.0f;
    private uint minutes = 0;

    internal void Awake()
    {
      _instance = this;

      playerOneTime = PlayerOneTime.GetComponent<Text>();
      if (PlayerTwoTime != null)
      {
        playerTwoTime = PlayerTwoTime.GetComponent<Text>();
      }
    }

    public void ResetTime()
    {
      seconds = 0.0f;
      minutes = 0;
    }

    internal void Update()
    {
      UpdateTime();
    }

    private void UpdateTime()
    {
      seconds += Time.deltaTime;
      if (seconds >= 60.0f)
      {
        seconds -= 60.0f;
        minutes += 1;
      }
    
      if (PlayerOneActive)
      {
        playerOneTime.text = (minutes < 10 ? "0" : string.Empty) + minutes + ":" + (seconds < 10.0f ? "0" : string.Empty) + (int)seconds; 
      }
    
      if (PlayerTwoTime != null && PlayerTwoActive)
      {
        playerTwoTime.text = (minutes < 10 ? "0" : string.Empty) + minutes + ":" + (seconds < 10.0f ? "0" : string.Empty) + (int)seconds; 
      }
    }
  }
}
