﻿// <copyright file="SmokebombController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
namespace Domain.Interactables.Smokebomb
{
/// <summary>
/// Controls the spawning of smoke bombs at the players hand.
/// </summary>
  public class SmokebombController : MonoBehaviour
  {
    [Tooltip("Smokebomb prefab.")]
    public GameObject
      SmokeBomb;
    [Tooltip("Maximum number of bombs player can deploy.")]
    public int
      NumberOfBombs;
    [Tooltip("Smokebomb shield")]
    public GameObject
      SmokeBombShield;

    private GameObject hand;
    private int currentBombCount;
    private Animator[] animators;
    private int animParamThrowing;
    private float floorYPos;

    public int CurrentBombCount
    {
      get
      {
        return currentBombCount;
      }
    }

    internal void Awake()
    {
      if (!DepthFirstSearch("RightHand_bind", transform))
      {
        Debug.LogError("Failed to find player hand for bomb throw.");
      }

      currentBombCount = 0;
      animators = GetComponentsInChildren<Animator>();
      animParamThrowing = Animator.StringToHash("IsThrowing");
    }

    internal void Update()
    {
      for (int i = 0; i < animators.Length; ++i)
      {
        animators[i].SetBool(animParamThrowing, false);
      }
    }

    /// <summary>
    /// Plays animation if smoke bomb should be deployed.
    /// </summary>
    public void Activate()
    {
      if (currentBombCount < NumberOfBombs)
      {
        RaycastHit hit;
        int inverseLayer = ~(1 << 8 | 1 << 9 | 1 << 10 | 1 << 11 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 15 | 1 << 16 | 1 << 18 | 1 << 19);
        if (Physics.Raycast(hand.transform.position, Vector3.down, out hit, 100f, inverseLayer))
        {
          Debug.Log(hit.collider.name);
          Debug.Log(hit.point);
          floorYPos = hit.point.y;
          GameObject shield = Instantiate(SmokeBombShield, this.gameObject.transform.position, Quaternion.identity) as GameObject;
          shield.transform.parent = this.gameObject.transform;
          ++currentBombCount;
          StartCoroutine(ThrowSmokebomb());
          for (int i = 0; i < animators.Length; ++i)
          {
            animators[i].SetBool(animParamThrowing, true);
          }
        }
      }
    }

    private IEnumerator ThrowSmokebomb()
    {
      yield return new WaitForSeconds(0.7f);
      GameObject bomb = Instantiate(SmokeBomb, hand.transform.position, Quaternion.identity) as GameObject;
      bomb.GetComponent<SmokeBomb>().YActivationValue = floorYPos;
    }

    /// <summary>
    /// Set reference to hand object by searching player model tree until the hand is found.
    /// </summary>
    /// <returns><c>true</c>If the hand is found<c>false</c>Otherwise</returns>
    /// <param name="searchName">Name of the game object</param>
    /// <param name="currentTransform">Transform of current object traversed to</param>
    private bool DepthFirstSearch(string searchName, Transform currentTransform)
    {
      foreach (Transform t in currentTransform)
      {
        if (t.name == searchName)
        {
          hand = t.gameObject;
          return true;
        }
        else
        {
          bool result = DepthFirstSearch(searchName, t);
          if (result)
          {
            return true;
          }
        }
      }

      return false;
    }
  }
}