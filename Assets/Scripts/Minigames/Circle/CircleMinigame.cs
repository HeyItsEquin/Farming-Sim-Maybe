using System;
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
        circleManager.StartSpawning();
    }

    // TODO: Actually do something when it's over
    void EndGame(bool lost)
    {
        circleManager.OnGameLost();
        minigameUI.OnGameLost();

        SceneManager.LoadScene("MinigameSelect");
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
}