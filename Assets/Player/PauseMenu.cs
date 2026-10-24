using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Меню паузы. Добавлять в сцену ничего не нужно: скрипт сам появляется
// в любой сцене, кроме "Menu", и сам строит интерфейс.
// Esc или P — поставить на паузу / продолжить.
public class PauseMenu : MonoBehaviour
{
    public const string MenuSceneName = "Menu";

    private GameObject panel;
    private bool isPaused = false;
    private CursorLockMode prevLockState;
    private bool prevCursorVisible;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MenuSceneName) return;
        if (FindFirstObjectByType<PauseMenu>() != null) return;

        new GameObject("PauseMenu").AddComponent<PauseMenu>();
    }

    void Start()
    {
        EnsureEventSystem();
        BuildUI();
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                Resume();
            else if (Time.timeScale > 0f) // если игра уже остановлена (победа/поражение) — паузу не включаем
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        prevLockState = Cursor.lockState;
        prevCursorVisible = Cursor.visible;

        panel.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        panel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = prevLockState;
        Cursor.visible = prevCursorVisible;
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(MenuSceneName);
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OnDestroy()
    {
        // На случай перезагрузки сцены во время паузы
        if (isPaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }

    // ---------- построение интерфейса ----------

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("PauseCanvas");
        canvasObj.transform.SetParent(transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Затемнение на весь экран
        panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObj.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.65f);
        RectTransform bgRect = bg.rectTransform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        CreateLabel("Пауза", font, 90, new Vector2(0, 250));
        CreateButton("Продолжить", font, new Vector2(0, 80), Resume);
        CreateButton("В главное меню", font, new Vector2(0, -50), GoToMenu);
        CreateButton("Выход", font, new Vector2(0, -180), QuitGame);
    }

    void CreateLabel(string text, Font font, int size, Vector2 pos)
    {
        GameObject obj = new GameObject("Title");
        obj.transform.SetParent(panel.transform, false);

        Text t = obj.AddComponent<Text>();
        t.text = text;
        t.font = font;
        t.fontSize = size;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;

        RectTransform r = t.rectTransform;
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(800, 150);
        r.anchoredPosition = pos;
    }

    void CreateButton(string text, Font font, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject obj = new GameObject("Button_" + text);
        obj.transform.SetParent(panel.transform, false);

        Image img = obj.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.92f);

        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        RectTransform r = img.rectTransform;
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(520, 100);
        r.anchoredPosition = pos;

        GameObject labelObj = new GameObject("Text");
        labelObj.transform.SetParent(obj.transform, false);
        Text label = labelObj.AddComponent<Text>();
        label.text = text;
        label.font = font;
        label.fontSize = 44;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        RectTransform lr = label.rectTransform;
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.offsetMin = Vector2.zero;
        lr.offsetMax = Vector2.zero;
    }
}
