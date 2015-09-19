// <copyright file="Elavator.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
namespace Domain.Interactables.Elevator
{
  public class Elevator : MonoBehaviour
  {
    [Tooltip("If elevator is active")]
    public bool
      Status;
    [Tooltip("If elvator has been slowed.")]
    public bool
      SlowStatus;
    [Tooltip("Material to change termimal status image to once it is pressed.")]
    public Material
      UnlockedMaterial;
    [Tooltip("Rate at which the elevator moves.")]
    public float
      DescendSpeed;
    [Tooltip("Rate at which the elevator door closes.")]
    public float
      DoorCloseSpeed;
    [Tooltip("Factor by which the elevator is slowed when slowTerminal is activated.")]
    public float
      SlowFactor;
    [Tooltip("How long the elevator is slowed when slowTerminal is activated..")]
    public float
      SlowDuration;
    [Tooltip("Sound played when terminal is pressed.")]
    public AudioSource
      ActivatorSound;
    [Tooltip("Sound played when slowTerminal is pressed.")]
    public AudioSource
      SlowActivatorSound;

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
    private AudioSource elevatorSoundEffect;

    /// <summary>
    /// Start moving elevator.
    /// Play elevator sound effect
    /// Set terminal to activated.
    /// </summary>
    public void Activate()
    {
      if (!Status)
      {
        ActivatorSound.Play();
        Status = true;
        Renderer screen = transform.Find("prop_switchUnit_screen").renderer;
        screen.material = UnlockedMaterial;
        currentAmountDescended = 0.0f;
        currentDoorAscended = 0.0f;
        elevatorSoundEffect.Play();
      }
    }

    /// <summary>
    /// Start slowing elevator movement.
    /// Set terminal to activated.
    /// Play spark particle effect.
    /// </summary>
    public void ActivateSlow()
    {
      if (Status && !SlowStatus)
      {
        SlowActivatorSound.Play();
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
      elevatorSoundEffect = elevatorFloor.GetComponent<AudioSource>();
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

    /// <summary>
    /// Update elevator
    /// </summary>
    internal void FixedUpdate()
    {
      if (Status)
      {
        MoveElevator();
      }
    }

    /// <summary>
    /// Moves elevator some increment
    /// Closes elevator door
    /// </summary>
    private void MoveElevator()
    {
      if (currentAmountDescended < dropHeight)
      {
        float deltaHeight;
        if (SlowStatus && currentSlowDuration < SlowDuration)
        {
          deltaHeight = slowDescendSpeed * Time.fixedDeltaTime;
          currentSlowDuration += Time.fixedDeltaTime;
        }
        else
        {
          deltaHeight = DescendSpeed * Time.fixedDeltaTime;
        }
      
        elevatorFloor.transform.position = elevatorFloor.transform.position + (Vector3.down * deltaHeight);
        currentAmountDescended += deltaHeight;
      }
      else
      {
        elevatorSoundEffect.Stop();
      }
    
      if (currentDoorAscended < RaiseHeight)
      {
        float deltaHeight = DoorCloseSpeed * Time.fixedDeltaTime;
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