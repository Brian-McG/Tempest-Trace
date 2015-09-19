// <copyright file="DirectableObject.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

namespace Domain.ArtificialIntelligence
{
/// <summary>
/// An object that can face some direction.
/// </summary>
  public class DirectableObject
  {
    private float rotateSpeed;
    private GameObject directableObject;
  
    public DirectableObject(float rotSpeed,
                          GameObject directableObject)
    {
      this.directableObject = directableObject;
      this.rotateSpeed = rotSpeed;
    }
  
    public float RotateSpeed
    {
      get
      {
        return this.rotateSpeed;
      }
    }

    /// <summary>
    /// Rotates object by an increment to a given direction.
    /// </summary>
    /// <param name="direction">Direction.</param>
    /// <param name="rotationRate">Rotation rate.</param>
    public void FaceDirection(Vector3 direction, float rotationRate = 9001.0f)
    {
      float localRotationRate = this.rotateSpeed;
      if (System.Math.Abs(rotationRate - 9001.0f) > Mathf.Epsilon)
      {
        localRotationRate = rotationRate;
      }

      float angle = this.AngleBetween(new Vector3(this.directableObject.transform.forward.x, 0.0f, this.directableObject.transform.forward.z),
                                    new Vector3(direction.x, 0.0f, direction.z),
                                    Vector3.up);
      if (angle < 90.0f || angle > 270.0f)
      {
        Vector3 increment = Vector3.RotateTowards(this.directableObject.transform.forward, direction, localRotationRate * Time.deltaTime, 0.0f);
        this.directableObject.transform.rotation = Quaternion.LookRotation(increment);
      }
      else
      {
        Vector3 increment = Vector3.RotateTowards(this.directableObject.transform.forward, new Vector3(direction.x, 0.0f, direction.z), localRotationRate * Time.deltaTime, 0.0f);
        this.directableObject.transform.rotation = Quaternion.LookRotation(increment);
      }
    }

    /// <summary>
    /// Calcualates angle between directionOne and directionTwo for a given normal.
    /// </summary>
    /// <returns>An angle</returns>
    /// <param name="directionOne">Direction one.</param>
    /// <param name="directionTwo">Direction two.</param>
    /// <param name="normal">Normal.</param>
    public float AngleBetween(Vector3 directionOne, Vector3 directionTwo, Vector3 normal)
    {
      // [0,180]
      float angle = Vector3.Angle(directionOne, directionTwo);
      float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(directionOne, directionTwo))); 
    
      // [-179,180]
      float signed_angle = angle * sign;
    
      // angle in [0,360] (not used but included here for completeness)
      float angle360 = (signed_angle + 180) % 360;
      angle360 += 180;
      if (angle360 >= 360.0f)
      {
        angle360 -= 360.0f;
      }
    
      return angle360;
    }
  }
}