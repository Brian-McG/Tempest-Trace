using UnityEngine;
using System.Collections;

public class Jumping : MonoBehaviour {
	public int jumpMode=0;
	public bool jumpable;
	public float height;
	private RaycastHit jPoint;
	private RaycastHit hCheck;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//jumpMode = 0;
		//jumpable = false;
		//Debug.DrawRay (transform.position, transform.forward, Color.blue);
		if (Input.GetButtonDown ("Jump")) {
			jumpMode =1;
			Physics.Raycast (transform.position, transform.forward, out jPoint, 10.0f);
			jumpable = (jPoint.transform.tag == "Jumpable");
			if(jPoint.distance<4.5 && jumpable){ // jump over obstacle
				Vector3 pointer = new Vector3(jPoint.point.x + transform.forward.x*0.01f,transform.position.y+10, jPoint.point.z+transform.forward.z*0.01f);
				if(Physics.Raycast (pointer, Vector3.down,out hCheck,20)){
					height = 11.07f - hCheck.distance;
					if (height<2){
						jumpMode=2; //fast jump
					}
					else if (height <3.5){
						jumpMode = 3; //Wall Climb
					}
					else{
						jumpMode = 6;// no jump
					}
				}
			}
			else if (jPoint.distance<4.5){//jump onto obstacle
				Vector3 pointer = new Vector3(jPoint.point.x + transform.forward.x*0.1f,transform.position.y+10, jPoint.point.z+transform.forward.z*0.1f);
				if(Physics.Raycast (pointer, Vector3.down,out hCheck,20)){
					height = 11.07f - hCheck.distance;
					if (height<2){
						jumpMode=4; //fast jump
					}
					else if (height <3.5){
						jumpMode = 5; //Wall Climb
					}
					else{
						jumpMode = 6;// no jump
					}
				}
			}

		}
	
	}
}
