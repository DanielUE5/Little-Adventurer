using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_Manager : MonoBehaviour
{
    public GameObject dmgText;
    public GameObject hpText;
    public Canvas canvas;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
    }

    private void OnEnable()
    {
        CharacterEventsHandler.damageTaken += TakenHits;
        CharacterEventsHandler.healed += Heal;
    }

    private void OnDisable()
    {
        CharacterEventsHandler.damageTaken -= TakenHits;
        CharacterEventsHandler.healed -= Heal;
    }

    public void TakenHits(GameObject actor, int damageTaken)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(actor.transform.position);

        TMP_Text tmp_text = Instantiate(dmgText, spawnPos, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();
        tmp_text.text = damageTaken.ToString();
    }

    public void Heal(GameObject actor, int healed)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(actor.transform.position);

        TMP_Text tmp_text = Instantiate(hpText, spawnPos, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();
        tmp_text.text = healed.ToString();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnExitGame();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }
    }

    public void OnExitGame()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        SceneManager.LoadScene("QuitScene");
#elif UNITY_STANDALONE
    Application.Quit();
#else
    Application.Quit();
#endif
    }

    public void HomePage()
    {
        Application.OpenURL("https://itch.io/");
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
