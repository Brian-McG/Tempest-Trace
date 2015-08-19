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
  public Vector3 Velocity;
  public float ClimbSpeed;
  
  private Animator animator;
  private int animParamJump;
  private int animParamSpeed;
  
  private CharacterController charController;
  private bool isGrounded;
  
  private DefinedMotion currentMotion;
  private List<Vector3> motionTargets;
  private int motionProgress;
  
  private void Awake()
  {
    charController = GetComponent<CharacterController>();
    
    animator = GetComponentInChildren<Animator>();
    animParamJump = Animator.StringToHash("Jump");
    animParamSpeed = Animator.StringToHash("Speed");
    
    Velocity = Vector3.zero;
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
    
    // TODO: These need to be a little more coordinated, at the moment we're a little fuzzy on when to vault and when to climb
    //       Similarly, the vaulting height is quite low so for example the low bars next to the AC switch get climbed rather than vaulted
    float vaultCheckHeight = 0.3f;
    float maxVaultHeight = 1.0f;
    float maxVaultDistance = 3.0f;
    RaycastHit vaultCheckInfo;
    Vector3 vaultCheckPosition = currentPosition + new Vector3(0.0f, vaultCheckHeight, 0.0f);
    Debug.DrawLine(vaultCheckPosition, vaultCheckPosition + (forwardDir * motionCheckDistance), Color.green, 1.0f, false);
    bool canVault = Physics.Raycast(vaultCheckPosition, forwardDir, out vaultCheckInfo, motionCheckDistance);
    
    float climbCheckHeight = 1.0f;
    float maxClimbHeight = 2.0f;
    RaycastHit climbCheckInfo;
    Vector3 climbCheckPosition = currentPosition + new Vector3(0.0f, climbCheckHeight, 0.0f);
    Debug.DrawLine(climbCheckPosition, climbCheckPosition + (forwardDir * motionCheckDistance), Color.magenta, 1.0f, false);
    bool canClimb = Physics.Raycast(climbCheckPosition, forwardDir, out climbCheckInfo, motionCheckDistance);
    
    if (canVault && !canClimb)
    {
      // TODO: At the moment our vault picks us up, shifts us conveniently over the obstacle, and puts us down nicely on the other side
      //       While this is really neat, it also doesnt feel right, it feels far more like climbing than like anything called a "vault"
      //       The player should be able to be running, and without noticing a difference in speed/smoothness, vault over a low object
      Vector3 vaultCheckPoint = vaultCheckInfo.point;
      Vector3 vaultCeilingBasePoint = vaultCheckPoint + (forwardDir * 0.2f);
      Vector3 vaultCeiling = vaultCeilingBasePoint + new Vector3(0.0f, maxVaultHeight, 0.0f);
      
      RaycastHit vaultApexInfo;
      if (Physics.Raycast(vaultCeiling, Vector3.down,
                          out vaultApexInfo, maxClimbHeight))
      {
        Vector3 vaultApex = vaultApexInfo.point + new Vector3(0.0f, 0.1f, 0.0f);
        Vector3 vaultMidpoint = vaultCheckPoint;
        vaultMidpoint.y = vaultApex.y;
        
        Debug.Log("Vault");
        currentMotion = DefinedMotion.VAULT;
        motionProgress = 0;
        motionTargets.Clear();
        motionTargets.Add(vaultMidpoint);
        
        RaycastHit vaultBackCheckInfo;
        bool vaultEndInRange = Physics.Raycast(currentPosition + (forwardDir * maxVaultDistance),
                                               -forwardDir, out vaultBackCheckInfo,
                                               maxVaultDistance - vaultCheckInfo.distance);
        
        // NOTE: If we know where the end of the object is (IE so we can get over it and down the other side)
        //       Then we move over it in 3 steps (up, along, down). If it has no end (or is too long) then we
        //       move over it in 2 steps (up, slightly along)
        if (vaultEndInRange)
        {
          Vector3 vaultBackPoint = vaultBackCheckInfo.point;
          Vector3 vaultEndMidpoint = vaultBackPoint;
          vaultEndMidpoint.y = vaultApex.y;
          
          Vector3 vaultStartApexOffset = vaultMidpoint - currentPosition;
          vaultStartApexOffset.y *= -1.0f;
          Vector3 vaultEndPoint = vaultEndMidpoint + vaultStartApexOffset;
          
          Debug.DrawLine(currentPosition, vaultMidpoint, Color.red, 10.0f, false);
          Debug.DrawLine(vaultMidpoint, vaultEndMidpoint, Color.red, 10.0f, false);
          Debug.DrawLine(vaultEndMidpoint, vaultEndPoint, Color.red, 10.0f, false);
          
          motionTargets.Add(vaultEndMidpoint);
          motionTargets.Add(vaultEndPoint);
        }
        else
        {
          Vector3 vaultEndPoint = vaultMidpoint + (transform.forward * 0.2f);
          
          motionTargets.Add(vaultEndPoint);
        }
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
      Velocity.x = moveVector.x;
      Velocity.z = moveVector.z;
    }
    else
    {
      Velocity.x = Mathf.Lerp(Velocity.x, moveVector.x, AirControlFactor);
      Velocity.z = Mathf.Lerp(Velocity.z, moveVector.z, AirControlFactor);
    }
    
    bool shouldJump = Input.GetKeyDown(KeyCode.Space);
    if (isGrounded && shouldJump)
    {
      CheckForDefinedMotions();
      
      Velocity.y = JumpForce;
      ////animator.SetBool(animParamJump, true);
    }
    else
    {
      Velocity.y -= 9.81f * Time.deltaTime;
    }
    
    Vector3 preMoveLoc = transform.position;
    charController.Move(Velocity * Time.deltaTime);
    Vector3 postMoveLoc = transform.position;
    Vector3 actualMoveOffset = postMoveLoc - preMoveLoc;
    Vector3 actualMoveVelocity = actualMoveOffset * (1.0f / Time.deltaTime);
    
    // Update velocity if we got blocked somewhere along the way
    if (Velocity.sqrMagnitude - actualMoveVelocity.sqrMagnitude > 0.01f)
    {
      Velocity = actualMoveVelocity;
    }
    
    isGrounded = false;
    RaycastHit hitInfo;
    if (Physics.Raycast(transform.position, Vector3.down,
                        out hitInfo, 0.02f))
    {
      isGrounded = true;
      ////animator.SetBool(animParamJump, false);
      Velocity.y = 0.0f;
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
        Velocity = new Vector3(0.0f, 0.0f, 0.0f);
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

  public void ResetState()
  {
    currentMotion = DefinedMotion.NONE;
    velocity  = Vector3.zero;

    Transform cameraChild = transform.Find("Camera");
    FirstPersonCameraVertical cameraVerticalControl = cameraChild.GetComponent<FirstPersonCameraVertical>();
    cameraChild.localRotation = Quaternion.identity;
    cameraVerticalControl.ResetState();
  }
}
