// <copyright file="Door.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
  public float OpenRate;
  public float DegreesToOpen;
  private bool status;
  private bool played;
  private float startAngle;
  private float totalOpened;

  public void Activate()
  {
    if (!status)
    {
      status = true;
    }
  }

  internal void Awake()
  {  
    status = false;
    played = false;
    startAngle = transform.localEulerAngles.y;
    totalOpened = 0.0f;
  }

  internal void Update()
  {
    if (status && !played && totalOpened < DegreesToOpen)
    {
      float deltaAngle = OpenRate * Time.deltaTime;
      totalOpened += deltaAngle;
      transform.Rotate(new Vector3(0, deltaAngle, 0));
    }
  }
}
