// <copyright file="SmokeSpawn.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;

public class SmokeSpawn : MonoBehaviour
{
  public GameObject SmokeBomb;
  private GameObject hand;

  private FirstPersonMovement fpsMove;

  internal void Awake()
  {
    if (!DepthFirstSearch("RightHand_bind", transform))
    {
      Debug.LogError("Failed to find player hand for bomb throw.");
    }

    fpsMove = GetComponent<FirstPersonMovement>();
  }

  internal void Update()
  {
    if (InputSplitter.GetSmokePressed(fpsMove.PlayerID))
    {
      Instantiate(SmokeBomb, hand.transform.position, Quaternion.identity);
    }
  }

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
