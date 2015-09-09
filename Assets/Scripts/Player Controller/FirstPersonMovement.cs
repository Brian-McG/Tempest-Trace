// <copyright file="FirstPersonMovement.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Add a "STEP" motion for when an obstacle is too long/the player is too slow to vault,
//       since the climb animation will probably look strange on an obstacle that is waist height
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
  VAULT,

  /// <summary>
  /// Slide under an obstacle or down a slope
  /// </summary>
  SLIDE,
}

public class FirstPersonMovement : MonoBehaviour
{
  public float DefaultRunSpeed;
  public float JumpForce;
  public float MaxSafeYVelocity;

  [Header("Motion Parameters")]
  [Tooltip("The number of seconds for which the obstacle detection ray gets shot forwards")]
  public float ObstacleCheckTime;
  [Tooltip("The minimum length of the obstacle check ray")]
  public float MinimumObstacleCheckDistance;
  [Tooltip("The minimum height at which an obstacle will be detected for vaulting/climbing")]
  public float MinimumObstacleScanHeight;

  [Header("Vault Parameters")]
  [Tooltip("The maximum height of an obstacle that can be vaulted over")]
  public float MaximumVaultHeight;
  [Tooltip("The maximum length of an object that can be vaulted over")]
  public float MaximumVaultDistance;
  [Tooltip("The minimum speed that the player must be running in order to vault")]
  public float MinimumSpeedToVault;

  [Header("Climb Parameters")]
  [Tooltip("The maximum height of an object that can be climbed")]
  public float MaximumClimbHeight;
  public float ClimbSpeed;

  [Header("Slide Parameters")]
  public float SlideDeceleration;
  public float SlideStopSpeedThreshold;

  private Animator animator;
  private int animParamSpeed;
  private int animParamSlide;
  private int animParamClimb;
  
  private CharacterController charController;
  private PlayerLifeHandler lifeHandler;
  private Vector3 velocity;
  
  private DefinedMotion currentMotion;
  private List<Vector3> motionTargets;
  private int motionProgress;
  
  private bool isJumping;
  private bool wasGrounded;

  public float RunSpeed
  {
    get;
    set;
  }

  public Vector3 Velocity
  {
    get { return velocity; }
  }

  public bool IsGrounded
  {
    get { return charController.isGrounded; }
  }

  public DefinedMotion CurrentMotion
  {
    get { return currentMotion; }
  }

  public void ResetState()
  {
    currentMotion = DefinedMotion.NONE;
    animator.SetFloat(animParamSpeed, 0.0f);
    animator.SetBool(animParamClimb, false);
    animator.SetBool(animParamSlide, false);
    velocity  = Vector3.zero;

    Transform cameraChild = transform.Find("Camera");
    FirstPersonCameraVertical cameraVerticalControl = cameraChild.GetComponent<FirstPersonCameraVertical>();
    cameraChild.localRotation = Quaternion.identity;
    cameraVerticalControl.ResetState();
  }

  private void Awake()
  {
    charController = GetComponent<CharacterController>();
    lifeHandler = GetComponent<PlayerLifeHandler>();
    
    animator = GetComponentInChildren<Animator>();
    animParamSpeed = Animator.StringToHash("MoveSpeed");
    animParamSlide = Animator.StringToHash("IsSliding");
    animParamClimb = Animator.StringToHash("IsClimbing");

    RunSpeed = DefaultRunSpeed;
    velocity = Vector3.zero;
    wasGrounded = false;
    currentMotion = DefinedMotion.NONE;
    motionTargets = new List<Vector3>();
  }
  
  // NOTE: We assume here that the player has height 2 and that the player's origin is at height
  //       0 from the ground (IE at the foot of the player)
  // TODO: We should be able to jump at a wall and if we hit the wall mid-jump, climb up it
  //       (or at the very least grab onto the ledge)
  // TODO: Check that the motion won't force the player through a wall (for example let them climb onto
  //       a low object next a wall, instead of vaulting over it and through the wall
  private void CheckForVaultClimbMotion()
  {
    float epsilon = 0.0001f;
    Vector3 horizontalVelocity = velocity;
    horizontalVelocity.y = 0;
    float horizontalSpeed = horizontalVelocity.magnitude;

    float motionCheckDistance = horizontalSpeed * ObstacleCheckTime;
    motionCheckDistance = Mathf.Max(motionCheckDistance, MinimumObstacleCheckDistance);

    Vector3 currentPosition = transform.position;
    Vector3 forwardDir = transform.forward;
    
    int checkLayerMask = ~(1 << 12);

    RaycastHit vaultCheckInfo;
    Vector3 vaultCheckPosition = currentPosition + new Vector3(0.0f, MinimumObstacleScanHeight, 0.0f);
    Debug.DrawLine(vaultCheckPosition, vaultCheckPosition + (forwardDir * motionCheckDistance), Color.green, 1.0f, false);
    bool canVault = Physics.Raycast(vaultCheckPosition, forwardDir, out vaultCheckInfo, motionCheckDistance, checkLayerMask);
    
    float climbCheckHeight = MaximumVaultHeight + epsilon;
    RaycastHit climbCheckInfo;
    Vector3 climbCheckPosition = currentPosition + new Vector3(0.0f, climbCheckHeight, 0.0f);
    Debug.DrawLine(climbCheckPosition, climbCheckPosition + (forwardDir * motionCheckDistance), Color.magenta, 1.0f, false);
    bool canClimb = Physics.Raycast(climbCheckPosition, forwardDir, out climbCheckInfo, motionCheckDistance, checkLayerMask);

    if (canVault && !canClimb && (horizontalSpeed > MinimumSpeedToVault))
    {
      // TODO: At the moment our vault picks us up, shifts us conveniently over the obstacle, and puts us down nicely on the other side
      //       While this is really neat, it also doesnt feel right, it feels far more like climbing than like anything called a "vault"
      //       The player should be able to be running, and without noticing a difference in speed/smoothness, vault over a low object
      Vector3 vaultCheckPoint = vaultCheckInfo.point;
      Vector3 vaultCeilingBasePoint = vaultCheckPoint + (forwardDir * epsilon);
      float vaultCeilingHeight = MaximumVaultHeight - MinimumObstacleScanHeight; // We subtract the height from our initial ray
      Vector3 vaultCeiling = vaultCeilingBasePoint + new Vector3(0.0f, vaultCeilingHeight, 0.0f);
      
      RaycastHit vaultApexInfo;
      if (Physics.Raycast(vaultCeiling, Vector3.down,
                          out vaultApexInfo, MaximumVaultHeight))
      {
        Vector3 vaultApex = vaultApexInfo.point + new Vector3(0.0f, 0.1f, 0.0f);
        Vector3 vaultMidpoint = vaultCheckPoint - (forwardDir * charController.radius);
        vaultMidpoint.y = vaultApex.y;
        
        Debug.Log("Vault");
        currentMotion = DefinedMotion.VAULT;
        motionProgress = 0;
        motionTargets.Clear();
        motionTargets.Add(vaultMidpoint);
        
        RaycastHit vaultBackCheckInfo;
        bool vaultEndInRange = Physics.Raycast(currentPosition + (forwardDir * MaximumVaultDistance),
                                               -forwardDir, out vaultBackCheckInfo,
                                               MaximumVaultDistance - vaultCheckInfo.distance);
        
        // NOTE: If we know where the end of the object is (IE so we can get over it and down the other side)
        //       Then we move over it in 3 steps (up, along, down). If it has no end (or is too long) then we
        //       move over it in 2 steps (up, slightly along)
        if (vaultEndInRange)
        {
          Vector3 vaultBackPoint = vaultBackCheckInfo.point;
          Vector3 vaultEndMidpoint = vaultBackPoint + (forwardDir * charController.radius);
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
          Vector3 vaultEndPoint = vaultMidpoint + (transform.forward * epsilon);
          
          motionTargets.Add(vaultEndPoint);
        }
      }
    }
    else if (canClimb)
    {
      Vector3 climbCheckPoint = climbCheckInfo.point + (forwardDir * epsilon);
      float climbCeilingHeight = MaximumClimbHeight - climbCheckHeight; // Subtract the height form our initial ray
      Vector3 climbCeiling = climbCheckPoint + new Vector3(0.0f, climbCeilingHeight, 0.0f);
      
      if (Physics.Raycast(climbCeiling, Vector3.down,
                          out climbCheckInfo, MaximumClimbHeight))
      {
        Vector3 climbTarget = climbCheckInfo.point + new Vector3(0.0f, 0.05f, 0.0f);
        Vector3 climbMidpoint = transform.position;
        climbMidpoint.y = climbTarget.y;
        
        Debug.Log("Climb");
        currentMotion = DefinedMotion.CLIMB;
        animator.SetBool(animParamClimb, true);
        motionTargets.Clear();
        motionTargets.Add(climbMidpoint);
        motionTargets.Add(climbTarget);
        motionProgress = 0;
      }
    }
  }
 
  private void CheckForSlideMotion()
  {
    float horizontalVelocityThreshold = 1.0f;
    Vector3 horizontalVelocity = velocity;
    horizontalVelocity.y = 0;

    if (horizontalVelocity.sqrMagnitude > horizontalVelocityThreshold *
                                         horizontalVelocityThreshold)
    {
      currentMotion = DefinedMotion.SLIDE;
      animator.SetBool(animParamSlide, true);
      transform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
    }
  }

  private void MoveAndUpdateVelocity()
  {
    Vector3 preMoveLoc = transform.position;
    charController.Move(velocity * Time.fixedDeltaTime);
    Vector3 postMoveLoc = transform.position;
    Vector3 actualMoveOffset = postMoveLoc - preMoveLoc;
    Vector3 actualMovevelocity = actualMoveOffset * (1.0f / Time.fixedDeltaTime);
    
    // Update velocity if we got blocked somewhere along the way
    if (velocity.sqrMagnitude - actualMovevelocity.sqrMagnitude > 0.01f)
    {
      velocity = actualMovevelocity;
    }
  }

  private void UpdateOnPlayerInput()
  {
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

    // Constrain change of movement vector while in the air
    if (charController.isGrounded)
    {
      velocity.x = moveVector.x;
      velocity.z = moveVector.z;
      isJumping = false;
    }
    
    else
    {
      if (isJumping)
      {
        CheckForVaultClimbMotion();
      }
    }
    
    // TODO: Since velocity has been update here, we have the velocity based on the input alone
    //       and does not consider the actual velocity based on collision
    bool shouldJump = Input.GetKeyDown(KeyCode.Space);
    if (charController.isGrounded && shouldJump)
    {
      CheckForVaultClimbMotion();
      
      if (currentMotion == DefinedMotion.NONE)
      {
        isJumping = true;
        velocity.y = JumpForce;
      }
    }
    else
    {
      velocity.y -= 9.81f * Time.fixedDeltaTime;
    }

    bool shouldSlide = Input.GetKeyDown(KeyCode.LeftShift);
    if (charController.isGrounded && !shouldJump && shouldSlide)
    {
      CheckForSlideMotion();
    }
    
    Vector3 preMoveLoc = transform.position;
    charController.Move(velocity * Time.fixedDeltaTime);
    Vector3 postMoveLoc = transform.position;
    Vector3 actualMoveOffset = postMoveLoc - preMoveLoc;
    Vector3 actualMoveVelocity = actualMoveOffset * (1.0f / Time.fixedDeltaTime);
   
    if ((charController.collisionFlags & CollisionFlags.Below) != 0)
    {
      if (velocity.y <  (-1.0f * MaxSafeYVelocity))
      {
        lifeHandler.KillPlayer();
      }
    }

    // Update velocity if we got blocked somewhere along the way
    if (velocity.sqrMagnitude - actualMoveVelocity.sqrMagnitude > 0.01f)
    {
      velocity = actualMoveVelocity;
    }
    animator.SetFloat(animParamSpeed, actualMoveVelocity.magnitude);
    
    if (charController.isGrounded)
    {
      velocity.y = 0.0f;
    }
    else
    {
      if (wasGrounded && !shouldJump)
      {
        // We just left the ground but we're moving downwards, check if theres more ground
        // just out of reach and if so then we're on a slope and we can just move down
        RaycastHit slopeCheckInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out slopeCheckInfo, 0.20f))
        {
          Vector3 offset = slopeCheckInfo.point - transform.position;
          charController.Move(offset);
        }
      }
    }

    wasGrounded = charController.isGrounded;
  }
  
  /// <summary>
  /// Updates the character controller based on the state of a vault or climb motion
  /// </summary>
  /// <returns>
  /// True if and only if the motion should stop playing (either because an obstacle was hit
  /// or because we just got to the end of the motion)
  /// </returns>
  private bool UpdateVaultClimbMotion()
  {
    float motionMoveDistance = ClimbSpeed * Time.fixedDeltaTime;
    Vector3 targetOffset = motionTargets[motionProgress] - transform.position;
    if (targetOffset.sqrMagnitude < motionMoveDistance * motionMoveDistance)
    {
      charController.Move(targetOffset);
      ++motionProgress;
      
      if (motionProgress >= motionTargets.Count)
      {
        return true;
      }
      
      return false;
    }
    
    Vector3 preMovePosition = transform.position;
    Vector3 targetDirection = targetOffset.normalized;
    charController.Move(targetDirection * motionMoveDistance);
    Vector3 postMovePosition = transform.position;

    Vector3 moveOffset = postMovePosition - preMovePosition;
    if (moveOffset.sqrMagnitude < (motionMoveDistance * motionMoveDistance) - 0.001f)
    {
      return true;
    }

    return false;
  }
  
  private void UpdateSlideMotion()
  {
    float movementZ = 1.0f;
    float movementX = Input.GetAxis("Horizontal");
    Vector3 moveVector = new Vector3(movementX, 0, movementZ);
    
    if (moveVector != Vector3.zero)
    {
      float moveMagnitude = moveVector.magnitude;
      moveVector /= moveMagnitude;
      moveMagnitude = Mathf.Min(1.0f, moveMagnitude);
      
      moveVector = (transform.rotation * moveVector) * RunSpeed * moveMagnitude;
    }

    velocity.x = moveVector.x;
    velocity.z = moveVector.z;
    
    // Apply Gravity
    velocity.y -= 9.81f * Time.fixedDeltaTime;

    // Actually move
    // TODO: Check new velocity after update, we might need to exit slide (e.g if we slide into a wall)
    MoveAndUpdateVelocity();
    
    if (charController.isGrounded)
    {
      velocity.y = 0.0f;
    }
    
    // Apply sliding slowdown
    RunSpeed -= SlideDeceleration * Time.fixedDeltaTime;
    if (RunSpeed < SlideStopSpeedThreshold)
    {
      currentMotion = DefinedMotion.NONE;
      animator.SetBool(animParamSlide, false);
      RunSpeed = DefaultRunSpeed;
      transform.localScale = Vector3.one;
    }

    // Check that we're still holding the slide key, otherwise go back to standard running
    if (!Input.GetKey(KeyCode.LeftShift))
    {
      currentMotion = DefinedMotion.NONE;
      animator.SetBool(animParamSlide, false);
      RunSpeed = DefaultRunSpeed;
      transform.localScale = Vector3.one;
    }
  }

  // TODO: Since we do all of this in FixedUpdate, we might want to get input in Update,
  //       just to prevent the possibility of missing some of the input?
  private void FixedUpdate()
  {
    switch (currentMotion)
    {
      case DefinedMotion.NONE:
        UpdateOnPlayerInput();
        break;

      case DefinedMotion.VAULT:
      case DefinedMotion.CLIMB:
        bool motionComplete = UpdateVaultClimbMotion();
        if (motionComplete)
        {
          currentMotion = DefinedMotion.NONE;
          animator.SetBool(animParamClimb, false);
        }

        break;

      case DefinedMotion.SLIDE:
        UpdateSlideMotion();
        break;

      default:
        Debug.Log("Attempt to update on unrecognized motion");
        break;
    }
  }
}
