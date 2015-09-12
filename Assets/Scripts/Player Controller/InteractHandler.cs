// <copyright file="InteractHandler.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
  public float InteractDistance;

  private int interactLayer = 1 << 8;
  private FirstPersonMovement fpsMove;
  private Transform cameraTransform;
  
  private void Awake()
  {
    fpsMove = GetComponent<FirstPersonMovement>();
    cameraTransform = GetComponentInChildren<Camera>().transform;
  }
  
  private void Update()
  {
    if (InputSplitter.GetInteractPressed(fpsMove.PlayerID))
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hitInfo,
                          InteractDistance, interactLayer))
      {
        GameObject hitObj = hitInfo.transform.parent.gameObject;
        Aircon airconController = hitObj.GetComponent<Aircon>();
        if (airconController)
        {
          airconController.Activate();
        }
      }
    }
  }
}
