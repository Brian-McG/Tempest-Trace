﻿// <copyright file="PlayerSlow.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{ 
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
		playerOneTime = PlayerOneTime.GetComponent<Text> ();
		// PlayerTwoTime = PlayerTwoTime.getComponent<Text>();
	}
	
	internal void Update() {
		seconds += Time.deltaTime;
		if (seconds >= 60.0f) {
			seconds -= 60.0f;
			minutes += 1;
		}
		if (PlayerOneActive) {
			playerOneTime.text = "Time " + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10.0f ? "0" : "") + (int)seconds; 
		}
		if (PlayerTwoActive) {

		}
	}
}