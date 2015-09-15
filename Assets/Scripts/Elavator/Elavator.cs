// <copyright file="Elavator.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Elavator : MonoBehaviour
{
  public bool Status;
  public bool SlowStatus;
  public Material UnlockedMaterial;
  public float DescendSpeed;
  public float DoorCloseSpeed;
  public float SlowFactor;
  public float SlowDuration;
  private const float RaiseHeight = 3.0f;
  private GameObject elevatorFloor;
  private GameObject elevatorDoor;
  private GameObject elevatorSlowTerminal;
  private GameObject elevatorSparks;
  private float dropHeight;
  private float currentAmountDescended;
  private float currentDoorAscended;
  private float currentSlowDuration;
  private float slowDescendSpeed;
  
  public void Activate()
  {
    if (!Status)
    {
      Status = true;
      Renderer screen = transform.Find("prop_switchUnit_screen").renderer;
      screen.material = UnlockedMaterial;
      currentAmountDescended = 0.0f;
      currentDoorAscended = 0.0f;
    }
  }

  public void ActivateSlow()
  {
    if (Status && !SlowStatus)
    {
      Renderer screen = elevatorSlowTerminal.transform.Find("prop_switchUnit_screen").renderer;
      screen.material = UnlockedMaterial;
      currentSlowDuration = 0.0f;
      SlowStatus = true;
      elevatorSparks.particleSystem.Play();
    }
  }

  internal void Awake()
  {
    Status = false;
    SlowStatus = false;
    RaycastHit hit;
    slowDescendSpeed = DescendSpeed / SlowFactor;
    elevatorFloor = GameObject.FindGameObjectWithTag("ElevatorFloor");
    elevatorDoor = GameObject.FindGameObjectWithTag("ElevatorDoor");
    elevatorSlowTerminal = GameObject.FindGameObjectWithTag("ElevatorSlowTerminal");
    elevatorSparks = GameObject.FindGameObjectWithTag("ElevatorSparks");
    if (Physics.Raycast(elevatorFloor.transform.position, Vector3.down, out hit, 1000.0f, ~(1 << LayerMask.NameToLayer("Ignore Raycast"))))
    {
      dropHeight = elevatorFloor.transform.position.y - hit.point.y;
    }
    else
    {
      dropHeight = 13.5245f; // Estimated height as backup
    }
  }

  internal void Update()
  {
    if (Status)
    {
      if (currentAmountDescended < dropHeight)
      {
        float deltaHeight;
        if (SlowStatus && currentSlowDuration < SlowDuration)
        {
          deltaHeight = slowDescendSpeed * Time.deltaTime;
          currentSlowDuration += Time.deltaTime;
        }
        else
        {
          deltaHeight = DescendSpeed * Time.deltaTime;
        }

        elevatorFloor.transform.position = elevatorFloor.transform.position + (Vector3.down * deltaHeight);
        currentAmountDescended += deltaHeight;
      }

      if (currentDoorAscended < RaiseHeight)
      {
        float deltaHeight = DoorCloseSpeed * Time.deltaTime;
        elevatorDoor.transform.localPosition = elevatorDoor.transform.localPosition + (Vector3.up * deltaHeight);
        currentDoorAscended += deltaHeight;
        if (currentDoorAscended > RaiseHeight)
        {
          elevatorDoor.transform.localPosition = new Vector3(elevatorDoor.transform.localPosition.x, -3.0f, elevatorDoor.transform.localPosition.z);
        }
      }
    }
  }
}
