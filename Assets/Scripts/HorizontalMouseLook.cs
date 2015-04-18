using UnityEngine;

public class HorizontalMouseLook : MonoBehaviour
{
    public float sensitivity;
    
    void Update()
    {
          float rotation = Input.GetAxis("Mouse X") * sensitivity;
          transform.Rotate(0, rotation, 0);
    }
}
