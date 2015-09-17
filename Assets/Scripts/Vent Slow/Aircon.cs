// <copyright file="Aircon.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

/// <summary>
/// Blows steam from aircon when activated.
/// </summary>
public class Aircon : MonoBehaviour
{
  [Tooltip("Material that terminal interface changes to when terminal is activated.")]
  public Material
    UnlockedMaterial;
  [Tooltip("Reference to the aircon fan animator game object.")]
  public GameObject
    AirconAnimator;
  [Tooltip("Reference to aircon particle system game object")]
  public GameObject
    AirconParticle;
  [Tooltip("Reference to audio source that plays when the terminal is activated.")]
  public AudioSource
    ActivatorSound;

  private bool status;
  private Animator airconAnimator;
  private ParticleSystem airconParticle;

  /// <summary>
  /// Gets a value indicating whether this <see cref="Aircon"/> is active.
  /// </summary>
  public bool Status
  {
    get
    {
      return status;
    }
  }

  /// <summary>
  /// Activates aircon.
  /// </summary>
  public void Activate()
  {
    if (!Status)
    {
      ActivatorSound.Play();
      status = true;
      Renderer screen = transform.Find("prop_switchUnit_screen").renderer;
      screen.material = UnlockedMaterial;
      airconAnimator.SetBool("AirConOn", Status);
      airconParticle.Play();
    }
  }

  internal void Awake()
  {
    status = false;
    airconAnimator = AirconAnimator.GetComponent<Animator>();
    airconParticle = AirconParticle.GetComponent<ParticleSystem>();
  }
}
