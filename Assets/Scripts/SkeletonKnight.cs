using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CollisioningDirections))]
public class SkeletonKnight : MonoBehaviour
{
    [SerializeField]
    private float walkSpeedup = 100;
    [SerializeField]
    private float walkPauseDuration = 0.03f;
    [SerializeField]
    private float maxSpeed = 3;

    public HasTargetZone attackZone;
    private Rigidbody2D rb;
    private CollisioningDirections collisioningDirections;
    Animator animator;

    public enum MovingDirection { Left, Right }
    private MovingDirection _walkDirection = MovingDirection.Right;

    private bool hasFlipped = false;
    private Vector3 initialScale;
    private IsVulnerable isVulnerable;

    public MovingDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                Vector3 scale = transform.localScale;
                scale.x = initialScale.x * (_walkDirection == MovingDirection.Left ? 1 : -1);
                transform.localScale = scale;

                _walkDirection = value;
            }
        }
    }

    private Vector2 WalkDirectionVector => (_walkDirection == MovingDirection.Right) ? Vector2.right : Vector2.left;

    public bool _targetDetected = false;
    public bool TagetDetected
    {
        get
        {
            return _targetDetected;
        }
        private set
        {
            _targetDetected = value;
            animator.SetBool("playerTarget", value);
        }
    }

    public bool CanAct
    {
        get
        {
            return animator.GetBool("canAct");
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collisioningDirections = GetComponent<CollisioningDirections>();
        initialScale = transform.localScale;
        animator = GetComponent<Animator>();
        isVulnerable = GetComponent<IsVulnerable>();
    }


    void Update()
    {
        TagetDetected = attackZone.triggeredColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (!hasFlipped && (collisioningDirections.IsTouchingWall || collisioningDirections.IsAtEdge) && collisioningDirections.IsOnFloor)
        {
            FlipDirection();
            hasFlipped = true;
        }
        else if ((!collisioningDirections.IsTouchingWall && !collisioningDirections.IsAtEdge) || !collisioningDirections.IsOnFloor)
        {
            hasFlipped = false;
        }

        if (CanAct)
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (walkSpeedup * WalkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
        else
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkPauseDuration), rb.velocity.y);
    }

    private void FlipDirection()
    {
        if (WalkDirection == MovingDirection.Right)
            WalkDirection = MovingDirection.Left;
        else
            WalkDirection = MovingDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
