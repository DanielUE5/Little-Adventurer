using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitSceneManager : MonoBehaviour
{
    private const string GamePageUrl = "https://dani08lozano.itch.io/little-adventurer";
    private const float RedirectDelay = 10f;

    private readonly Dictionary<RectTransform, Coroutine> buttonAnimations = new Dictionary<RectTransform, Coroutine>();
    private readonly List<RectTransform> motes = new List<RectTransform>();
    private readonly List<Vector2> moteOrigins = new List<Vector2>();

    public UI_Manager uiManager;

    private CanvasGroup screenGroup;
    private RectTransform farewellCard;
    private TMP_FontAsset pixelFont;
    private TMP_Text countdownText;
    private Image countdownFill;
    private bool redirectStarted;

    private void Start()
    {
        TMP_Text existingText = FindObjectOfType<TMP_Text>();
        if (existingText != null)
            pixelFont = existingText.font;

        Canvas oldCanvas = FindObjectOfType<Canvas>();
        if (oldCanvas != null)
            oldCanvas.gameObject.SetActive(false);

        BuildFarewellScreen();
        StartCoroutine(AnimateScreenEntrance());
        StartCoroutine(AnimateMotes());
        StartCoroutine(CountdownToItchPage());
    }

    public void OnReturnHomeButton()
    {
        OpenItchPage();
    }

    private void BuildFarewellScreen()
    {
        GameObject root = CreateUIObject("Farewell Screen", transform);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();

        screenGroup = root.AddComponent<CanvasGroup>();
        screenGroup.alpha = 0f;

        Image sky = CreateImage("Sky", root.transform, new Color32(104, 189, 240, 255));
        Stretch(sky.rectTransform);

        Image horizon = CreateImage("Soft Horizon", root.transform, new Color32(164, 220, 247, 255));
        SetAnchoredRect(horizon.rectTransform, new Vector2(0f, 0.34f), new Vector2(1f, 0.72f), Vector2.zero, Vector2.zero);

        CreateCloud(root.transform, new Vector2(-610f, 300f), 1.05f);
        CreateCloud(root.transform, new Vector2(590f, 230f), 0.78f);

        CreateHill(root.transform, "Distant Hill Left", new Vector2(-560f, -260f), new Vector2(680f, 680f), new Color32(92, 67, 75, 255));
        CreateHill(root.transform, "Distant Hill Right", new Vector2(560f, -310f), new Vector2(760f, 760f), new Color32(74, 51, 63, 255));

        Image ground = CreateImage("Earth", root.transform, new Color32(58, 36, 53, 255));
        SetAnchoredRect(ground.rectTransform, Vector2.zero, new Vector2(1f, 0.18f), Vector2.zero, Vector2.zero);

        Image grassShadow = CreateImage("Grass Shadow", root.transform, new Color32(45, 86, 48, 255));
        SetAnchoredRect(grassShadow.rectTransform, new Vector2(0f, 0.18f), new Vector2(1f, 0.194f), Vector2.zero, Vector2.zero);

        Image grass = CreateImage("Grass", root.transform, new Color32(69, 197, 66, 255));
        SetAnchoredRect(grass.rectTransform, new Vector2(0f, 0.194f), new Vector2(1f, 0.211f), Vector2.zero, Vector2.zero);

        Image water = CreateImage("Water", root.transform, new Color32(25, 215, 204, 255));
        SetAnchoredRect(water.rectTransform, new Vector2(0.68f, 0f), new Vector2(0.86f, 0.075f), Vector2.zero, Vector2.zero);
        Image waterShine = CreateImage("Water Shine", water.transform, new Color32(104, 239, 225, 230));
        SetAnchoredRect(waterShine.rectTransform, new Vector2(0f, 0.62f), new Vector2(1f, 0.76f), Vector2.zero, Vector2.zero);

        Image atmosphere = CreateImage("Atmosphere", root.transform, new Color32(28, 17, 29, 94));
        Stretch(atmosphere.rectTransform);

        CreateMotes(root.transform);

        Image glow = CreateImage("Card Glow", root.transform, new Color32(83, 202, 62, 34));
        SetCenteredRect(glow.rectTransform, new Vector2(870f, 620f), new Vector2(0f, -4f));

        Image shadow = CreateImage("Card Shadow", root.transform, new Color32(12, 7, 13, 165));
        SetCenteredRect(shadow.rectTransform, new Vector2(760f, 520f), new Vector2(14f, -20f));

        Image card = CreateImage("Farewell Card", root.transform, new Color32(49, 30, 42, 250));
        farewellCard = card.rectTransform;
        SetCenteredRect(farewellCard, new Vector2(760f, 520f), new Vector2(0f, 8f));

        Outline cardOutline = card.gameObject.AddComponent<Outline>();
        cardOutline.effectColor = new Color32(116, 76, 69, 255);
        cardOutline.effectDistance = new Vector2(4f, -4f);

        Image greenAccent = CreateImage("Grass Accent", card.transform, new Color32(83, 202, 62, 255));
        SetTopStrip(greenAccent.rectTransform, 10f, 0f);
        Image skyAccent = CreateImage("Sky Accent", card.transform, new Color32(104, 189, 240, 255));
        SetTopStrip(skyAccent.rectTransform, 5f, 10f);

        CreateCornerDecoration(card.transform, new Vector2(-338f, 220f), new Color32(229, 177, 119, 255));
        CreateCornerDecoration(card.transform, new Vector2(338f, 220f), new Color32(229, 177, 119, 255));

        TMP_Text eyebrow = CreateText("Eyebrow", card.transform, "THANKS, ADVENTURER", 22f, new Color32(164, 220, 247, 255), FontStyles.Bold);
        SetCenteredRect(eyebrow.rectTransform, new Vector2(650f, 42f), new Vector2(0f, 166f));
        eyebrow.characterSpacing = 4f;

        TMP_Text title = CreateText("Title", card.transform, "UNTIL NEXT TIME!", 58f, new Color32(255, 239, 193, 255), FontStyles.Bold);
        SetCenteredRect(title.rectTransform, new Vector2(680f, 90f), new Vector2(0f, 92f));

        TMP_Text message = CreateText(
            "Message",
            card.transform,
            "Thank you for playing this early build.\nYour adventure is only just beginning.",
            26f,
            new Color32(224, 202, 169, 255),
            FontStyles.Normal);
        SetCenteredRect(message.rectTransform, new Vector2(650f, 82f), new Vector2(0f, 22f));
        message.enableWordWrapping = true;
        message.lineSpacing = 7f;

        countdownText = CreateText("Countdown", card.transform, "Opening the itch.io page in 10 seconds...", 21f, new Color32(193, 226, 127, 255), FontStyles.Bold);
        SetCenteredRect(countdownText.rectTransform, new Vector2(650f, 38f), new Vector2(0f, -52f));

        Image countdownTrack = CreateImage("Countdown Track", card.transform, new Color32(32, 20, 30, 255));
        SetCenteredRect(countdownTrack.rectTransform, new Vector2(520f, 10f), new Vector2(0f, -78f));
        countdownFill = CreateImage("Countdown Fill", countdownTrack.transform, new Color32(83, 202, 62, 255));
        Stretch(countdownFill.rectTransform);

        GameObject actions = CreateUIObject("Actions", card.transform);
        RectTransform actionsRect = actions.GetComponent<RectTransform>();
        SetCenteredRect(actionsRect, new Vector2(330f, 76f), new Vector2(0f, -134f));

        HorizontalLayoutGroup layout = actions.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 22f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        Button itchButton = CreateButton(
            "Itch Page",
            actions.transform,
            "VISIT ITCH.IO",
            new Color32(74, 176, 57, 255),
            new Color32(104, 218, 76, 255),
            new Color32(38, 25, 35, 255),
            new Color32(193, 226, 127, 255),
            OnReturnHomeButton);

        TMP_Text hint = CreateText("Hint", card.transform, "You can continue now, or wait for the automatic redirect.", 18f, new Color32(169, 143, 133, 255), FontStyles.Italic);
        SetCenteredRect(hint.rectTransform, new Vector2(650f, 34f), new Vector2(0f, -194f));

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(itchButton.gameObject);
    }

    private IEnumerator CountdownToItchPage()
    {
        float remaining = RedirectDelay;
        int displayedSeconds = -1;

        while (remaining > 0f && !redirectStarted)
        {
            int seconds = Mathf.CeilToInt(remaining);
            if (seconds != displayedSeconds)
            {
                displayedSeconds = seconds;
                countdownText.text = $"Opening the itch.io page in {seconds} second{(seconds == 1 ? "" : "s")}...";
            }

            SetCountdownProgress(remaining / RedirectDelay);
            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (redirectStarted)
            yield break;

        SetCountdownProgress(0f);
        countdownText.text = "Opening itch.io...";
        yield return null;
        OpenItchPage();
    }

    private void OpenItchPage()
    {
        if (redirectStarted)
            return;

        redirectStarted = true;
        countdownText.text = "Opening itch.io...";
        Application.OpenURL(GamePageUrl);
    }

    private void SetCountdownProgress(float progress)
    {
        RectTransform fillRect = countdownFill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(Mathf.Clamp01(progress), 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
    }

    private IEnumerator AnimateScreenEntrance()
    {
        const float duration = 0.45f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            screenGroup.alpha = eased;
            farewellCard.localScale = Vector3.one * Mathf.Lerp(0.9f, 1f, eased);
            yield return null;
        }

        screenGroup.alpha = 1f;
        farewellCard.localScale = Vector3.one;
    }

    private IEnumerator AnimateMotes()
    {
        float time = 0f;
        while (true)
        {
            time += Time.unscaledDeltaTime;
            for (int i = 0; i < motes.Count; i++)
            {
                Vector2 origin = moteOrigins[i];
                float phase = i * 0.83f;
                motes[i].anchoredPosition = origin + new Vector2(
                    Mathf.Sin(time * 0.7f + phase) * 9f,
                    Mathf.Sin(time * 0.5f + phase) * 14f);
            }
            yield return null;
        }
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

        TMP_Text text = CreateText("Label", image.transform, label, 24f, foreground, FontStyles.Bold);
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
        if (buttonAnimations.TryGetValue(buttonRect, out Coroutine activeAnimation))
            StopCoroutine(activeAnimation);

        buttonAnimations[buttonRect] = StartCoroutine(AnimateButtonHover(buttonRect, outline, hoverOutline, hovered));
    }

    private IEnumerator AnimateButtonHover(RectTransform buttonRect, Outline outline, Color hoverOutline, bool hovered)
    {
        const float duration = 0.12f;
        float elapsed = 0f;
        Vector3 startScale = buttonRect.localScale;
        Vector3 targetScale = Vector3.one * (hovered ? 1.055f : 1f);
        Color startOutline = outline.effectColor;
        Color targetOutline = hovered ? hoverOutline : new Color(hoverOutline.r, hoverOutline.g, hoverOutline.b, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            buttonRect.localScale = Vector3.LerpUnclamped(startScale, targetScale, eased);
            outline.effectColor = Color.LerpUnclamped(startOutline, targetOutline, eased);
            yield return null;
        }

        buttonRect.localScale = targetScale;
        outline.effectColor = targetOutline;
        buttonAnimations.Remove(buttonRect);
    }

    private void CreateCloud(Transform parent, Vector2 position, float scale)
    {
        GameObject cloud = CreateUIObject("Pixel Cloud", parent);
        RectTransform cloudRect = cloud.GetComponent<RectTransform>();
        SetCenteredRect(cloudRect, new Vector2(280f, 80f) * scale, position);

        Image center = CreateImage("Cloud Center", cloud.transform, new Color32(217, 239, 250, 230));
        SetCenteredRect(center.rectTransform, new Vector2(210f, 54f) * scale, Vector2.zero);
        Image left = CreateImage("Cloud Left", cloud.transform, new Color32(164, 220, 247, 230));
        SetCenteredRect(left.rectTransform, new Vector2(75f, 38f) * scale, new Vector2(-103f, 7f) * scale);
        Image top = CreateImage("Cloud Top", cloud.transform, new Color32(217, 239, 250, 230));
        SetCenteredRect(top.rectTransform, new Vector2(92f, 42f) * scale, new Vector2(22f, 38f) * scale);
    }

    private static void CreateHill(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        Image hill = CreateImage(name, parent, color);
        SetCenteredRect(hill.rectTransform, size, position);
        hill.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
    }

    private void CreateMotes(Transform parent)
    {
        Vector2[] positions =
        {
            new Vector2(-760f, 115f), new Vector2(-650f, -40f), new Vector2(-470f, 225f),
            new Vector2(470f, 250f), new Vector2(635f, 80f), new Vector2(770f, -25f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            Color color = i % 2 == 0 ? new Color32(193, 226, 127, 150) : new Color32(164, 220, 247, 145);
            Image mote = CreateImage("Ambient Mote", parent, color);
            SetCenteredRect(mote.rectTransform, new Vector2(12f, 12f), positions[i]);
            mote.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            motes.Add(mote.rectTransform);
            moteOrigins.Add(positions[i]);
        }
    }

    private static void CreateCornerDecoration(Transform parent, Vector2 position, Color color)
    {
        Image diamond = CreateImage("Corner Gem", parent, color);
        SetCenteredRect(diamond.rectTransform, new Vector2(18f, 18f), position);
        diamond.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
    }

    private TMP_Text CreateText(string name, Transform parent, string content, float size, Color color, FontStyles style)
    {
        GameObject result = CreateUIObject(name, parent);
        TextMeshProUGUI text = result.AddComponent<TextMeshProUGUI>();
        if (pixelFont != null)
            text.font = pixelFont;
        text.text = content;
        text.fontSize = size;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        return text;
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

    private static void SetAnchoredRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static void SetTopStrip(RectTransform rect, float height, float topOffset)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(0f, -(topOffset + height));
        rect.offsetMax = new Vector2(0f, -topOffset);
    }
}
