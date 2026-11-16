using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CircleMinigameUI : MenuScript
{
    [SerializeField] CircleMinigame game;

    VisualElement mainInterfaceContainer;

    VisualElement healthBar;
    Label pointsDisplay;

    VisualElement victoryScreenContainer;

    VisualElement victoryScreenTextElements;
    Label victoryHeading;
    Label victorySubheading;

    Button victoryRetryButton;
    Button victoryMainMenuButton;

    VisualElement filler;
    
    protected override void OnRootReady()
    {
        ShowElement(mainInterfaceContainer);
        HideElement(victoryScreenContainer);

        pointsDisplay.text = $"{game.Points:000}";
        game.pointsUpdated += (p) => pointsDisplay.text = $"{game.Points:000}";

        SetScale(victoryScreenTextElements,0);

        victoryHeading.RemoveFromClassList("end-screen-failure");
        victoryHeading.RemoveFromClassList("end-screen-victory");

        victoryRetryButton.clicked += () => game.Restart();
        victoryMainMenuButton.clicked += () => game.ReturnToMainMenu();

        ShowElement(filler);
    }

    void ShowEndScreen(bool lost)
    {
        ShowElement(victoryScreenContainer);
        SetScale(victoryScreenTextElements,1);
        victorySubheading.text = $"Your final score: {game.Points}";
        
        SetEndScreenHeader(lost);
    }

    void SetEndScreenHeader(bool lost)
    {
        victoryHeading.RemoveFromClassList(lost ? "end-heading-victory" : "end-heading-failure");
        victoryHeading.AddToClassList(lost ? "end-heading-failure" : "end-heading-victory");
        victoryHeading.text = lost ? "You Failed!" : "You Won!";
    }

    public void OnGameEnd(bool lost)
    {
        HideElement(mainInterfaceContainer);
        HideElement(filler);

        ShowEndScreen(lost);
    }

    public void UpdateHearts()
    {
        foreach (var (i, child) in healthBar.Children().ToList().WithIndex())
        {
            if (i >= game.CurrentHealth)
                child.AddToClassList("lost");
            else
                child.RemoveFromClassList("lost");
        }
    }
}