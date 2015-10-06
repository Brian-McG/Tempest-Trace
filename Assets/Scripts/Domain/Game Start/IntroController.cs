// <copyright file="CinematicCameraMover.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Player.Health;
namespace Domain.GameStart
{
/// <summary>
/// Lerp camera between a set of waypoints.
/// </summary>
  public class IntroController : MonoBehaviour
  {
    public float[] Speed;
    public float TransformEpsilon = 1f;
    public float RotationEpsilon = 1f;
    public float ScaleEpsilon = 1f;
    public GameObject[] Locations;
    public AudioSource introDialog;
    public AudioSource startGameSound;
    private int currentLocation;
    private float lerpTime = 10f;
    private float currentLerpTime;
    private Transform startTransform;
    private PlayerLifeHandler fadeController;
    private bool runClose;

    public void LerpTransform(Transform t1, Transform t2, float t)
    {
      this.camera.transform.position = Vector3.Lerp(t1.position, t2.position, t);
      this.camera.transform.transform.rotation = Quaternion.Lerp(t1.rotation, t2.rotation, t);
      this.camera.transform.transform.localScale = Vector3.Lerp(t1.localScale, t2.localScale, t);
    }

    internal void Start()
    { 
      currentLerpTime = 0;
      currentLocation = 0;
      startTransform = Locations[0].transform;
      fadeController = this.gameObject.GetComponent<PlayerLifeHandler>();
      fadeController.BlackToClear();
      runClose = false;
    }

    internal void Update()
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