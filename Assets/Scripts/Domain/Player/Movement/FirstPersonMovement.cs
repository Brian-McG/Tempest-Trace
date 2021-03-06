// <copyright file="FirstPersonMovement.cs" company="University of Cape Town">
//     Jacques Heunis
//     HNSJAC003
// </copyright>
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using Domain.Player.Look;
using Domain.Player.Health;
using Domain.GameInput;
using Domain.CameraEffects;


namespace Domain.Player.Movement
{
// TODO: Add a "STEP" motion for when an obstacle is too long/the player is too slow to vault,
//       since the climb animation will probably look strange on an obstacle that is waist height
public enum DefinedMotion
{
  /// <summary>
  /// Not currently enacting a predefined motion (IE just running around)
  /// </summary>
  NONE,

  /// <summary>
  /// Hanging from a ledge, waiting to climb up or drop down
  /// </summary>
  HANG,
  
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
  public int PlayerID;
  public float DefaultRunSpeed;
  public float JumpForce;
  public float MaxSafeYVelocity;
  [Tooltip("The time since last being grounded within which the player can still jump")]
  public float MaxUngroundedJumpTime;

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
  [Tooltip("The heigh from the player's feet above which they will hang instead of climbing if they hit a ledge mid-air")]
  public float HangClimbThresholdHeight;
  public float ClimbSpeed;

  [Header("Slide Parameters")]
  public float SlideDeceleration;
  public float SlideStopSpeedThreshold;

  [Header("Sound Paramters")]
  public AudioSource JumpSound;
  public AudioSource ClimbSound;
  public AudioSource VaultSound;
  public AudioSource SlideSound;

  private Animator localViewAnimator;
  private Animator enemyViewAnimator;
  private int animParamSpeed;
  private int animParamSlide;
  private int animParamClimb;
  private int animParamVault;
  private int animParamJump;
  private int animParamHeight;
  private int animParamYOffset;
  private int animParamCameraY;
  private int animParamCameraZ;
  
  private Headbob headBob;
  private CharacterController charController;
  private Transform cameraChild;
  private PlayerLifeHandler lifeHandler;
  private Vector3 velocity;
  
  private DefinedMotion currentMotion;
  private List<Vector3> motionTargets;
  private int motionProgress;

  private bool hasOverhead;
  private bool previousHasOverhead;
  private bool midAirChecksEnabled;
  private bool isJumping;
  private bool wasGrounded;
  private float lastGroundedTime;

  private float movementX;
  private float movementZ;
  private bool shouldJump;
  private bool shouldSlide;

  public float RunSpeed
  {
    get;
    set;
  }

  public Vector3 Velocity
  {
    get { return velocity; }
  }

  public void SetVelocity (Vector3 v)
  {
			velocity = v;

  }


  public Vector3 HorizontalVelocity
  {
    get
    {
      Vector3 velocity = Velocity;
      velocity.y = 0.0f;
      return velocity;
    }
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
    SetAnimFloat(animParamSpeed, 0.0f);
    SetAnimBool(animParamClimb, false);
    SetAnimBool(animParamSlide, false);
    velocity  = Vector3.zero;
    RunSpeed = DefaultRunSpeed;
    
    FirstPersonCameraVertical cameraVerticalControl = cameraChild.GetComponent<FirstPersonCameraVertical>();
    cameraChild.localRotation = Quaternion.identity;
    cameraVerticalControl.ResetState();
  }

  private void Awake()
  {
    charController = GetComponent<CharacterController>();
    headBob = transform.Find("Camera").GetComponent<Headbob>();
    lifeHandler = GetComponent<PlayerLifeHandler>();
    cameraChild = transform.Find("Camera");
    
    localViewAnimator = transform.Find("LocalViewAvatarContainer").GetChild(0).GetComponent<Animator>();
    enemyViewAnimator = transform.Find("Avatar").GetComponent<Animator>();
    animParamSpeed = Animator.StringToHash("MoveSpeed");
    animParamSlide = Animator.StringToHash("IsSliding");
    animParamClimb = Animator.StringToHash("IsClimbing");
    animParamVault = Animator.StringToHash("IsVaulting");
    animParamJump = Animator.StringToHash("IsJumping");
    animParamHeight = Animator.StringToHash("PlayerHeight");
    animParamYOffset = Animator.StringToHash("AvatarYOffset");
    animParamCameraY = Animator.StringToHash("CameraYOffset");
    animParamCameraZ = Animator.StringToHash("CameraZOffset");

    RunSpeed = DefaultRunSpeed;
    velocity = Vector3.zero;
    midAirChecksEnabled = true;
    wasGrounded = false;
    hasOverhead = false;
    previousHasOverhead = false;
    currentMotion = DefinedMotion.NONE;
    motionTargets = new List<Vector3>();
  }
  
  private void CheckForVaultClimbMotion()
  {
    float epsilon = 0.02f;
    float horizontalSpeed = HorizontalVelocity.magnitude;

    // NOTE: We compute the time it would take the player to jump to the max vault height here
    //       This should prevent 'super-jumps' that come from colliding with an obstacle while
    //       jumping and getting bounced into the air
    float accelerateTerm = -0.5f * Physics.gravity.y;
    float velocityTerm = -1.0f * JumpForce;
    float distanceTerm = MaximumVaultHeight;
    float discriminantSquared = (velocityTerm * velocityTerm) - (4 * accelerateTerm * distanceTerm);
    float discriminant = Mathf.Sqrt(discriminantSquared);
    float solution1 = (-velocityTerm + discriminant) / (2.0f * accelerateTerm);
    float solution2 = (-velocityTerm - discriminant) / (2.0f * accelerateTerm);

    // NOTE: We're assuming here that we get solutions where sol1 > sol2 and sol1,sol2 > 0
    float vaultCollideTime = solution2;

    float motionCheckTime = Mathf.Max(ObstacleCheckTime, vaultCollideTime);
    float motionCheckDistance = horizontalSpeed * motionCheckTime;
    motionCheckDistance = Mathf.Max(motionCheckDistance, MinimumObstacleCheckDistance);

    Vector3 currentPosition = transform.position;
    Vector3 forwardDir = transform.forward;
    
    int checkLayerMask = ~((1 << 12) | (1 << 11) | (1 << 13) | (1 << 14) | (1 << 15) | (1 << 16));

    RaycastHit vaultCheckInfo;
    Vector3 vaultCheckPosition = currentPosition + new Vector3(0.0f, MinimumObstacleScanHeight, 0.0f);
    Debug.DrawLine(vaultCheckPosition, vaultCheckPosition + (forwardDir * motionCheckDistance), Color.green, 1.0f, false);
    bool canVault = Physics.Raycast(vaultCheckPosition, forwardDir, out vaultCheckInfo, motionCheckDistance, checkLayerMask);
    
    float climbCheckHeight = MaximumVaultHeight + epsilon;
    RaycastHit climbCheckInfo;
    Vector3 climbCheckPosition = currentPosition + new Vector3(0.0f, climbCheckHeight, 0.0f);
    Debug.DrawLine(climbCheckPosition, climbCheckPosition + (forwardDir * motionCheckDistance), Color.magenta, 1.0f, false);
    bool canClimb = Physics.Raycast(climbCheckPosition, forwardDir, out climbCheckInfo, motionCheckDistance, checkLayerMask);

    // NOTE: If you can vault and climb 2 different objects then
    //       you clearly cant climb the vault object, so don't climb
    if(canVault && canClimb && (vaultCheckInfo.transform.gameObject != climbCheckInfo.transform.gameObject))
    {
      canClimb = false;
    }

    if (canVault && !canClimb && (horizontalSpeed > MinimumSpeedToVault))
    {
      // TODO: At the moment our vault picks us up, shifts us conveniently over the obstacle, and puts us down nicely on the other side
      //       While this is really neat, it also doesnt feel right, it feels far more like climbing than like anything called a "vault"
      //       The player should be able to be running, and without noticing a difference in speed/smoothness, vault over a low object
      Vector3 vaultCheckPoint = vaultCheckInfo.point;
      Vector3 vaultCeilingBasePoint = vaultCheckPoint + (forwardDir * epsilon);
      float vaultHeight = MaximumVaultHeight;
      if(velocity.y < 0.0f)
      {
        vaultHeight *= 0.5f; // NOTE: If you're falling you shouldn't be able to jump up very much
      }
      float vaultCeilingHeight = vaultHeight - MinimumObstacleScanHeight; // We subtract the height from our initial ray
      Vector3 vaultCeiling = vaultCeilingBasePoint + new Vector3(0.0f, vaultCeilingHeight, 0.0f);
      
      RaycastHit vaultApexInfo;
      if (Physics.Raycast(vaultCeiling, Vector3.down,
                          out vaultApexInfo, MaximumVaultHeight))
      {
        Vector3 vaultApex = vaultApexInfo.point + new Vector3(0.0f, 0.1f, 0.0f);
        Vector3 vaultMidpoint = vaultCheckPoint - (forwardDir * charController.radius);
        vaultMidpoint.y = vaultApex.y;
        
        velocity.y = 0.0f; // Reset vertical velocity to zero so we don't do an extra hop afterwards
        VaultSound.Play();
        currentMotion = DefinedMotion.VAULT;
        SetAnimBool(animParamJump, false);
        SetAnimBool(animParamVault, true);
        motionProgress = 0;
        motionTargets.Clear();
        motionTargets.Add(vaultMidpoint);
        
        RaycastHit vaultBackCheckInfo;
        bool vaultEndInRange = false;
        if (MaximumVaultDistance > vaultCheckInfo.distance)
        {
          vaultEndInRange = Physics.Raycast(currentPosition + (forwardDir * MaximumVaultDistance),
                                            -forwardDir, out vaultBackCheckInfo,
                                            MaximumVaultDistance - vaultCheckInfo.distance);
        }
        else
        {
          // NOTE: This is a complete hack to get around vaultBackCheckInfo being unassigned
          vaultBackCheckInfo = new RaycastHit();
        }
        
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
      Transform climbCheckTransform = climbCheckInfo.transform;
      Vector3 climbCheckPoint = climbCheckInfo.point + (forwardDir * epsilon);
      float climbCeilingHeight = MaximumClimbHeight - climbCheckHeight; // Subtract the height form our initial ray
      Vector3 climbCeiling = climbCheckPoint + new Vector3(0.0f, climbCeilingHeight, 0.0f);
      
      RaycastHit climbCeilingInfo;
      if (Physics.Raycast(climbCeiling, Vector3.down,
                          out climbCeilingInfo, MaximumClimbHeight))
      {
        Transform climbCeilingTransform = climbCeilingInfo.transform;
        if (climbCheckTransform == climbCeilingTransform)
        {
          bool shouldHang = !charController.isGrounded;
          if(!charController.isGrounded)
          {
            RaycastHit hangCheckInfo;
            Vector3 hangCheckOrigin = transform.position + new Vector3(0.0f, HangClimbThresholdHeight, 0.0f);
            Debug.DrawLine(hangCheckOrigin, hangCheckOrigin + forwardDir*motionCheckDistance, Color.blue, 10.0f, false);
            if(!Physics.Raycast(hangCheckOrigin, forwardDir, out hangCheckInfo, motionCheckDistance))
            {
              shouldHang = false;
            }
          }

          if(shouldHang && (velocity.y < 0.0f) && (velocity.y > -15.0f))
          {
            Vector3 hangTarget = climbCheckInfo.point - forwardDir*charController.radius;
            hangTarget.y = climbCeilingInfo.point.y - charController.height + epsilon;

            velocity.y = 0.0f;
            currentMotion = DefinedMotion.HANG;

            // NOTE: Here we force the player to look at the position where the raycast hit
            //       and freeze horizontal rotation, this is to prevent the player not looking
            //       at the ledge they're hanging on and therefore not being able to climb up
            // TODO: Check that this actually solves the problem? It kinda seems like it doesnt do
            //       anything because our raycast goes out in the transform.forward direction anyways
            Vector3 cameraTargetLoc = hangTarget + cameraChild.localPosition;
            Vector3 cameraLookTarget = climbCheckInfo.point;
            cameraLookTarget.y = cameraTargetLoc.y;
            Quaternion cameraHangOrientation = Quaternion.LookRotation(cameraLookTarget - cameraTargetLoc, Vector3.up);
            transform.rotation = cameraHangOrientation;
            GetComponent<FirstPersonCameraHorizontal>().enabled = false;

            SetAnimBool(animParamJump, false);
            SetAnimBool(animParamClimb, true);
            motionTargets.Clear();
            motionTargets.Add(hangTarget);
            motionProgress = 0;
            
            midAirChecksEnabled = false;
          }
          else if(velocity.y > -epsilon)
          {
            Vector3 climbTarget = climbCeilingInfo.point + new Vector3(0.0f, 0.05f, 0.0f);
            Vector3 climbMidpoint = climbTarget;
            climbMidpoint -= forwardDir * epsilon*10;
            
            Debug.DrawLine(currentPosition, climbMidpoint, Color.red, 10.0f, false);
            Debug.DrawLine(climbMidpoint, climbTarget, Color.red, 10.0f, false);
            
            velocity.y = 0.0f;
            ClimbSound.Play();
            headBob.enabled = false;
            currentMotion = DefinedMotion.CLIMB;
            SetAnimBool(animParamJump, false);
            SetAnimBool(animParamClimb, true);
            motionTargets.Clear();
            motionTargets.Add(climbMidpoint);
            motionTargets.Add(climbTarget);
            motionProgress = 0;
          }
        }
      }
    }
  }
 
  private void CheckForSlideMotion()
  {
    float horizontalVelocityThreshold = 1.0f;
    if ((movementZ > 0.1f) &&
        (HorizontalVelocity.sqrMagnitude > horizontalVelocityThreshold *
                                           horizontalVelocityThreshold))
    {
      SlideSound.Play();
      hasOverhead = true;
      currentMotion = DefinedMotion.SLIDE;
      SetAnimBool(animParamSlide, true);
      headBob.enabled = false;
      transform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
      SetAvatarScale(new Vector3(1.3f, 2.0f*1.3f, 1.3f));
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
      lastGroundedTime = Time.time;

      velocity.x = moveVector.x;
      velocity.z = moveVector.z;
      isJumping = false;
      midAirChecksEnabled = true;
    }
    else
    {
      if (isJumping && midAirChecksEnabled)
      {
        CheckForVaultClimbMotion();
      }
    }
    
    // TODO: Since velocity has been update here, we have the velocity based on the input alone
    //       and does not consider the actual velocity based on collision
    if (((Time.time - lastGroundedTime) <= MaxUngroundedJumpTime) && shouldJump && !isJumping)
    {
      CheckForVaultClimbMotion();
      
      if (currentMotion == DefinedMotion.NONE)
      {
        JumpSound.Play();
        isJumping = true;
        velocity.y = JumpForce;
        SetAnimBool(animParamJump, true);
      }
    }
    else if(currentMotion == DefinedMotion.NONE) // Check that we aren't hanging/vaulting/whatever
    {
      velocity.y -= 9.81f * Time.fixedDeltaTime;
    }

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
      if (velocity.y < (-1.0f * MaxSafeYVelocity))
      {
        lifeHandler.KillPlayer();
      }
    }

    // Update velocity if we got blocked somewhere along the way
    if (velocity.sqrMagnitude - actualMoveVelocity.sqrMagnitude > 0.01f)
    {
      velocity = actualMoveVelocity;
    }

    Vector2 actualHorizontalVelocity = new Vector3(actualMoveVelocity.x, actualMoveVelocity.z);
    SetAnimFloat(animParamSpeed, actualHorizontalVelocity.magnitude);
    
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
    shouldJump = false;
    shouldSlide = false;
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
    Vector3 newCamLoc = cameraChild.localPosition;
    newCamLoc.y = GetAnimFloat(animParamCameraY);
    newCamLoc.z = GetAnimFloat(animParamCameraZ);
    cameraChild.localPosition = newCamLoc;

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

    // NOTE: We need to check if we were prevented from moving so we can return control to the player
    Vector3 moveOffset = postMovePosition - preMovePosition;
    if (moveOffset.sqrMagnitude < (motionMoveDistance * motionMoveDistance) - 0.001f)
    {
      return true;
    }

    return false;
  }

  /// <summary>
  /// Updates the character based on the current state of hanging
  /// </summary>
  private void UpdateHangMotion()
  {
    if(motionProgress < motionTargets.Count)
    {
      float motionMoveDistance = 1.5f * ClimbSpeed * Time.fixedDeltaTime;
      Vector3 targetOffset = motionTargets[motionProgress] - transform.position;
      if(targetOffset.sqrMagnitude < motionMoveDistance * motionMoveDistance)
      {
        charController.Move(targetOffset);
        ++motionProgress;

        if(motionProgress == motionTargets.Count)
        {
          SetAnimSpeed(0.0f);
        }
      }
      else
      {
        Vector3 targetDirection = targetOffset.normalized;
        charController.Move(targetDirection * motionMoveDistance);
      }
    }
    else
    {
      bool shouldExit = false;
      if(shouldJump)
      {
        shouldExit = true;
        CheckForVaultClimbMotion();
        if(currentMotion == DefinedMotion.HANG)
        {
          Debug.Log("We tried to climb but couldn't! AAAAAAAAAAAHHH (This shouldnt happen =/)");
        }
      }
      if(shouldSlide)
      {
        currentMotion = DefinedMotion.NONE;
        shouldExit = true;
      }

      if(shouldExit)
      {
        SetAnimSpeed(1.0f);
        SetAnimBool(animParamClimb, false);
        GetComponent<FirstPersonCameraHorizontal>().enabled = true;
      }
    }
  }
  
  private void UpdateSlideMotion()
  {
    float movementZ = 1.0f;
    float movementX = InputSplitter.GetHorizontalAxis(PlayerID);
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

    // We should stop sliding if we are either:
    //  1) No longer moving fast enough to warrant a slide
    //  2) No longer holding down the slide key
    bool shouldStopSlide = false;
    shouldStopSlide = shouldStopSlide || (RunSpeed < SlideStopSpeedThreshold);
    shouldStopSlide = shouldStopSlide || (!InputSplitter.GetSlide(PlayerID));
    if (shouldStopSlide)
    {
      headBob.enabled = true;

      currentMotion = DefinedMotion.NONE;
      SetAnimBool(animParamSlide, false);
      RunSpeed = DefaultRunSpeed;
    }
  }

  private void Update()
  {
    movementZ = InputSplitter.GetVerticalAxis(PlayerID);
    movementX = InputSplitter.GetHorizontalAxis(PlayerID);
    if (InputSplitter.GetSlidePressed(PlayerID))
    {
      shouldSlide = true;
    }

    if (InputSplitter.GetJumpPressed(PlayerID))
    {
      shouldJump = true;
    }
  }

  private void FixedUpdate()
  {
    float avatarYOffset = GetAnimFloat(animParamYOffset);
    Vector3 localAvatarLoc = localViewAnimator.transform.localPosition;
    localAvatarLoc.y = avatarYOffset;
    localViewAnimator.transform.localPosition = localAvatarLoc;
    Vector3 enemyAvatarLoc = enemyViewAnimator.transform.localPosition;
    enemyAvatarLoc.y = avatarYOffset;
    enemyViewAnimator.transform.localPosition = enemyAvatarLoc;

    switch (currentMotion)
    {
      case DefinedMotion.NONE:
      {
        previousHasOverhead = hasOverhead;
        hasOverhead = ((charController.collisionFlags & CollisionFlags.Above) != 0);
        if(!hasOverhead && previousHasOverhead)
        {
          hasOverhead = Physics.Raycast(transform.position, Vector3.up, 1.8f, 1 << 0);
          if(!hasOverhead)
          {
            transform.localScale = Vector3.one;
            SetAvatarScale(1.3f*Vector3.one);
          }
        }
        UpdateOnPlayerInput();
      } break;

      case DefinedMotion.HANG:
      {
        UpdateHangMotion();
      } break;

      case DefinedMotion.VAULT:
      case DefinedMotion.CLIMB:
      {
        bool motionComplete = UpdateVaultClimbMotion();
        if (motionComplete)
        {
          cameraChild.localPosition = new Vector3(0.0f, 1.7f, 0.0f);
          headBob.enabled = true;
          currentMotion = DefinedMotion.NONE;
          SetAnimBool(animParamClimb, false);
          SetAnimBool(animParamVault, false);
        }
      } break;

      case DefinedMotion.SLIDE:
      {
        UpdateSlideMotion();
      } break;

      default:
        Debug.Log("Attempt to update on unrecognized motion");
        break;
    }

    if(currentMotion == DefinedMotion.NONE)
    {
      SetAnimBool(animParamJump, !charController.isGrounded);
    }
    else
    {
      SetAnimBool(animParamJump, false);
    }
  }

  private float GetAnimFloat(int paramHash)
  {
    return localViewAnimator.GetFloat(paramHash);
  }

  private void SetAnimFloat(int paramHash, float newVal)
  {
    localViewAnimator.SetFloat(paramHash, newVal);
    enemyViewAnimator.SetFloat(paramHash, newVal);
  }

  private void SetAnimBool(int paramHash, bool newVal)
  {
    localViewAnimator.SetBool(paramHash, newVal);
    enemyViewAnimator.SetBool(paramHash, newVal);
  }

  private void SetAnimSpeed(float newSpeed)
  {
    localViewAnimator.speed = newSpeed;
    enemyViewAnimator.speed = newSpeed;
  }

  private void SetAvatarScale(Vector3 newScale)
  {
    localViewAnimator.transform.localScale = newScale;
    enemyViewAnimator.transform.localScale = newScale;
  }
}
}
