#if DEBUG
using ImGuiNET;
using UImGui;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DebugHost : MonoBehaviour
{
    [SerializeField] Camera imguiCamera;

    static bool created;
    float currentFps = 0;
    float lastFrameTime = 0f;

    float fpsUpdateTimeElapsed = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetCreated() => created = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (created) return;
        var prefab = Resources.Load<GameObject>("DebugMenu");
        if (!prefab)
        {
            Debug.LogError("Resources/DebugMenu.prefab not found");
            return;
        }
        var go = Instantiate(prefab);
        go.name = "[DebugMenu]";
        DontDestroyOnLoad(go);
        created = true;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UImGuiUtility.Layout += OnLayout;
        DebugUI.Add(this, "Scene", DrawSceneSection);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UImGuiUtility.Layout -= OnLayout;
        DebugUI.Remove(this);
    }

    void Start() => StartCoroutine(Attach());
    void OnSceneLoaded(Scene s, LoadSceneMode m) => StartCoroutine(Attach());

    System.Collections.IEnumerator Attach()
    {
        Camera cam = null;
        for (int i = 0; i < 60 && !(cam = Camera.main); i++)
            yield return null;
        if (!cam) yield break;

        var stack = cam.GetUniversalAdditionalCameraData().cameraStack;
        stack.RemoveAll(c => !c);
        stack.Remove(imguiCamera);
        stack.Add(imguiCamera);
    }

    void Update()
    {
        fpsUpdateTimeElapsed += Time.unscaledDeltaTime;
        if (fpsUpdateTimeElapsed > 0.25f)
        {
            currentFps = 1f / Time.unscaledDeltaTime;
            lastFrameTime = Time.unscaledDeltaTime*1000;
            fpsUpdateTimeElapsed = 0;
        }

        var kb = Keyboard.current;
        if (kb != null && kb.f1Key.wasPressedThisFrame)
            DebugUI.Visible = !DebugUI.Visible;
    }

    void ApplyFontScale()
    {
        var style = ImGui.GetStyle();
        style.FontScaleDpi = Mathf.Clamp(Screen.height/1080f, 0.75f, 3f);

        #if UNITY_EDITOR
        style.FontScaleMain = 1.2f;
        #endif
    }

    void DrawSceneSection()
    {
        ImGui.Text($"Scene: {SceneManager.GetActiveScene().name}");
        ImGui.Text($"FPS: {currentFps:F0}");
        ImGui.Text($"Frame Time: {lastFrameTime:F2}ms");
        ImGui.Spacing();
        ImGui.Text($"Window Resolution: {Screen.width}x{Screen.height}");
        if (ImGui.Button("Reload scene"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnLayout(UImGui.UImGui ui)
    {
        if (!DebugUI.Visible) return;

        ImGui.SetNextWindowSize(new System.Numerics.Vector2(320, 400), ImGuiCond.FirstUseEver);
        ApplyFontScale();
        if (ImGui.Begin("Debug"))
        {
            foreach (var s in DebugUI.Snapshot())
            {
                ImGui.PushID(s.GetHashCode());
                if (ImGui.CollapsingHeader(s.title, ImGuiTreeNodeFlags.DefaultOpen))
                    s.draw();
                ImGui.PopID();
            }
        }
        ImGui.End();
    }
}
#endif