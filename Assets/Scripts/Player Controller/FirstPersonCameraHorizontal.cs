// <copyright file="FirstPersonCameraHorizontal.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using UnityEngine;

public class FirstPersonCameraHorizontal : MonoBehaviour
{
    public float Sensitivity = 6.0f;

    private void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        float deltaRotationY = Input.GetAxis("Mouse X") * Sensitivity;
        float rotationY = currentRotation.y + deltaRotationY;

        transform.localEulerAngles = new Vector3(currentRotation.x, rotationY, currentRotation.z);
    }
}
