// <copyright file="CinematicCameraMover.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Lerp camera between a set of waypoints.
/// </summary>
public class CinematicCameraMover : MonoBehaviour
{
  public float Speed;
  public float TransformEpsilon = 1f;
  public float RotationEpsilon = 1f;
  public float ScaleEpsilon = 1f;
  public GameObject[] Locations;
  private int currentLocation;
  private float lerpTime = 10f;
  private float currentLerpTime;
  private Transform startTransform;

  public void LerpTransform(Transform t1, Transform t2, float t)
  {
    this.camera.transform.position = Vector3.Lerp(t1.position, t2.position, t);
    this.camera.transform.transform.rotation = Quaternion.Lerp(t1.rotation, t2.rotation, t);
    this.camera.transform.transform.localScale = Vector3.Lerp(t1.localScale, t2.localScale, t);
  }

  internal void Start()
  { 
    currentLocation = 0;
    startTransform = Locations[0].transform;
  }

  internal void Update()
  {
    ApplyCameraLerp();
  }

  private void ApplyCameraLerp()
  {
    if (currentLocation + 2 < Locations.Length)
    {
      currentLerpTime += Time.deltaTime;
      if (currentLerpTime > lerpTime)
      {
        currentLerpTime = 0;
        ++currentLocation;
        startTransform = Locations[currentLocation].transform;
      }
    }
    
    float perc = currentLerpTime / lerpTime;
    LerpTransform(startTransform, Locations[currentLocation + 1].transform, perc);
  }
}
