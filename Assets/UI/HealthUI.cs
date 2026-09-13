using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public Vector3 displayingSpeed = new Vector3(0, 80, 0);
    public float fadeTime = 1f;
    RectTransform rectTransform;
    TextMeshProUGUI textMeshProUGUI;
    private float timePassed = 0f;
    private Color color;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        color = textMeshProUGUI.color;
    }
    void Update()
    {
        rectTransform.position += displayingSpeed * Time.deltaTime;

        timePassed += Time.deltaTime;

        if (timePassed < fadeTime)
        {
            float fadeAlpha = color.a * 1 - (timePassed / fadeTime);
            textMeshProUGUI.color = new Color(color.r, color.g, color.b, fadeAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
