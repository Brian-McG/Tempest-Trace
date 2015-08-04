using UnityEngine;

public class FirstPersonCameraVertical : MonoBehaviour
{
    public float sensitivity = 6.0f;

    public float minimumAngle = -60.0f;
    public float maximumAngle =  60.0f;

    private float currentAngle;

    void Awake()
    {
        currentAngle = transform.localEulerAngles.x;
    }

    void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        float xDeltaRotation = Input.GetAxis("Mouse Y")*sensitivity;
        currentAngle -= xDeltaRotation;
        currentAngle = Mathf.Clamp(currentAngle, minimumAngle, maximumAngle);

        transform.localEulerAngles = new Vector3(currentAngle, currentRotation.y, currentRotation.z);
    }

}
