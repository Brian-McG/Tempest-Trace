// <copyright file="DynamicHUDMovement.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DynamicHUDMovement : MonoBehaviour
{
  public GameObject Player;
  public GameObject[] HudElements;
  public float XSwapTime;
  public float YSwapTime;
  public float MaxXMovement;
  public float MaxYMovement;
  public float XMovement;
  public float YMovement;
  private FirstPersonMovement firstPersonMovement;
  private RectTransform[] hudElements;
  private Vector2[] startPositions;
  private bool movingForward;
  private float currentXTime;
  private float currentYTime;

  internal void Awake()
  {  
    firstPersonMovement = Player.GetComponent<FirstPersonMovement>();
    hudElements = new RectTransform[HudElements.Length];
    startPositions = new Vector2[HudElements.Length];
    for (uint i = 0; i < HudElements.Length; ++i)
    {
      hudElements[i] = HudElements[i].GetComponent<RectTransform>();
      startPositions[i] = hudElements[i].anchoredPosition;
    }

    currentXTime = 0.0f;
    currentYTime = 0.0f;
    movingForward = true;
  }

  internal void Update()
  {
    // Should this effect play if you are not grounded?
    if (firstPersonMovement.Velocity.magnitude > 0.0f && firstPersonMovement.IsGrounded)
    {
      for (uint i = 0; i < hudElements.Length; ++i)
      {
        RectTransform element = hudElements[i];
        float x = element.anchoredPosition.x;
        float y = element.anchoredPosition.y;    
        hudElements[i].anchoredPosition = new Vector2(x + (XMovement * Time.deltaTime), y + (YMovement * Time.deltaTime));
      }

      currentXTime += Time.deltaTime;
      currentYTime += Time.deltaTime;
      if (currentXTime > XSwapTime)
      {
        XMovement *= -1.0f;
        currentXTime -= XSwapTime;
      }

      if (currentYTime > YSwapTime)
      {
        YMovement *= -1.0f;
        currentYTime -= YSwapTime;
      }
    }
  }
}
