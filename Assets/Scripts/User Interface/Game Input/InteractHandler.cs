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
  private int elevatorActivateLayer;
  private int elevatorSlowActivateLayer;
  private GameObject elevatorTerminal;
  private SmokebombController smokeBombController;

  private void Awake()
  {
    fpsMove = GetComponent<FirstPersonMovement>();
    cameraTransform = GetComponentInChildren<Camera>().transform;
    airconLayer = LayerMask.NameToLayer("AirconSwitch");
    doorLayer = LayerMask.NameToLayer("Door");
    elevatorActivateLayer = LayerMask.NameToLayer("ElevatorActivate");
    elevatorSlowActivateLayer = LayerMask.NameToLayer("ElevatorSlowTerminal");
    interactLayer = 1 << airconLayer | 1 << doorLayer | 1 << elevatorActivateLayer | 1 << elevatorSlowActivateLayer;
    elevatorTerminal = GameObject.FindGameObjectWithTag("ElevatorTerminal");
    smokeBombController = GetComponent<SmokebombController>();
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
        else if (hitInfo.collider.gameObject.layer == elevatorActivateLayer)
        {
          GameObject hitObj = hitInfo.transform.parent.gameObject;
          Elevator elevatorController = hitObj.GetComponent<Elevator>();
          if (elevatorController)
          {
            elevatorController.Activate();
          }
        }
        else if (hitInfo.collider.gameObject.layer == elevatorSlowActivateLayer)
        {
          Elevator elevatorController = elevatorTerminal.GetComponent<Elevator>();
          if (elevatorController)
          {
            elevatorController.ActivateSlow();
          }
        }
      }
    }
    if (InputSplitter.GetSmokePressed(fpsMove.PlayerID))
    {
      smokeBombController.Activate();
    }
  }
}
