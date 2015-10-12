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
	  if (address == 1 || address == 2) 
	  {
		Screen.showCursor = false;
	  }

      Application.LoadLevel(address);
    }
    public void Quit()
    {
      Application.Quit();
    }
  }
}