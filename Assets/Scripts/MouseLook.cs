using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {
	public float looksensitivity = 5f;
	public float yrotation;
	public float xrotation;
	public float Currentyrotation;
	public float Currentxrotation; 
	public float yRotaionV; 
	public float xRotationV; 
	public float lookSmoothDamp = 0.1f;
	public float speed = 5;
	
	void Update () {
		if (Input.GetKey (KeyCode.Escape)) {
			Application.Quit();
		}
			yrotation += Input.GetAxis ("Mouse X") * looksensitivity;
			xrotation -= Input.GetAxis ("Mouse Y") * looksensitivity;
			Currentxrotation = Mathf.SmoothDamp (Currentxrotation, xrotation, ref xRotationV, lookSmoothDamp);
			Currentyrotation = Mathf.SmoothDamp (Currentyrotation, yrotation, ref yRotaionV, lookSmoothDamp);
			transform.rotation = Quaternion.Euler (xrotation, yrotation, 0);
		float forwards = Input.GetAxis ("Vertical");
		float sideways = Input.GetAxis ("Horizontal");
		transform.position = transform.position + transform.forward*Time.deltaTime*speed*forwards;
		transform.position = transform.position + transform.right*Time.deltaTime*speed*sideways;
		if (Input.GetKey (KeyCode.Space)) {
			transform.position = transform.position + transform.up*Time.deltaTime*speed;
		}
		if (Input.GetKey (KeyCode.LeftControl)) {
			transform.position = transform.position - transform.up*Time.deltaTime*speed;
		}
	} 
}
