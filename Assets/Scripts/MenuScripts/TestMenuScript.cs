using UnityEngine;
using UnityEngine.UIElements;

public class TestMenuScript : MenuScript
{
    Label victoryHeading;

    Button victoryRetryButton;
    Button victoryMainMenuButton;

    [UIName("PointsDisplay")]
    Label scoreBoard;

    [UIIgnore]
    Button thisButtonAlsoDoesNotExistButWeDontCare;

    [UIOptional]
    Button thisButtonMightExistButWeStillDontCare;

    protected override void OnRootReady()
    {
        victoryHeading.text = "It worked! :D";

        victoryRetryButton.clicked += () => Debug.Log("Yippie! It worked!");
        victoryMainMenuButton.clicked += () => Debug.Log("Main menu button works too!!! :D");

        scoreBoard.text = "This also works fine :3";
    }
}