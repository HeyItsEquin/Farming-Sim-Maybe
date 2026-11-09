using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using System.Linq;
using System;

public class CircleMinigameUI : MonoBehaviour
{
    [SerializeField] CircleMinigame game;

    [SerializeField] CircleManager circleManager;

    UIDocument document;
    VisualElement root;
    
    VisualElement healthBar;

    Label pointsDisplay;

    void Start()
    {
        Assert.IsNotNull(game, "Must assign Game");
        Assert.IsNotNull(circleManager, "Must assign Circle Manager");
    }

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        healthBar = root.Q<VisualElement>("HealthBar");

        pointsDisplay = root.Q<Label>("PointsDisplay");
        pointsDisplay.text = $"{game.Points:000}";
        game.pointsUpdated +=
            (p) => pointsDisplay.text = $"{game.Points:000}";
    }

    public void OnGameLost()
    {
        UIHelper.HideElement(healthBar);
    }

    public void UpdateHearts()
    {
        var children = healthBar.Children().ToList();
        var indexed = children.Select((v,i) => new { Index = i, Value = v });

        foreach (var item in indexed)
        {
            if (item.Index >= game.CurrentHealth)
                item.Value.AddToClassList("lost");
            else
                item.Value.RemoveFromClassList("lost");
        }
    }
}