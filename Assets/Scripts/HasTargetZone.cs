using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HasTargetZone : MonoBehaviour
{
    Collider2D coll;
    public List<Collider2D> triggeredColliders = new List<Collider2D>();

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggeredColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        triggeredColliders.Remove(collision);
    }
}
