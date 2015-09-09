// <copyright file="MoveableObject.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class MoveableObject
{
  private float moveSpeed;
  private float rotateSpeed;
  private GameObject moveObj;
  private Rigidbody moveRigidbody;

  public MoveableObject(float moveSpd,
                        float rotSpeed,
                        GameObject moveableObject)
  {
    moveObj = moveableObject;
    moveRigidbody = this.moveObj.rigidbody;
    moveSpeed = moveSpd;
    rotateSpeed = rotSpeed;
  }

  public void Move(Vector3 location)
  {
    float angle = AngleBetween(new Vector3(moveObj.transform.forward.x, 0.0f, moveObj.transform.forward.z),
                               new Vector3(location.x, 0.0f, location.z),
                               Vector3.up);
    if (angle < 2.0f)
    {
      moveRigidbody.transform.position = moveRigidbody.transform.position + (location.normalized * moveSpeed * Time.deltaTime);
    }
  }

  public void FaceDirection(Vector3 location)
  {
    float angle = AngleBetween(new Vector3(moveObj.transform.forward.x, 0.0f, moveObj.transform.forward.z),
                               new Vector3(location.x, 0.0f, location.z),
                               Vector3.up);

    float rotate;
    if (angle > 2 && angle <= 180.0f)
    {
      rotate = rotateSpeed;
    }
    else
      if (angle > 180.0f)
    {
      rotate = -rotateSpeed;
    }
    else
    {
      rotate = 0.0f;
    }

    moveObj.transform.localEulerAngles = new Vector3(
      moveObj.transform.localEulerAngles.x,
      moveObj.transform.localEulerAngles.y + (rotate * Time.deltaTime),
      moveObj.transform.localEulerAngles.z);
  }

  public float AngleBetween(Vector3 a, Vector3 b, Vector3 n)
  {
    //[0,180]
    float angle = Vector3.Angle(a, b);
    float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));  
    //[-179,180]
    float signed_angle = angle * sign;

    // angle in [0,360] (not used but included here for completeness)
    float angle360 = (signed_angle + 180) % 360;
    angle360 += 180;
    if (angle360 >= 360.0f)
    {
      angle360 -= 360.0f;
      ;
    }

    return angle360;
  }
}
