// <copyright file="FirstPersonCameraVertical.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using UnityEngine;

public class FirstPersonCameraVertical : MonoBehaviour
{
  public float Sensitivity = 6.0f;

  public float MinimumAngle = -60.0f;
  public float MaximumAngle = 60.0f;

  private float currentAngle;

  public void ResetState()
  {
    currentAngle = 0.0f;
  }

  private void Awake()
  {
    currentAngle = transform.localEulerAngles.x;
  }

  private void Update()
  {
    Vector3 currentRotation = transform.localEulerAngles;

    float deltaRotationX = Input.GetAxis("Mouse Y") * Sensitivity;
    currentAngle -= deltaRotationX;
    currentAngle = Mathf.Clamp(currentAngle, MinimumAngle, MaximumAngle);

    transform.localEulerAngles = new Vector3(currentAngle, currentRotation.y, currentRotation.z);
  }
}
