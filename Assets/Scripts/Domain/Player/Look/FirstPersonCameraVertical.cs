// <copyright file="FirstPersonCameraVertical.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using UnityEngine;
using Domain.Player.Movement;

namespace Domain.Player.Look
{
  public class FirstPersonCameraVertical : MonoBehaviour
  {
    public float Sensitivity = 6.0f;

    public float MinimumAngle = -60.0f;
    public float MaximumAngle = 60.0f;

    private float currentAngle;

    private FirstPersonMovement fpsMove;

    public void ResetState()
    {
      currentAngle = 0.0f;
    }

    private void Awake()
    {
      currentAngle = transform.localEulerAngles.x;

      fpsMove = transform.parent.GetComponent<FirstPersonMovement>();
    }

    private void Update()
    {
      Vector3 currentRotation = transform.localEulerAngles;

      float verticalViewAxis = InputSplitter.GetVerticalViewAxis(fpsMove.PlayerID);
      float deltaRotationX = verticalViewAxis * Sensitivity;
      currentAngle -= deltaRotationX;
      currentAngle = Mathf.Clamp(currentAngle, MinimumAngle, MaximumAngle);

      transform.localEulerAngles = new Vector3(currentAngle, currentRotation.y, currentRotation.z);
    }
  }
}