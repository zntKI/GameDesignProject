using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    // Contains all the player variables which can be set from unity editor
    [Header("Movement variables")]
    [Space(8)]
    [SerializeField]
    public float MOVE_SPEED_BASIC = 5f;
    [SerializeField]
    public float MOVE_SPEED_MARIO = 10f;
    [SerializeField]
    public float MARIO_MAXIMUM_VEL = 10f;
    [SerializeField]
    public float MOVE_SPEED_HOLLOW_KNIGHT = 10f;
    [SerializeField]
    public float MOVE_SPEED_CELESTE = 10f;
    [Space(6)]
    [SerializeField]
    public float ACCEL_RATE_HOLLOW_KNIGHT = 9f;
    [SerializeField]
    public float ACCEL_RATE_CELESTE = 13f;
    [Space(6)]
    [SerializeField]
    public float DECCEL_RATE_HOLLOW_KNIGHT = 9f;
    [SerializeField]
    public float DECCEL_RATE_CELESTE = 16f;

    [Header("Jump variables")]
    [Space(8)]
    [SerializeField]
    public Transform groundCheck;
    [SerializeField]
    public LayerMask groundLayer;

    [Space(6)]
    [SerializeField]
    public float GRAVITY_SCALE_MARIO;
    [SerializeField]
    public float GRAVITY_SCALE_HOLLOW_KNIGHT;
    [SerializeField]
    public float GRAVITY_SCALE_CELESTE;

    [Space(6)]
    [SerializeField]
    public float JUMP_POWER_MARIO;
    [SerializeField]
    public float JUMP_POWER_HOLLOW_KNIGHT;
    [SerializeField]
    public float JUMP_POWER_CELESTE;

    [Space(6)]
    [SerializeField]
    public float VARY_GRAVITY_AMAOUNT_MARIO;
    [SerializeField]
    public float VARY_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
    [SerializeField]
    public float VARY_GRAVITY_AMAOUNT_CELESTE;
    [Space(6)]
    [SerializeField]
    public float FALLING_GRAVITY_AMAOUNT_MARIO;
    [SerializeField]
    public float FALLING_GRAVITY_AMAOUNT_HOLLOW_KNIGHT;
    [SerializeField]
    public float FALLING_GRAVITY_AMAOUNT_CELESTE;
    [Space(6)]
    [SerializeField]
    public float MAX_FALL_SPEED_MARIO;
    [SerializeField]
    public float MAX_FALL_SPEED_HOLLOW_KNIGHT;
    [SerializeField]
    public float MAX_FALL_SPEED_CELESTE;

    [Header("Wall sliding variables")]
    [Space(8)]
    [SerializeField]
    public Transform wallCheck;
    [SerializeField]
    public float wallSlidingSpeed;

    [Header("Wall jumping variables")]
    [Space(8)]
    [SerializeField]
    public float WALL_JUMPING_DURATION_HOLLOW_KNIGHT;
    [SerializeField]
    public float WALL_JUMPING_DURATION_CELESTE;
    [Space(6)]
    [SerializeField]
    public Vector2 WALL_JUMPING_POWER_HOLLOW_KNIGHT;
    [SerializeField]
    public Vector2 WALL_JUMPING_POWER_CELESTE;
    [Space(6)]
    [SerializeField]
    public float WALL_JUMPING_GRAVITY_SCALE_HOLLOW_KNIGHT;
    [SerializeField]
    public float WALL_JUMPING_GRAVITY_SCALE_CELESTE;

    [Space(6)]
    [SerializeField]
    public float wallJumpingTime = 0.2f;

    [Header("Dashing variables")]
    [Space(8)]
    [SerializeField]
    public float dashPower = 10f;
    [SerializeField]
    public float dashingTime = 0.3f;
}
