using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public int attackDmg;
    public Vector2 knockback = Vector2.zero;


    private void OnTriggerExit2D(Collider2D collision)
    {
        IsVulnerable isVulnerable = collision.GetComponent<IsVulnerable>();

        if (isVulnerable != null)
        {
            Vector2 oppositeKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool isHit = isVulnerable.AttackHit(attackDmg, oppositeKnockback);

            if (isHit)
                Debug.Log(collision.name + "DMG: " + attackDmg);
        }
    }
}
