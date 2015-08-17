// <copyright file="FirstPersonMovement.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public enum DefinedMotion
{
    /// <summary>
    /// Not currently enacting a predefined motion (IE just running around)
    /// </summary>
    NONE,

    /// <summary>
    /// Climbing a tall obstacle
    /// </summary>
    CLIMB,

    /// <summary>
    /// Vaulting over a low obstacle
    /// </summary>
    VAULT
}

public class FirstPersonMovement : MonoBehaviour
{
    public float RunSpeed;
    public float JumpForce;
    public float AirControlFactor;

    public float ClimbSpeed;

    private Animator animator;
    private int animParamJump;
    private int animParamSpeed;

    private CharacterController charController;
    private bool isGrounded;
    private Vector3 velocity;

    private DefinedMotion currentMotion;
    private List<Vector3> motionTargets;
    private int motionProgress;

    private void Awake()
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
    //       0 from the ground (IE at the foot of the player)
    private void CheckForDefinedMotions()
    {
        float motionCheckDistance = 1.0f;
        Vector3 currentPosition = transform.position;
        Vector3 forwardDir = transform.forward;

        float vaultCheckHeight = 0.3f;
        float maxVaultHeight = 0.6f;
        RaycastHit vaultCheckInfo;
        Vector3 vaultCheckPosition = currentPosition + new Vector3(0.0f, vaultCheckHeight, 0.0f);
        Debug.DrawLine(vaultCheckPosition, vaultCheckPosition + forwardDir*motionCheckDistance, Color.green, 1.0f, false);
        bool canVault = Physics.Raycast(vaultCheckPosition, forwardDir, out vaultCheckInfo, motionCheckDistance);

        float climbCheckHeight = 1.0f;
        float maxClimbHeight = 2.0f;
        RaycastHit climbCheckInfo;
        Vector3 climbCheckPosition = currentPosition + new Vector3(0.0f, climbCheckHeight, 0.0f);
        Debug.DrawLine(climbCheckPosition, climbCheckPosition + forwardDir*motionCheckDistance, Color.magenta, 1.0f, false);
        bool canClimb = Physics.Raycast(climbCheckPosition, forwardDir, out climbCheckInfo, motionCheckDistance);

        if (canVault && !canClimb)
        {
            Vector3 vaultCheckPoint = vaultCheckInfo.point + (forwardDir * 0.2f);
            Vector3 vaultCeiling = vaultCheckPoint + new Vector3(0.0f, maxVaultHeight, 0.0f);

            if (Physics.Raycast(vaultCeiling, Vector3.down,
                                out vaultCheckInfo, maxClimbHeight))
            {
                Vector3 vaultTarget = vaultCheckInfo.point + new Vector3(0.0f, 0.1f, 0.0f);
                Vector3 vaultMidpoint = currentPosition;
                vaultMidpoint.y = vaultTarget.y;

                Vector3 vaultEnd = currentPosition + (forwardDir * 2.0f);
                Vector3 vaultEndMidpoint = vaultEnd;
                vaultEndMidpoint.y = vaultTarget.y;

                Debug.Log("Vault");
                currentMotion = DefinedMotion.VAULT;
                motionTargets.Clear();
                motionTargets.Add(vaultMidpoint);
                motionTargets.Add(vaultTarget);
                motionTargets.Add(vaultEndMidpoint);
                motionTargets.Add(vaultEnd);
                motionProgress = 0;
            }
        }
        else if (canClimb)
        {
            Vector3 climbCheckPoint = climbCheckInfo.point + (forwardDir * 0.2f);
            Vector3 climbCeiling = climbCheckPoint + new Vector3(0.0f, maxClimbHeight, 0.0f);

            if (Physics.Raycast(climbCeiling, Vector3.down,
                                out climbCheckInfo, maxClimbHeight))
            {
                Vector3 climbTarget = climbCheckInfo.point + new Vector3(0.0f, 0.05f, 0.0f);
                Vector3 climbMidpoint = transform.position;
                climbMidpoint.y = climbTarget.y;

                Debug.Log("Climb");
                currentMotion = DefinedMotion.CLIMB;
                motionTargets.Clear();
                motionTargets.Add(climbMidpoint);
                motionTargets.Add(climbTarget);
                motionProgress = 0;
            }
        }
    }
    
    private void UpdateOnPlayerInput()
    {
        // Disallow change of movement vector while in the air
        float movementZ = Input.GetAxis("Vertical");
        float movementX = Input.GetAxis("Horizontal");
        Vector3 moveVector = new Vector3(movementX, 0, movementZ);

        if (moveVector != Vector3.zero)
        {
            float moveMagnitude = moveVector.magnitude;
            moveVector /= moveMagnitude;
            moveMagnitude = Mathf.Min(1.0f, moveMagnitude);

            moveVector = (transform.rotation * moveVector) * RunSpeed * moveMagnitude;
        }

        if (isGrounded)
        {
            velocity.x = moveVector.x;
            velocity.z = moveVector.z;
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, moveVector.x, AirControlFactor);
            velocity.z = Mathf.Lerp(velocity.z, moveVector.z, AirControlFactor);
        }

        bool shouldJump = Input.GetKeyDown(KeyCode.Space);
        if (isGrounded && shouldJump)
        {
            CheckForDefinedMotions();

            velocity.y = JumpForce;
            ////animator.SetBool(animParamJump, true);
        }
        else
        {
            velocity.y -= 9.81f * Time.deltaTime;
        }

        Vector3 preMoveLoc = transform.position;
        charController.Move(velocity * Time.deltaTime);
        Vector3 postMoveLoc = transform.position;
        Vector3 actualMoveOffset = postMoveLoc - preMoveLoc;
        Vector3 actualMoveVelocity = actualMoveOffset * (1.0f / Time.deltaTime);

        // Update velocity if we got blocked somewhere along the way
        if (velocity.sqrMagnitude - actualMoveVelocity.sqrMagnitude > 0.01f)
        {
            velocity = actualMoveVelocity;
        }

        isGrounded = false;
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down,
                               out hitInfo, 0.02f))
        {
            isGrounded = true;
            ////animator.SetBool(animParamJump, false);
            velocity.y = 0.0f;
        }
    }

    private void UpdateOnCurrentMotion()
    {
        float motionMoveDistance = ClimbSpeed * Time.deltaTime;
        Vector3 targetOffset = motionTargets[motionProgress] - transform.position;
        if (targetOffset.sqrMagnitude < motionMoveDistance * motionMoveDistance)
        {
            transform.position = motionTargets[motionProgress];
            ++motionProgress;

            if (motionProgress >= motionTargets.Count)
            {
                currentMotion = DefinedMotion.NONE;
                velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }

            return;
        }

        Vector3 targetDirection = targetOffset.normalized;
        transform.position += targetDirection * motionMoveDistance;
    }

    // TODO: May be worthwhile moving movement to FixedUpdate for better physics interaction
    //       If we do we might want to store movement input each frame lest we miss any
    private void Update()
    {
        if (currentMotion == DefinedMotion.NONE)
        {
            UpdateOnPlayerInput();
        }
        else
        {
            UpdateOnCurrentMotion();
        }
    }
}
