// <copyright file="Aircon.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class Aircon : MonoBehaviour
{
  public bool Status = false;
  public GameObject PlayerOneCamera;
  public GameObject PlayerTwoCamera;
  public Material Unlocked;
  public GameObject AirconAnimator;
  public GameObject AirconParticle;
  public float RayCastDistance = 2.0f;
  private Animator airconAnimator;
  private ParticleSystem airconParticle;
  private int layerToHit = 1 << 8;

  internal void Start()
  {
    airconAnimator = AirconAnimator.GetComponent<Animator>();
    airconParticle = AirconParticle.GetComponent<ParticleSystem>();
  }

  internal void Update()
  {
    if (Input.GetButtonDown("Interact_P1") &&
      Physics.Raycast(PlayerOneCamera.transform.position, PlayerOneCamera.transform.forward.normalized, RayCastDistance, layerToHit))
    {
      Hit();
    }
    else if (Input.GetButtonDown("Interact_P2") &&
      Physics.Raycast(PlayerTwoCamera.transform.position, PlayerTwoCamera.transform.forward.normalized, RayCastDistance, layerToHit))
    {
      Hit();
    }
  }

  private void Hit()
  {
    Status = true;
    Renderer screen = transform.Find("prop_switchUnit_screen").renderer;
    screen.material = Unlocked;
    airconAnimator.SetBool("AirConOn", Status);
    airconParticle.Play();
  }
}
