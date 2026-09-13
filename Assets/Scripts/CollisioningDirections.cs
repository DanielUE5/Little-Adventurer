using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisioningDirections : MonoBehaviour
{
    public ContactFilter2D castCheck;
    public float altitude = 0.05f;
    public float checkWallDistance = 0.2f;
    public float checkRoofDistance = 0.05f;

    CapsuleCollider2D collDirections;
    Animator animator;

    RaycastHit2D[] hitTerrains = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] roofHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isOnFloor;
    public bool IsOnFloor
    {
        get { return _isOnFloor; }
        private set
        {
            _isOnFloor = value;
            animator.SetBool("isOnFloor", value);
        }
    }

    [SerializeField]
    private bool _isTouchingWall;
    public bool IsTouchingWall
    {
        get { return _isTouchingWall; }
        private set
        {
            _isTouchingWall = value;
            animator.SetBool("isTouchingWall", value);
        }
    }

    [SerializeField]
    private bool _isOnRoof;
    private Vector2 checkWallDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool IsOnRoof
    {
        get { return _isOnRoof; }
        private set
        {
            _isOnRoof = value;
            animator.SetBool("isOnRoof", value);
        }
    }

    [SerializeField]
    private bool _isAtEdge;
    public bool IsAtEdge => _isAtEdge;

    private void Awake()
    {
        collDirections = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        IsOnFloor = collDirections.Cast(Vector2.down, castCheck, hitTerrains, altitude) > 0;
        IsTouchingWall = collDirections.Cast(checkWallDirection, castCheck, wallHits, checkWallDistance) > 0;
        IsOnRoof = collDirections.Cast(Vector2.up, castCheck, roofHits, checkRoofDistance) > 0;

        Vector2 colliderCenter = collDirections.bounds.center;
        float colliderWidth = collDirections.bounds.extents.x;
        float colliderHeight = collDirections.bounds.extents.y;

        Vector2 checkDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        Vector2 edgeCheckPosition = new Vector2(colliderCenter.x + checkDirection.x * colliderWidth, colliderCenter.y - colliderHeight);

        _isAtEdge = !Physics2D.Raycast(edgeCheckPosition, Vector2.down, 0.1f, castCheck.layerMask);
    }
}
