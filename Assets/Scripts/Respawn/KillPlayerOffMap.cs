﻿// <copyright file="KillPlayerOffMap.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

// TODO: Faciliate for two players
public class KillPlayerOffMap : MonoBehaviour
{
  public float FadeOutSpeed = 1.0f;
  public float FadeInSpeed = 1.0f;
  public GUITexture Fadeout;
  private FirstPersonMovement firstPersonMovement;
  private Checkpoint checkpoint;
  private bool active;

  internal void Awake()
  {  
    firstPersonMovement = GetComponent<FirstPersonMovement>();
    checkpoint = GetComponent<Checkpoint>();
    Fadeout.guiTexture.enabled = false;
    Fadeout.guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height); 
    Fadeout.guiTexture.color = Color.clear;
  }

  internal void Update()
  {
    if (!active && firstPersonMovement.Velocity.y < -15.0f)
    {
      Fadeout.guiTexture.enabled = true;
      Fadeout.guiTexture.color = Color.Lerp(Fadeout.guiTexture.color, Color.black, FadeOutSpeed * Time.deltaTime);
      if (Fadeout.guiTexture.color.a >= 0.7f)
      {
        active = true;
        Fadeout.guiTexture.color = Color.black;
        transform.position = checkpoint.Position;
      }
    }
    else
    {
      Fadeout.guiTexture.color = Color.Lerp(Fadeout.guiTexture.color, Color.clear, FadeInSpeed * Time.deltaTime);
      if (Fadeout.guiTexture.color.a < 0.05f)
      {
        active = false;
        Fadeout.guiTexture.enabled = false;
        Fadeout.guiTexture.color = Color.clear;
      }
    }
  }
}