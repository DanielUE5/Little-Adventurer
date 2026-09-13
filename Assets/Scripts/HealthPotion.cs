using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField]
    private int restoredHP;
    [SerializeField]
    private float floatSpeed;
    [SerializeField]
    private float floatY;

    private Vector3 startPosition;
    AudioSource healSource;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {

        float newY = Mathf.Sin(Time.time * floatSpeed) * floatY;
        transform.position = startPosition + new Vector3(0, newY, 0);
    }

    private void Awake()
    {
        healSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsVulnerable isVulnerable = collision.GetComponent<IsVulnerable>();

        if (isVulnerable && isVulnerable.HP < isVulnerable.FullHP)
        {
            bool isHealed = isVulnerable.Heal(restoredHP);

            if (isHealed)
            {
                if (healSource)
                    AudioSource.PlayClipAtPoint(healSource.clip, gameObject.transform.position, healSource.volume);

                Destroy(gameObject);
            }
        }
    }
}
