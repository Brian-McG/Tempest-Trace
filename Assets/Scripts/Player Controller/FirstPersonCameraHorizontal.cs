using UnityEngine;

public class FirstPersonCameraHorizontal : MonoBehaviour
{
    public float sensitivity = 6.0f;

    void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        float yDeltaRotation = Input.GetAxis("Mouse X")*sensitivity;
        float yRotation = currentRotation.y + yDeltaRotation;

        transform.localEulerAngles = new Vector3(currentRotation.x, yRotation, currentRotation.z);
    }

}
