// <copyright file="SmokeBomb.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
namespace Domain.Interactables.Smokebomb
{
/// <summary>
/// Manages behaviour of smokebomb
/// </summary>
  public class SmokeBomb : MonoBehaviour
  {
    [Tooltip("The amount the radius of the collider grows per time interval.")]
    public float
      ColliderGrowthRate;
    [Tooltip("The time at which the collider stops growing.")]
    public float
      GrowthStopTime;
    [Tooltip("The sound played when bomb hits the ground")]
    public AudioSource
      SmokeBombExplosionSound;
    [Tooltip("The sound played as smoke is released.")]
    public AudioSource
      SmokeBombExpansionSound;
    public float YActivationValue = float.MinValue;

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

    /*
    // Disabled for now, a more reliable implementation is getting applied.

    /// <summary>
    /// Smokebomb activates when it hits the ground
    /// </summary>
    /// <param name="collision">Object collided with</param>
    internal void OnCollisionEnter(Collision collision)
    {
      if (!activated)
      {
        smoke.Play();
        activated = true;
      }
    }
    */
    internal void Update()
    {
      if (!activated && transform.position.y <= YActivationValue)
      {
        transform.position = new Vector3(transform.position.x, YActivationValue, transform.position.z);
        smoke.Play();
        activated = true;
      }
      else if (!activated)
      {
        transform.Translate(new Vector3(0, -9.8f * Time.deltaTime, 0));
      }
      ExpandSmokebomb();
    }

    /// <summary>
    /// Updates smoke bomb expansion
    /// Increases radius of collider
    /// Increases growth timer
    /// Once the smoke bomb particle effect is complete, the smoke bomb is destroyed.
    /// </summary>
    private void ExpandSmokebomb()
    {
      if (!smoke.isPlaying && activated)
      {
        Destroy(this.gameObject);
      }
    
      if (timer < GrowthStopTime && activated)
      {
        smokeCollider.radius += ColliderGrowthRate * Time.deltaTime;
        timer += Time.deltaTime;
        if (!SmokeBombExplosionSound.isPlaying && !SmokeBombExpansionSound.isPlaying)
        {
          SmokeBombExpansionSound.Play();
        }
      }
      if (timer > GrowthStopTime && activated)
      {
        SmokeBombExpansionSound.Stop();
      }
    }
  }
}