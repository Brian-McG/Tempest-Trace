// <copyright file="CountDown.cs" company="University of Cape Town">
//     Brian Mc George
//     MCGBRI004
// </copyright>
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UserInterface.HeadsUpDisplay;
namespace Domain.GameStart
{
  public class CountDown : MonoBehaviour
  {
    public Text CountDownText;
    public GameTime GameTimeObj;
    public bool IsActive;
    private float currentScale;
    private float currentTime;
    private byte timeToSet;
    private bool up;
    private bool finished;

    public bool Finished
    {
      get
      {
        return finished;
      }
    }

    public void StartTimer()
    {
      GameTimeObj.PlayerOneActive = false;
      GameTimeObj.PlayerTwoActive = false;
      GameTimeObj.ResetTime();
      IsActive = true;
      Awake();
    }

    public void EndTimer()
    {
      GameTimeObj.PlayerOneActive = true;
      GameTimeObj.PlayerTwoActive = true;
      GameTimeObj.ResetTime();
      finished = true;
      IsActive = false;
    }

    internal void Awake()
    {  
      currentScale = 1.0f;
      currentTime = 0.0f;
      timeToSet = 5;
      up = false;
      finished = false;
    }

    internal void Update()
    {
      if (!IsActive)
      {
        CountDownText.enabled = false;
      }
      else
      {
        CountDownText.enabled = true;
        currentTime += Time.deltaTime;
        if (!up)
        {
          currentScale -= Time.deltaTime * 2;
        }
        else
        {
          currentScale += Time.deltaTime * 2;
        }

        if (currentTime <= 5.0f)
        {
          if (currentScale < 1.0f && currentScale > 0)
          {
            CountDownText.gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
          }
          else if (currentScale <= 0)
          {
            currentScale = 0;
            up = true;
            --timeToSet;
            if (timeToSet == 0)
            {
              EndTimer();
            }
            CountDownText.text = "" + timeToSet;
          }
          else if (currentScale >= 1.0f)
          {
            currentScale = 1.0f;
            up = false;
          }
        }
      }
    }
  }
}