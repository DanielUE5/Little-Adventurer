using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Tracks the enemies in the first level and presents the victory screen when
/// the last SkeletonKnight dies. The UI is generated at runtime so the level
/// does not need any manual Inspector references.
/// </summary>
public class LevelCompleteUI : MonoBehaviour
{
    private const string FirstLevelScene = "GeneralScene";
    private readonly HashSet<int> livingSkeletons = new HashSet<int>();

    private CanvasGroup victoryGroup;
    private RectTransform victoryCard;
    private bool levelCompleted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForFirstLevel()
    {
        if (SceneManager.GetActiveScene().name != FirstLevelScene || FindObjectOfType<LevelCompleteUI>() != null)
            return;

        new GameObject("Level Complete Controller").AddComponent<LevelCompleteUI>();
    }

    private void OnEnable()
    {
        CharacterEventsHandler.characterDied += OnCharacterDied;
    }

    private void Start()
    {
        SkeletonKnight[] skeletons = FindObjectsOfType<SkeletonKnight>();
        foreach (SkeletonKnight skeleton in skeletons)
            livingSkeletons.Add(skeleton.gameObject.GetInstanceID());

        BuildVictoryScreen();

        // A level with no enemies is considered complete as well. This also
        // makes the controller behave sensibly while the scene is being edited.
        if (livingSkeletons.Count == 0)
            CompleteLevel();
    }

    private void OnDisable()
    {
        CharacterEventsHandler.characterDied -= OnCharacterDied;
        Time.timeScale = 1f;
    }

    private void OnCharacterDied(GameObject character)
    {
        if (levelCompleted || character.GetComponent<SkeletonKnight>() == null)
            return;

        if (livingSkeletons.Remove(character.GetInstanceID()) && livingSkeletons.Count == 0)
            CompleteLevel();
    }

    private void CompleteLevel()
    {
        if (levelCompleted)
            return;

        levelCompleted = true;
        victoryGroup.gameObject.SetActive(true);
        Time.timeScale = 0f;

        Button firstButton = victoryGroup.GetComponentInChildren<Button>();
        if (firstButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);

        StartCoroutine(AnimateVictoryScreen());
    }

    private IEnumerator AnimateVictoryScreen()
    {
        const float duration = 0.35f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            victoryGroup.alpha = eased;
            victoryCard.localScale = Vector3.one * Mathf.Lerp(0.88f, 1f, eased);
            yield return null;
        }

        victoryGroup.alpha = 1f;
        victoryCard.localScale = Vector3.one;
    }

    private void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_WEBGL || UNITY_EDITOR
        SceneManager.LoadScene("QuitScene");
#else
        Application.Quit();
#endif
    }

    private void BuildVictoryScreen()
    {
        GameObject root = CreateUIObject("Victory Screen", transform);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();

        victoryGroup = root.AddComponent<CanvasGroup>();
        victoryGroup.alpha = 0f;

        Image dimmer = CreateImage("Backdrop", root.transform, new Color32(5, 8, 18, 215));
        Stretch(dimmer.rectTransform);

        Image glow = CreateImage("Card Glow", root.transform, new Color32(45, 212, 191, 35));
        SetCenteredRect(glow.rectTransform, new Vector2(780f, 520f), new Vector2(0f, -10f));

        Image shadow = CreateImage("Card Shadow", root.transform, new Color32(0, 0, 0, 125));
        SetCenteredRect(shadow.rectTransform, new Vector2(690f, 440f), new Vector2(12f, -18f));

        Image card = CreateImage("Victory Card", root.transform, new Color32(19, 27, 45, 250));
        victoryCard = card.rectTransform;
        SetCenteredRect(victoryCard, new Vector2(690f, 440f), Vector2.zero);

        Image accent = CreateImage("Top Accent", card.transform, new Color32(45, 212, 191, 255));
        RectTransform accentRect = accent.rectTransform;
        accentRect.anchorMin = new Vector2(0f, 1f);
        accentRect.anchorMax = Vector2.one;
        accentRect.pivot = new Vector2(0.5f, 1f);
        accentRect.offsetMin = new Vector2(0f, -8f);
        accentRect.offsetMax = Vector2.zero;

        TMP_Text eyebrow = CreateText("Status", card.transform, "LEVEL 1 COMPLETE", 24f, new Color32(45, 212, 191, 255), FontStyles.Bold);
        SetCenteredRect(eyebrow.rectTransform, new Vector2(600f, 45f), new Vector2(0f, 132f));
        eyebrow.characterSpacing = 5f;

        TMP_Text title = CreateText("Title", card.transform, "VICTORY!", 72f, Color.white, FontStyles.Bold);
        SetCenteredRect(title.rectTransform, new Vector2(620f, 95f), new Vector2(0f, 65f));

        TMP_Text message = CreateText("Message", card.transform, "All skeletons have been defeated.", 28f, new Color32(190, 201, 220, 255), FontStyles.Normal);
        SetCenteredRect(message.rectTransform, new Vector2(620f, 55f), new Vector2(0f, -5f));

        GameObject buttons = CreateUIObject("Actions", card.transform);
        RectTransform buttonsRect = buttons.GetComponent<RectTransform>();
        SetCenteredRect(buttonsRect, new Vector2(550f, 76f), new Vector2(0f, -118f));
        HorizontalLayoutGroup layout = buttons.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 22f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        CreateButton("Play Again", buttons.transform, "PLAY AGAIN", new Color32(45, 212, 191, 255), new Color32(10, 30, 32, 255), PlayAgain);
        CreateButton("Quit", buttons.transform, "QUIT", new Color32(40, 50, 70, 255), Color.white, QuitGame);

        root.SetActive(false);
    }

    private static Button CreateButton(string name, Transform parent, string label, Color background, Color foreground, UnityEngine.Events.UnityAction action)
    {
        Image image = CreateImage(name, parent, background);
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = background;
        colors.highlightedColor = Color.Lerp(background, Color.white, 0.15f);
        colors.pressedColor = Color.Lerp(background, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        TMP_Text text = CreateText("Label", image.transform, label, 25f, foreground, FontStyles.Bold);
        Stretch(text.rectTransform);
        text.characterSpacing = 2f;
        return button;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject result = new GameObject(name, typeof(RectTransform));
        result.transform.SetParent(parent, false);
        return result;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject result = CreateUIObject(name, parent);
        Image image = result.AddComponent<Image>();
        image.color = color;
        return image;
    }

    private static TMP_Text CreateText(string name, Transform parent, string content, float size, Color color, FontStyles style)
    {
        GameObject result = CreateUIObject(name, parent);
        TextMeshProUGUI text = result.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = size;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetCenteredRect(RectTransform rect, Vector2 size, Vector2 position)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
    }
}
