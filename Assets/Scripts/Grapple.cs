using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {
	public GameObject grappleBody;
	public GameObject grappleHinge;
	public bool isGrappling;
	// Use this for initialization
	void Start () {
		Collider[] cols  = GameObject.FindGameObjectWithTag("grappleHelper").GetComponents<Collider>();
		foreach (Collider c in cols)
		{
			Physics.IgnoreCollision(c, this.GetComponent<Collider>().collider);
		}
		isGrappling = false;
	}
	void Update() {
		if(Input.GetKeyDown(KeyCode.G)) {
			if(isGrappling) {
				grappleBody.GetComponent<HingeJoint>().connectedBody = null;
				grappleBody.GetComponent<SphereCollider>().enabled = false;
				grappleBody.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
				grappleBody.GetComponent<Rigidbody>().useGravity = false;
				grappleHinge.GetComponent<Transform>().rotation = Quaternion.identity;
				grappleBody.GetComponent<LineRenderer>().enabled = false;
				isGrappling = false;
			}
			else {

				//Ray r = Camera.main.transform.forward);
				RaycastHit hit;
				if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward, out hit, 100.0f))
				{
					grappleHinge.transform.position = hit.point;
					grappleBody.transform.position = this.transform.position;
					//grappleBody.transform.LookAt(grappleHinge.transform.position, Vector3.up); // TODO: Make grappleBody shape match character controller
					grappleBody.GetComponent<HingeJoint>().connectedBody = grappleHinge.GetComponent<Rigidbody>().rigidbody;
					grappleBody.GetComponent<SphereCollider>().enabled = true;
					grappleBody.GetComponent<Rigidbody>().useGravity = true;
					Collider[] cols  = GameObject.FindGameObjectWithTag("grappleHelper").GetComponents<Collider>();
					foreach (Collider c in cols)
					{
						Physics.IgnoreCollision(c, this.GetComponent<Collider>().collider);
					}
					grappleBody.GetComponent<Rigidbody>().velocity = this.GetComponent<CharacterController>().velocity*1.4f;
					grappleBody.GetComponent<LineRenderer>().enabled = true;
					isGrappling = true;
				}

			}
		}
	}
	// Update is called once per frame
	void LateUpdate() {
		if(isGrappling)
		{
			this.GetComponent<CharacterController>().Move(grappleBody.transform.position - this.transform.position);
			grappleBody.GetComponent<LineRenderer>().SetPosition(0,this.transform.position);
			grappleBody.GetComponent<LineRenderer>().SetPosition(1,grappleHinge.transform.position);
		}
	}
}
