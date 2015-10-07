// <copyright file="MusicController.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using Domain.Player.Checkpoint;

namespace Domain.Sound
{
  public class MusicController : MonoBehaviour
  {
    public byte[] IndicesToTransitionTo;
    public AudioSource[] MusicList;
    public AudioSource LastMusic;
    public float TransitionSpeed;
    private float volume;
    private byte currentIndex;
    private CheckpointController playerOneCheckpoint;
    private CheckpointController playerTwoCheckpoint;
    private bool transitioning;
    private bool endTransition;
    internal void Awake()
    {
      volume = MusicList[0].volume;
      transitioning = false;
      endTransition = false;
      playerOneCheckpoint = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<CheckpointController>();
      GameObject playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");
      if (playerTwo != null)
      { 
        playerTwoCheckpoint = playerTwo.GetComponent<CheckpointController>();
      }
    }

    internal void Update()
    {
      // Debug.Log("CurrentIndex: " + currentIndex + "\nNumberPassed: " + playerOneCheckpoint.NumberPassed);
      if (!transitioning && currentIndex < IndicesToTransitionTo.Length && (playerOneCheckpoint.NumberPassed >= IndicesToTransitionTo[currentIndex] || (playerTwoCheckpoint != null && playerTwoCheckpoint.NumberPassed >= IndicesToTransitionTo[currentIndex])))
      {
        ++currentIndex;
        transitioning = true;
        MusicList[currentIndex].volume = 0;
        MusicList[currentIndex].Play();
      }
      if (transitioning)
      {
        MusicList[currentIndex - 1].volume -= Time.deltaTime * TransitionSpeed;
        MusicList[currentIndex].volume += Time.deltaTime * TransitionSpeed;
        if (MusicList[currentIndex].volume > (volume - 0.1f))
        {
          MusicList[currentIndex - 1].volume = 0;
          MusicList[currentIndex - 1].Stop();
          MusicList[currentIndex].volume = volume;
          transitioning = false;
        }
      }
      if (!endTransition && currentIndex == IndicesToTransitionTo.Length && !MusicList[currentIndex].isPlaying)
      {
        endTransition = true;
        LastMusic.volume = 0;
        LastMusic.Play();
      }
      if (endTransition)
      {
        MusicList[currentIndex].volume -= Time.deltaTime * TransitionSpeed;
        LastMusic.volume += Time.deltaTime * TransitionSpeed;
        if (LastMusic.volume > (volume - 0.1f))
        {
          MusicList[currentIndex].volume = 0;
          MusicList[currentIndex].Stop();
          LastMusic.volume = volume;
          ++currentIndex;
          endTransition = false;
        }
      }
    }
  }
}
