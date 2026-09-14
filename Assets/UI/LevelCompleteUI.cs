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
    private readonly Dictionary<RectTransform, Coroutine> buttonHoverAnimations = new Dictionary<RectTransform, Coroutine>();

    private CanvasGroup victoryGroup;
    private RectTransform victoryCard;
    private TMP_FontAsset levelFont;
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
        TMP_Text existingText = FindObjectOfType<TMP_Text>();
        if (existingText != null)
            levelFont = existingText.font;

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

        Image dimmer = CreateImage("Backdrop", root.transform, new Color32(28, 17, 29, 218));
        Stretch(dimmer.rectTransform);

        Image glow = CreateImage("Card Glow", root.transform, new Color32(85, 205, 67, 38));
        SetCenteredRect(glow.rectTransform, new Vector2(780f, 520f), new Vector2(0f, -10f));

        Image shadow = CreateImage("Card Shadow", root.transform, new Color32(12, 7, 13, 165));
        SetCenteredRect(shadow.rectTransform, new Vector2(690f, 440f), new Vector2(12f, -18f));

        Image card = CreateImage("Victory Card", root.transform, new Color32(49, 30, 42, 252));
        victoryCard = card.rectTransform;
        SetCenteredRect(victoryCard, new Vector2(690f, 440f), Vector2.zero);
        Outline cardOutline = card.gameObject.AddComponent<Outline>();
        cardOutline.effectColor = new Color32(116, 76, 69, 255);
        cardOutline.effectDistance = new Vector2(4f, -4f);

        Image accent = CreateImage("Top Accent", card.transform, new Color32(83, 202, 62, 255));
        RectTransform accentRect = accent.rectTransform;
        accentRect.anchorMin = new Vector2(0f, 1f);
        accentRect.anchorMax = Vector2.one;
        accentRect.pivot = new Vector2(0.5f, 1f);
        accentRect.offsetMin = new Vector2(0f, -8f);
        accentRect.offsetMax = Vector2.zero;

        Image skyAccent = CreateImage("Sky Accent", card.transform, new Color32(104, 189, 240, 255));
        RectTransform skyAccentRect = skyAccent.rectTransform;
        skyAccentRect.anchorMin = new Vector2(0f, 1f);
        skyAccentRect.anchorMax = Vector2.one;
        skyAccentRect.pivot = new Vector2(0.5f, 1f);
        skyAccentRect.offsetMin = new Vector2(0f, -12f);
        skyAccentRect.offsetMax = new Vector2(0f, -8f);

        TMP_Text eyebrow = CreateText("Status", card.transform, "LEVEL 1 COMPLETE", 24f, new Color32(164, 220, 247, 255), FontStyles.Bold);
        SetCenteredRect(eyebrow.rectTransform, new Vector2(600f, 45f), new Vector2(0f, 132f));
        eyebrow.characterSpacing = 5f;

        TMP_Text title = CreateText("Title", card.transform, "VICTORY!", 72f, new Color32(255, 239, 193, 255), FontStyles.Bold);
        SetCenteredRect(title.rectTransform, new Vector2(620f, 95f), new Vector2(0f, 65f));

        TMP_Text message = CreateText("Message", card.transform, "All skeletons have been defeated.", 28f, new Color32(224, 202, 169, 255), FontStyles.Normal);
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

        CreateButton(
            "Play Again",
            buttons.transform,
            "PLAY AGAIN",
            new Color32(74, 176, 57, 255),
            new Color32(104, 218, 76, 255),
            new Color32(38, 25, 35, 255),
            new Color32(193, 226, 127, 255),
            PlayAgain);
        CreateButton(
            "Quit",
            buttons.transform,
            "QUIT",
            new Color32(112, 70, 64, 255),
            new Color32(151, 94, 78, 255),
            new Color32(255, 239, 193, 255),
            new Color32(229, 177, 119, 255),
            QuitGame);

        root.SetActive(false);
    }

    private Button CreateButton(
        string name,
        Transform parent,
        string label,
        Color background,
        Color highlightedBackground,
        Color foreground,
        Color hoverOutline,
        UnityEngine.Events.UnityAction action)
    {
        Image image = CreateImage(name, parent, Color.white);
        image.raycastTarget = true;
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = background;
        colors.highlightedColor = highlightedBackground;
        colors.pressedColor = Color.Lerp(background, Color.black, 0.18f);
        colors.selectedColor = highlightedBackground;
        colors.disabledColor = new Color(background.r, background.g, background.b, 0.45f);
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        Outline outline = image.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(hoverOutline.r, hoverOutline.g, hoverOutline.b, 0f);
        outline.effectDistance = new Vector2(2f, -2f);

        TMP_Text text = CreateText("Label", image.transform, label, 25f, foreground, FontStyles.Bold);
        Stretch(text.rectTransform);
        text.characterSpacing = 2f;

        EventTrigger trigger = image.gameObject.AddComponent<EventTrigger>();
        AddTrigger(trigger, EventTriggerType.PointerEnter, _ => SetButtonHover(image.rectTransform, outline, hoverOutline, true));
        AddTrigger(trigger, EventTriggerType.PointerExit, _ => SetButtonHover(image.rectTransform, outline, hoverOutline, false));
        AddTrigger(trigger, EventTriggerType.Select, _ => SetButtonHover(image.rectTransform, outline, hoverOutline, true));
        AddTrigger(trigger, EventTriggerType.Deselect, _ => SetButtonHover(image.rectTransform, outline, hoverOutline, false));
        return button;
    }

    private static void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void SetButtonHover(RectTransform buttonRect, Outline outline, Color hoverOutline, bool hovered)
    {
        if (buttonHoverAnimations.TryGetValue(buttonRect, out Coroutine currentAnimation))
            StopCoroutine(currentAnimation);

        buttonHoverAnimations[buttonRect] = StartCoroutine(AnimateButtonHover(buttonRect, outline, hoverOutline, hovered));
    }

    private IEnumerator AnimateButtonHover(RectTransform buttonRect, Outline outline, Color hoverOutline, bool hovered)
    {
        const float duration = 0.12f;
        float elapsed = 0f;
        Vector3 startScale = buttonRect.localScale;
        Vector3 targetScale = Vector3.one * (hovered ? 1.055f : 1f);
        Color startOutline = outline.effectColor;
        Color targetOutline = hovered
            ? hoverOutline
            : new Color(hoverOutline.r, hoverOutline.g, hoverOutline.b, 0f);
        Vector2 startDistance = outline.effectDistance;
        Vector2 targetDistance = hovered ? new Vector2(3f, -3f) : new Vector2(2f, -2f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            buttonRect.localScale = Vector3.LerpUnclamped(startScale, targetScale, eased);
            outline.effectColor = Color.LerpUnclamped(startOutline, targetOutline, eased);
            outline.effectDistance = Vector2.LerpUnclamped(startDistance, targetDistance, eased);
            yield return null;
        }

        buttonRect.localScale = targetScale;
        outline.effectColor = targetOutline;
        outline.effectDistance = targetDistance;
        buttonHoverAnimations.Remove(buttonRect);
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
        image.raycastTarget = false;
        return image;
    }

    private TMP_Text CreateText(string name, Transform parent, string content, float size, Color color, FontStyles style)
    {
        GameObject result = CreateUIObject(name, parent);
        TextMeshProUGUI text = result.AddComponent<TextMeshProUGUI>();
        if (levelFont != null)
            text.font = levelFont;
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
