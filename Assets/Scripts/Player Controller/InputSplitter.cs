// <copyright file="InputSplitter.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using UnityEngine;

public class InputSplitter
{
  private InputSplitter()
  {
  }

  public static float GetHorizontalViewAxis(int player)
  {
    if (player == 0)
    {
      return Input.GetAxis("ViewX");
    }
    else if (player == 1)
    {
      return Input.GetAxis("ViewXJoy");
    }

    return 0.0f;
  }

  public static float GetVerticalViewAxis(int player)
  {
    if (player == 0)
    {
      return Input.GetAxis("ViewY");
    }
    else if (player == 1)
    {
      return Input.GetAxis("ViewYJoy");
    }

    return 0.0f;
  }

  public static float GetHorizontalAxis(int player)
  {
    if (player == 0)
    {
      return Input.GetAxis("Horizontal");
    }
    else if (player == 1)
    {
      return Input.GetAxis("HorizontalJoy");
    }

    return 0.0f;
  }

  public static float GetVerticalAxis(int player)
  {
    if (player == 0)
    {
      return Input.GetAxis("Vertical");
    }
    else if (player == 1)
    {
      return Input.GetAxis("VerticalJoy");
    }

    return 0.0f;
  }

  public static bool GetJumpPressed(int player)
  {
    if (player == 0)
    {
      return Input.GetButtonDown("Jump");
    }
    else if (player == 1)
    {
      return Input.GetButtonDown("JumpJoy");
    }

    return false;
  }

  public static bool GetSlidePressed(int player)
  {
    if (player == 0)
    {
      return Input.GetButtonDown("Slide");
    }
    else if (player == 1)
    {
      return Input.GetButtonDown("SlideJoy");
    }

    return false;
  }

  public static bool GetSlide(int player)
  {
    if (player == 0)
    {
      return Input.GetButton("Slide");
    }
    else if (player == 1)
    {
      return Input.GetButton("SlideJoy");
    }

    return false;
  }

  public static bool GetInteractPressed(int player)
  {
    if (player == 0)
    {
      return Input.GetButtonDown("Interact");
    }
    else if (player == 1)
    {
      return Input.GetButtonDown("InteractJoy");
    }

    return false;
  }

  public static bool GetSmokePressed(int player)
  {
    if (player == 0)
    {
      return Input.GetButtonDown("Smoke");
    }
    else if (player == 1)
    {
      return Input.GetButtonDown("SmokeJoy");
    }

    return false;
  }
}
