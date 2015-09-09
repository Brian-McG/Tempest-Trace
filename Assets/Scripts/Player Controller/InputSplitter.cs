using UnityEngine;
using System;

public class InputSplitter
{
  private InputSplitter()
  {
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
}
