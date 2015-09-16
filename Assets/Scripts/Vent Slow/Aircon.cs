// <copyright file="Aircon.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Aircon : MonoBehaviour
{
  public bool Status = false;
  public Material UnlockedMaterial;
  public GameObject AirconAnimator;
  public GameObject AirconParticle;
  public AudioSource ActivatorSound;
  private Animator airconAnimator;
  private ParticleSystem airconParticle;

  public void Activate()
  {
    if (!Status)
    {
      ActivatorSound.Play();
      Status = true;
      Renderer screen = transform.Find("prop_switchUnit_screen").renderer;
      screen.material = UnlockedMaterial;
      airconAnimator.SetBool("AirConOn", Status);
      airconParticle.Play();
    }
  }

  internal void Awake()
  {
    airconAnimator = AirconAnimator.GetComponent<Animator>();
    airconParticle = AirconParticle.GetComponent<ParticleSystem>();
  }
}
