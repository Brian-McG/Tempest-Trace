using UnityEngine;
using System.Collections;

public class InteractHandler : MonoBehaviour
{
  public float InteractDistance;

  private int interactLayer = 1 << 8;
  private FirstPersonMovement fpsMove;
  private Transform cameraTransform;

  void Awake()
  {
    fpsMove = GetComponent<FirstPersonMovement>();
    cameraTransform = GetComponentInChildren<Camera>().transform;
  }

	void Update ()
  {
    if (InputSplitter.GetInteractPressed(fpsMove.PlayerID))
    {
      RaycastHit hitInfo;
      if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hitInfo,
                          InteractDistance, interactLayer))
      {
        GameObject hitObj = hitInfo.transform.parent.gameObject;
        Aircon acController = hitObj.GetComponent<Aircon>();
        if (acController)
        {
          acController.Activate();
        }
      }
    }
	}
}
