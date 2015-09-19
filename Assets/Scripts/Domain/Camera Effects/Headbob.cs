// <copyright file="Headbob.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Player.Movement;

namespace Domain.CameraEffects
{
/// <summary>
/// Moves the player camera up and down in a realistic fashion when player is running.
/// </summary>
  public class Headbob : MonoBehaviour
  {
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

    private Camera fpsCam;
    private FirstPersonMovement fpsMove;
    private float movementX;
    private float movementY;
    private float timeX;
    private float timeY;
    private Vector3 defaultCameraPosition;
  
    internal void Awake()
    {  
      fpsCam = GetComponent<Camera>();
      fpsMove = transform.parent.GetComponent<FirstPersonMovement>();
      defaultCameraPosition = fpsCam.transform.localPosition;
      movementX = XMovement;
      movementY = YMovement;
      timeX = 0.0f;
      timeY = 0.0f;
    }

    /// <summary>
    /// Update head position with bob
    /// </summary>
    internal void Update()
    {
      // Update Player one head position
      Vector3 playerHorizontalVelocity = fpsMove.Velocity;
      playerHorizontalVelocity.y = 0.0f;
      if ((playerHorizontalVelocity.sqrMagnitude > 0.01f) &&
        fpsMove.IsGrounded &&
        (fpsMove.CurrentMotion == DefinedMotion.NONE))
      {
        // Set head position
        Vector3 deltaBob = new Vector3(movementX, movementY, 0) * Time.deltaTime;
        fpsCam.transform.localPosition += deltaBob;
      
        timeX += Time.deltaTime;
        timeY += Time.deltaTime;

        // Swap horizontal head movement direction
        if (timeX > XSwapTime)
        {
          movementX *= -1.0f;
          timeX -= XSwapTime;
        }

        // Swap vertical head movement direction
        if (timeY > YSwapTime)
        {
          movementY *= -1.0f;
          timeY -= YSwapTime;
        }
      }
      else
      {
        // Reset head position 
        fpsCam.transform.localPosition = defaultCameraPosition;
      }
    }
  }
}