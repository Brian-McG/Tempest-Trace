// <copyright file="SmokebombController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

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

  private GameObject hand;
  private int currentBombCount;
  private Animator animator;
  private int animParamThrowing;

  internal void Awake()
  {
    if (!DepthFirstSearch("RightHand_bind", transform))
    {
      Debug.LogError("Failed to find player hand for bomb throw.");
    }

    currentBombCount = 0;
    animator = GetComponentInChildren<Animator>();
    animParamThrowing = Animator.StringToHash("IsThrowing");
  }

  internal void Update()
  {
    animator.SetBool(animParamThrowing, false);
  }

  /// <summary>
  /// Plays animation if smoke bomb should be deployed.
  /// </summary>
  public void Activate()
  {
    if (currentBombCount < NumberOfBombs)
    {
      StartCoroutine(ThrowSmokebomb());
      ++currentBombCount;
      animator.SetBool(animParamThrowing, true);
    }
  }

  private IEnumerator ThrowSmokebomb()
  {
    yield return new WaitForSeconds(0.7f);
    Instantiate(SmokeBomb, hand.transform.position, Quaternion.identity);
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
