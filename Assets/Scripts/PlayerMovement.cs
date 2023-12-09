using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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

    //General variables
    private Rigidbody2D rb;

    //Horizontal movement variables
    private float dirRaw;
    private float dir;

    private float speedX;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        speedX = 0f;
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

        }

        if (Input.GetKey(KeyCode.A))
        {
            speedX -= 0.3f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            speedX += 0.3f;
        }

        speedX *= 0.95f;
        transform.Translate(new Vector3(speedX * Time.fixedDeltaTime, 0));
    }
}
