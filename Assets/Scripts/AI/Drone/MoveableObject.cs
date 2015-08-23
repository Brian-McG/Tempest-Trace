﻿// <copyright file="MoveableObject.cs" company="University of Cape Town">
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
                        float rotateSpeed,
                        GameObject moveableObject)
  {
    this.moveObj = moveableObject;
    this.moveRigidbody = this.moveObj.rigidbody;
    this.moveSpeed = moveSpd;
    this.rotateSpeed = rotateSpeed;
  }

  public void Move(Vector3 direction)
  {
    moveRigidbody.AddForce(direction * moveSpeed);
  }

  public void FaceDirection(Vector3 location)
  {
    Vector3 direction = location - moveObj.transform.position;
    float angle = Vector3.Angle(moveObj.transform.forward, location);

    float rotate;

    if (angle > 2.0f)
    {
      rotate = -rotateSpeed;
    }
    else if (angle < -2.0f)
    {
      rotate = rotateSpeed;
    }
    else
    {
      rotate = 0.0f;
    }
    Debug.Log(angle);
    moveObj.transform.localEulerAngles = new Vector3(
      moveObj.transform.localEulerAngles.x,
      moveObj.transform.localEulerAngles.y + (rotate * Time.deltaTime),
      moveObj.transform.localEulerAngles.z);
  }
}