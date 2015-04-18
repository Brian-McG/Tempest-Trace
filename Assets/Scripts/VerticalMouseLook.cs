using UnityEngine;

public class VerticalMouseLook : MonoBehaviour
{
    public float sensitivity; //TODO: Ideally we'd extract this to some central place
                              //      that stores settings values

    private const float mininumAngle = -80.0f;
    private const float maximumAngle = 80.0f;
    private float verticalRotation;

	void Start ()
    {	
	}
	
	void Update ()
    {
	   verticalRotation += Input.GetAxis("Mouse Y") * sensitivity;
       verticalRotation = Mathf.Clamp(verticalRotation, mininumAngle, maximumAngle);

       Vector3 currentRotation = transform.localEulerAngles;
       transform.localEulerAngles = new Vector3(-verticalRotation, 
                                                currentRotation.y, 
                                                currentRotation.z);
	}
}
