using UnityEngine;
using System.Collections;

public class FPSMovement : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float jumpForce = 4.0f;

    public float maxDeltaVelocity = 10.0f;

    private Rigidbody rigidbody;
    private bool isGrounded;

	void Start ()
    {
	   rigidbody = GetComponent<Rigidbody>();
       rigidbody.freezeRotation = true;
       //rigidbody.useGravity = false;

       isGrounded = false;
	}
	
	void FixedUpdate ()
    {
        // Check  to see if the player is grounded
        RaycastHit hitInfo;
        // TODO: Currently the player's transform position is the middle of the player, we'd rather it be at the player's feet
        if(Physics.Raycast(transform.position, -Vector3.up, out hitInfo, 1.1f, 1 << 8))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if(isGrounded && rigidbody.velocity.y > 0)
        {
            Debug.Log("Grounded upwards!");
        }

	   if(isGrounded)
       {
            float xVelocity = Input.GetAxis("Horizontal");
            float zVelocity = Input.GetAxis("Vertical");
            Vector3 targetVelocity = new Vector3(xVelocity, 0, zVelocity);
            targetVelocity = transform.TransformDirection(targetVelocity); // Transform the velocity vector from local space into world space
            targetVelocity *= moveSpeed;

            // Modify the target velocity to account for the slope of the ground
            Vector3 sideways = Vector3.Cross(Vector3.up, targetVelocity);
            Vector3 newVelocityDir = Vector3.Cross(sideways, hitInfo.normal).normalized;
            //Debug.Log("Velocity "+targetVelocity+" to "+newVelocityDir);
            //targetVelocity = newVelocityDir * targetVelocity.magnitude;
            //Debug.Log("Change = " + ((newVelocityDir * targetVelocity.magnitude) - targetVelocity));

            // Apply the required force to get us to our target velocity
            Vector3 velocity = rigidbody.velocity;
            Vector3 deltaVelocity = (targetVelocity - velocity);

            // Prevent speeding up too fast
            deltaVelocity.x = Mathf.Clamp(deltaVelocity.x, -maxDeltaVelocity, maxDeltaVelocity);
            deltaVelocity.z = Mathf.Clamp(deltaVelocity.z, -maxDeltaVelocity, maxDeltaVelocity);
            deltaVelocity.y = 0;//Mathf.Min(deltaVelocity.y, 0);

            // TODO: This seems to be quite buggy, fairly often it won't jump (presumably because its not grounded) when it really seems like it should
            if(Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody.velocity = new Vector3(velocity.x, jumpForce, velocity.z);
            }

            rigidbody.AddForce(deltaVelocity, ForceMode.VelocityChange);
       }

       //Apply gravitational acceleration force
       rigidbody.AddForce(Physics.gravity, ForceMode.Force);
	}
}
