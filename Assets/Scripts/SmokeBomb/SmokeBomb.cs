// <copyright file="SmokeBomb.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class SmokeBomb : MonoBehaviour
{
  [Tooltip("The amount the radius of the collider grows per time interval.")]
  public float
    ColliderGrowthRate;
  [Tooltip("The time at which the collider stops growing.")]
  public float
    GrowthStopTime;

  private SphereCollider smokeCollider;
  private float timer;
  private ParticleSystem smoke;
  private bool activated;

  internal void Awake()
  {  
    smokeCollider = this.gameObject.GetComponent<SphereCollider>();
    smoke = this.gameObject.GetComponentInChildren<ParticleSystem>();
    timer = 0.0f;
    activated = false;
  }

  internal void OnCollisionEnter(Collision collision)
  {
    if (!activated)
    {
      smoke.Play();
      activated = true;
    }

  }

  internal void Update()
  {
    if (!smoke.isPlaying && activated)
    {
      Destroy(this.gameObject);
    }
    if (timer < GrowthStopTime && activated)
    {
      smokeCollider.radius += ColliderGrowthRate * Time.deltaTime;
      timer += Time.deltaTime;
    }

  }
}
