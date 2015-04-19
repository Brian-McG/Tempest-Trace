using UnityEngine;
using System.Collections;

public class FPSMovement : MonoBehaviour
{
    private float maxMoveSpeed = 5.0f;

    private bool isRunningForwards = false;
    private float startedRunningForwards = 0.0f;
    private float timeRunningForwards;

    private float jumpForce = 7.0f;

    private float gravitationalAcceleration = -20f;
    private float terminalFallVelocity = -30.0f;

    private CharacterController characterController;
    private Vector3 currentVelocity;

    private Vector3 previousPosition;

    /*
    TODO:
    Shouldn't we store more than 1 of these? We get many Updates for each FixedUpdate
    so we probably want to take all of them into account instead of just the last one
    that ran before FixedUpdate
    */
    private Vector2 moveInput;
    private bool shouldJump;


	void Start ()
    {
        characterController = GetComponent<CharacterController>();

        isRunningForwards = false;


        previousPosition = transform.position;
	}
	
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        shouldJump = Input.GetKey(KeyCode.Space);
    }

	void FixedUpdate ()
    {
        float moveSpeed = maxMoveSpeed;
        if(isRunningForwards && (timeRunningForwards > 1.0f))
        {
            moveSpeed += Mathf.Min(5*(timeRunningForwards - 1.0f), 5);
        }

        Vector3 inputVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        inputVelocity = transform.rotation * inputVelocity;
        inputVelocity.y = currentVelocity.y;

        inputVelocity = fixedUpdateVerticalMovement(inputVelocity);

        // TODO: We might need to push the player down a bit further here if we're walking down
        // sudden drops such as steps or after vaulting over a box

        currentVelocity = inputVelocity;
        characterController.Move(currentVelocity * Time.fixedDeltaTime);

        Vector3 actualMove = transform.position - previousPosition;
        Vector3 actualVelocity = actualMove / Time.fixedDeltaTime;
        Vector3 actualLocalVelocity = transform.InverseTransformVector(actualVelocity);

        // Start timing if we're currently running at full speed
        if(actualLocalVelocity.z > maxMoveSpeed - 0.5f)
        {
            if(!isRunningForwards)
            {
                startedRunningForwards = Time.time;
            }
            isRunningForwards = true;
            timeRunningForwards = Time.time - startedRunningForwards;
        }
        else
        {
            isRunningForwards = false;
        }

        previousPosition = transform.position;

        // TODO: Handle velocity modification as a result of collisions (both horizontally and vertically)
        // IE We might have moved less because we hit a wall, or hit something above us
        // in which case we should probably set the y-velocity to 0
	}

    private Vector3 fixedUpdateVerticalMovement(Vector3 velocity)
    {
        if(characterController.isGrounded)
        {
            velocity.y = 0;
            // TODO: Let the player jump higher by holding down the jump button
            //       This is more to make small jumps smaller than large jumps larger
            // TODO: Allow for jumping if the jump button was pressed in the near past,
            //       not just in the last update (to allow for small mis-timings)
            if(shouldJump)
            {
                velocity.y = jumpForce;
            }
        }
        velocity.y += gravitationalAcceleration * Time.fixedDeltaTime;

        velocity.y = Mathf.Max(velocity.y, terminalFallVelocity);

        return velocity;
    } 
}
