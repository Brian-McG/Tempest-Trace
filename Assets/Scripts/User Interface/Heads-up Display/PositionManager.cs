// <copyright file="PositionManager.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Domain.Player.Checkpoint;

namespace UserInterface.HeadsUpDisplay
{
  public class PositionManager : MonoBehaviour
  {
    public GameObject PlayerOne;
    public GameObject PlayerTwo;
    public GameObject CheckpointParent;
    public GameObject PlayerOnePosition;
    public GameObject PlayerTwoPosition;
    private GameObject[] checkpoints;
    private CheckpointController checkpointP1;
    private CheckpointController checkpointP2;
    private Text playerOnePosition;
    private Text playerTwoPosition;

    internal void Awake()
    { 
      checkpoints = new GameObject[CheckpointParent.transform.childCount];
      for (int i = 0; i < CheckpointParent.transform.childCount; ++i)
      {
        checkpoints[i] = CheckpointParent.transform.GetChild(i).gameObject;
      }
      checkpointP1 = PlayerOne.GetComponent<CheckpointController>();
      playerOnePosition = PlayerOnePosition.GetComponent<Text>();

      if (PlayerTwo != null)
      {
        checkpointP2 = PlayerTwo.GetComponent<CheckpointController>();
        playerTwoPosition = PlayerTwoPosition.GetComponent<Text>();
      }
    }

    internal void Update()
    {
      UpdatePosition();
    }

    /// <summary>
    /// Update position on UI
    /// </summary>
    private void UpdatePosition()
    {
      if (PlayerTwo == null || checkpointP1.NumberPassed > checkpointP2.NumberPassed)
      {
        SetP1Ahead();
      }
      else if (checkpointP1.NumberPassed < checkpointP2.NumberPassed)
      {
        SetP2Ahead();
      }
      else
      {
        // Calculate the linear distance to next checkpoint
        // Player with lowest distance to next checkpoint is in the lead
        uint nextCheckpointIndex = checkpointP1.NumberPassed;
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
      if (PlayerTwo == null)
      {
        playerOnePosition.text = "Position\n1 of 1";
      }
      else
      {
        playerOnePosition.text = "Position\n1 of 2";
        playerTwoPosition.text = "Position\n2 of 2";
      }
    }

    private void SetP2Ahead()
    {
      playerOnePosition.text = "Position\n2 of 2";
      playerTwoPosition.text = "Position\n1 of 2";
    }
  }
}