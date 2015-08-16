using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;

enum DefinedMotion
{
    NONE,
    CLIMB,
    VAULT
}

public class FirstPersonMovement : MonoBehaviour
{
    public float RunSpeed;
    public float JumpForce;
    public float AirControlFactor;

    private Animator animator;
    private int animParamJump;
    private int animParamSpeed;

    private CharacterController charController;
    private bool isGrounded;
    private Vector3 velocity;

    private DefinedMotion currentMotion;
    private List<Vector3> motionTargets;
    private int motionProgress;

    void Awake()
    {
        charController = GetComponent<CharacterController>();

        animator = GetComponentInChildren<Animator>();
        animParamJump = Animator.StringToHash("Jump");
        animParamSpeed = Animator.StringToHash("Speed");

        velocity = Vector3.zero;
        isGrounded = true;
        currentMotion = DefinedMotion.NONE;
        motionTargets = new List<Vector3>();
    }

    // NOTE: We assume here that the player has height 2 and that the player's origin is at height
    //       1 from the ground (IE in the centre of the player)
    private void checkForDefinedMotions()
    {
        float motionCheckDistance = 1.0f;
        Vector3 currentPosition = transform.position;
        Vector3 forwardDir = transform.forward;
        
        float vaultCheckHeight = 0.3f;
        float maxVaultHeight = 0.6f;
        RaycastHit vaultCheckInfo;
        Vector3 vaultCheckPosition = currentPosition - new Vector3(0.0f, 1.0f, 0.0f) + new Vector3(0.0f, vaultCheckHeight, 0.0f);
        bool canVault = Physics.Raycast(vaultCheckPosition, forwardDir, out vaultCheckInfo, motionCheckDistance);
        
        float climbCheckHeight = 1.0f;
        float maxClimbHeight = 2.0f;
        RaycastHit climbCheckInfo;
        Vector3 climbCheckPosition = currentPosition - new Vector3(0.0f, 1.0f, 0.0f) + new Vector3(0.0f, climbCheckHeight, 0.0f);
        bool canClimb = Physics.Raycast(climbCheckPosition, forwardDir, out climbCheckInfo, motionCheckDistance);

        if(canVault && !canClimb)
        {
            Vector3 vaultCheckPoint = vaultCheckInfo.point + forwardDir * 0.2f;
            Vector3 vaultCeiling = vaultCheckPoint + new Vector3(0.0f, maxVaultHeight, 0.0f);

            if(Physics.Raycast(vaultCeiling, Vector3.down,
                               out vaultCheckInfo, maxClimbHeight))
            {
                Vector3 vaultTarget = vaultCheckInfo.point + new Vector3(0.0f, 1.0f, 0.0f) +
                                      new Vector3(0.0f, 0.1f, 0.0f);
                Vector3 vaultMidpoint = currentPosition;
                vaultMidpoint.y = vaultTarget.y;

                Vector3 vaultEnd = currentPosition + forwardDir * 2.0f;
                Vector3 vaultEndMidpoint = vaultEnd;
                vaultEndMidpoint.y = vaultTarget.y;

                currentMotion = DefinedMotion.VAULT;
                motionTargets.Clear();
                motionTargets.Add(vaultMidpoint);
                motionTargets.Add(vaultTarget);
                motionTargets.Add(vaultEndMidpoint);
                motionTargets.Add(vaultEnd);
                motionProgress = 0;
            }
        }
        else if(canClimb)
        {
            Vector3 climbCheckPoint = climbCheckInfo.point + forwardDir * 0.2f;
            Vector3 climbCeiling = climbCheckPoint + new Vector3(0.0f, maxClimbHeight, 0.0f);

            if(Physics.Raycast(climbCeiling, Vector3.down,
                               out climbCheckInfo, maxClimbHeight))
            {
                Vector3 climbTarget = climbCheckInfo.point + new Vector3(0.0f, 1.0f, 0.0f) +
                                      new Vector3(0.0f, 0.1f, 0.0f);
                Vector3 climbMidpoint = transform.position;
                climbMidpoint.y = climbTarget.y;

                currentMotion = DefinedMotion.CLIMB;
                motionTargets.Clear();
                motionTargets.Add(climbMidpoint);
                motionTargets.Add(climbTarget);
                motionProgress = 0;
            }
        }
    }
    
    private void updateOnPlayerInput()
    {
        // Disallow change of movement vector while in the air
        float zMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");
        Vector3 moveVector = new Vector3(xMovement, 0, zMovement);

        if (moveVector != Vector3.zero)
        {
            float moveMagnitude = moveVector.magnitude;
            moveVector /= moveMagnitude;
            moveMagnitude = Mathf.Min(1.0f, moveMagnitude);

            moveVector = (transform.rotation * moveVector) * RunSpeed * moveMagnitude;
        }
        if(isGrounded)
        {
            velocity.x = moveVector.x;
            velocity.z = moveVector.z;
        } else
        {
            velocity.x = Mathf.Lerp(velocity.x, moveVector.x, AirControlFactor);
            velocity.z = Mathf.Lerp(velocity.z, moveVector.z, AirControlFactor);
        }

        bool shouldJump = Input.GetKeyDown(KeyCode.Space);
        Debug.DrawLine(transform.position, transform.position+transform.forward, Color.green);
        if (isGrounded && shouldJump)
        {
            checkForDefinedMotions();

            velocity.y = JumpForce;
            //animator.SetBool(animParamJump, true);
        } else
        {
            velocity.y -= 9.81f * Time.deltaTime;
        }

        Vector3 preMoveLoc = transform.position;
        charController.Move(velocity * Time.deltaTime);
        Vector3 postMoveLoc = transform.position;
        Vector3 actualMoveOffset = postMoveLoc - preMoveLoc;
        Vector3 actualMoveVelocity = actualMoveOffset * (1.0f/Time.deltaTime);

        // Update velocity if we got blocked somewhere along the way
        if(velocity.sqrMagnitude - actualMoveVelocity.sqrMagnitude > 0.01f)
        {
            velocity = actualMoveVelocity;
        }

        isGrounded = false;
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0),
                               out hitInfo, 1.02f))
        {
            isGrounded = true;
            //animator.SetBool(animParamJump, false);
            velocity.y = 0.0f;
        }

    }

    private void updateOnCurrentMotion()
    {
        float motionSpeed = 6.0f * Time.deltaTime;
        Vector3 targetOffset = motionTargets[motionProgress] - transform.position;
        if(targetOffset.sqrMagnitude < motionSpeed * motionSpeed)
        {
            transform.position = motionTargets[motionProgress];
            ++motionProgress;

            if(motionProgress >= motionTargets.Count)
            {
                currentMotion = DefinedMotion.NONE;
                velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
            return;
        }

        Vector3 targetDirection = targetOffset.normalized;
        transform.position += targetDirection * motionSpeed;
    }

    // TODO: May be worthwhile moving movement to FixedUpdate for better physics interaction
    //       If we do we might want to store movement input each frame lest we miss any
    void Update()
    {
        if(currentMotion == DefinedMotion.NONE)
        {
            updateOnPlayerInput();
        }
        else
        {
            updateOnCurrentMotion();
        }
    }
}
