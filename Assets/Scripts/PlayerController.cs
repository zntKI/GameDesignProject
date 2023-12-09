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
    [SerializeField]
    private MovementFeel movementFeel;
    [SerializeField]
    private JumpFeel jumpFeel;
    [SerializeField]
    private WallJumpFeel wallJumpFeel;
    [SerializeField]
    private WallSlideFeel wallSlideFeel;

    [Header("Movement variables")]
    [SerializeField]
    private float MOVE_SPEED_MARIO = 10f;
    [SerializeField]
    private float MOVE_SPEED_HOLLOW_KNIGHT = 10f;
    [SerializeField]
    private float MOVE_SPEED_CELESTE = 10f;
    [SerializeField]
    private float ACCEL_RATE_HOLLOW_KNIGHT = 9f;
    [SerializeField] 
    private float ACCEL_RATE_CELESTE = 13f;
    [SerializeField] 
    private float DECCEL_RATE_HOLLOW_KNIGHT = 9f;
    [SerializeField] 
    private float DECCEL_RATE_CELESTE = 16f;

    //General variables
    private Rigidbody2D rb;

    //Movement variables
    private float dirRaw;
    private float dir;

    private float moveSpeed;
    private float accelRate;
    private float deccelRate;

    private float lastDirRaw;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        dirRaw = Input.GetAxisRaw("Horizontal");
        dir = Input.GetAxis("Horizontal");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyHorizontalMovement();
    }

    private void ApplyHorizontalMovement()
    {
        if (movementFeel == MovementFeel.MARIO)
        {
            moveSpeed = MOVE_SPEED_MARIO;

            if (Mathf.Abs(rb.velocity.x) < moveSpeed)
            {
                if (dirRaw != 0)
                {
                    rb.AddForce(dirRaw * moveSpeed * Vector2.right);
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
}
