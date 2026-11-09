using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MinigameSelectUI : MonoBehaviour
{
    UIDocument document;
    VisualElement root;

    Button startCircleMinigame;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        startCircleMinigame = root.Q<Button>("StartCircleMinigame");
        startCircleMinigame.clicked +=
            () => SceneManager.LoadScene("CircleMinigame");
    }
}