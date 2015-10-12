using UnityEngine;
using System.Collections;
namespace Domain.UserInterface.Menu
{
  public class ChangeScene : MonoBehaviour
  {
    public void Start()
    {
      Screen.showCursor = true;
    }
    // Update is called once per frame
    public void ChangeToScene(int address)
    {
      Application.LoadLevel(address);
    }
    public void Quit()
    {
      Application.Quit();
    }
  }
}