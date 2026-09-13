using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CollisioningDirections), typeof(IsVulnerable))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkS = 5;
    [SerializeField]
    private float runS = 10;
    [SerializeField]
    private float airS = 4;
    [SerializeField]
    private float jumpVector = 8;

    Vector2 moveInput;
    CollisioningDirections collisioningDirections;
    IsVulnerable isVulnerable;

    public float MovingSpeed
    {
        get
        {
            if (canAct)
            {
                if (IsMoving && !collisioningDirections.IsTouchingWall)
                {
                    if (collisioningDirections.IsOnFloor)
                    {
                        if (IsRunning)
                        {
                            return runS;
                        }
                        else
                        {
                            return walkS;
                        }
                    }
                    else
                    {
                        return airS;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }


    private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }

    private bool _isRunning = false;

    public bool IsRunning
    {
        get { return _isRunning; }
        set
        {
            _isRunning = value;
            animator.SetBool("isRunning", value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                _isFacingRight = value;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }


    Rigidbody2D rb;
    Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collisioningDirections = GetComponent<CollisioningDirections>();
        isVulnerable = GetComponent<IsVulnerable>();
    }

    private void FixedUpdate()
    {
        if (!isVulnerable.IsHurt && !isVulnerable.IsKnockbackActive() && isVulnerable.HasHP)
        {   
            rb.velocity = new Vector2(moveInput.x * MovingSpeed, rb.velocity.y);
        }

        animator.SetFloat("yVelocity", rb.velocity.y);
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (HasHP)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public bool canAct
    {
        get
        {
            return animator.GetBool("canAct");
        }
    }

    public bool HasHP
    {
        get
        {
            return animator.GetBool("hasHP");
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && collisioningDirections.IsOnFloor && canAct)
        {
            animator.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpVector);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger("attack");
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}