using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;

public class CircleMinigameUI : MonoBehaviour
{
    [SerializeField] CircleManager circleManager;

    UIDocument document;
    VisualElement root;

    void Start()
    {
        Assert.IsNotNull(circleManager, "Must assign Circle Manager");
    }

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        var startBtn = root.Q<Button>("StartButton");
        startBtn.clicked += OnStartClicked;
    }

    void OnStartClicked()
    {
        circleManager.StartGame();

        root.style.display = DisplayStyle.None;
    }
}