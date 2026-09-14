using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IsVulnerable : MonoBehaviour
{
    public UnityEvent<int, Vector2> vulnerableHit;
    public UnityEvent<int, int> healthChanged;
    Animator animator;
    Rigidbody2D rb;

    [SerializeField] private int _fullHP;
    [SerializeField] private int _hp;
    [SerializeField] private bool _hasHP = true;
    [SerializeField] private bool isInvulnerable = false;

    public bool IsHurt
    {
        get
        {
            return animator.GetBool("isHurt");
        }
        private set
        {
            animator.SetBool("isHurt", value);
        }
    }

    private float hitTimer = 0;
    public float noDamageTime = 0.25f;

    private float knockbackTime = 0.1f;
    private float knockbackTimer = 0f;
    private Vector2 currentKnockback = Vector2.zero;

    public MonoBehaviour[] scriptsToDisableOnDeath;
    public UnityEvent onDeath;

    public int FullHP
    {
        get { return _fullHP; }
        set { _fullHP = value; }
    }

    public int HP
    {
        get { return _hp; }
        set
        {
            _hp = value;
            healthChanged?.Invoke(_hp, _fullHP);

            // If health drops to zero, trigger death
            if (_hp <= 0 && HasHP)
            {
                Die();
            }
        }
    }

    public bool HasHP
    {
        get { return _hasHP; }
        private set
        {
            _hasHP = value;
            animator.SetBool("hasHP", value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            rb.velocity = new Vector2(currentKnockback.x, rb.velocity.y + currentKnockback.y);
        }

        if (isInvulnerable)
        {
            if (hitTimer > noDamageTime)
            {
                isInvulnerable = false;
                hitTimer = 0;
                ResetHurt();
            }

            hitTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            HP = 0;
        }
    }

    public bool AttackHit(int damage, Vector2 knockback)
    {
        if (HasHP && !isInvulnerable)
        {
            HP -= damage;
            isInvulnerable = true;
            IsHurt = true;

            currentKnockback = knockback;
            knockbackTimer = knockbackTime;

            vulnerableHit?.Invoke(damage, knockback);
            CharacterEventsHandler.damageTaken.Invoke(gameObject, damage);

            return true;
        }

        return false;
    }

    public bool Heal(int restoredHP)
    {
        if (HasHP && HP < FullHP)
        {
            int fullHeal = Mathf.Max(FullHP - HP, 0);
            int healAmount = Mathf.Min(fullHeal, restoredHP);
            HP += healAmount;
            CharacterEventsHandler.healed(gameObject, healAmount);
            return true;
        }

        return false;
    }

    public void Die()
    {
        HasHP = false;
        animator.SetBool("canAct", false);

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        foreach (var script in scriptsToDisableOnDeath)
        {
            if (script != null)
                script.enabled = false;
        }

        onDeath?.Invoke();
        CharacterEventsHandler.characterDied?.Invoke(gameObject);
    }

    public void ResetHurt()
    {
        IsHurt = false;
    }

    public bool IsKnockbackActive()
    {
        return knockbackTimer > 0f;
    }
}
