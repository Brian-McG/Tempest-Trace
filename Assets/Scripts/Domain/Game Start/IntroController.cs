// <copyright file="CinematicCameraMover.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Player.Health;
using UserInterface.HeadsUpDisplay;
namespace Domain.GameStart
{
/// <summary>
/// Lerp camera between a set of waypoints.
/// </summary>
  public class IntroController : MonoBehaviour
  {
    public bool Enabled;
    public float[] Speed;
    public float TransformEpsilon = 1f;
    public float RotationEpsilon = 1f;
    public float ScaleEpsilon = 1f;
    public GameObject[] Locations;
    public AudioSource IntroDialog;
    public AudioSource StartGameSound;
    private int currentLocation;
    private float lerpTime = 10f;
    private float currentLerpTime;
    private Transform startTransform;
    private PlayerLifeHandler fadeController;
    private bool runClose;
    private GameObject playerOne;
    private GameObject playerTwo;
    private GameObject[] playerCameras;
    private GameObject playerOneUI;
    private GameObject playerTwoUI;

    public void LerpTransform(Transform t1, Transform t2, float t)
    {
      this.camera.transform.position = Vector3.Lerp(t1.position, t2.position, t);
      this.camera.transform.transform.rotation = Quaternion.Lerp(t1.rotation, t2.rotation, t);
      this.camera.transform.transform.localScale = Vector3.Lerp(t1.localScale, t2.localScale, t);
    }

    internal void Start()
    {
      playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
      playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
      playerCameras = GameObject.FindGameObjectsWithTag("MainCamera");
      playerOneUI = GameObject.FindGameObjectWithTag("PlayerOneUI");
      playerTwoUI = GameObject.FindGameObjectWithTag("PlayerTwoUI");
      DisablePlayerMode();
      if (Enabled)
      {
        currentLerpTime = 0;
        currentLocation = 0;
        startTransform = Locations[0].transform;
        fadeController = this.gameObject.GetComponent<PlayerLifeHandler>();
        fadeController.BlackToClear();
        runClose = false;
      }
      else
      {
        EnablePlayerMode();
      }
    }

    private void DisablePlayerMode()
    {
      playerOne.GetComponent<PlayerLifeHandler>().AllowMovement = false;
      playerTwo.GetComponent<PlayerLifeHandler>().AllowMovement = false;
      playerOneUI.SetActive(false);
      playerTwoUI.SetActive(false);
      foreach (GameObject cameraObj in playerCameras)
      {
        cameraObj.camera.enabled = false;
      }
    }

    private void EnablePlayerMode()
    {
      IntroDialog.Stop();
      this.gameObject.GetComponent<AudioListener>().enabled = false;
      this.gameObject.camera.enabled = false;
      playerOne.GetComponentInChildren<AudioListener>().enabled = true;
      StartGameSound.Play();
      playerOneUI.SetActive(true);
      playerTwoUI.SetActive(true);
      PlayerLifeHandler playerOneLifeHander = playerOne.GetComponent<PlayerLifeHandler>();
      PlayerLifeHandler playerTwoLifeHander = playerTwo.GetComponent<PlayerLifeHandler>();
      playerOneLifeHander.BlackToClear();
      playerTwoLifeHander.BlackToClear();
      GameObject.FindGameObjectWithTag("UIController").GetComponent<GameTime>().ResetTime();
      playerOneLifeHander.AllowMovement = true;
      playerTwoLifeHander.AllowMovement = true;
      foreach (GameObject cameraObj in playerCameras)
      {
        cameraObj.camera.enabled = true;
      }

      enabled = false;
    }

    internal void Update()
    {
      if (Enabled)
      {
        if (currentLocation + 1 != Locations.Length)
        {
          ApplyCameraLerp();
        }
        if (!runClose && currentLocation + 1 == Locations.Length)
        {
          fadeController.KillPlayer();
          runClose = true;
        }
        if (runClose)
        {
          if (fadeController.FadeoutAlpha > 0.95)
          {
            EnablePlayerMode();
          }
        }
      }
    }

    private void ApplyCameraLerp()
    {
      if (currentLocation + 1 < Locations.Length)
      {
        if (currentLocation < Speed.Length)
        {
          currentLerpTime += Speed[currentLocation] * Time.deltaTime;
        }
        else
        {
          currentLerpTime += Speed[Speed.Length - 1] * Time.deltaTime;
        }
        if (currentLerpTime > 1)
        {
          currentLerpTime = 0;
          ++currentLocation;
          startTransform = Locations[currentLocation].transform;
        }
      }
      if (currentLocation + 1 != Locations.Length)
      {
        LerpTransform(startTransform, Locations[currentLocation + 1].transform, currentLerpTime);
      }
    }
  }
}