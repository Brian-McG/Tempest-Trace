using UnityEngine;
using UnityEngine.EventSystems;

public class FirstPersonMovement : MonoBehaviour
{
    public float RunSpeed;
    public float JumpForce;

    private Animator animator;
    private int animParamJump;
    private int animParamSpeed;

    private CharacterController charController;
    private bool isGrounded;
    private float yVelocity;

    void Awake()
    {
        charController = GetComponent<CharacterController>();

        animator = GetComponentInChildren<Animator>();
        animParamJump = Animator.StringToHash("Jump");
        animParamSpeed = Animator.StringToHash("Speed");
    }

    // TODO: May be worthwhile moving movement to FixedUpdate for better physics interaction
    //       If we do we might want to store movement input each frame lest we miss any
    void Update()
    {
        float zMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");
        Vector3 moveVector = new Vector3(xMovement, 0, zMovement);

        if(moveVector != Vector3.zero)
        {
            float moveMagnitude = moveVector.magnitude;
            moveVector /= moveMagnitude;
            moveMagnitude = Mathf.Min(1.0f, moveMagnitude);

            moveVector = (transform.rotation * moveVector) *
                         RunSpeed * moveMagnitude;
        }


        bool shouldJump = Input.GetKeyDown(KeyCode.Space);
        if(isGrounded && shouldJump)
        {
            yVelocity = JumpForce;
            //animator.SetBool(animParamJump, true);
        }
        else
        {
            yVelocity -= 9.81f*Time.deltaTime;
        }

        moveVector += new Vector3(0.0f, yVelocity, 0.0f);

        charController.Move(moveVector * Time.deltaTime);
        // TODO: What if we couldnt move the full distance?
        //       - We might have hit a roof (set Y velocity to 0)
        //       - We might have hit an obstacle (so we didnt run as far as we thought)

        isGrounded = false;
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, new Vector3(0,-1,0),
                           out hitInfo, 1.02f, (1 << 8) | (1 << 10)))
        {
            isGrounded = true;
            //animator.SetBool(animParamJump, false);
            yVelocity = 0.0f;
        }
    }
}
