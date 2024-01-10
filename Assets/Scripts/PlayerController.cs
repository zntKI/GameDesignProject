using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum MovementFeel { MARIO, HOLLOW_KNIGHT, CELESTE };
    private enum JumpFeel { MARIO, HOLLOW_KNIGHT, CELESTE };
    private enum WallJumpFeel { MARIO, HOLLOW_KNIGHT, CELESTE };
    private enum WallSlideFeel { MARIO, HOLLOW_KNIGHT_CELESTE };
    private enum SpecialAbility { DASH, DOUBLE_JUMP };


    private MovementFeel movementFeel;
    private JumpFeel jumpFeel;
    private WallJumpFeel wallJumpFeel;
    private WallSlideFeel wallSlideFeel;
    private SpecialAbility specialAbility;

    [Header("Movement variables")]
    [Space(8)]
    [SerializeField]
    private float MOVE_SPEED_MARIO = 10f;
    [SerializeField]
    private float MARIO_MAXIMUM_VEL = 10f;
    [SerializeField]
    private float MOVE_SPEED_HOLLOW_KNIGHT = 10f;
    [SerializeField]
    private float MOVE_SPEED_CELESTE = 10f;
    [Space(6)]
    [SerializeField]
    private float ACCEL_RATE_HOLLOW_KNIGHT = 9f;
    [SerializeField]
    private float ACCEL_RATE_CELESTE = 13f;
    [Space(6)]
    [SerializeField]
    private float DECCEL_RATE_HOLLOW_KNIGHT = 9f;
    [SerializeField]
    private float DECCEL_RATE_CELESTE = 16f;

    [Header("Jump variables")]
    [Space(8)]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundLayer;

    [Space(6)]
    [SerializeField]
    private float GRAVITY_SCALE_MARIO;
    [SerializeField]
    private float GRAVITY_SCALE_HOLLOW_KNIGHT;
    [SerializeField]
    private float GRAVITY_SCALE_CELESTE;

    [Space(6)]
    [SerializeField]
    private float JUMP_POWER_MARIO;
    [SerializeField]
    private float JUMP_POWER_HOLLOW_KNIGHT;
    [SerializeField]
    private float JUMP_POWER_CELESTE;

    [Space(6)]
    [SerializeField]
    private float VARY_GRAVITY_AMAOUNT_MARIO;
    [SerializeField]
    private float VARY_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
    [SerializeField]
    private float VARY_GRAVITY_AMAOUNT_CELESTE;
    [Space(6)]
    [SerializeField]
    private float FALLING_GRAVITY_AMAOUNT_MARIO;
    [SerializeField]
    private float FALLING_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
    [SerializeField]
    private float FALLING_GRAVITY_AMAOUNT_CELESTE;
    [Space(6)]
    [SerializeField]
    private float MAX_FALL_SPEED_MARIO;
    [SerializeField]
    private float MAX_FALL_SPEED_HOLLOW_KNIGHT;
    [SerializeField]
    private float MAX_FALL_SPEED_CELESTE;

    [Header("Wall sliding variables")]
    [Space(8)]
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private float wallSlidingSpeed;

    [Header("Wall jumping variables")]
    [Space(8)]
    [SerializeField]
    private float WALL_JUMPING_DURATION_HOLLOW_KNIGHT;
    [SerializeField]
    private float WALL_JUMPING_DURATION_CELESTE;
    [Space(6)]
    [SerializeField]
    private Vector2 WALL_JUMPING_POWER_HOLLOW_KNIGHT;
    [SerializeField]
    private Vector2 WALL_JUMPING_POWER_CELESTE;
    [Space(6)]
    [SerializeField]
    private float WALL_JUMPING_GRAVITY_SCALE_HOLLOW_KNIGHT;
    [SerializeField]
    private float WALL_JUMPING_GRAVITY_SCALE_CELESTE;

    [Space(6)]
    [SerializeField]
    private float wallJumpingTime = 0.2f;

    //General variables
    private Rigidbody2D rb;
    private PhysicsMaterial2D material2D;

    private bool isFacingRight = true;

    //Movement variables
    private float dirRaw;
    private float lastDirRaw;

    private float moveSpeed;
    private float accelRate;
    private float deccelRate;

    //Jump variables
    private float jumpPower;
    private float originalGravityScale;

    private float varyGravityAmount;
    private float fallingGravityAmount;
    private float maxFallSpeed;

    private bool shouldJump = false;
    private bool shouldVaryJumpHeight = false;

    //Wall slide variables
    private bool isWallSliding = false;

    //Wall juming variables
    private bool isWallJumping = false;
    private float wallJumpingDirection;
    private float wallJumpingCounter;
    private float wallJumpingDuration;
    private Vector2 wallJumpingPower;

    private float wallJumpGravity;

    //Dash variables
    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField]
    private float dashPower = 10f;
    [SerializeField]
    private float dashingTime = 0.3f;
    private Vector2 dashDirection = Vector2.zero;

    private bool doubleJump;


    // Start is called before the first frame update
    void Start()
    {
        InitializeDependencies();

        UpdateMoveFeel("MARIO");
        UpdateJumpFeel("MARIO");
        UpdateWallSlideFeel("MARIO");
        UpdateWallJumpFeel("MARIO");
        UpdateSpecialAbility("DOUBLE_JUMP");
    }

    private void InitializeDependencies()
    {
        rb = GetComponent<Rigidbody2D>();
        material2D = GetComponent<CapsuleCollider2D>().sharedMaterial;
    }

    void Update()
    {
        //Horizontal movement
        dirRaw = Input.GetAxisRaw("Horizontal");

        if (IsGrounded() && !Input.GetButton("Jump"))
        {
            doubleJump = false;
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (specialAbility == SpecialAbility.DOUBLE_JUMP && (IsGrounded() || doubleJump))
            {
                shouldJump = true;
                doubleJump = !doubleJump;
            }
            else if (specialAbility != SpecialAbility.DOUBLE_JUMP && IsGrounded())
            {
                shouldJump = true;
            }
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            shouldVaryJumpHeight = true;
        }

        ApplyWallJump();

        if (!isWallJumping)
        {
            Flip();
        }

        if (specialAbility == SpecialAbility.DASH && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            isDashing = true;
            canDash = false;
            dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (dashDirection == Vector2.zero)
            {
                dashDirection = new Vector2(transform.localScale.x, 0).normalized;
            }
            StartCoroutine(StopDashing());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsGrounded())
        {
            canDash = true;
        }

        if (!isWallJumping)
        {
            ApplyHorizontalMovement();
            ApplyJump();
        }
        ApplyWallSlide();
        if (isDashing)
        {
            rb.velocity = new Vector2(dashPower * dashDirection.x, dashPower * dashDirection.y);
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
    }

    private void ApplyHorizontalMovement()
    {
        if (movementFeel == MovementFeel.MARIO)
        {
            if (dirRaw != 0)
            {
                if (IsGrounded())
                    rb.AddForce(dirRaw * moveSpeed * Vector2.right);
                else
                    rb.AddForce(dirRaw * moveSpeed / 2 * Vector2.right);

                if (Mathf.Abs(rb.velocity.x) > MARIO_MAXIMUM_VEL)
                {
                    rb.velocity = new Vector2(MARIO_MAXIMUM_VEL * Mathf.Sign(rb.velocity.x), rb.velocity.y);
                }

                lastDirRaw = dirRaw;
            }
            else if (lastDirRaw != 0 && rb.velocity.x != 0)
            {
                rb.AddForce(-lastDirRaw * MARIO_MAXIMUM_VEL / 5 * Vector2.right);
                if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(lastDirRaw))
                {
                    lastDirRaw = 0;
                }
            }
        }
        else
        {
            float topSpeed = dirRaw * moveSpeed;
            float speedDif = topSpeed - rb.velocity.x;
            float accAction = Mathf.Abs(speedDif) > 0.01f ? accelRate : deccelRate;

            float amountToAdd = Mathf.Abs(speedDif) * accAction * Mathf.Sign(speedDif);

            rb.AddForce(amountToAdd * Vector2.right);

            if (movementFeel == MovementFeel.HOLLOW_KNIGHT || movementFeel == MovementFeel.CELESTE)
            {
                #region Friction

                if (IsGrounded() && dirRaw == 0)
                {
                    float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), 0.2f);
                    amount *= Mathf.Sign(rb.velocity.x);
                    rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
                }

                #endregion
            }
        }
    }

    private void ApplyWallSlide()
    {
        if (wallSlideFeel == WallSlideFeel.HOLLOW_KNIGHT_CELESTE)
        {
            bool isWalled = IsWalled();

            if (isWalled && !IsGrounded() && dirRaw != 0f)
            {
                isWallSliding = true;
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
                rb.gravityScale = 0;
            }
            else
            {
                isWallSliding = false;
                if ( (isWalled && dirRaw == 0) || (!isWalled && dirRaw != 0) )
                {
                    rb.gravityScale = originalGravityScale;
                }
            }
        }
    }

    private void ApplyJump()
    {
        if (shouldVaryJumpHeight)
        {
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            rb.gravityScale *= varyGravityAmount;

            shouldVaryJumpHeight = false;
        }

        if (rb.velocity.y < 0f)
        {
            if (rb.velocity.y >= -maxFallSpeed)
            {
                rb.gravityScale *= fallingGravityAmount;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
            }
        }
        else if (rb.velocity.y == 0f)
        {
            rb.gravityScale = originalGravityScale;
        }

        if (shouldJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

            shouldJump = false;
        }
    }

    private void ApplyWallJump() 
    {
        if (wallJumpFeel == WallJumpFeel.MARIO)
        {
            return;
        }

        if (IsWalled() && !IsGrounded())
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }

        if (isWallJumping)
        {
            rb.gravityScale = wallJumpGravity;
        }
    }

    private void StopWallJumping() 
    {
        isWallJumping = false;

        //Increasing gravity scale so that the player doesn't try to misuse the wall jumping mechanic by doing more a Celeste thing
        if (wallJumpFeel == WallJumpFeel.HOLLOW_KNIGHT && Mathf.Sign(wallJumpingDirection) == dirRaw)
        {
            rb.gravityScale = WALL_JUMPING_GRAVITY_SCALE_HOLLOW_KNIGHT * 5f;
        }
    }

    private void Flip()
    {
        if (isFacingRight && dirRaw < 0f || !isFacingRight && dirRaw > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool IsWalled() 
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void UpdateMoveFeel(string currentMoveFeel)
    {
        switch (currentMoveFeel)
        {
            case "MARIO":
                movementFeel = MovementFeel.MARIO;

                material2D.friction = 0.2f;
                moveSpeed = MOVE_SPEED_MARIO;
                break;
            case "HOLLOW_KNIGHT":
                movementFeel = MovementFeel.HOLLOW_KNIGHT;

                material2D.friction = 0f;

                moveSpeed = MOVE_SPEED_HOLLOW_KNIGHT;
                accelRate = ACCEL_RATE_HOLLOW_KNIGHT;
                deccelRate = DECCEL_RATE_HOLLOW_KNIGHT;
                break;
            case "CELESTE":
                movementFeel = MovementFeel.CELESTE;

                material2D.friction = 0f;

                moveSpeed = MOVE_SPEED_CELESTE;
                accelRate = ACCEL_RATE_CELESTE;
                deccelRate = DECCEL_RATE_CELESTE;
                break;
            default:
                Debug.Log("Wrong image name in the game hierarchy!!!");
                break;
        }
    }

    public void UpdateJumpFeel(string currentJumpFeel)
    {
        switch (currentJumpFeel)
        {
            case "MARIO":
                jumpFeel = JumpFeel.MARIO;

                originalGravityScale = GRAVITY_SCALE_MARIO;

                jumpPower = JUMP_POWER_MARIO;

                varyGravityAmount = VARY_GRAVITY_AMAOUNT_MARIO;
                fallingGravityAmount = FALLING_GRAVITY_AMAOUNT_MARIO;
                maxFallSpeed = MAX_FALL_SPEED_MARIO;
                break;
            case "HOLLOW_KNIGHT":
                jumpFeel = JumpFeel.HOLLOW_KNIGHT;

                originalGravityScale = GRAVITY_SCALE_HOLLOW_KNIGHT;

                jumpPower = JUMP_POWER_HOLLOW_KNIGHT;

                varyGravityAmount = VARY_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
                fallingGravityAmount = FALLING_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
                maxFallSpeed = MAX_FALL_SPEED_HOLLOW_KNIGHT;
                break;
            case "CELESTE":
                jumpFeel = JumpFeel.CELESTE;

                originalGravityScale = GRAVITY_SCALE_CELESTE;

                jumpPower = JUMP_POWER_CELESTE;

                varyGravityAmount = VARY_GRAVITY_AMAOUNT_CELESTE;
                fallingGravityAmount = FALLING_GRAVITY_AMAOUNT_CELESTE;
                maxFallSpeed = MAX_FALL_SPEED_CELESTE;
                break;
            default:
                Debug.Log("Wrong image name in the game hierarchy!!!");
                break;
        }
    }

    public void UpdateWallSlideFeel(string currentWallSlideFeel)
    {
        switch (currentWallSlideFeel)
        {
            case "MARIO":
                wallSlideFeel = WallSlideFeel.MARIO;
                break;
            case "HOLLOW_KNIGHT/CELESTE":
                wallSlideFeel = WallSlideFeel.HOLLOW_KNIGHT_CELESTE;
                break;
            default:
                Debug.Log("Wrong image name in the game hierarchy!!!");
                break;
        }
    }

    public void UpdateWallJumpFeel(string currentWallJumpFeel)
    {
        switch (currentWallJumpFeel)
        {
            case "MARIO":
                wallJumpFeel = WallJumpFeel.MARIO;
                break;
            case "HOLLOW_KNIGHT":
                wallJumpFeel = WallJumpFeel.HOLLOW_KNIGHT;

                wallJumpingDuration = WALL_JUMPING_DURATION_HOLLOW_KNIGHT;
                wallJumpingPower = WALL_JUMPING_POWER_HOLLOW_KNIGHT;

                wallJumpGravity = WALL_JUMPING_GRAVITY_SCALE_HOLLOW_KNIGHT;
                break;
            case "CELESTE":
                wallJumpFeel = WallJumpFeel.CELESTE;

                wallJumpingDuration = WALL_JUMPING_DURATION_CELESTE;
                wallJumpingPower = WALL_JUMPING_POWER_CELESTE;

                wallJumpGravity = WALL_JUMPING_GRAVITY_SCALE_CELESTE;
                break;
            default:
                Debug.Log("Wrong image name in the game hierarchy!!!");
                break;
        }
    }

    public void UpdateSpecialAbility(string currentSpecialAbility)
    {
        switch (currentSpecialAbility)
        {
            case "DASH":
                specialAbility = SpecialAbility.DASH;
                break;
            case "DOUBLE_JUMP":
                specialAbility = SpecialAbility.DOUBLE_JUMP;
                break;
            default:
                Debug.Log("Wrong image name in the game hierarchy!!!");
                break;
        }
    }
}
