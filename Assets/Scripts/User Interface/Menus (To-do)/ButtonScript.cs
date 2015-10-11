using UnityEngine;
using System.Collections;
namespace Domain.UserInterface.Menu
{
  public class ButtonScript : MonoBehaviour
  {
    public GameObject liftCap;
    public GameObject shaftCap;
    public GameObject arrows1;
    public GameObject arrows2;
    // Use this for initialization
    public void ModifyWorld()
    {
      liftCap.transform.position += new Vector3(-5, 0, 0);
      shaftCap.transform.position += new Vector3(0, 0, 20);
      arrows1.SetActive(false);
      arrows2.SetActive(true);
    }
  }
}