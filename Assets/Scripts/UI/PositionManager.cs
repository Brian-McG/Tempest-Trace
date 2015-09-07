// <copyright file="PositionManager.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PositionManager : MonoBehaviour
{
  public GameObject PlayerOne;
  public GameObject PlayerTwo;
  public GameObject CheckpointParent;
  public GameObject PlayerOnePosition;
  public GameObject PlayerTwoPosition;
  private GameObject[] checkpoints;
  private Checkpoint checkpointP1;
  private Checkpoint checkpointP2;
  private Text playerOnePosition;
  private Text playerTwoPosition;

  internal void Awake()
  { 
    checkpoints = new GameObject[CheckpointParent.transform.childCount];
    for (int i = 0; i < CheckpointParent.transform.childCount; ++i)
    {
      checkpoints[i] = CheckpointParent.transform.GetChild(i).gameObject;
    }

    checkpointP1 = PlayerOne.GetComponent<Checkpoint>();
    checkpointP2 = PlayerTwo.GetComponent<Checkpoint>();
    playerOnePosition = PlayerOnePosition.GetComponent<Text>();
    playerTwoPosition = PlayerTwoPosition.GetComponent<Text>();
  }

  internal void Update()
  {
    if (checkpointP1.NumberPassed > checkpointP2.NumberPassed)
    {
      SetP1Ahead();
    }
    else if (checkpointP1.NumberPassed < checkpointP2.NumberPassed)
    {
      SetP2Ahead();
    }
    else
    {

      uint nextCheckpointIndex = checkpointP1.NumberPassed;
      /*
      uint currentCheckpointIndex = checkpointP1.NumberPassed - 1;
      Vector3 a = checkpoints[currentCheckpointIndex].transform.position;
      Vector3 b = checkpoints[nextCheckpointIndex].transform.position;
      Vector3 p1 = PlayerOne.transform.position;
      Vector3 p2 = PlayerTwo.transform.position;
      Vector3 ap1 = p1 - a;
      Vector3 ap2 = p2 - a;
      Vector3 ab = b - a;
      Vector3 onTheLineP1 = a + ((Vector3.Dot(ap1, ab) / Vector3.Dot(ab, ab)) * ab);
      Vector3 onTheLineP2 = a + ((Vector3.Dot(ap2, ab) / Vector3.Dot(ab, ab)) * ab);
      */
      Vector3 p1 = PlayerOne.transform.position;
      Vector3 p2 = PlayerTwo.transform.position;
      Vector3 b = checkpoints[nextCheckpointIndex].transform.position;
      Vector3 p1b = b - p1;
      Vector3 p2b = b - p2;
      if (p2b.magnitude >= p1b.magnitude)
      {
        SetP1Ahead();
      }
      else
      {
        SetP2Ahead();
      }
    }
  }

  private void SetP1Ahead()
  {
    playerOnePosition.text = "Position\n1 of 2";
    playerTwoPosition.text = "Position\n2 of 2";
  }

  private void SetP2Ahead()
  {
    playerOnePosition.text = "Position\n2 of 2";
    playerTwoPosition.text = "Position\n1 of 2";
  }
}
