using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InvalidSceneException : Exception
{
    public InvalidSceneException(string message)
        : base(message)
    {
    }
}

public class MinigameSelectUI : MonoBehaviour
{
    UIDocument document;
    VisualElement root;

    Button startCircleMinigame;
    Button startDodgeMinigame;

    readonly Dictionary<Minigame,string> minigameScenes = new()
    {
        { Minigame.Circles, "CircleMinigame" },
        { Minigame.Dodging, "DodgeMinigame" },
    };

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        startCircleMinigame = root.Q<Button>("StartCircleMinigame");
        startCircleMinigame.clicked += () => StartMinigame(Minigame.Circles);

        startDodgeMinigame = root.Q<Button>("StartDodgeMinigame");
        startDodgeMinigame.clicked += () => StartMinigame(Minigame.Dodging);
    }

    void StartMinigame(Minigame m)
    {
        if (!minigameScenes.TryGetValue(m,out var scene))
            throw new ArgumentException($"No corresponding scene was found for {m.FullName()}");
        if (!SceneUtils.SceneExists(scene))
            throw new InvalidSceneException($"Minigame scene {scene} (from: {m.FullName()}) does not exist. (Did you add it to the build profile?)");
        SceneManager.LoadScene(scene);
    }

    public enum Minigame
    {
        Circles,
        Dodging,
    }
}