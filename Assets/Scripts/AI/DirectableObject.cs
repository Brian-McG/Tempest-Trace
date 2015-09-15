// <copyright file="DirectableObject.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

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
  
  public float AngleBetween(Vector3 a, Vector3 b, Vector3 n)
  {
    // [0,180]
    float angle = Vector3.Angle(a, b);
    float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b))); 
    
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
