using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum MovementFeel { MARIO, HOLLOW_KNIGHT, CELESTE };
    private enum JumpFeel { MARIO, HOLLOW_KNIGHT, CELESTE };
    private enum WallJumpFeel { HOLLOW_KNIGHT, CELESTE };
    private enum WallSlideFeel { MARIO, HOLLOW_KNIGHT, CELESTE };


    [Header("Basic movement options")]
    [Space(8)]
    [SerializeField]
    private MovementFeel movementFeel;
    [SerializeField]
    private JumpFeel jumpFeel;
    [SerializeField]
    private WallJumpFeel wallJumpFeel;
    [SerializeField]
    private WallSlideFeel wallSlideFeel;

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

    //General variables
    private Rigidbody2D rb;

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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Horizontal movement
        dirRaw = Input.GetAxisRaw("Horizontal");

        //Jump
        Debug.Log(rb.velocity.y);
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            shouldJump = true;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            shouldVaryJumpHeight = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyJump();
    }

    private void ApplyHorizontalMovement()
    {
        if (movementFeel == MovementFeel.MARIO)
        {
            moveSpeed = MOVE_SPEED_MARIO;

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
                rb.AddForce(-lastDirRaw * moveSpeed * Vector2.right);
                if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(lastDirRaw))
                {
                    lastDirRaw = 0;
                }
            }
            return;
        }
        else if (movementFeel == MovementFeel.HOLLOW_KNIGHT)
        {
            moveSpeed = MOVE_SPEED_HOLLOW_KNIGHT;
            accelRate = ACCEL_RATE_HOLLOW_KNIGHT;
            deccelRate = DECCEL_RATE_HOLLOW_KNIGHT;
        }
        else
        {
            moveSpeed = MOVE_SPEED_CELESTE;
            accelRate = ACCEL_RATE_CELESTE;
            deccelRate = DECCEL_RATE_CELESTE;
        }

        float topSpeed = dirRaw * moveSpeed;
        float speedDif = topSpeed - rb.velocity.x;
        float accAction = Mathf.Abs(speedDif) > 0.01f ? accelRate : deccelRate;

        float amountToAdd = Mathf.Abs(speedDif) * accAction * Mathf.Sign(speedDif);

        rb.AddForce(amountToAdd * Vector2.right);
    }

    private void ApplyJump()
    {
        if (jumpFeel == JumpFeel.MARIO)
        {
            originalGravityScale = GRAVITY_SCALE_MARIO;

            jumpPower = JUMP_POWER_MARIO;

            varyGravityAmount = VARY_GRAVITY_AMAOUNT_MARIO;
            fallingGravityAmount = FALLING_GRAVITY_AMAOUNT_MARIO;
            maxFallSpeed = MAX_FALL_SPEED_MARIO;
        }
        else if (jumpFeel == JumpFeel.HOLLOW_KNIGHT)
        {
            originalGravityScale = GRAVITY_SCALE_HOLLOW_KNIGHT;

            jumpPower = JUMP_POWER_HOLLOW_KNIGHT;

            varyGravityAmount = VARY_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
            fallingGravityAmount = FALLING_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
            maxFallSpeed = MAX_FALL_SPEED_HOLLOW_KNIGHT;
        }
        else
        {
            originalGravityScale = GRAVITY_SCALE_CELESTE;

            jumpPower = JUMP_POWER_CELESTE;

            varyGravityAmount = VARY_GRAVITY_AMAOUNT_CELESTE;
            fallingGravityAmount = FALLING_GRAVITY_AMAOUNT_CELESTE;
            maxFallSpeed = MAX_FALL_SPEED_CELESTE;
        }

        if (shouldVaryJumpHeight)
        {
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            rb.gravityScale *= varyGravityAmount;

            shouldVaryJumpHeight = false;
        }

        if (rb.velocity.y < 0)
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
        else if (rb.velocity.y == 0)
        {
            rb.gravityScale = originalGravityScale;
        }

        if (shouldJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

            shouldJump = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
