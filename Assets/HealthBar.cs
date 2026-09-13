using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    IsVulnerable isPlayerVulnerable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }
        isPlayerVulnerable = player.GetComponent<IsVulnerable>();
    }
    void Start()
    {
        healthSlider.value = CalculateSliderPercentage(isPlayerVulnerable.HP, isPlayerVulnerable.FullHP);
        healthBarText.text = "HP " + isPlayerVulnerable.HP + " / " + isPlayerVulnerable.FullHP;
    }

    void OnEnable()
    {
        isPlayerVulnerable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        isPlayerVulnerable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private float CalculateSliderPercentage(int currentHP, int maxHP)
    {
        return (float)currentHP / maxHP;
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = "HP " + newHealth + " / " + maxHealth;
    }
}
