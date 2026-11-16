using System;
using ImGuiNET;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class CircleMinigame : MonoBehaviour
{
    [SerializeField] CircleManager circleManager;
    [SerializeField] CircleMinigameUI minigameUI;

    public Action<int> pointsUpdated;

    public int Points { get; private set; }
    public int CurrentHealth { get; private set; }
    private int maxHealth = 3;

    void Start()
    {
        Assert.IsNotNull(circleManager);
        Assert.IsNotNull(minigameUI);
    
        StartGame();
    }

    void StartGame()
    {
        CurrentHealth = maxHealth;
        minigameUI.UpdateHearts();
        circleManager.StartSpawning();
        minigameUI.Refresh();
    }

    public void Restart()
    {
        StartGame(); // All that needs to be done for now
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MinigameSelect");
    }

    public void EndGame(bool lost)
    {
        circleManager.OnGameEnd();
        minigameUI.OnGameEnd(lost); // This will handle everything that happens after game ends
    }

    public void AddPoints(int amnt)
    {
        Points += amnt;
        pointsUpdated?.Invoke(Points);
    }

    public void LoseHealth()
    {
        CurrentHealth = Math.Max(0,CurrentHealth-1);
        minigameUI.UpdateHearts();

        if (CurrentHealth <= 0)
            EndGame(true);
    }

    #if DEBUG
    void OnEnable() => DebugUI.Add(this, "Circle Minigame", DrawDebug);
    void OnDisable() => DebugUI.Remove(this);

    int desiredHealth = 0;

    void DrawDebug()
    {
        ImGui.Text($"Points: {Points}");
        ImGui.Text($"Health: {CurrentHealth}");

        ImGui.Spacing();

        ImGui.Text("Set Health: ");
        ImGui.SameLine();
        ImGui.SliderInt(" ", ref desiredHealth, 0, 3);
        ImGui.SameLine();
        if (ImGui.Button("Set"))
        {
            CurrentHealth = desiredHealth+1;
            LoseHealth();
        }

        if (ImGui.Button("Win Game")) EndGame(false);
        ImGui.SameLine();
        if (ImGui.Button("Lose Game")) EndGame(true);
    }
    #endif
}