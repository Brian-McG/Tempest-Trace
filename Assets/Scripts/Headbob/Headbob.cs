// <copyright file="Headbob.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Headbob : MonoBehaviour
{
  public GameObject PlayerOne;
  public GameObject PlayerTwo;
  [Tooltip("Time it takes for x movement to switch to opposite direction")]
  public float
    XSwapTime;
  [Tooltip("Time it takes for y movement to switch to opposite direction")]
  public float
    YSwapTime;
  [Tooltip("Amount of movement along x-axis per time interval")]
  public float
    XMovement;
  [Tooltip("Amount of movement along y-axis per time interval")]
  public float
    YMovement;

  private Camera playerOneCamera;
  private Camera playerTwoCamera;
  private FirstPersonMovement playerOneMovement;
  private FirstPersonMovement playerTwoMovement;
  private float playerOneXMovement;
  private float playerOneYMovement;
  private float playerTwoXMovement;
  private float playerTwoYMovement;
  private float playerOneXTime;
  private float playerOneYTime;
  private float playerTwoXTime;
  private float playerTwoYTime;
  
  internal void Awake()
  {  
    playerOneCamera = PlayerOne.GetComponentInChildren<Camera>();
    playerTwoCamera = PlayerTwo.GetComponentInChildren<Camera>();
    playerOneMovement = PlayerOne.GetComponent<FirstPersonMovement>();
    playerTwoMovement = PlayerTwo.GetComponent<FirstPersonMovement>();
    playerOneXMovement = XMovement;
    playerOneYMovement = YMovement;
    playerTwoXMovement = XMovement;
    playerTwoYMovement = YMovement;
    playerOneXTime = 0.0f;
    playerOneYTime = 0.0f;
    playerTwoXTime = 0.0f;
    playerTwoYTime = 0.0f;
  }

  internal void Update()
  {
    // Update Player one head position
    if (playerOneMovement.Velocity.magnitude > 0.0f && playerOneMovement.IsGrounded)
    {
      playerOneCamera.transform.localPosition = new Vector3(playerOneCamera.transform.localPosition.x + (playerOneXMovement * Time.deltaTime),
                                                            playerOneCamera.transform.localPosition.y + (playerOneYMovement * Time.deltaTime),
                                                            playerOneCamera.transform.localPosition.z);
      playerOneXTime += Time.deltaTime;
      playerOneYTime += Time.deltaTime;
      if (playerOneXTime > XSwapTime)
      {
        playerOneXMovement *= -1.0f;
        playerOneXTime -= XSwapTime;
      }
      
      if (playerOneYTime > YSwapTime)
      {
        playerOneYMovement *= -1.0f;
        playerOneYTime -= YSwapTime;
      }
    }
    
    // Update Player two head position
    if (playerTwoMovement.Velocity.magnitude > 0.0f && playerTwoMovement.IsGrounded)
    {
      playerTwoCamera.transform.localPosition = new Vector3(playerTwoCamera.transform.localPosition.x + (playerTwoXMovement * Time.deltaTime),
                                                            playerTwoCamera.transform.localPosition.y + (playerTwoYMovement * Time.deltaTime),
                                                            playerTwoCamera.transform.localPosition.z);
      playerTwoXTime += Time.deltaTime;
      playerTwoYTime += Time.deltaTime;
      if (playerTwoXTime > XSwapTime)
      {
        playerTwoXMovement *= -1.0f;
        playerTwoXTime -= XSwapTime;
      }
      
      if (playerTwoYTime > YSwapTime)
      {
        playerTwoYMovement *= -1.0f;
        playerTwoYTime -= YSwapTime;
      }
    }
  }
}
