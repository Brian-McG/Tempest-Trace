// <copyright file="InteractHandler.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
  public float InteractDistance;

  private int interactLayer;
  private FirstPersonMovement fpsMove;
  private Transform cameraTransform;
  private int airconLayer;
  private int doorLayer;
  
  private void Awake()
  {
    fpsMove = GetComponent<FirstPersonMovement>();
    cameraTransform = GetComponentInChildren<Camera>().transform;
    airconLayer = LayerMask.NameToLayer("AirconSwitch");
    doorLayer = LayerMask.NameToLayer("Door");
    interactLayer = 1 << airconLayer | 1 << doorLayer;
  }
  
  private void Update()
  {
    if (InputSplitter.GetInteractPressed(fpsMove.PlayerID))
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hitInfo,
                          InteractDistance, interactLayer))
      {
        if (hitInfo.collider.gameObject.layer == airconLayer)
        {
          GameObject hitObj = hitInfo.transform.parent.gameObject;
          Aircon airconController = hitObj.GetComponent<Aircon>();
          if (airconController)
          {
            airconController.Activate();
          }
        }
        else if (hitInfo.collider.gameObject.layer == doorLayer)
        {
          GameObject hitObj = hitInfo.transform.parent.parent.gameObject;
          Door doorController = hitObj.GetComponent<Door>();
          if (doorController)
          {
            doorController.Activate();
          }
        }
      }
    }
  }
}
