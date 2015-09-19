// <copyright file="FirstPersonCameraHorizontal.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using UnityEngine;
using Domain.Player.Movement;

namespace Domain.Player.Look
{
  public class FirstPersonCameraHorizontal : MonoBehaviour
  {
    public float Sensitivity = 6.0f;

    private FirstPersonMovement fpsMove;

    private void Awake()
    {
      fpsMove = transform.GetComponent<FirstPersonMovement>();
    }

    private void Update()
    {
      Vector3 currentRotation = transform.localEulerAngles;

      float horizontalViewAxis = InputSplitter.GetHorizontalViewAxis(fpsMove.PlayerID);
      float deltaRotationY = horizontalViewAxis * Sensitivity;
      float rotationY = currentRotation.y + deltaRotationY;

      transform.localEulerAngles = new Vector3(currentRotation.x, rotationY, currentRotation.z);
    }
  }
}