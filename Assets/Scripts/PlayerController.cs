using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Sprite basic;
    [SerializeField]
    private Sprite mario;
    [SerializeField]
    private Sprite hollow_knight;
    [SerializeField]
    private Sprite celeste;
    [SerializeField]
    private Sprite mixed;


    public List<string> moveFeels = new List<string>() { "BASIC" };
    public List<string> specialAbilities = new List<string>();

    public bool isAtNextLevel = true;

    public enum BasicAbilityFeel { BASIC, MARIO, HOLLOW_KNIGHT, CELESTE };
    public enum SpecialAbility { NONE, JUMP, DASH, DOUBLE_JUMP, WALL_JUMP };


    public BasicAbilityFeel basicAbilityFeel;
    public SpecialAbility specialAbility;

    //General variables
    private PlayerData Data;

    private Rigidbody2D rb;
    private PhysicsMaterial2D material2D;

    private bool isFacingRight = true;

    public int coinsNum;
    public int geoNum;
    public List<string> tempStrawberries = new List<string>();
    public List<string> strawberries = new List<string>();

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
    public bool isDashing = false;
    private bool canDash = true;
    private Vector2 dashDirection = Vector2.zero;

    private bool doubleJump;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeDependencies();

        UpdateMoveVariables("BASIC");
        UpdateJumpVariables("MARIO");
        //UpdateWallJumpVariables("MARIO");
        //UpdateSpecialAbility("DOUBLE_JUMP");
    }

    private void InitializeDependencies()
    {
        Data = GetComponent<PlayerData>();

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

            if (tempStrawberries.Count != 0)
            {
                //Debug.Log("IN1");
                for (int i = 0; i < tempStrawberries.Count; i++)
                {
                    strawberries.Add(tempStrawberries[0]);
                    tempStrawberries.RemoveAt(i);
                }
            }
        }

        //Jump
        if ((specialAbility == SpecialAbility.JUMP || specialAbility == SpecialAbility.DOUBLE_JUMP) && Input.GetButtonDown("Jump"))
        {
            if (specialAbility == SpecialAbility.DOUBLE_JUMP && (IsGrounded() || (doubleJump && !IsWalled())))
            {
                if (doubleJump)
                {
                    rb.gravityScale = originalGravityScale;
                }
                shouldJump = true;
                //Debug.Log("IN2");
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
            rb.gravityScale = 0;

            canDash = false;
            dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (dashDirection == Vector2.zero)
            {
                dashDirection = new Vector2(transform.localScale.x, 0).normalized;
            }
            StartCoroutine(StopDashing());
        }

        CheckIfDead();
    }

    private void CheckIfDead()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        if ((index == 1 || index == 2) && this.transform.position.y < 0)
        {
            var start = GameObject.Find("Start");
            this.transform.position = new Vector3(start.transform.position.x + 2f, start.transform.position.y, this.transform.position.z);

            FindObjectOfType<AbilitySwitchPanelController>().HidePanel();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsGrounded())
        {
            canDash = true;
        }

        if (!isWallJumping && !isDashing)
        {
            ApplyHorizontalMovement();
            ApplyJump();
        }
        //ApplyWallSlide();
        if (isDashing)
        {
            //transform.Translate(Data.dashPower * dashDirection.x, Data.dashPower * dashDirection.y, 0f);
            rb.velocity = new Vector2(Data.dashPower * dashDirection.x, Data.dashPower * dashDirection.y);
        }
    }

    public void SwitchSprite(string mode)
    {
        var spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        if (mode == "MARIO")
        {
            spriteRenderer.sprite = mario;
        }
        else if (mode == "HOLLOW_KNIGHT")
        {
            spriteRenderer.sprite = hollow_knight;
        }
        else if (mode == "CELESTE")
        {
            spriteRenderer.sprite = celeste;
        }
        else if (mode == "MIXED")
        {
            spriteRenderer.sprite = mixed;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(Data.dashingTime);
        rb.gravityScale = originalGravityScale;
        isDashing = false;
    }

    private void ApplyHorizontalMovement()
    {
        if (basicAbilityFeel == BasicAbilityFeel.BASIC)
        {
            rb.velocity = new Vector2(moveSpeed * dirRaw, rb.velocity.y);
        }
        else if (basicAbilityFeel == BasicAbilityFeel.MARIO)
        {
            if (dirRaw != 0)
            {
                if (IsGrounded())
                    rb.AddForce(dirRaw * moveSpeed * Vector2.right);
                else
                    rb.AddForce(dirRaw * moveSpeed / 2 * Vector2.right);

                if (Mathf.Abs(rb.velocity.x) > Data.MARIO_MAXIMUM_VEL)
                {
                    rb.velocity = new Vector2(Data.MARIO_MAXIMUM_VEL * Mathf.Sign(rb.velocity.x), rb.velocity.y);
                }

                lastDirRaw = dirRaw;
            }
            else if (lastDirRaw != 0 && rb.velocity.x != 0)
            {
                rb.AddForce(-lastDirRaw * Data.MARIO_MAXIMUM_VEL / 5 * Vector2.right);
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

            if (basicAbilityFeel == BasicAbilityFeel.HOLLOW_KNIGHT || basicAbilityFeel == BasicAbilityFeel.CELESTE)
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
        if (basicAbilityFeel == BasicAbilityFeel.HOLLOW_KNIGHT || basicAbilityFeel == BasicAbilityFeel.CELESTE)
        {
            bool isWalled = IsWalled();

            if (isWalled && !IsGrounded() && dirRaw != 0f)
            {
                isWallSliding = true;
                rb.velocity = new Vector2(rb.velocity.x, -Data.wallSlidingSpeed);
                rb.gravityScale = 0;
            }
            else
            {
                isWallSliding = false;
                if ((isWalled && dirRaw == 0) /*|| (!isWalled && dirRaw != 0)*/)
                {
                    //Debug.Log("IN");
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

        //Debug.Log(rb.velocity.y);
        if (rb.velocity.y < 0f && !IsGrounded())
        {
            if (rb.velocity.y >= -maxFallSpeed)
            {
                rb.gravityScale *= fallingGravityAmount;
                //Debug.Log("IN2");
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
            }
        }
        else if (rb.velocity.y <= .1f)
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
        if (basicAbilityFeel == BasicAbilityFeel.MARIO || basicAbilityFeel == BasicAbilityFeel.BASIC)
        {
            return;
        }

        if (IsWalled() && !IsGrounded())
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = Data.wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (specialAbility == SpecialAbility.WALL_JUMP && Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
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
        if (basicAbilityFeel == BasicAbilityFeel.HOLLOW_KNIGHT && Mathf.Sign(wallJumpingDirection) == dirRaw)
        {
            rb.gravityScale = Data.WALL_JUMPING_GRAVITY_SCALE_HOLLOW_KNIGHT * 5f;
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
        return Physics2D.OverlapCircle(Data.wallCheck.position, 0.2f, Data.groundLayer);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(Data.groundCheck.position, 0.2f, Data.groundLayer);
    }

    public void Bounce(float amount, Vector2 dir)
    {
        rb.velocity = dir * amount;
        rb.gravityScale = originalGravityScale;

        doubleJump = true;
        canDash = true;
        //Debug.Log("IN3");
    }

    public void IncreaseScore(string collectable)
    {
        if (collectable.Contains("Coin"))
        {
            coinsNum++;
        }
        else if (collectable.Contains("Geo"))
        {
            geoNum++;
        }
        else if (collectable.Contains("Strawberry"))
        {
            tempStrawberries.Add(collectable);
        }
        else
        {
            Debug.Log("Incorrect collectable name!!!");
        }
    }

    public void ResetGravityScale()
    {
        rb.gravityScale = originalGravityScale;
    }

    private void UpdateMoveVariables(string currentMoveFeel)
    {
        switch (currentMoveFeel)
        {
            case "BASIC":
                material2D.friction = 0f;
                moveSpeed = Data.MOVE_SPEED_BASIC;
                break;
            case "MARIO":
                material2D.friction = 0.2f;
                moveSpeed = Data.MOVE_SPEED_MARIO;
                break;
            case "HOLLOW_KNIGHT":
                material2D.friction = 0f;

                moveSpeed = Data.MOVE_SPEED_HOLLOW_KNIGHT;
                accelRate = Data.ACCEL_RATE_HOLLOW_KNIGHT;
                deccelRate = Data.DECCEL_RATE_HOLLOW_KNIGHT;
                break;
            case "CELESTE":
                material2D.friction = 0f;

                moveSpeed = Data.MOVE_SPEED_CELESTE;
                accelRate = Data.ACCEL_RATE_CELESTE;
                deccelRate = Data.DECCEL_RATE_CELESTE;
                break;
            default:
                break;
        }
    }

    private void UpdateJumpVariables(string currentJumpFeel)
    {
        switch (currentJumpFeel)
        {
            case "MARIO":
                originalGravityScale = Data.GRAVITY_SCALE_MARIO;

                jumpPower = Data.JUMP_POWER_MARIO;

                varyGravityAmount = Data.VARY_GRAVITY_AMAOUNT_MARIO;
                fallingGravityAmount = Data.FALLING_GRAVITY_AMAOUNT_MARIO;
                maxFallSpeed = Data.MAX_FALL_SPEED_MARIO;
                break;
            case "HOLLOW_KNIGHT":
                originalGravityScale = Data.GRAVITY_SCALE_HOLLOW_KNIGHT;

                jumpPower = Data.JUMP_POWER_HOLLOW_KNIGHT;

                varyGravityAmount = Data.VARY_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
                fallingGravityAmount = Data.FALLING_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
                maxFallSpeed = Data.MAX_FALL_SPEED_HOLLOW_KNIGHT;
                break;
            case "CELESTE":
                originalGravityScale = Data.GRAVITY_SCALE_CELESTE;

                jumpPower = Data.JUMP_POWER_CELESTE;

                varyGravityAmount = Data.VARY_GRAVITY_AMAOUNT_CELESTE;
                fallingGravityAmount = Data.FALLING_GRAVITY_AMAOUNT_CELESTE;
                maxFallSpeed = Data.MAX_FALL_SPEED_CELESTE;
                break;
            default:
                break;
        }
    }

    private void UpdateWallJumpVariables(string currentWallJumpFeel)
    {
        switch (currentWallJumpFeel)
        {
            case "HOLLOW_KNIGHT":
                wallJumpingDuration = Data.WALL_JUMPING_DURATION_HOLLOW_KNIGHT;
                wallJumpingPower = Data.WALL_JUMPING_POWER_HOLLOW_KNIGHT;

                wallJumpGravity = Data.WALL_JUMPING_GRAVITY_SCALE_HOLLOW_KNIGHT;
                break;
            case "CELESTE":
                wallJumpingDuration = Data.WALL_JUMPING_DURATION_CELESTE;
                wallJumpingPower = Data.WALL_JUMPING_POWER_CELESTE;

                wallJumpGravity = Data.WALL_JUMPING_GRAVITY_SCALE_CELESTE;
                break;
            default:
                break;
        }
    }

    public void UpdateBasicAbilityFeel(string currentBasicAbilityFeel)
    {
        switch (currentBasicAbilityFeel)
        {
            case "BASIC":
                basicAbilityFeel = BasicAbilityFeel.BASIC;

                UpdateMoveVariables("BASIC");
                UpdateJumpVariables("MARIO");
                break;
            case "MARIO":
                basicAbilityFeel = BasicAbilityFeel.MARIO;

                UpdateMoveVariables("MARIO");
                UpdateJumpVariables("MARIO");
                break;
            case "HOLLOW_KNIGHT":
                basicAbilityFeel = BasicAbilityFeel.HOLLOW_KNIGHT;

                UpdateMoveVariables("HOLLOW_KNIGHT");
                UpdateJumpVariables("HOLLOW_KNIGHT");
                UpdateWallJumpVariables("HOLLOW_KNIGHT");
                break;
            case "CELESTE":
                basicAbilityFeel = BasicAbilityFeel.CELESTE;

                UpdateMoveVariables("CELESTE");
                UpdateJumpVariables("CELESTE");
                UpdateWallJumpVariables("CELESTE");
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
            case "JUMP":
                specialAbility = SpecialAbility.JUMP;
                break;
            case "DASH":
                specialAbility = SpecialAbility.DASH;
                break;
            case "DOUBLE_JUMP":
                //if (specialAbility == SpecialAbility.DASH)
                //{
                doubleJump = true;
                //}

                specialAbility = SpecialAbility.DOUBLE_JUMP;
                break;
            case "WALL_JUMP":
                specialAbility = SpecialAbility.WALL_JUMP;
                break;
            default:
                Debug.Log("Wrong image name in the game hierarchy!!!");
                break;
        }
    }
}
