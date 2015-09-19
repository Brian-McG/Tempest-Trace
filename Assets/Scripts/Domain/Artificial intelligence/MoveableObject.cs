// <copyright file="MoveableObject.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

namespace Domain.ArtificialIntelligence
{
/// <summary>
/// An object that can move in some direction.
/// </summary>
  public class MoveableObject : DirectableObject
  {
    private float moveSpeed;
    private GameObject moveObj;
    private Rigidbody moveRigidbody;

    public MoveableObject(float moveSpd,
    float rotSpeed,
    GameObject moveableObject)
    : base(rotSpeed, moveableObject)
    {
      this.moveObj = moveableObject;
      this.moveRigidbody = this.moveObj.rigidbody;
      this.moveSpeed = moveSpd;
    }

    public float MoveSpeed
    {
      get
      {
        return this.moveSpeed;
      }
    }

    /// <summary>
    /// Moves the object if it is facing the direction of movement.
    /// </summary>
    /// <param name="location">Location to move to.</param>
    public void Move(Vector3 direction)
    {
      // Calculates angle between the currect facing direction and direction of movement.
      float angle = this.AngleBetween(new Vector3(this.moveObj.transform.forward.x, 0.0f, this.moveObj.transform.forward.z),
                               new Vector3(direction.x, 0.0f, direction.z),
                               Vector3.up);
      if (angle < 2.0f || angle > 358.0f)
      {
        this.moveRigidbody.transform.position = this.moveRigidbody.transform.position + (direction.normalized * this.moveSpeed * Time.deltaTime);
      }
    }
  }
}